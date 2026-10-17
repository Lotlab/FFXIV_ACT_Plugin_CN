using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PluginLoadTest
{
    /// <summary>
    /// Verifies that a woven FFXIV_ACT_Plugin.dll is genuinely self-contained.
    ///
    /// The DLL is copied on its own into an empty directory (no sibling
    /// FFXIV_ACT_Plugin.*.dll, Machina or Newtonsoft.Json), loaded from there the way
    /// ACT loads plugins, and then every assembly it bundles is forced to load.  A
    /// bundled assembly loaded out of the embedded Costura resources has an empty
    /// <see cref="Assembly.Location"/>, which is what this harness asserts.
    /// </summary>
    internal static class Program
    {
        /// <summary>Supplied by the ACT host process; must never be bundled.</summary>
        private const string HostAssembly = "Advanced Combat Tracker";

        /// <summary>
        /// Bundled assemblies that the plugin uses indirectly, so they never show up in
        /// its own AssemblyRef table but must still be reachable from the embedded
        /// resources: Network needs Machina, Resource needs Newtonsoft.Json.
        /// </summary>
        private static readonly string[] TransitivelyBundled = { "Machina", "Newtonsoft.Json" };

        private static int _failures;

        private static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("usage: PluginLoadTest <path to FFXIV_ACT_Plugin.dll>");
                return 2;
            }

            var source = Path.GetFullPath(args[0]);
            if (!File.Exists(source))
            {
                Console.Error.WriteLine("not found: " + source);
                return 2;
            }

            // Load from an otherwise empty directory, exactly like an ACT plugin folder
            // that only contains the woven plugin DLL.
            var isolated = Path.Combine(Path.GetTempPath(), "PluginLoadTest-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(isolated);
            var pluginPath = Path.Combine(isolated, Path.GetFileName(source));
            File.Copy(source, pluginPath);

            Console.WriteLine("plugin        : " + pluginPath);
            Console.WriteLine("size          : " + new FileInfo(pluginPath).Length.ToString("N0") + " bytes");
            Console.WriteLine("siblings      : " + (Directory.GetFiles(isolated).Length - 1) + " other file(s) in the folder");

            try
            {
                var plugin = Assembly.LoadFrom(pluginPath);
                Console.WriteLine("loaded        : " + plugin.FullName);

                // Costura attaches its resolver from the module initializer; run it
                // explicitly so the test does not depend on loader timing.
                RuntimeHelpers.RunModuleConstructor(plugin.ManifestModule.ModuleHandle);

                var resources = plugin.GetManifestResourceNames();
                var referenced = plugin.GetReferencedAssemblies()
                                       .ToDictionary(n => n.Name, n => n, StringComparer.OrdinalIgnoreCase);

                Console.WriteLine("resources     : " + resources.Count(r => r.StartsWith("costura.")) +
                                  " costura payload(s)");

                Console.WriteLine();
                Console.WriteLine("bundled assemblies");
                var bundled = new List<string>();
                foreach (var name in referenced.Keys.OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
                {
                    if (IsFramework(name))
                    {
                        continue;
                    }
                    if (string.Equals(name, HostAssembly, StringComparison.OrdinalIgnoreCase))
                    {
                        CheckHostSupplied(name);
                        continue;
                    }
                    CheckEmbedded(name, referenced[name]);
                    bundled.Add(name);
                }

                foreach (var name in TransitivelyBundled)
                {
                    CheckEmbedded(name, new AssemblyName(name));
                    bundled.Add(name);
                }

                Console.WriteLine();
                Console.WriteLine("provided by the environment");
                foreach (var name in referenced.Keys.Where(IsFramework)
                                                    .OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
                {
                    CheckFramework(name, referenced[name]);
                }

                // Prove the JSON dependency actually works from the embedded copy:
                // FFXIV_ACT_Plugin.Resource deserialises its job definitions with it.
                Console.WriteLine();
                CheckNewtonsoftWorks();

                Console.WriteLine();
                if (_failures == 0 && bundled.Count > 0)
                {
                    Console.WriteLine("PASS - " + bundled.Count +
                                      " assemblies resolved from the embedded resources, nothing on disk");
                    return 0;
                }

                Console.Error.WriteLine("FAIL - " + _failures + " problem(s)");
                return 1;
            }
            finally
            {
                try { Directory.Delete(isolated, true); } catch { /* best effort */ }
            }
        }

        private static void CheckEmbedded(string simpleName, AssemblyName fullName)
        {
            Assembly loaded;
            try
            {
                loaded = Assembly.Load(fullName);
            }
            catch (Exception ex)
            {
                Fail(string.Format("{0,-32} did NOT resolve: {1}", simpleName, ex.GetType().Name));
                return;
            }

            if (!string.IsNullOrEmpty(loaded.Location))
            {
                Fail(string.Format("{0,-32} came from {1} (expected embedded)", simpleName, loaded.Location));
                return;
            }

            Console.WriteLine("  OK   {0,-32} v{1}  <embedded>", simpleName, loaded.GetName().Version);
        }

        private static void CheckHostSupplied(string name)
        {
            try
            {
                var loaded = Assembly.Load(new AssemblyName(name));
                if (!string.IsNullOrEmpty(loaded.Location))
                {
                    Fail(name + " resolved from " + loaded.Location + " - it must come from the ACT process");
                    return;
                }
                Fail(name + " is embedded in the plugin; the host must supply it");
                return;
            }
            catch (FileNotFoundException)
            {
                // exactly what we want: not bundled, and not available on its own
            }
            Console.WriteLine("  OK   {0,-32} not bundled, supplied by the host", name);
        }

        private static void CheckFramework(string name, AssemblyName fullName)
        {
            try
            {
                var loaded = Assembly.Load(fullName);
                Console.WriteLine("  OK   {0,-32} {1}", name,
                                  string.IsNullOrEmpty(loaded.Location) ? "<in memory>" : loaded.Location);
            }
            catch (Exception ex)
            {
                // A framework facade the plugin references but this harness cannot load
                // is worth reporting, but it is not a packaging defect, so do not fail.
                Console.WriteLine("  --   {0,-32} not loadable here ({1})", name, ex.GetType().Name);
            }
        }

        private static void CheckNewtonsoftWorks()
        {
            try
            {
                var jsonConvert = Type.GetType("Newtonsoft.Json.JsonConvert, Newtonsoft.Json", true);
                var serialize = jsonConvert.GetMethod("SerializeObject", new[] { typeof(object) });
                var text = (string)serialize.Invoke(null, new object[] { new { hello = "world" } });
                if (text == null || !text.Contains("world"))
                {
                    Fail("Newtonsoft.Json.SerializeObject returned " + (text ?? "<null>"));
                    return;
                }
                Console.WriteLine("  OK   {0,-32} {1}", "Newtonsoft.Json round-trip", text);
            }
            catch (Exception ex)
            {
                Fail("Newtonsoft.Json is not usable: " + ex.Message);
            }
        }

        private static bool IsFramework(string name)
        {
            return name == "mscorlib" || name == "netstandard" ||
                   name.StartsWith("System", StringComparison.Ordinal);
        }

        private static void Fail(string message)
        {
            _failures++;
            Console.Error.WriteLine("  FAIL " + message);
        }
    }
}
