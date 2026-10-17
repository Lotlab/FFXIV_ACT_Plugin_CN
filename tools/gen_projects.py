"""Regenerate the SDK-style .csproj files for the decompiled FFXIV_ACT_Plugin sources.

Input  : <repo>/.work/out/<key>/<ilspycmd generated>.csproj   (produced by tools/decompile.py)
Output : <repo>/src/<ProjectName>/<ProjectName>.csproj

The ilspycmd generated projects reference sibling assemblies via `<Reference><HintPath>`
pointing at the folder the input DLLs lived in.  We rewrite those into
`<ProjectReference>` items (first-party FFXIV_ACT_Plugin.* modules) or into
`<Reference>` items pointing at the `refs/` folder (third-party binaries), so the
solution builds from a clean checkout.  Embedded-resource item groups that ilspycmd
reconstructed are preserved verbatim; everything else is regenerated from
Directory.Build.props plus a minimal per-project PropertyGroup.
"""
import os
import re

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
OUT = os.path.join(REPO, ".work", "out")
SRC = os.path.join(REPO, "src")

MAP = {
    "main": "FFXIV_ACT_Plugin",
    "common": "FFXIV_ACT_Plugin.Common",
    "config": "FFXIV_ACT_Plugin.Config",
    "logfile": "FFXIV_ACT_Plugin.Logfile",
    "memory": "FFXIV_ACT_Plugin.Memory",
    "network": "FFXIV_ACT_Plugin.Network",
    "parse": "FFXIV_ACT_Plugin.Parse",
    "resource": "FFXIV_ACT_Plugin.Resource",
}

# extra per-project properties that ilspycmd detected and that we want to keep
EXTRA_PROPS = {
    "FFXIV_ACT_Plugin": ["<UseWindowsForms>true</UseWindowsForms>"],
    "FFXIV_ACT_Plugin.Common": ["<UseWindowsForms>true</UseWindowsForms>"],
}

# Costura.Fody turns the main project into the single shipping
# FFXIV_ACT_Plugin.dll.  Keep this in sync with src/FFXIV_ACT_Plugin/FodyWeavers.xml.
COSTURA_REFERENCES = """  <ItemGroup>
    <!--
      Single-file packaging: Costura.Fody weaves every copy-local dependency (the
      seven FFXIV_ACT_Plugin.* project outputs, Machina, Machina.FFXIV and the
      transitively required Newtonsoft.Json) into FFXIV_ACT_Plugin.dll as
      `costura.*.compressed` resources, and injects the assembly resolver that loads
      them at runtime.  This is how the shipped plugin was built.
    -->
    <PackageReference Include="Fody" Version="6.9.3" PrivateAssets="all" />
    <PackageReference Include="Costura.Fody" Version="6.2.0" PrivateAssets="all" />
  </ItemGroup>

  <!--
    Stage the woven DLL on its own, so the single-file deliverable is not mixed in
    with the sibling project outputs that share artifacts/bin.  This directory is
    exactly what gets copied into ACT's plugin folder (plus the deucalion DLLs).
  -->
  <Target Name="StageSingleFilePlugin" AfterTargets="Build">
    <PropertyGroup>
      <SingleFileStageDir>$(RepoRoot)artifacts\\dist\\$(Configuration)\\</SingleFileStageDir>
    </PropertyGroup>
    <RemoveDir Directories="$(SingleFileStageDir)" />
    <MakeDir Directories="$(SingleFileStageDir)" />
    <Copy SourceFiles="$(TargetPath)" DestinationFolder="$(SingleFileStageDir)" />
    <Message Importance="high" Text="Single-file plugin staged: $(SingleFileStageDir)$(TargetFileName)" />
  </Target>
"""

# third-party binaries that live in refs/ -> (file name, copy local?)
# Advanced Combat Tracker is the *host* process assembly: it must never be copied or
# embedded.  Everything else has to be copy-local so Costura.Fody embeds it, which is
# what makes single-file deployment work.
THIRDPARTY = {
    "Advanced Combat Tracker": ("Advanced Combat Tracker.exe", False),
    "Machina": ("Machina.dll", True),
    "Machina.FFXIV": ("Machina.FFXIV.dll", True),
    "Newtonsoft.Json": ("Newtonsoft.Json.dll", True),
}

# framework references that the .NET Framework SDK provides by default
DEFAULT_FRAMEWORK_REFS = {"System.Core", "System", "System.Xml", "mscorlib"}

REF_RE = re.compile(
    r'<Reference Include="([^"]+)">\s*<HintPath>([^<]+)</HintPath>\s*</Reference>')
PLAIN_REF_RE = re.compile(r'<Reference Include="([^"]+)"\s*/>')
GROUP_RE = re.compile(r"\n  <ItemGroup>(.*?)</ItemGroup>", re.S)


def collect_references(text):
    project_refs, third_party, plain = [], [], []
    for name, hint in REF_RE.findall(text):
        if name in MAP.values():
            project_refs.append(
                '    <ProjectReference Include="..\\%s\\%s.csproj" />' % (name, name))
        elif name in THIRDPARTY:
            file_name, copy_local = THIRDPARTY[name]
            note = ("" if copy_local else
                    "      <!-- host assembly: never copy or embed -->\n")
            third_party.append(
                "    <Reference Include=\"%s\">\n"
                "      <HintPath>$(RefsDir)%s</HintPath>\n"
                "      <SpecificVersion>false</SpecificVersion>\n"
                "%s"
                "      <Private>%s</Private>\n"
                "    </Reference>" % (name, file_name, note,
                                      "true" if copy_local else "false"))
        else:
            raise SystemExit("unmapped hint-path reference %r" % name)
    for name in PLAIN_REF_RE.findall(text):
        if name not in DEFAULT_FRAMEWORK_REFS:
            plain.append('    <Reference Include="%s" />' % name)
    return project_refs, third_party, plain


def collect_resource_groups(text):
    """Keep the EmbeddedResource / None bookkeeping ilspycmd reconstructed.

    Costura payloads (``costura.*.compressed``) were dropped from the sources: the
    sub-assemblies they carried are now built from the decompiled sibling projects and
    pulled in through <ProjectReference>, so there is nothing left to embed.
    """
    groups = []
    for body in GROUP_RE.findall(text):
        if "<EmbeddedResource" not in body and "<None Remove" not in body:
            continue
        lines = [l for l in body.splitlines() if "costura." not in l]
        body = "\n".join(lines).strip("\n")
        if not body.strip():
            continue
        groups.append("  <ItemGroup>\n%s\n  </ItemGroup>" % body)
    return groups


def dedupe_resources(groups, project):
    """Drop duplicate embedded-resource logical names.

    The shipped FFXIV_ACT_Plugin.Resource assembly embeds
    ``...Generated.ActionList_Chinese.txt`` and ``...Generated.StatusList_Chinese.txt``
    twice (two byte-identical payloads under one manifest name), which the decompiler
    materialises as ``*_2.txt``.  MSBuild rejects that with CS1508, and .NET would only
    ever resolve one of the two copies anyway, so the redundant file is removed.
    """
    seen = {}
    kept_groups, dropped = [], []
    for group in groups:
        kept_lines = []
        for line in group.splitlines():
            m = re.search(r'<EmbeddedResource Include="([^"]+)" LogicalName="([^"]+)"', line)
            if m:
                include, logical = m.group(1), m.group(2)
                if logical in seen:
                    dropped.append(include)
                    continue
                seen[logical] = include
            kept_lines.append(line)
        text = "\n".join(kept_lines)
        if "<EmbeddedResource" in text or "<None Remove" in text:
            kept_groups.append(text)
    for name in dropped:
        path = os.path.join(SRC, project, name)
        if os.path.exists(path):
            os.remove(path)
            print("  dropped duplicate embedded resource", name)
    return kept_groups


def prebuilt_resource_group(project):
    """Embed raw ``*.resources`` blobs that were extracted from the shipped assembly.

    ``FFXIV_ACT_Plugin.dll`` carries two empty WinForms designer resource containers
    (`FFXIV_ACT_Plugin.{Settings,Benchmark}PropertyPage.resources`).  They are not
    reachable from the decompiled IL, but they were in the original manifest, so the
    extracted blobs are embedded verbatim to keep the resource table identical.
    """
    d = os.path.join(SRC, project)
    if not os.path.isdir(d):
        return []
    names = sorted(f for f in os.listdir(d) if f.endswith(".resources"))
    if not names:
        return []
    group = ["  <ItemGroup>"]
    for name in names:
        group.append('    <EmbeddedResource Include="%s" LogicalName="%s" />' % (name, name))
    group.append("  </ItemGroup>")
    return group


def build(project, text):
    project_refs, third_party, plain = collect_references(text)
    resource_groups = dedupe_resources(collect_resource_groups(text), project)
    resource_groups += prebuilt_resource_group(project)

    parts = ['<Project Sdk="Microsoft.NET.Sdk">', "  <PropertyGroup>"]
    parts.append("    <AssemblyName>%s</AssemblyName>" % project)
    parts.append("    <RootNamespace>%s</RootNamespace>" % project)
    parts.extend("    " + p for p in EXTRA_PROPS.get(project, []))
    parts.append("  </PropertyGroup>")

    refs = plain + third_party
    if project_refs:
        parts.append("  <ItemGroup>")
        parts.extend(sorted(project_refs))
        parts.append("  </ItemGroup>")
    if refs:
        parts.append("  <ItemGroup>")
        parts.extend(sorted(refs))
        parts.append("  </ItemGroup>")
    parts.extend(resource_groups)
    if project == "FFXIV_ACT_Plugin":
        parts.append(COSTURA_REFERENCES.rstrip("\n"))
    parts.append("</Project>")
    return "\n".join(parts) + "\n"


def main():
    for key, project in MAP.items():
        d = os.path.join(OUT, key)
        cands = [f for f in os.listdir(d) if f.endswith(".csproj")]
        assert len(cands) == 1, (d, cands)
        text = open(os.path.join(d, cands[0]), encoding="utf-8-sig").read()
        dst_dir = os.path.join(SRC, project)
        os.makedirs(dst_dir, exist_ok=True)
        dst = os.path.join(dst_dir, project + ".csproj")
        # LF, like the ilspycmd-generated sources: .gitattributes is `* -text`, so
        # writing CRLF here would record whole-file churn on every regeneration
        with open(dst, "w", encoding="utf-8", newline="\n") as fh:
            fh.write(build(project, text))
        print("wrote", os.path.relpath(dst, REPO))


if __name__ == "__main__":
    main()
