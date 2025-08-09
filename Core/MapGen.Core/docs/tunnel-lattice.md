# Tunnel Lattice Generator (tunnel-lattice)

Produces a seamless, loopy tunnel pattern using a toroidal SDF built from a small graph. Shaded to 8‑bit grayscale and emitted as RGBA in the service.

Status

- Version: 0.3.0
- Phase 1 implemented: isotropic tileable noise for radius modulation.
- Phase 2 implemented: curvy polylines using curl noise for meandering tunnels.

Parameters

- nodeCount (int): default 20 — number of graph nodes
- extraLoops (int): default 10 — extra edges after MST
- baseRadius (double): default 12.0 — tunnel thickness baseline (px)
- radiusNoiseAmp (double): default 0.15 — variation amplitude
- noiseOctaves (int): default 4 — fBm octaves (tileable value noise)
- noiseScale (double): default 1.5 — fBm base frequency
- curviness (double): default 1.0 — curl noise deviation strength
- curveSteps (int): default 16 — polyline segments per edge
- insideBase, insideRange, outsideBase, outsideRange (bytes): shading ramps

Artifacts & Tuning

- If bands appear, reduce noiseScale or increase noiseOctaves.
- Increase nodeCount and extraLoops for denser networks; reduce for sparse flow.
- For finer tunnels, lower baseRadius proportionally to output size.

Roadmap

- ✅ Phase 2: Curvy polylines via curl noise; capsule chain SDF.
- Phase 3: Chambers (rooms) + k‑nearest branching with length clamp.
- Phase 4: SDF normal shading (rim), micro‑grain, AO.

Tests

- Seam: wrap edges equal within 1 gray level.
- Determinism: same seed → identical output; reroll → >1% pixels differ.
- No banding: FFT anisotropy < 1.5 (target).
- Fill ratio: inside pixels 25–45%.

References

- Implementation: `Core/MapGen.Core/TunnelLatticeGenerator.cs`
- Design plan: `Core/TUNNEL_LATTICE_PLAN.md`
