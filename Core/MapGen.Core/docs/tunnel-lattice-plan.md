# Goal

Match the organic, meandering cave‑tunnel look of the reference tile while staying seamless and keeping the code portable in C#.

The current tunnel generator shows linear banding and mostly straight segments. This document outlines a concrete plan to evolve `TunnelLatticeGenerator` into the target aesthetic in four phases, with code stubs and test criteria.

---

## Why the current output looks wrong

1. Radius modulation = directional stripes. `NoiseToroidal(u,v)` used sines that create axis‑aligned bands.
2. Edges are straight capsules. Straight segments read as pipes, not caves.
3. No chambers / bulges. Real caves have pockets.
4. Single‑scale shading. Lacks soft occlusion, rim lights, micro‑texture.

---

## Phase 1 — Kill the bands (tileable isotropic noise)

- Replace trig noise with tileable, isotropic value noise (fBm 2–3 octaves).
- Parameters added: `noiseOctaves`, `noiseScale`.
- Status: Implemented in `MapGen.Core/TunnelLatticeGenerator.cs`.

Test: FFT anisotropy ratio < 1.5.

---

## Phase 2 — Make tunnels meander (curl‑noise polylines) ✅

- ✅ Convert each MST edge into a curvy polyline by advecting points through a tileable curl noise field.
- ✅ Replace single segment SDF with capsule‑chain union along the polyline.
- ✅ Parameters: `curveSteps=16`, `curviness=1.0` multiplier.
- Status: **Implemented** in v0.3.0

Key improvements:

- Tunnels now curve and meander instead of being straight lines
- Higher node count (20) and extra loops (10) for denser networks
- Smaller radius (12.0) for more detailed tunnels
- Better contrast with adjusted shading parameters

---

## Phase 3 — Chambers & branching control

- Add Poisson‑disk‑like chambers and union circular/elliptical SDFs.
- Replace global shortest extra edges with k‑nearest (k=1–2) per node and clamp max length.

Params: `chamberDensity`, `roomSizeRange`.

---

## Phase 4 — Shading pass (depth, rim, micro‑texture)

- Compute SDF normals for soft rim light, add micro‑grain only inside tunnels, and ambient occlusion from SDF gradient.

Pseudo:

```
inside = d < 0
base = lerp(OutsideBase, OutsideBase+OutsideRange, clamp(d/baseR,0,1)) if d>=0
base = lerp(InsideBase+InsideRange, InsideBase, clamp((-d)/baseR,0,1)) if d<0
rim  = saturate(0.5 + 0.5*dot(normal, lightDir)) * inside
grain= tileable_noise(u,v, small_scale) * inside
final = base - 12*AO + 8*rim + 6*grain
```

Light direction ≈ normalize((-0.6, -0.4)).

---

## Parameter preset to match reference

- nodeCount: 16–22
- extraLoops: 8–12 (k‑nearest)
- baseRadius: 22–30
- radiusNoiseAmp: 0.10–0.18
- curviness: 0.8–1.5
- chamberDensity: 12–20 per 512×512, roomSizeRange: [1.1×, 1.8×] baseRadius
- noiseOctaves: 3, noiseScale: 0.8–1.2

---

## Test checklist (automatable)

1. Seam: edge equality holds.
2. Determinism: same seed → identical; different seed → >1% pixels differ.
3. No banding: FFT anisotropy < 1.5.
4. Curvature: avg turning angle > threshold (e.g., >10° per 64px).
5. Fill ratio: inside pixels between 25–45% of tile.
