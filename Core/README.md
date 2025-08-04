# MapGen Core

The heart of the MapGen project - a .NET 9 class library containing all map generation algorithms and core data structures.

## Overview

MapGen.Core is a pure C# library that implements deterministic procedural map generation algorithms. It's designed to be:

- **Deterministic**: Same seed = same map, always
- **Fast**: Optimized for real-time generation
- **Extensible**: Easy to add new generators
- **Testable**: Comprehensive test coverage

## Project Structure

```
MapGen.Core/
├── Generators/          # Map generation algorithms
├── Models/             # Data structures (MapData, GenParams, etc.)
├── Utilities/          # Helper classes and extensions
└── Interfaces/         # Core abstractions (IGenerator)

MapGen.Core.Tests/
├── GeneratorTests/     # Tests for each generator
├── ModelTests/         # Tests for data structures
└── Utilities/          # Test utilities and fixtures
```

## Key Interfaces

### IGenerator

All map generators implement this interface:

```csharp
public interface IGenerator
{
    string Name { get; }
    MapData Generate(in GenParams parameters);
}
```

### Core Data Types

- **`GenParams`**: Input parameters (width, height, seed, algorithm-specific settings)
- **`MapData`**: Output data structure containing the generated map
- **`TerrainType`**: Enumeration of different terrain types

## Adding a New Generator

1. **Create the generator class** in `Generators/`:

```csharp
public class MyCustomGenerator : IGenerator
{
    public string Name => "MyCustom";

    public MapData Generate(in GenParams parameters)
    {
        // Your generation logic here
        return new MapData(/* ... */);
    }
}
```

2. **Write unit tests** in `MapGen.Core.Tests/GeneratorTests/`:

```csharp
[Fact]
public void MyCustomGenerator_SameSeed_ProducesSameOutput()
{
    var generator = new MyCustomGenerator();
    var parameters = new GenParams
    {
        Width = 100,
        Height = 100,
        Seed = 12345
    };

    var result1 = generator.Generate(parameters);
    var result2 = generator.Generate(parameters);

    Assert.Equal(result1.GetHashCode(), result2.GetHashCode());
}
```

3. **Run tests** to verify:

```bash
dotnet watch test
```

The Service layer will automatically discover your new generator via assembly scanning - no configuration needed!

## Development Workflow

### Running Tests

```bash
# One-time test run
dotnet test

# Continuous testing (recommended)
dotnet watch test
```

### Building the Library

```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release
```

### Testing Individual Generators

Use the test utilities to test specific generators:

```csharp
[Theory]
[InlineData(1, 100, 100)]
[InlineData(42, 256, 256)]
[InlineData(999, 512, 512)]
public void Generator_DifferentSeeds_ProduceDifferentMaps(int seed, int width, int height)
{
    var generator = new MyGenerator();
    var parameters = new GenParams { Seed = seed, Width = width, Height = height };

    var result = generator.Generate(parameters);

    Assert.NotNull(result);
    Assert.Equal(width, result.Width);
    Assert.Equal(height, result.Height);
}
```

## Dependencies

- **.NET 9**: Modern C# features and performance
- **Noise**: Perlin/Simplex noise generation
- **System.Numerics.Vectors**: SIMD optimization
- **xUnit**: Testing framework

## Performance Guidelines

1. **Use `in` parameters** for large structs to avoid copying
2. **Leverage SIMD** with `System.Numerics.Vectors` for array operations
3. **Pre-allocate arrays** when possible
4. **Use `Span<T>`** for stack-allocated buffers
5. **Avoid allocations** in hot paths

## Testing Guidelines

Every generator should have tests for:

- **Determinism**: Same seed produces same output
- **Bounds checking**: Handles edge cases (0x0, 1x1, very large maps)
- **Parameter validation**: Proper error handling for invalid inputs
- **Performance**: Reasonable generation times for typical map sizes

## Integration

The Core library is referenced by:

- **MapGen.Service**: Exposes generators via HTTP API
- **MapGen.Core.Tests**: Comprehensive test suite

No direct UI dependencies - keep this library pure and focused on generation logic.

## Quick Start

```bash
# Clone and navigate to Core
cd Core

# Restore dependencies
dotnet restore

# Run tests with watch (recommended for development)
dotnet watch test

# Or run tests once
dotnet test
```

Your tests will run automatically whenever you save changes to any C# file. This tight feedback loop is essential for rapid algorithm development.
