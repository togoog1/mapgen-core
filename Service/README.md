# MapGen.Service

ASP.NET Core Web API exposing generator algorithms.

- Default URL: `http://localhost:5023`
- Swagger: `/swagger`

Endpoints

- `GET /api/map/health` — basic health
- `GET /api/map/algorithms` — list algorithms with version and defaults
- `POST /api/map/generate` — generate with random seed
- `POST /api/map/generate/{seed}` — generate with provided seed

Output

- `format: "raw"` — base64-encoded RGBA bytes; width/height come from request

Development

- Requires .NET 9 SDK
- See top-level `start-all.sh` to run Core tests + Service + Viewer.

Docs

- Root docs: `../../docs/README.md`
- Core docs: `../../Core/MapGen.Core/docs/README.md`
