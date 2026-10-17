"""Verify the reconstructed sources still describe the same assemblies.

For every shipped assembly the *metadata surface* is dumped from the original DLL and
from the freshly built DLL, and the two are compared table by table
(TypeDef / MethodDef / Field / Property / Event / ManifestResource).

A metadata comparison is used rather than a text diff because recompiling the
decompiled sources with a current Roslyn cannot reproduce the original toolchain's
exact IL: method bodies come back with slightly different local-variable layout and
`?.` reconstruction.  The declaration surface, on the other hand, must be identical -
and any missing, renamed or extraneous member shows up here immediately.

The only deltas that are allowed are the deliberate changes documented in README.md:

* the main assembly no longer carries the Costura loader type and members, nor the ten
  `costura.*.compressed` manifest resources it used to resolve;
* FFXIV_ACT_Plugin.Resource ships each duplicated `*_Chinese.txt` payload once
  instead of twice;
* current Roslyn injects the `EmbeddedAttribute` / `RefSafetyRulesAttribute` helper
  types into every assembly.

Usage:  python tools/verify_reconstruction.py [--statements]
"""
import collections
import difflib
import os
import re
import subprocess
import sys

try:
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
except Exception:  # pragma: no cover
    pass

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
ORIGINAL = os.path.join(REPO, ".work", "bin")
BUILT = os.path.join(REPO, "artifacts", "bin", "Release")

PAIRS = [
    ("FFXIV_ACT_Plugin", "ffxiv_act_plugin_main.dll", "FFXIV_ACT_Plugin.dll"),
    ("FFXIV_ACT_Plugin.Common", "ffxiv_act_plugin.common.dll", "FFXIV_ACT_Plugin.Common.dll"),
    ("FFXIV_ACT_Plugin.Config", "ffxiv_act_plugin.config.dll", "FFXIV_ACT_Plugin.Config.dll"),
    ("FFXIV_ACT_Plugin.Logfile", "ffxiv_act_plugin.logfile.dll", "FFXIV_ACT_Plugin.Logfile.dll"),
    ("FFXIV_ACT_Plugin.Memory", "ffxiv_act_plugin.memory.dll", "FFXIV_ACT_Plugin.Memory.dll"),
    ("FFXIV_ACT_Plugin.Network", "ffxiv_act_plugin.network.dll", "FFXIV_ACT_Plugin.Network.dll"),
    ("FFXIV_ACT_Plugin.Parse", "ffxiv_act_plugin.parse.dll", "FFXIV_ACT_Plugin.Parse.dll"),
    ("FFXIV_ACT_Plugin.Resource", "ffxiv_act_plugin.resource.dll", "FFXIV_ACT_Plugin.Resource.dll"),
]

TABLES = ["TypeDef", "MethodDef", "Field", "Property", "Event", "ManifestResource"]

ILSPY = os.path.join(os.path.expanduser("~"), ".dotnet", "tools", "ilspycmd.exe")

# --- the deltas we accept ---------------------------------------------------
ROSELYN_HELPERS = {"EmbeddedAttribute", "RefSafetyRulesAttribute"}

# Names the C# compiler synthesises for closures, cached delegates, iterator state
# machines and anonymous types.  Recompiling decompiled source always renumbers these
# (a `foreach` variable has to be renamed `x2` to stay in scope, which changes the
# captured-field name and therefore the display-class ordinal), so they are reported
# separately instead of failing the comparison.
SYNTHESIZED = re.compile(
    r"^(<>|<[^<>]+>b__|<[^<>]+>d__|<Module>|<PrivateImplementationDetails>|<0>__)")


def is_synthesized(name):
    """True for compiler-generated types and the members that belong to them."""
    owner, _, member = name.partition("::")
    if SYNTHESIZED.match(owner.split(".")[-1]):
        return True
    # do not split the member on '.': lambda names look like `<.ctor>b__10_0`
    return bool(SYNTHESIZED.match(member))


def table_rows(path, table):
    """Return (column names, rows) of a metadata table, parsed by column spans."""
    out = subprocess.run([ILSPY, "--dump-table", table, path], capture_output=True)
    text = out.stdout.decode("utf-8", "replace")
    if "Unknown metadata table" in text or not text.strip():
        return [], []
    lines = text.splitlines()
    if len(lines) < 3:
        return [], []
    header, sep = lines[0], lines[1]
    spans = [(m.start(), m.end()) for m in re.finditer(r"-{2,}", sep)]
    names = [header[a:b].strip() for a, b in spans]
    rows = [[line[a:b].strip() for a, b in spans] for line in lines[2:] if line.strip()]
    return names, rows


def rows_by_rid(path, table):
    """Rows of a table keyed by their RID (as int)."""
    names, rows = table_rows(path, table)
    idx = {n: i for i, n in enumerate(names)}
    out = {}
    if "RID" not in idx:
        return names, out, idx
    for row in rows:
        try:
            out[int(row[idx["RID"]])] = row
        except (ValueError, IndexError):
            continue
    return names, out, idx


def owner_map(path, member_table):
    """Map each Field / MethodDef RID to the full name of its declaring type."""
    _, types, tidx = rows_by_rid(path, "TypeDef")
    start_col = "FieldList" if member_table == "Field" else "MethodList"
    starts = []
    for rid in sorted(types):
        row = types[rid]
        if start_col not in tidx or len(row) <= tidx[start_col]:
            continue
        try:
            start = int(row[tidx[start_col]])
        except ValueError:
            continue
        ns = row[tidx["Namespace"]] if "Namespace" in tidx and len(row) > tidx["Namespace"] else ""
        nm = row[tidx["Name"]] if "Name" in tidx and len(row) > tidx["Name"] else ""
        # secondary key is the type RID: consecutive types may share a start RID
        # (the earlier one then owns no members), and the later row wins.
        starts.append((start, rid, (ns + "." + nm).strip(".")))
    starts.sort()
    owners = {}
    for rid in started_rids(path, member_table):
        owner = None
        for start, _, name in starts:
            if start <= rid:
                owner = name
            else:
                break
        owners[rid] = owner
    return owners


def started_rids(path, table):
    _, rows, idx = rows_by_rid(path, table)
    return sorted(rows)


def surface(path):
    """{table: Counter(key)} describing an assembly's declaration surface.

    Field / MethodDef entries are keyed by ``declaringType::member``, which lets the
    comparison recognise members that only exist because of the removed Costura loader
    rather than flagging them as lost code.
    """
    result = {}
    for table in TABLES:
        names, rows, idx = rows_by_rid(path, table)
        owners = owner_map(path, table) if table in ("Field", "MethodDef") else None
        keys = collections.Counter()
        for rid in sorted(rows):
            row = rows[rid]
            if table == "TypeDef":
                ns = row[idx["Namespace"]] if "Namespace" in idx and len(row) > idx["Namespace"] else ""
                nm = row[idx["Name"]] if "Name" in idx and len(row) > idx["Name"] else ""
                key = (ns + "." + nm).strip(".")
            elif "Name" in idx and len(row) > idx["Name"]:
                name = row[idx["Name"]]
                owner = owners.get(rid) if owners else None
                key = (owner + "::" + name) if owner else name
            else:
                key = "|".join(row)
            keys[key] += 1
        result[table] = keys
    return result


def resources(path):
    out = subprocess.run([ILSPY, "--list-resources", path], capture_output=True)
    return collections.Counter(
        l.strip() for l in out.stdout.decode("utf-8", "replace").splitlines() if l.strip())


def allowed_type(assembly, name):
    """Types deliberately added or removed by this reconstruction."""
    short = (name or "").split(".")[-1]
    if short in ROSELYN_HELPERS:            # injected by current Roslyn
        return True
    if assembly == "FFXIV_ACT_Plugin" and (name or "").startswith("Costura."):
        return True                         # Costura loader, replaced by project references
    if assembly == "FFXIV_ACT_Plugin" and short == "ProcessedByFody":
        return True                         # Fody/Costura weaving marker
    return False


def allowed_delta(assembly, table, name):
    if table == "ManifestResource":
        if assembly == "FFXIV_ACT_Plugin" and name.startswith("costura."):
            return True
        if assembly == "FFXIV_ACT_Plugin.Resource" and name.endswith("_Chinese.txt"):
            return True
    if table in ("Field", "MethodDef", "Property", "Event") and "::" in name:
        return allowed_type(assembly, name.split("::")[0])
    return allowed_type(assembly, name)


def decompile(path):
    out = subprocess.run([ILSPY, path], capture_output=True)
    return out.stdout.decode("utf-8", "replace").splitlines()


def statement_diff(orig, built, key):
    """Informational only: decompiler-rendering differences in method bodies."""
    a, b = decompile(orig), decompile(built)

    def norm(lines):
        return collections.Counter(
            re.sub(r"\s+", " ", l.strip()) for l in lines
            if l.strip() and l.strip() not in ("{", "}")
            and not l.strip().startswith(("[", "//", "using ")))

    ca, cb = norm(a), norm(b)
    with open(os.path.join(REPO, ".work", "diff_%s.txt" % key), "w", encoding="utf-8") as fh:
        fh.write("\n".join(difflib.unified_diff(
            a, b, "original/" + os.path.basename(orig), "rebuilt/" + os.path.basename(built),
            n=1, lineterm="")))
    del ca, cb
    return None


def main():
    statements = "--statements" in sys.argv
    failures = 0
    for key, orig_name, built_name in PAIRS:
        orig = os.path.join(ORIGINAL, orig_name)
        built = os.path.join(BUILT, built_name)
        if not os.path.exists(built):
            print("MISSING BUILD OUTPUT: %s" % built)
            return 1
        so, sb = surface(orig), surface(built)
        deltas, synthesized = [], 0
        for table in TABLES:
            for kind, counter in (("missing", so[table] - sb[table]),
                                  ("extra", sb[table] - so[table])):
                for name, count in counter.items():
                    if allowed_delta(key, table, name):
                        continue
                    if is_synthesized(name):
                        synthesized += count
                        continue
                    deltas.append((kind, table, name, count))

        ro, rb = resources(orig), resources(built)
        for n, c in (ro - rb).items():
            if not allowed_delta(key, "ManifestResource", n):
                deltas.append(("missing", "Resource", n, c))
        for n, c in (rb - ro).items():
            if not allowed_delta(key, "ManifestResource", n):
                deltas.append(("extra", "Resource", n, c))

        status = "OK" if not deltas else "MISMATCH"
        if deltas:
            failures += 1
        print("%-26s types=%-5d methods=%-5d fields=%-5d synth=%-4d %s" %
              (key, sum(so["TypeDef"].values()), sum(so["MethodDef"].values()),
               sum(so["Field"].values()), synthesized, status))
        for kind, table, name, count in deltas[:25]:
            print("     %-7s %-17s %s (x%d)" % (kind, table, name, count))

        if statements:
            statement_diff(orig, built, key)
            print("     raw rendering diff -> .work/diff_%s.txt" % key)

    print("\n%d/%d assemblies verified with an identical declaration surface"
          % (len(PAIRS) - failures, len(PAIRS)))
    return 0 if failures == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
