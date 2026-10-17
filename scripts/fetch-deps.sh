#!/usr/bin/env bash
# Fetches the build-time dependencies that cannot be redistributed from this
# repository.
#
#   lib/Advanced Combat Tracker.exe   host application, referenced by
#                                     FFXIV_ACT_Plugin.Common and the plugin
#                                     project itself.
#
# Usage:  scripts/fetch-deps.sh
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LIB="$ROOT/lib"
ACT_VERSION="${ACT_VERSION:-3.8.5.288}"
ACT_URL="https://github.com/EQAditu/AdvancedCombatTracker/releases/download/${ACT_VERSION}/ACTv3.zip"

mkdir -p "$LIB"
TARGET="$LIB/Advanced Combat Tracker.exe"

if [ -f "$TARGET" ]; then
  echo "already present: $TARGET"
else
  echo "downloading Advanced Combat Tracker ${ACT_VERSION} ..."
  tmp="$(mktemp -d)"
  trap 'rm -rf "$tmp"' EXIT
  curl -fsSL -o "$tmp/ACTv3.zip" "$ACT_URL"
  unzip -o -q "$tmp/ACTv3.zip" -d "$tmp/act"
  cp "$tmp/act/Advanced Combat Tracker.exe" "$TARGET"
  echo "wrote $TARGET"
fi
