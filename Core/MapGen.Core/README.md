# MapGen.Core

Core library containing map generation algorithms and a registry service.

## Generators

Implemented algorithms and docs:

- See `docs/README.md` for per-generator pages.

To add a new generator:

1. Implement `IMapGenerator` in `MapGen.Core`.
2. Register it in `MapGenerationService` constructor with a unique algorithm key.
3. Provide `DefaultParameters`, a semantic `AlgorithmName`, and a `Version` string.
4. Add a doc page under `Core/MapGen.Core/docs/` and link it from `docs/README.md`.

## Service Integration

The `Service/MapGen.Service` layer exposes:

- `GET /api/map/algorithms` (name, version, defaults)
- `POST /api/map/generate` (returns base64 RGBA bytes)

## Tests

See `Core/MapGen.Core.Tests` for examples and image comparisons.
