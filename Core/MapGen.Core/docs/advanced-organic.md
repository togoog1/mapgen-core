# Advanced Organic Generator (advanced-organic)

Organic, high-detail depth map generator using layered growth, noise, and smoothing.

Version: 1.0.0

Parameters (key)

- cavityDensity, cavityDepth, wallThickness
- organicVariation, branchingFactor, textureDetail
- fractalOctaves, noiseScale, voronoiPoints
- smoothingPasses, depthContrast, cavityConnectivity

Notes

- Output is RGBA with grayscale depth; darker cavities, lighter walls.
- For reduced speckle, raise smoothingPasses and lower textureDetail.

Tests

- Determinism by seed
- Fill ratio sanity (config dependent)

File: `Core/MapGen.Core/AdvancedOrganicGenerator.cs`
