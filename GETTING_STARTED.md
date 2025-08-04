# Getting Started with MapGen

A step-by-step guide to get you up and running with the MapGen playground in under 5 minutes.

## 🚀 Quick Start

### Option 1: One-Command Startup (Recommended)

```bash
# Clone the repository
git clone https://github.com/togoog1/mapgen-core.git
cd mapgen-core

# Start everything at once
./start-all.sh
```

That's it! The script will:

- ✅ Check prerequisites (.NET 9, Node.js 20)
- ✅ Install dependencies (npm packages, restore .NET packages)
- ✅ Open three terminal windows/tabs with each service running
- ✅ Show you the URLs where everything is available

### Option 2: Manual Startup

If the automatic script doesn't work on your system:

```bash
# Terminal 1 - Core library tests
cd Core && dotnet watch test

# Terminal 2 - API service
cd Service/MapGen.Service && dotnet watch run --urls=http://localhost:5023

# Terminal 3 - React frontend
cd Viewer && npm run dev
```

## 📍 Where to Find Things

Once everything is running:

| Service        | URL                           | What you'll see           |
| -------------- | ----------------------------- | ------------------------- |
| **Viewer**     | http://localhost:5173         | Interactive playground UI |
| **Service**    | http://localhost:5023         | API endpoints (JSON)      |
| **API Docs**   | http://localhost:5023/swagger | Swagger documentation     |
| **Core Tests** | Terminal output               | Test results as you code  |

## 🎮 Your First Map

1. **Open the Viewer** at http://localhost:5173
2. **Choose a generator** from the dropdown (e.g., "Perlin")
3. **Adjust parameters** with the sliders
4. **Watch the map** update in real-time

## 🛠 Development Workflow

### Adding a New Generator

1. **Write the generator** in `Core/MapGen.Core/Generators/`:

   ```csharp
   public class MyGenerator : IGenerator
   {
       public string Name => "MyCustom";
       public MapData Generate(in GenParams p) => /* your logic */;
   }
   ```

2. **Write tests** in `Core/MapGen.Core.Tests/`

3. **Save the file** → Tests run automatically, Service restarts, Viewer updates

4. **Test in the UI** → Your generator appears in the dropdown

### Typical Development Loop

```
Edit C# → Tests run → Service rebuilds → Try in Viewer → Repeat
     ↑                                                      ↓
     └─────────────── All automatic, < 1 second ──────────┘
```

## 🔧 Troubleshooting

### "Command not found" errors

**Missing .NET 9:**

```bash
# macOS
brew install dotnet

# Windows
winget install Microsoft.DotNet.SDK.9

# Linux
# See: https://docs.microsoft.com/en-us/dotnet/core/install/linux
```

**Missing Node.js 20:**

```bash
# macOS
brew install node@20

# Windows
winget install OpenJS.NodeJS.LTS

# Linux
curl -fsSL https://deb.nodesource.com/setup_lts.x | sudo -E bash -
sudo apt-get install -y nodejs
```

### Services won't start

**Port conflicts:**

- Kill any existing processes on ports 5023 or 5173
- Or modify the URLs in the startup commands

**Permission denied:**

```bash
chmod +x start-all.sh
```

**Packages not found:**

```bash
# Restore .NET packages
cd Core && dotnet restore
cd ../Service && dotnet restore

# Reinstall npm packages
cd ../Viewer && rm -rf node_modules package-lock.json && npm install
```

### Script opens wrong terminal

The script detects your platform and tries common terminals. If it doesn't work:

```bash
# See manual commands
./start-all.sh --manual

# Then copy-paste into your preferred terminal
```

## 📚 Next Steps

- **Read the main [README.md](./readme.md)** for detailed architecture info
- **Explore [Core/README.md](./Core/README.md)** for generator development
- **Check [Viewer/README.md](./Viewer/README.md)** for UI customization
- **Browse existing generators** in `Core/MapGen.Core/Generators/`

## 🆘 Still Stuck?

1. **Check prerequisites** with `./start-all.sh` (it validates everything)
2. **Try manual mode** with `./start-all.sh --manual`
3. **File an issue** with your platform details and error messages

Happy map generation! 🗺️
