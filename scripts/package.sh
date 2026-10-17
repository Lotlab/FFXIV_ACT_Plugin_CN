#!/usr/bin/env bash
# Builds the release archive the same way the reference release was packaged:
#   FFXIV_ACT_Plugin.dll plus the deucalion distributables that ship next to it.
#
# Usage:  scripts/package.sh [output-dir]
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT="${1:-$ROOT/artifacts}"
CONFIG="${CONFIG:-Release}"

PLUGIN_DLL="$ROOT/src/FFXIV_ACT_Plugin/bin/$CONFIG/FFXIV_ACT_Plugin.dll"
if [ ! -f "$PLUGIN_DLL" ]; then
  echo "missing $PLUGIN_DLL" >&2
  echo "run: dotnet build -c $CONFIG \"$ROOT/FFXIV_ACT_Plugin_CN.sln\"" >&2
  exit 1
fi

VERSION="$(sed -nE 's/.*AssemblyVersion\("([^"]+)"\).*/\1/p' \
  "$ROOT/src/FFXIV_ACT_Plugin/Properties/AssemblyInfo.cs")"

STAGE="$(mktemp -d)"
trap 'rm -rf "$STAGE"' EXIT

cp "$PLUGIN_DLL" "$STAGE/FFXIV_ACT_Plugin.dll"

# deucalion ships as a versioned distributable next to the plugin; the file name
# pattern is what ACT_PluginUpdate.FinalizeDeucalionDistrib() and
# DeucalionInjector look for.
shopt -s nullglob
for src in "$ROOT"/third_party/machina/Machina.FFXIV/Deucalion/Distrib/*.dll; do
  base="$(basename "$src")"                 # deucalion-1.5.0.dll
  cp "$src" "$STAGE/${base%.dll}.distrib.dll"
done

mkdir -p "$OUT"
ARCHIVE="$OUT/FFXIV_ACT_Plugin_${VERSION}_CN.zip"
rm -f "$ARCHIVE"
(cd "$STAGE" && zip -q -9 "$ARCHIVE" ./*)
echo "$ARCHIVE"
