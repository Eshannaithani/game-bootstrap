#!/bin/bash
# Batch-mode import and compile of a Unity project that is NOT open in an editor.
# Usage: compile_check.sh <project> [unity-version]        -> exit 0 when no "error CS" in the log
#        compile_check.sh --create <project> [unity-version] -> create an empty project first
# Why: a scaffold that does not compile is worse than none, and a closed project cannot be checked through MCP.
set -u
CREATE=0
if [ "${1:-}" = "--create" ]; then CREATE=1; shift; fi
PROJECT="${1:?project path}"
VERSION="${2:-}"
if [ -n "$VERSION" ]; then UNITY="/Applications/Unity/$VERSION/Unity.app/Contents/MacOS/Unity"; else UNITY=$(ls -d /Applications/Unity/6*/Unity.app/Contents/MacOS/Unity 2>/dev/null | sort -V | tail -1); fi
[ -x "$UNITY" ] || { echo "no Unity editor found (looked in /Applications/Unity)"; exit 2; }
LOG="$PROJECT/compile_check.log"
if [ "$CREATE" = 1 ]; then
  "$UNITY" -batchmode -nographics -createProject "$PROJECT" -quit -logFile "$PROJECT.create.log" || { echo "create failed, see $PROJECT.create.log"; exit 3; }
fi
"$UNITY" -batchmode -nographics -quit -projectPath "$PROJECT" -logFile "$LOG"
if grep -E "error CS[0-9]+" "$LOG" >/dev/null; then
  echo "COMPILE ERRORS:"; grep -E "error CS[0-9]+" "$LOG" | sort -u | head -40; exit 1
fi
echo "compile OK ($(grep -c 'Compilation' "$LOG" 2>/dev/null || echo 0) compilation lines, log $LOG)"
