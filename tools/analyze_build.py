"""Summarize MSBuild/dotnet build output: group errors and warnings."""
import collections
import re
import sys

path = sys.argv[1] if len(sys.argv) > 1 else "build.log"
limit = int(sys.argv[2]) if len(sys.argv) > 2 else 40

lines = open(path, encoding="utf-8", errors="replace").read().splitlines()
errs = [l for l in lines if ": error " in l]
warns = [l for l in lines if ": warning " in l]
print("errors", len(errs), "warnings", len(warns))

print("--- top messages ---")
c = collections.Counter(re.sub(r"^.*: error ", "", l).split("[")[0].strip()[:110] for l in errs)
for k, v in c.most_common(limit):
    print(v, "|", k)

print("--- by file ---")
c2 = collections.Counter(l.split("(")[0].split("\\")[-1] for l in errs)
for k, v in c2.most_common(limit):
    print(v, "|", k)

if warns:
    print("--- warnings ---")
    c3 = collections.Counter(re.sub(r"^.*: warning ", "", l).split("[")[0].strip()[:110] for l in warns)
    for k, v in c3.most_common(limit):
        print(v, "|", k)
