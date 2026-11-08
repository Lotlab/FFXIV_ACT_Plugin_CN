#!/usr/bin/env python3
"""Generate the CN resource payloads in ``cn/resource`` from an ffxiv-datamining-cn dump.

The CN build of the plugin differs from the upstream build only in the payloads embedded
in ``FFXIV_ACT_Plugin.Resource``.  Until now those payloads were lifted out of a
TundraWork CN release (see ``tools/refresh_cn_resources.py``), which made every new CN
client patch depend on a third party shipping a new binary.  Everything the Resource
assembly actually reads is either plain text derived from the game sheets or
language-independent data that already comes from upstream, so the payloads can be
produced locally from ``ffxiv-datamining-cn`` CSVs instead.

What is generated (see ``PAYLOADS`` below for the exact rules):

==========================================  ==========================  ==============
payload                                     source sheet(s)             key / value
==========================================  ==========================  ==============
Generated.ActionList_{Chinese,English}.txt  Action                      hex / Name
Generated.StatusList_{Chinese,English}.txt  Status                      hex / Name
Generated.MapList_English.txt               Map + PlaceName             hex / 3 fields
Generated.TerritoryList_English.txt         TerritoryType + PlaceName   dec / Name
Generated.ContentFinderList_English.txt     ContentFinderCondition      dec / Name
Generated.WorldList_English.txt             World + WorldDCGroupType    dec / Name
Generated.ActionCategoryList_English.txt    Action                      hex / category
Definitions.*.json                          Action + Status (names)     hex / name
==========================================  ==========================  ==============

What is *not* generated, and why:

* ``Generated.Override.global_*.txt`` - the ``_rsv_<id>_<lang>_...`` entries supply the
  plaintext for sheet names that are themselves obfuscated string tokens (the client
  resolves those from its string table at runtime, so the dump only has the token).
  ``ResourceFile.LoadResourceDictionary`` matches an override *key* against an entry
  *name* and substitutes the value, so these files are version bound: they matter only
  while the sheet holds tokens for the ids they cover.  The CSV dump cannot produce them,
  so the 8 CN payloads (the localised P5S-P12S set) are kept as-is; see README 3.5.
* ``Generated.ItemActionList.txt`` - 9 hand-curated action ids (``f540a`` ...) that do
  not exist in the Action sheet.
* ``Generated.ItemList_English.txt`` / ``Generated.MountList_English.txt`` - upstream
  ships them as a single ``0|`` placeholder and no code reads them.
* ``Definitions.*.json`` potencies / combos / status-effect multipliers - the Action
  sheet has no potency column any more, so that data stays hand-maintained; only the
  names inside those files are re-localised from the CSV.

Usage::

    # write cn/resource from a CSV dump
    python tools/generate_cn_resources.py --csv ../ffxiv-automatic-data/csv

    # only report what would change, and diff against a known-good payload set
    python tools/generate_cn_resources.py --csv ../ffxiv-automatic-data/csv --check

The dump is expected to be a checkout of https://github.com/thewakingsands/ffxiv-datamining-cn
(``csv/`` directory) or any copy of it; only the sheets listed above are read.
"""
import argparse
import csv
import json
import os
import re
import sys

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
CN_RESOURCE = os.path.join(REPO, "cn", "resource")
DEFAULT_CHECK_DIR = os.path.join(REPO, ".work", "cn-resources", "cn")
DEFAULT_CSV = os.path.abspath(os.path.join(REPO, "..", "ffxiv-automatic-data", "csv"))
DEFAULT_OVERRIDES = os.path.join(REPO, "cn", "resource-overrides.json")

PREFIX = "FFXIV_ACT_Plugin.Resource."

# ContentFinderCondition.ContentType values that never belong in a territory-name table:
# 6 对战 (PvP), 9 寻宝, 16 大地使者, 19 金碟游乐场, 20 初学者学堂, 22 多变迷宫,
# 23 无人岛/新月岛探索, 27, 40.  Battle content (2 迷宫, 3 行会令, 4 讨伐, 5 大型,
# 7 任务战斗, 21 深层迷宫, 26 优雷卡, 28-30/37-39 newer types) is kept.
EXCLUDED_CONTENT_TYPES = {6, 9, 16, 19, 20, 22, 23, 27, 40}

# Worlds below this id are the legacy "test"/pre-launch CN worlds (s-bella, 陆行鸟, ...);
# the live CN worlds start at 1042.
WORLD_ID_FLOOR = 1000

# Column names in the CSV header row; every sheet starts with ``key,0,1,...`` followed by
# a name row, a type row and the data rows.
KEY_COLUMN = "#"


class Sheet:
    """One ``*.csv`` sheet of an ffxiv-datamining-cn dump."""

    def __init__(self, directory, name):
        path = os.path.join(directory, name + ".csv")
        if not os.path.exists(path):
            raise SystemExit("missing sheet: %s" % path)
        with open(path, encoding="utf-8-sig", newline="") as handle:
            rows = list(csv.reader(handle))
        if len(rows) < 3:
            raise SystemExit("sheet %s has no data rows" % name)
        self.name = name
        self.names = rows[1]
        self.types = rows[2]
        self.rows = [row for row in rows[3:] if row and row[0] != ""]
        self._index = {}
        for position, column in enumerate(self.names):
            if column and column not in self._index:
                self._index[column] = position

    def column(self, name):
        if name not in self._index:
            raise SystemExit("sheet %s has no column %r (have: %s)"
                             % (self.name, name, ", ".join(c for c in self.names if c)))
        return self._index[name]

    def text(self, row, name):
        return row[self.column(name)]

    def integer(self, row, name):
        value = self.text(row, name).strip()
        if not value:
            return 0
        return int(float(value))

    def key(self, row):
        return int(float(row[0]))

    def by_key(self, name):
        return {self.key(row): self.text(row, name) for row in self.rows}


# --- payload writers --------------------------------------------------------

def format_lines(entries):
    """Render ``[(key, [field, ...]), ...]`` the way the Resource assembly expects.

    CRLF separated, no BOM, keys already formatted by the caller.
    """
    return "".join("%s|%s\r\n" % (key, "|".join(fields)) for key, fields in entries)


def write_payload(directory, payload, content):
    """Write one payload as UTF-8 without BOM, CRLF line endings."""
    with open(os.path.join(directory, PREFIX + payload), "wb") as handle:
        handle.write(content.encode("utf-8"))


def hex_key(value):
    return format(value, "x")


# --- generators -------------------------------------------------------------

def generate_name_list(sheet, value_column="Name"):
    """``ActionList_*`` / ``StatusList_*``: every row that has a display name."""
    entries = []
    for row in sheet.rows:
        name = sheet.text(row, value_column)
        if not name.strip():
            continue
        entries.append((hex_key(sheet.key(row)), [name]))
    entries.sort(key=lambda item: int(item[0], 16))
    return entries


def generate_map_list(maps, places):
    entries = []
    for row in maps.rows:
        if maps.key(row) == 0:
            # row 0 is the empty sheet default and is not a map
            continue
        entries.append((hex_key(maps.key(row)),
                        [places.get(maps.integer(row, "PlaceName{Region}"), ""),
                         places.get(maps.integer(row, "PlaceName"), ""),
                         places.get(maps.integer(row, "PlaceName{Sub}"), "")]))
    entries.sort(key=lambda item: int(item[0], 16))
    return entries


def generate_territory_list(territories, places):
    entries = []
    for row in territories.rows:
        name = places.get(territories.integer(row, "PlaceName"), "")
        if not name.strip():
            continue
        entries.append((str(territories.key(row)), [name]))
    entries.sort(key=lambda item: int(item[0]))
    return entries


def generate_content_finder_list(conditions):
    """``ContentFinderList_*``: keyed by *territory*, not by ContentFinderCondition id.

    ``TerritoryList`` is merged on top of this table at runtime, so a duty territory gets
    the duty name and everything else keeps its place name.  Several conditions can share
    a territory (deep-dungeon floors, Crystalline Conflict stages); the lowest id wins.
    """
    entries = {}
    for row in sorted(conditions.rows, key=conditions.key):
        territory = conditions.integer(row, "TerritoryType")
        if not territory or territory in entries:
            continue
        name = conditions.text(row, "Name")
        if not name.strip():
            continue
        if conditions.text(row, "PvP").strip().lower() == "true":
            continue
        if conditions.integer(row, "ContentType") in EXCLUDED_CONTENT_TYPES:
            continue
        entries[territory] = name
    return [(str(key), [name]) for key, name in sorted(entries.items())]


def generate_world_list(worlds, dc_groups):
    """``WorldList_*``: only the worlds that belong to a CN region.

    ``World.Region`` holds the ``WorldRegionGroup`` id; the four CN groups (陆行鸟, 莫古力,
    猫小胖, 豆豆柴) are the ones whose ``WorldDCGroupType.Name`` is not latin.  Worlds
    below :data:`WORLD_ID_FLOOR` are the legacy pre-launch rows (``s-bella``, 陆行鸟, ...).
    """
    cn_regions = {dc_groups.key(row) for row in dc_groups.rows
                  if dc_groups.text(row, "Name").strip()
                  and re.search(r"[\u3400-\u9fff]", dc_groups.text(row, "Name"))}
    entries = []
    for row in worlds.rows:
        if worlds.integer(row, "Region") not in cn_regions:
            continue
        if worlds.key(row) < WORLD_ID_FLOOR:
            continue
        name = worlds.text(row, "Name")
        if not name.strip():
            continue
        entries.append((str(worlds.key(row)), [name]))
    entries.sort(key=lambda item: int(item[0]))
    return entries


def generate_action_category_list(actions):
    """``ActionCategoryList_*``: every action row, value written as a decimal string.

    ``ActionList.LoadActionCategories`` parses the value with
    ``int.Parse(value, NumberStyles.HexNumber)``, so the payload is *not* hex - upstream
    writes the raw byte and the parser misreads anything above 9.  Reproduced verbatim so
    the generated data stays behaviour-compatible.
    """
    entries = []
    for row in actions.rows:
        entries.append((hex_key(actions.key(row)), [str(actions.integer(row, "ActionCategory"))]))
    entries.sort(key=lambda item: int(item[0], 16))
    return entries


DEFINITION_KEY = re.compile(r'^(\s*")([0-9A-Fa-f]+)(" *: *")([^"]*)(")')


def localise_definitions(source_path, action_names, status_names):
    """Rewrite the name of every ``"<hex id>": "<name>"`` pair in a job definition.

    The surrounding text (potencies, combos, status-effect effects) is preserved byte for
    byte; only the display names are replaced, so the hand-maintained numbers stay
    authoritative.  Section state is tracked so that ``actions`` use the Action sheet and
    ``statuseffects`` the Status sheet.
    """
    with open(source_path, encoding="utf-8-sig", newline="") as handle:
        text = handle.read()
    out = []
    section = None
    changed = 0
    for line in text.splitlines(keepends=True):
        stripped = line.lstrip()
        if stripped.startswith('"actions"'):
            section = "actions"
        elif stripped.startswith('"statuseffects"'):
            section = "statuseffects"
        match = DEFINITION_KEY.match(line)
        if match and section:
            identifier = int(match.group(2), 16)
            table = action_names if section == "actions" else status_names
            replacement = table.get(identifier)
            if replacement and replacement != match.group(4):
                line = line[:match.start(4)] + replacement + line[match.end(4):]
                changed += 1
        out.append(line)
    return "".join(out), changed


# --- driver -----------------------------------------------------------------

def build(csv_dir, definitions_dir, mirror_english):
    """Return ``{payload name: bytes}`` for everything that can be generated."""
    actions = Sheet(csv_dir, "Action")
    statuses = Sheet(csv_dir, "Status")
    places = Sheet(csv_dir, "PlaceName").by_key("Name")
    maps = Sheet(csv_dir, "Map")
    territories = Sheet(csv_dir, "TerritoryType")
    conditions = Sheet(csv_dir, "ContentFinderCondition")
    worlds = Sheet(csv_dir, "World")
    dc_groups = Sheet(csv_dir, "WorldDCGroupType")

    action_names = actions.by_key("Name")
    status_names = statuses.by_key("Name")

    payloads = {}
    action_list = format_lines(generate_name_list(actions))
    status_list = format_lines(generate_name_list(statuses))
    payloads["Generated.ActionList_Chinese.txt"] = action_list
    payloads["Generated.StatusList_Chinese.txt"] = status_list
    if mirror_english:
        # The CN release ships the Chinese names in the English slots too; keeping that
        # makes this a drop-in replacement.  Without it the upstream English payloads stay
        # in place and an English UI keeps showing English names.
        payloads["Generated.ActionList_English.txt"] = action_list
        payloads["Generated.StatusList_English.txt"] = status_list
    payloads["Generated.MapList_English.txt"] = format_lines(generate_map_list(maps, places))
    payloads["Generated.TerritoryList_English.txt"] = format_lines(
        generate_territory_list(territories, places))
    payloads["Generated.ContentFinderList_English.txt"] = format_lines(
        generate_content_finder_list(conditions))
    payloads["Generated.WorldList_English.txt"] = format_lines(
        generate_world_list(worlds, dc_groups))
    payloads["Generated.ActionCategoryList_English.txt"] = format_lines(
        generate_action_category_list(actions))

    if definitions_dir and os.path.isdir(definitions_dir):
        for name in sorted(os.listdir(definitions_dir)):
            if not name.startswith(PREFIX + "Definitions.") or not name.endswith(".json"):
                continue
            content, _ = localise_definitions(
                os.path.join(definitions_dir, name), action_names, status_names)
            payloads[name[len(PREFIX):]] = content
    return payloads


def apply_overrides(payloads, overrides):
    """Apply the hand-maintained deltas in ``cn/resource-overrides.json``.

    The CSV dump is authoritative for everything it contains, but a handful of rows are
    only correct in the CN client: text the dump never translated, rows the client keeps
    although the sheet dropped them, and the odd name the client shows differently.  Those
    live in a small checked-in file instead of being hard-coded here, so the generator
    stays a pure function of the dump and every manual delta stays reviewable.

    Format::

        {"Generated.WorldList_English.txt": {"1168": "黄金港"},
         "Generated.MapList_English.txt": {"1": ["？？？？", "艾欧泽亚", ""]}}
    """
    if not overrides:
        return payloads, 0
    applied = 0
    for payload, entries in overrides.items():
        if payload.startswith("_") or payload == "comment":
            continue
        key = payload[len(PREFIX):] if payload.startswith(PREFIX) else payload
        if key not in payloads:
            raise SystemExit("override for unknown payload: %s" % payload)
        base, fields = SPECS.get(payload, (10, 1))
        current = parse_payload(payloads[key], base, fields)
        for raw_key, value in entries.items():
            if value is None:
                current.pop(int(raw_key, base), None)
            else:
                current[int(raw_key, base)] = tuple(value) if isinstance(value, list) else (value,)
            applied += 1
        payloads[key] = format_lines(
            [(format(k, "x") if base == 16 else str(k), list(v)) for k, v in sorted(current.items())])
    return payloads, applied


# key base and field count of each generated payload, used by --check to compare a
# generated file against the payloads extracted from a CN release.
SPECS = {
    "Generated.ActionList_Chinese.txt": (16, 1),
    "Generated.ActionList_English.txt": (16, 1),
    "Generated.StatusList_Chinese.txt": (16, 1),
    "Generated.StatusList_English.txt": (16, 1),
    "Generated.MapList_English.txt": (16, 3),
    "Generated.TerritoryList_English.txt": (10, 1),
    "Generated.ContentFinderList_English.txt": (10, 1),
    "Generated.WorldList_English.txt": (10, 1),
    "Generated.ActionCategoryList_English.txt": (16, 1),
}


def parse_payload(data, base, fields):
    """Parse a ``key|field...`` payload into ``{key: tuple(fields)}``."""
    if isinstance(data, bytes):
        data = data.decode("utf-8-sig")
    out = {}
    for line in re.split(r"\r\n|\n|\r", data):
        if not line or line.startswith("#"):
            continue
        parts = line.split("|")
        if len(parts) != fields + 1:
            continue
        out[int(parts[0], base)] = tuple(parts[1:])
    return out


UNICODE_ESCAPE = re.compile(r"\\u([0-9a-fA-F]{4})")


def unescape(text):
    return UNICODE_ESCAPE.sub(lambda match: chr(int(match.group(1), 16)), text)


def compare(payload, content, reference_bytes):
    """Return ``(entries, reference, same, drift, new, lost)`` for one payload."""
    if payload.endswith(".json"):
        # The CN release stores the names as \uXXXX escapes; compare decoded text so a
        # formatting difference is not reported as a data difference.
        generated = unescape(content).splitlines()
        reference = unescape(reference_bytes.decode("utf-8-sig")).splitlines()
        paired = list(zip(generated, reference))
        same = [i for i, (a, b) in enumerate(paired) if a == b]
        drift = [i for i, (a, b) in enumerate(paired) if a != b]
        new = list(range(len(reference), len(generated)))
        lost = list(range(len(generated), len(reference)))
        return generated, reference, same, drift, new, lost
    base, fields = SPECS[payload]
    entries = parse_payload(content, base, fields)
    reference = parse_payload(reference_bytes, base, fields)
    same = [k for k in entries if reference.get(k) == entries[k]]
    drift = [k for k in entries if k in reference and reference[k] != entries[k]]
    new = [k for k in entries if k not in reference]
    lost = [k for k in reference if k not in entries]
    return entries, reference, same, drift, new, lost


def report(payloads, check_dir, out_dir, dry_run):
    lost_total = 0
    print("%-46s %7s %7s %7s %7s %7s" %
          ("payload", "entries", "same", "drift", "new", "lost"))
    for payload in sorted(payloads):
        content = payloads[payload]
        reference_path = os.path.join(check_dir, PREFIX + payload) if check_dir else None
        if not reference_path or not os.path.exists(reference_path):
            print("%-46s %7s %7s %7s %7s %7s" %
                  (payload, "-", "-", "-", "-", "-"))
        else:
            with open(reference_path, "rb") as handle:
                entries, reference, same, drift, new, lost = compare(
                    payload, content, handle.read())
            if not payload.endswith(".json"):
                # An entry the reference lists with an empty name is one the CN client has
                # no text for; the generator drops those rows instead of emitting "id|".
                blank = [k for k in lost if not any(reference[k])]
                lost = [k for k in lost if k not in blank]
                if blank:
                    print("%-46s %7s %7s %7s %7s %7s  (dropped %d nameless rows)" %
                          (payload, "-", "-", "-", "-", "-", len(blank)))
            lost_total += len(lost)
            print("%-46s %7d %7d %7d %7d %7d" %
                  (payload, len(entries), len(same), len(drift), len(new), len(lost)))
            if not payload.endswith(".json"):
                for label, keys in (("lost", lost), ("drift", drift)):
                    for key in sorted(keys)[:5]:
                        print("    %s %s: csv=%r reference=%r" %
                              (label, key, entries.get(key), reference.get(key)))
        if not dry_run:
            write_payload(out_dir, payload, content)
    return lost_total


def main():
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--csv", default=DEFAULT_CSV,
                        help="ffxiv-datamining-cn csv directory (default: %(default)s)")
    parser.add_argument("--out", default=CN_RESOURCE,
                        help="payload output directory (default: cn/resource)")
    parser.add_argument("--definitions-from", default=CN_RESOURCE,
                        help="directory holding the Definitions.*.json whose names are "
                             "re-localised (default: cn/resource)")
    parser.add_argument("--check", nargs="?", const=DEFAULT_CHECK_DIR, default=None,
                        metavar="DIR",
                        help="compare against a payload set extracted from a CN release "
                             "(default: .work/cn-resources/cn)")
    parser.add_argument("--drop-english-lists", action="store_true",
                        help="do not mirror the Chinese names into the *_English.txt "
                             "action/status slots; the upstream English payloads are then "
                             "left in place (the CN release mirrors them, which makes an "
                             "English UI show Chinese names)")
    parser.add_argument("--overrides", default=DEFAULT_OVERRIDES,
                        help="hand-maintained deltas applied after generation "
                             "(default: cn/resource-overrides.json)")
    parser.add_argument("--dry-run", action="store_true",
                        help="report only, write nothing")
    args = parser.parse_args()

    if not os.path.isdir(args.csv):
        raise SystemExit("csv directory not found: %s" % args.csv)

    payloads = build(args.csv, args.definitions_from, not args.drop_english_lists)
    if args.overrides and os.path.exists(args.overrides):
        with open(args.overrides, encoding="utf-8") as handle:
            overrides = json.load(handle)
        payloads, applied = apply_overrides(payloads, overrides)
        print("applied %d override entries from %s\n"
              % (applied, os.path.relpath(args.overrides, REPO)))
    check_dir = args.check
    if check_dir == "":
        check_dir = DEFAULT_CHECK_DIR
    if args.drop_english_lists and not args.dry_run:
        for name in ("Generated.ActionList_English.txt", "Generated.StatusList_English.txt"):
            path = os.path.join(args.out, PREFIX + name)
            if os.path.exists(path):
                os.remove(path)
                print("removed %s so the upstream English payload is used"
                      % os.path.relpath(path, REPO))
    problems = report(payloads, check_dir, args.out, args.dry_run)
    if not args.dry_run:
        print("\nwrote %d payloads to %s" % (len(payloads), os.path.relpath(args.out, REPO)))
    if check_dir and problems:
        print("\n%d reference entries have no counterpart in the CSV dump" % problems)
        return 1
    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except KeyboardInterrupt:
        sys.exit(130)
