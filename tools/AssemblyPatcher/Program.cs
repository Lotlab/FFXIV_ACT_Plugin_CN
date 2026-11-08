using System.IO.Compression;
using System.Text.Json;
using Mono.Cecil;
using Mono.Cecil.Cil;

if (args.Length == 0) { Usage(); return 2; }

switch (args[0])
{
    case "extract": return Extract(args);
    case "strip": return Strip(args);
    case "patch": return Patch(args);
    case "resources": return DumpResources(args);
    case "dump": return Dump(args);
    default: Usage(); return 2;
}

static void Usage()
{
    Console.Error.WriteLine(
        "AssemblyPatcher <command> ...\n\n" +
        "  extract   <plugin.dll> <outdir>            unpack costura.*.compressed payloads\n" +
        "  strip     <indir> <outdir> <name1,name2>  strip strong names + rewrite refs among the set\n" +
        "  patch     <patch.json>                    apply fields / resources / strings / enum members / IL fixes\n" +
        "  resources <dll> <outdir>                  write every embedded resource as a file\n" +
        "  dump      <dll> <type> <method>           print the IL of one method");
}

static int DumpResources(string[] args)
{
    var asm = AssemblyDefinition.ReadAssembly(args[1]);
    Directory.CreateDirectory(args[2]);
    int n = 0;
    foreach (var res in asm.MainModule.Resources)
    {
        if (res is not EmbeddedResource er) continue;
        File.WriteAllBytes(Path.Combine(args[2], er.Name), er.GetResourceData());
        n++;
    }
    Console.WriteLine($"{n} resources -> {args[2]}");
    return 0;
}

static string NameOf(AssemblyDefinition a) => a.Name.Name;

static int Extract(string[] args)
{
    var asm = AssemblyDefinition.ReadAssembly(args[1]);
    Directory.CreateDirectory(args[2]);
    int n = 0;
    foreach (var res in asm.MainModule.Resources)
    {
        if (res is not EmbeddedResource er || !er.Name.StartsWith("costura.") || !er.Name.EndsWith(".compressed"))
            continue;
        var file = er.Name["costura.".Length..^".compressed".Length];
        using var input = new MemoryStream(er.GetResourceData());
        using var deflate = new DeflateStream(input, CompressionMode.Decompress);
        using var output = File.Create(Path.Combine(args[2], file));
        deflate.CopyTo(output);
        n++;
        Console.WriteLine($"  {file}");
    }
    Console.WriteLine($"extracted {n} assemblies -> {args[2]}");
    return 0;
}

static int Strip(string[] args)
{
    var indir = args[1];
    var outdir = args[2];
    var set = args[3].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
    Directory.CreateDirectory(outdir);

    var files = Directory.GetFiles(indir, "*.dll")
        .Where(f => set.Contains(Path.GetFileNameWithoutExtension(f)))
        .ToList();
    if (files.Count == 0) { Console.Error.WriteLine("no input assemblies matched"); return 1; }

    foreach (var f in files)
    {
        var asm = AssemblyDefinition.ReadAssembly(f);
        if (!set.Contains(NameOf(asm))) continue;

        asm.Name.HasPublicKey = false;
        asm.Name.PublicKey = Array.Empty<byte>();
        asm.Name.Attributes &= ~AssemblyAttributes.PublicKey;
        asm.MainModule.Attributes &= ~ModuleAttributes.StrongNameSigned;

        int refs = 0;
        foreach (var r in asm.MainModule.AssemblyReferences)
        {
            if (!set.Contains(r.Name)) continue;
            r.PublicKeyToken = Array.Empty<byte>();
            r.HasPublicKey = false;
            refs++;
        }

        var outPath = Path.Combine(outdir, Path.GetFileName(f));
        asm.Write(outPath, new WriterParameters { StrongNameKeyPair = null });
        Console.WriteLine($"  stripped {NameOf(asm)} ({refs} refs cleared) -> {Path.GetFileName(outPath)}");
    }
    Console.WriteLine($"stripped {files.Count} assemblies -> {outdir}");
    return 0;
}

static int Patch(string[] args)
{
    var manifest = Path.GetFullPath(args[1]);
    var doc = JsonDocument.Parse(File.ReadAllText(manifest));
    var root = doc.RootElement;
    var baseDir = Path.GetDirectoryName(manifest)!;

    foreach (var asmPatch in root.GetProperty("assemblies").EnumerateArray())
    {
        var path = Resolve(baseDir, asmPatch.GetProperty("path").GetString()!);
        // read through memory so the file handle is released before we rewrite it
        var asm = AssemblyDefinition.ReadAssembly(new MemoryStream(File.ReadAllBytes(path)));

        if (asmPatch.TryGetProperty("addFields", out var fields))
            foreach (var f in fields.EnumerateArray())
            {
                var td = asm.MainModule.GetType(f.GetProperty("type").GetString()!) ?? throw new Exception("type not found");
                var name = f.GetProperty("name").GetString()!;
                if (td.Fields.Any(x => x.Name == name)) continue;
                td.Fields.Add(new FieldDefinition(name, FieldAttributes.Public, TypeRef(asm, f.GetProperty("fieldType").GetString()!)));
                Console.WriteLine($"  + {td.FullName}::{name}");
            }

        if (asmPatch.TryGetProperty("replaceResources", out var resources))
            foreach (var r in resources.EnumerateArray())
            {
                var name = r.GetProperty("name").GetString()!;
                var data = File.ReadAllBytes(Resolve(baseDir, r.GetProperty("file").GetString()!));
                var res = asm.MainModule.Resources.FirstOrDefault(x => x.Name == name);
                if (res is EmbeddedResource er)
                {
                    typeof(EmbeddedResource).GetField("data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                        .SetValue(er, data);
                }
                else if (res is null)
                {
                    asm.MainModule.Resources.Add(new EmbeddedResource(name, ManifestResourceAttributes.Public, data));
                }
                else throw new Exception("resource is not embedded: " + name);
                Console.WriteLine($"  ~ resource {name} ({data.Length} bytes)");
            }

        if (asmPatch.TryGetProperty("replaceStrings", out var strings))
            foreach (var s in strings.EnumerateArray())
            {
                var from = s.GetProperty("from").GetString()!;
                var to = s.GetProperty("to").GetString()!;
                int n = 0;
                foreach (var m in AllMethods(asm))
                {
                    if (!m.HasBody) continue;
                    foreach (var ins in m.Body.Instructions)
                        if (ins.OpCode == OpCodes.Ldstr && (string)ins.Operand == from)
                        {
                            ins.Operand = to;
                            n++;
                        }
                }
                Console.WriteLine($"  ~ string \"{from}\" -> \"{to}\" ({n} sites)");
            }

        if (asmPatch.TryGetProperty("addEnumMembers", out var enums))
            foreach (var e in enums.EnumerateArray())
            {
                var td = asm.MainModule.GetType(e.GetProperty("type").GetString()!) ?? throw new Exception("enum not found");
                var name = e.GetProperty("name").GetString()!;
                if (td.Fields.Any(x => x.Name == name)) continue;
                var value = e.GetProperty("value").GetByte();
                td.Fields.Add(new FieldDefinition(name,
                    FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal | FieldAttributes.HasDefault,
                    td.Fields.First(f => f.Name == "value__").FieldType) { Constant = value });
                Console.WriteLine($"  + enum {td.FullName}::{name} = {value}");
            }

        if (asmPatch.TryGetProperty("fixFormatVersion", out var ffv) && ffv.GetBoolean())
            FixFormatVersion(asm);

        asm.Write(path);
        Console.WriteLine($"  wrote {Path.GetFileName(path)}");
    }
    return 0;

    static string Resolve(string baseDir, string p) => Path.IsPathRooted(p) ? p : Path.GetFullPath(Path.Combine(baseDir, p));

    static TypeReference TypeRef(AssemblyDefinition asm, string t) => t switch
    {
        "string" => asm.MainModule.TypeSystem.String,
        "bool" => asm.MainModule.TypeSystem.Boolean,
        "int" => asm.MainModule.TypeSystem.Int32,
        "uint" => asm.MainModule.TypeSystem.UInt32,
        _ => throw new Exception("unsupported field type: " + t)
    };

    static IEnumerable<MethodDefinition> AllMethods(AssemblyDefinition asm)
    {
        foreach (var t in asm.MainModule.Types)
            foreach (var m in Walk(t)) yield return m;

        static IEnumerable<MethodDefinition> Walk(TypeDefinition t)
        {
            foreach (var m in t.Methods) yield return m;
            foreach (var n in t.NestedTypes)
                foreach (var m in Walk(n)) yield return m;
        }
    }

    static void FixFormatVersion(AssemblyDefinition asm)
    {
        // LogFormat.FormatVersion() reads its own public key token.  On an unsigned
        // assembly that array is empty and BitConverter.ToUInt64 throws, so replace
        //   GetName(); GetPublicKeyToken(); ldc.i4.0; ToUInt64(...)
        // with
        //   ldc.i4.0; conv.i8
        var td = asm.MainModule.GetType("FFXIV_ACT_Plugin.Logfile.LogFormat") ?? throw new Exception("LogFormat not found");
        var m = td.Methods.First(x => x.Name == "FormatVersion" && x.HasBody);
        var ins = m.Body.Instructions;
        var call = ins.FirstOrDefault(i => i.OpCode == OpCodes.Call && i.Operand is MethodReference mr && mr.Name == "ToUInt64");
        if (call is null)
        {
            Console.WriteLine("  ~ FormatVersion already patched");
            return;
        }
        int idx = ins.IndexOf(call);

        // The token computation is a single chain:
        //   ldtoken LogOutput; GetTypeFromHandle; get_Assembly; GetName;
        //   GetPublicKeyToken; ldc.i4.0; ToUInt64
        // Rewind to the ldtoken that starts it so no stray values stay on the stack.
        int start = idx;
        while (start > 0 && ins[start].OpCode != OpCodes.Ldtoken) start--;
        if (ins[start].OpCode != OpCodes.Ldtoken) throw new Exception("FormatVersion prologue not found");

        var removed = ins.Skip(start).Take(idx - start + 1).ToList();
        var il = m.Body.GetILProcessor();
        var first = il.Create(OpCodes.Ldc_I4_0);
        var conv = il.Create(OpCodes.Conv_I8);
        il.InsertBefore(removed[0], first);
        il.InsertBefore(removed[0], conv);
        foreach (var r in removed) il.Remove(r);

        // keep any branch target that pointed into the replaced range valid
        foreach (var i in ins)
            if (i.Operand is Instruction t && removed.Contains(t))
                i.Operand = first;

        Console.WriteLine($"  ~ patched {td.FullName}.FormatVersion (unsigned token)");
    }
}

static int Dump(string[] args)
{
    var asm = AssemblyDefinition.ReadAssembly(args[1]);
    var td = asm.MainModule.GetType(args[2]) ?? throw new Exception("type not found");
    foreach (var m in td.Methods.Where(x => x.Name == args[3]))
    {
        Console.WriteLine($"{m.FullName}  maxstack={m.Body.MaxStackSize}");
        foreach (var i in m.Body.Instructions) Console.WriteLine("  " + i);
    }
    return 0;
}
