#!/usr/bin/env bash
set -euo pipefail

# MapGen cleanup tool
# - Moves analysis docs and example assets into docs/ and examples/ folders
# - Removes ephemeral test-output folders
# Usage:
#   ./scripts/cleanup.sh            # archive only (non-destructive)
#   ./scripts/cleanup.sh --delete   # also delete ephemeral outputs

ROOT_DIR=$(cd "$(dirname "$0")/.." && pwd)
ARCHIVE_DIR="$ROOT_DIR/docs/archive"
ASSETS_DIR="$ROOT_DIR/docs/assets"
SERVICE_EXAMPLES_DIR="$ROOT_DIR/Service/examples"

mkdir -p "$ARCHIVE_DIR" "$ASSETS_DIR" "$SERVICE_EXAMPLES_DIR"

archive_file() {
  local src="$1"; local dst_dir="$2"
  if [ -e "$src" ]; then
    mkdir -p "$dst_dir"
    echo "Archiving $src -> $dst_dir/"
    mv "$src" "$dst_dir/" 2>/dev/null || rsync -a --remove-source-files "$src" "$dst_dir/"
  fi
}

archive_dir() {
  local src="$1"; local dst="$2"
  if [ -d "$src" ]; then
    echo "Archiving dir $src -> $dst/"
    mkdir -p "$dst"
    rsync -a --remove-source-files "$src"/ "$dst"/
    rmdir "$src" 2>/dev/null || true
  fi
}

# 1) Archive analysis docs at repo root
for f in \
  "$ROOT_DIR/CAVITY_MAP_ANALYSIS.md" \
  "$ROOT_DIR/COMPARISON_RESULTS.md" \
  "$ROOT_DIR/FINAL_COMPARISON_SUMMARY.md" \
  "$ROOT_DIR/FINAL_IMPLEMENTATION_SUMMARY.md" \
  "$ROOT_DIR/ITERATION_ANALYSIS.md" \
  "$ROOT_DIR/EXECUTION_PLAN.md" \
  "$ROOT_DIR/IMPLEMENTATION_GUIDE.md"
 do
  archive_file "$f" "$ARCHIVE_DIR"
 done

# 2) Archive comparison images
archive_dir "$ROOT_DIR/iteration_comparisons" "$ARCHIVE_DIR/iteration_comparisons"

# 3) Move example assets to docs/assets
for f in \
  "$ROOT_DIR/example-cavity-map.png" \
  "$ROOT_DIR/reference_quality_map.json"
 do
  archive_file "$f" "$ASSETS_DIR"
 done

# 4) Service sample outputs to Service/examples
for f in \
  "$ROOT_DIR/Service/response_iteration1.json" \
  "$ROOT_DIR/Service/response_iteration2.json" \
  "$ROOT_DIR/Service/test_iteration1.png" \
  "$ROOT_DIR/Service/test_iteration2.png" \
  "$ROOT_DIR/Service/test_iteration1_fixed.png" \
  "$ROOT_DIR/Service/test_iteration2_fixed.png" \
  "$ROOT_DIR/Service/reference_quality_map.*"
 do
  for m in $f; do
    [ -e "$m" ] && archive_file "$m" "$SERVICE_EXAMPLES_DIR"
  done
 done

# 5) Delete ephemeral folders if --delete
if [[ "${1:-}" == "--delete" ]]; then
  echo "Deleting ephemeral outputs..."
  find "$ROOT_DIR" -maxdepth 1 -type d -name 'test-output-*' -print -exec rm -rf {} +
  rm -f "$ROOT_DIR/Service/service.log" || true
fi

echo "Cleanup complete. Archived at: $ARCHIVE_DIR and $SERVICE_EXAMPLES_DIR" 