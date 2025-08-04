#!/bin/bash

# MapGen Iteration Comparison Script
# Generates multiple test images with different parameters for comparison

echo "🎯 Generating MapGen Iteration Comparisons..."

# Create output directory
mkdir -p iteration_comparisons

# Iteration 1: Original parameters
echo "📊 Generating Iteration 1 (Original)..."
curl -X POST http://localhost:5042/api/map/generate \
  -H "Content-Type: application/json" \
  -d '{"width": 512, "height": 512, "algorithm": "organic-cavity", "parameters": {"cavityDensity": 0.4, "cavityDepth": 0.7, "wallThickness": 0.3}}' \
  > iteration_comparisons/response_original.json

# Iteration 2: Enhanced density
echo "📊 Generating Iteration 2 (Enhanced Density)..."
curl -X POST http://localhost:5042/api/map/generate \
  -H "Content-Type: application/json" \
  -d '{"width": 512, "height": 512, "algorithm": "organic-cavity", "parameters": {"cavityDensity": 0.6, "cavityDepth": 0.9, "wallThickness": 0.2, "organicVariation": 0.8, "branchingFactor": 0.9}}' \
  > iteration_comparisons/response_enhanced.json

# Iteration 3: Maximum connectivity
echo "📊 Generating Iteration 3 (Maximum Connectivity)..."
curl -X POST http://localhost:5042/api/map/generate \
  -H "Content-Type: application/json" \
  -d '{"width": 512, "height": 512, "algorithm": "organic-cavity", "parameters": {"cavityDensity": 0.7, "cavityDepth": 0.95, "wallThickness": 0.15, "organicVariation": 0.9, "branchingFactor": 0.95, "cavityConnectivity": 0.9, "wallComplexity": 0.7, "depthVariation": 0.8}}' \
  > iteration_comparisons/response_max_connectivity.json

# Iteration 4: Fine detail
echo "📊 Generating Iteration 4 (Fine Detail)..."
curl -X POST http://localhost:5042/api/map/generate \
  -H "Content-Type: application/json" \
  -d '{"width": 512, "height": 512, "algorithm": "organic-cavity", "parameters": {"cavityDensity": 0.5, "cavityDepth": 0.9, "wallThickness": 0.1, "organicVariation": 0.95, "branchingFactor": 0.98, "textureDetail": 0.9, "depthVariation": 0.9}}' \
  > iteration_comparisons/response_fine_detail.json

# Convert all responses to PNG images
echo "🖼️ Converting to PNG images..."

for file in iteration_comparisons/response_*.json; do
    basename=$(basename "$file" .json)
    echo "Converting $basename..."
    python3 -c "
import json, base64
data = json.load(open('$file'))
open('iteration_comparisons/${basename}.raw', 'wb').write(base64.b64decode(data['data']))
"
done

# Convert raw data to PNG
echo "🔄 Converting raw data to PNG..."
convert -size 512x512 -depth 8 RGBA:iteration_comparisons/response_original.raw iteration_comparisons/iteration_original.png
convert -size 512x512 -depth 8 RGBA:iteration_comparisons/response_enhanced.raw iteration_comparisons/iteration_enhanced.png
convert -size 512x512 -depth 8 RGBA:iteration_comparisons/response_max_connectivity.raw iteration_comparisons/iteration_max_connectivity.png
convert -size 512x512 -depth 8 RGBA:iteration_comparisons/response_fine_detail.raw iteration_comparisons/iteration_fine_detail.png

# Clean up raw files
rm iteration_comparisons/*.raw
rm iteration_comparisons/*.json

echo "✅ Iteration comparison complete!"
echo "📁 Images saved in: iteration_comparisons/"
echo "🖼️ Generated images:"
echo "  - iteration_original.png (Original parameters)"
echo "  - iteration_enhanced.png (Enhanced density)"
echo "  - iteration_max_connectivity.png (Maximum connectivity)"
echo "  - iteration_fine_detail.png (Fine detail)"

# Open the comparison folder
open iteration_comparisons/ 