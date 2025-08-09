# Organic Cavity Generator (organic-cavity)

Cellular-automata-based cavities with organic growth and texture.

Version: 1.0.0

Parameters

- iterations, birthThreshold, survivalThreshold
- initialDensity, organicGrowth, cavityDepth, wallThickness
- textureDetail, smoothingPasses

Tips

- Lower initialDensity and raise iterations for larger, fewer caves.
- Tune birth/survival thresholds to control connectivity.

Tests: seam N/A, determinism by seed, fill ratio bounds.

File: `Core/MapGen.Core/Class1.cs` (class `OrganicCavityGenerator`)
