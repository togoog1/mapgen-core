# MapGen Playground

A single repository that lets you **design, test, and preview procedural‑map algorithms** as fast as you can type. Three logical projects live side‑by‑side so you never rewrite code while you iterate:

```
mapgen-core   ← .NET 9 class‑library with all generation algorithms
mapgen-service← ASP.NET Core hot‑reload backend that exposes the generators over HTTP
mapgen-viewer ← React + Vite front‑end playground that fetches maps and shows them instantly
```

> **Why one repo?** One clone → one PR covers library + API + UI → sub‑second dev loop.
>
> **Why three folders?** Clean boundaries. Replace any piece without touching the others.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Quick Start](#quick-start)
3. [Folder breakdown](#folder-breakdown)
4. [Development workflow](#development-workflow)
5. [Adding a new generator](#adding-a-new-generator)
6. [Running the test‑suite](#running-the-test-suite)
7. [Build & release](#build--release)
8. [Roadmap / nice‑to‑haves](#roadmap--nice-to-haves)

---

## Prerequisites

| Requirement       | Recommended version | Notes                                      |
| ----------------- | ------------------- | ------------------------------------------ |
| .NET SDK          | **9.0.100‑preview** | for hot‑reload IL deltas & AVX‑10 code‑gen |
| Node.js           | **20 LTS**          | Vite HMR & React Router 7                  |
| Git               | 2.40 +              |                                            |
| (optional) Docker | 24 +                | containerised service build                |

---

## Quick Start

### Option 1: One-Command Startup (Recommended)

```bash
# Clone the repository
git clone https://github.com/togoog1/mapgen-core.git
cd mapgen-core

# Start everything at once
./start-all.sh
```

The script will automatically:

- ✅ Check prerequisites (.NET 9, Node.js 20)
- ✅ Install dependencies (npm packages, restore .NET packages)
- ✅ Open three terminal windows/tabs with each service running
- ✅ Show you the URLs where everything is available

### Option 2: Manual Startup

If the automatic script doesn't work on your system:

```bash
# Terminal 1 – unit tests & library
cd Core && dotnet watch test

# Terminal 2 – backend API (hot‑reload)
cd Service/MapGen.Service && dotnet watch run --urls=http://localhost:5023

# Terminal 3 – front‑end playground
cd Viewer && npm run dev
```

### Service URLs

Once running, you'll have access to:

| Service        | URL                           | Purpose                   |
| -------------- | ----------------------------- | ------------------------- |
| **Viewer**     | http://localhost:5173         | Interactive playground UI |
| **Service**    | http://localhost:5023         | API endpoints (JSON)      |
| **API Docs**   | http://localhost:5023/swagger | Swagger documentation     |
| **Core Tests** | Terminal output               | Test results as you code  |

---

## Folder breakdown

| Path                         | What lives here                                                                                                                                                     | Key tech                                                          | Typical commands                       |
| ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------- | -------------------------------------- |
| **/Core/MapGen.Core/**       | All deterministic map‑generation code. Pure C#, no HTTP. Implements `IGenerator`.                                                                                   | .NET 9 class‑lib, `noise`, `System.Numerics.Vectors`, xUnit tests | `dotnet watch test`                    |
| **/Service/MapGen.Service/** | Thin ASP.NET Core host that **auto‑loads every** `IGenerator` at runtime and exposes:<br>• `GET /generators` — list names<br>• `POST /generate/{name}` — PNG output | ASP.NET Core Minimal API, Scrutor assembly‑scan                   | `dotnet watch run` (defaults to :5023) |
| **/Viewer/**                 | Hot‑reload playground UI with param sliders. Proxies requests to the service.                                                                                       | React 18, React Router 7, Vite, TypeScript                        | `npm run dev`                          |
| **readme.md**                | What you're reading.                                                                                                                                                | Markdown                                                          |                                        |

### Ports

- **5023** — ASP.NET service
- **5173** — Vite dev server (auto‑chosen by Vite)

---

## Development workflow

### Your First Map

1. **Start all services** using `./start-all.sh`
2. **Open the Viewer** at http://localhost:5173
3. **Choose a generator** from the dropdown (e.g., "Perlin")
4. **Adjust parameters** with the sliders
5. **Watch the map** update in real-time

### The Feedback Loop

```
Edit C# in Core → Tests run → Service rebuilds → Test in Viewer → Repeat
     ↑ Sub-second feedback loop for rapid iteration ↓
```

1. Edit **any C# file** inside _Core_ → tests rerun, service rebuilds & restarts (< 1 s).
2. Front‑end fetches `/generate/...` again → browser redraws without page refresh.

That's the whole feedback loop.

### Adding a New Generator

1. **Create a class** in `/Core/MapGen.Core/Generators/` that implements:

   ```csharp
   public interface IGenerator
   {
       string Name { get; }
       MapData Generate(in GenParams p);
   }
   ```

2. **Write unit tests** in `/Core/MapGen.Core.Tests/` (max‑size, seed determinism, etc.).
3. Hit **save** — watcher recompiles, service restarts, viewer list now shows the new `Name`.
4. Tweak parameters in the playground and eyeball the result.

No service config changes, no manual DLL copying – Scrutor picks it up automatically.

---

## Running the test‑suite

```bash
cd Core && dotnet test        # one‑off
cd Core && dotnet watch test  # continuous while editing
```

All generators share the same deterministic test harness so you can spot regressions fast.

---

## Build & release

### Service

```bash
cd Service/MapGen.Service
# Release‑ready self‑contained build (Linux x64)
dotnet publish -c Release -p:PublishSingleFile=true -o ../_dist/service
```

### Viewer

```bash
cd Viewer && npm ci && npm run build
# outputs static assets to Viewer/dist
```

> Combine the two in a Dockerfile or push them separately—your call.

---

## Troubleshooting

### Startup Script Issues

**Permission denied:**

```bash
chmod +x start-all.sh
```

**Script opens wrong terminal:**

```bash
# See manual commands
./start-all.sh --manual
```

**Platform not supported:**
The script works on macOS and Linux. For Windows, use the manual commands or run in WSL.

### Common Issues

**Port conflicts:**

- Kill any existing processes on ports 5023 or 5173
- Or modify the URLs in the startup commands

**Packages not found:**

```bash
# Restore .NET packages
cd Core && dotnet restore
cd ../Service && dotnet restore

# Reinstall npm packages
cd ../Viewer && rm -rf node_modules package-lock.json && npm install
```

For more detailed troubleshooting, see [GETTING_STARTED.md](./GETTING_STARTED.md).

---

## Roadmap / nice‑to‑haves

- **Docker‑compose** file for one‑command spin‑up.
- **GitHub Actions** monorepo workflow that lints, tests, and uploads both service & front‑end artefacts.
- **DevContainer** for Codespaces / VS Code Remote.
- **Rust or C++ plug‑in POC** to prove cross‑language generators.
- **Godot GDExtension** target that links directly against `MapGen.Core` after .NET → NativeAOT.
- WebGL preview in Viewer for 3‑D height‑maps.

PRs welcome—file an issue first if you're planning a big change.
