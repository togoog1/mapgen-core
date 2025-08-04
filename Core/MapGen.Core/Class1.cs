using System.Numerics;

namespace MapGen.Core;

public interface IMapGenerator
{
    byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters);
    string AlgorithmName { get; }
    Dictionary<string, object> DefaultParameters { get; }
}

public class MapGenerationService
{
    private readonly Dictionary<string, IMapGenerator> _generators;

    public MapGenerationService()
    {
        _generators = new Dictionary<string, IMapGenerator>
        {
            { "perlin", new PerlinNoiseGenerator() },
            { "simplex", new SimplexNoiseGenerator() },
            { "cellular", new CellularAutomataGenerator() },
            { "diamond-square", new DiamondSquareGenerator() },
            { "fractal", new FractalNoiseGenerator() },
            { "voronoi", new VoronoiGenerator() }
        };
    }

    public byte[] GenerateMap(string algorithm, int width, int height, int seed, Dictionary<string, object> parameters)
    {
        if (!_generators.TryGetValue(algorithm.ToLower(), out var generator))
        {
            throw new ArgumentException($"Unknown algorithm: {algorithm}");
        }

        return generator.GenerateMap(width, height, seed, parameters);
    }

    public IEnumerable<string> GetAvailableAlgorithms()
    {
        return _generators.Keys;
    }

    public Dictionary<string, object> GetDefaultParameters(string algorithm)
    {
        if (!_generators.TryGetValue(algorithm.ToLower(), out var generator))
        {
            throw new ArgumentException($"Unknown algorithm: {algorithm}");
        }

        return generator.DefaultParameters;
    }
}

// Perlin Noise Generator
public class PerlinNoiseGenerator : IMapGenerator
{
    public string AlgorithmName => "perlin";
    
    public Dictionary<string, object> DefaultParameters => new()
    {
        { "scale", 0.1 },
        { "octaves", 4 },
        { "persistence", 0.5 },
        { "lacunarity", 2.0 }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var scale = parameters.GetParameter("scale", 0.1);
        var octaves = parameters.GetParameter("octaves", 4);
        var persistence = parameters.GetParameter("persistence", 0.5);
        var lacunarity = parameters.GetParameter("lacunarity", 2.0);

        var random = new Random(seed);
        var pixels = new byte[width * height * 4];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var noise = GeneratePerlinNoise(x * scale, y * scale, octaves, persistence, lacunarity, random);
                var value = (byte)((noise + 1) * 127.5); // Convert from [-1,1] to [0,255]
                
                var index = (y * width + x) * 4;
                pixels[index] = value;     // R
                pixels[index + 1] = value; // G
                pixels[index + 2] = value; // B
                pixels[index + 3] = 255;   // A
            }
        }

        return pixels;
    }

    private double GeneratePerlinNoise(double x, double y, int octaves, double persistence, double lacunarity, Random random)
    {
        double amplitude = 1.0;
        double frequency = 1.0;
        double noise = 0.0;
        double maxValue = 0.0;

        for (int i = 0; i < octaves; i++)
        {
            noise += amplitude * SimpleNoise(x * frequency, y * frequency, random);
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return noise / maxValue;
    }

    private double SimpleNoise(double x, double y, Random random)
    {
        // Simplified noise function for demonstration
        // In a real implementation, you'd use a proper Perlin noise algorithm
        var hash = (int)(x * 73856093 + y * 19349663);
        random = new Random(hash);
        return random.NextDouble() * 2 - 1;
    }
}

// Simplex Noise Generator
public class SimplexNoiseGenerator : IMapGenerator
{
    public string AlgorithmName => "simplex";
    
    public Dictionary<string, object> DefaultParameters => new()
    {
        { "scale", 0.05 },
        { "octaves", 6 },
        { "persistence", 0.6 },
        { "lacunarity", 2.2 }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var scale = parameters.GetParameter("scale", 0.05);
        var octaves = parameters.GetParameter("octaves", 6);
        var persistence = parameters.GetParameter("persistence", 0.6);
        var lacunarity = parameters.GetParameter("lacunarity", 2.2);

        var random = new Random(seed);
        var pixels = new byte[width * height * 4];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var noise = GenerateSimplexNoise(x * scale, y * scale, octaves, persistence, lacunarity, random);
                var value = (byte)((noise + 1) * 127.5);
                
                var index = (y * width + x) * 4;
                pixels[index] = value;     // R
                pixels[index + 1] = value; // G
                pixels[index + 2] = value; // B
                pixels[index + 3] = 255;   // A
            }
        }

        return pixels;
    }

    private double GenerateSimplexNoise(double x, double y, int octaves, double persistence, double lacunarity, Random random)
    {
        // Simplified Simplex-like noise
        double amplitude = 1.0;
        double frequency = 1.0;
        double noise = 0.0;
        double maxValue = 0.0;

        for (int i = 0; i < octaves; i++)
        {
            noise += amplitude * SimpleNoise(x * frequency, y * frequency, random);
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return noise / maxValue;
    }

    private double SimpleNoise(double x, double y, Random random)
    {
        var hash = (int)(x * 73856093 + y * 19349663);
        random = new Random(hash);
        return random.NextDouble() * 2 - 1;
    }
}

// Cellular Automata Generator
public class CellularAutomataGenerator : IMapGenerator
{
    public string AlgorithmName => "cellular";
    
    public Dictionary<string, object> DefaultParameters => new()
    {
        { "iterations", 5 },
        { "birthThreshold", 3 },
        { "survivalThreshold", 2 },
        { "initialDensity", 0.4 }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var iterations = parameters.GetParameter("iterations", 5);
        var birthThreshold = parameters.GetParameter("birthThreshold", 3);
        var survivalThreshold = parameters.GetParameter("survivalThreshold", 2);
        var initialDensity = parameters.GetParameter("initialDensity", 0.4);

        var random = new Random(seed);
        var grid = new bool[width, height];

        // Initialize with random cells
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = random.NextDouble() < initialDensity;
            }
        }

        // Run cellular automata iterations
        for (int i = 0; i < iterations; i++)
        {
            grid = RunCellularAutomataStep(grid, birthThreshold, survivalThreshold);
        }

        // Convert to pixel data
        var pixels = new byte[width * height * 4];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var value = grid[x, y] ? (byte)255 : (byte)0;
                var index = (y * width + x) * 4;
                pixels[index] = value;     // R
                pixels[index + 1] = value; // G
                pixels[index + 2] = value; // B
                pixels[index + 3] = 255;   // A
            }
        }

        return pixels;
    }

    private bool[,] RunCellularAutomataStep(bool[,] grid, int birthThreshold, int survivalThreshold)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);
        var newGrid = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var neighbors = CountNeighbors(grid, x, y);
                var isAlive = grid[x, y];

                if (isAlive)
                {
                    newGrid[x, y] = neighbors >= survivalThreshold;
                }
                else
                {
                    newGrid[x, y] = neighbors >= birthThreshold;
                }
            }
        }

        return newGrid;
    }

    private int CountNeighbors(bool[,] grid, int x, int y)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);
        var count = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                var nx = x + dx;
                var ny = y + dy;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (grid[nx, ny]) count++;
                }
            }
        }

        return count;
    }
}

// Diamond-Square Generator
public class DiamondSquareGenerator : IMapGenerator
{
    public string AlgorithmName => "diamond-square";
    
    public Dictionary<string, object> DefaultParameters => new()
    {
        { "roughness", 0.5 },
        { "size", 257 },
        { "seed", 0 }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var roughness = parameters.GetParameter("roughness", 0.5);
        var size = parameters.GetParameter("size", 257);
        
        // Ensure size is 2^n + 1
        size = (int)Math.Pow(2, (int)Math.Log(size - 1, 2)) + 1;
        
        var random = new Random(seed);
        var heightmap = new double[size, size];

        // Initialize corners
        heightmap[0, 0] = random.NextDouble();
        heightmap[0, size - 1] = random.NextDouble();
        heightmap[size - 1, 0] = random.NextDouble();
        heightmap[size - 1, size - 1] = random.NextDouble();

        // Run diamond-square algorithm
        DiamondSquare(heightmap, size, roughness, random);

        // Convert to pixel data
        var pixels = new byte[width * height * 4];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var mapX = (int)((double)x / width * (size - 1));
                var mapY = (int)((double)y / height * (size - 1));
                var value = (byte)(heightmap[mapX, mapY] * 255);
                
                var index = (y * width + x) * 4;
                pixels[index] = value;     // R
                pixels[index + 1] = value; // G
                pixels[index + 2] = value; // B
                pixels[index + 3] = 255;   // A
            }
        }

        return pixels;
    }

    private void DiamondSquare(double[,] heightmap, int size, double roughness, Random random)
    {
        var step = size - 1;
        var range = 1.0;

        while (step > 1)
        {
            // Diamond step
            for (int x = step / 2; x < size; x += step)
            {
                for (int y = step / 2; y < size; y += step)
                {
                    var avg = (heightmap[x - step / 2, y - step / 2] +
                              heightmap[x + step / 2, y - step / 2] +
                              heightmap[x - step / 2, y + step / 2] +
                              heightmap[x + step / 2, y + step / 2]) / 4.0;
                    
                    heightmap[x, y] = avg + (random.NextDouble() * 2 - 1) * range;
                }
            }

            // Square step
            for (int x = 0; x < size; x += step / 2)
            {
                for (int y = (x + step / 2) % step; y < size; y += step)
                {
                    var count = 0;
                    var sum = 0.0;

                    if (x >= step / 2)
                    {
                        sum += heightmap[x - step / 2, y];
                        count++;
                    }
                    if (x + step / 2 < size)
                    {
                        sum += heightmap[x + step / 2, y];
                        count++;
                    }
                    if (y >= step / 2)
                    {
                        sum += heightmap[x, y - step / 2];
                        count++;
                    }
                    if (y + step / 2 < size)
                    {
                        sum += heightmap[x, y + step / 2];
                        count++;
                    }

                    if (count > 0)
                    {
                        heightmap[x, y] = sum / count + (random.NextDouble() * 2 - 1) * range;
                    }
                }
            }

            step /= 2;
            range *= roughness;
        }
    }
}

// Fractal Noise Generator
public class FractalNoiseGenerator : IMapGenerator
{
    public string AlgorithmName => "fractal";
    
    public Dictionary<string, object> DefaultParameters => new()
    {
        { "scale", 0.02 },
        { "octaves", 8 },
        { "persistence", 0.7 },
        { "lacunarity", 2.5 }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var scale = parameters.GetParameter("scale", 0.02);
        var octaves = parameters.GetParameter("octaves", 8);
        var persistence = parameters.GetParameter("persistence", 0.7);
        var lacunarity = parameters.GetParameter("lacunarity", 2.5);

        var random = new Random(seed);
        var pixels = new byte[width * height * 4];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var noise = GenerateFractalNoise(x * scale, y * scale, octaves, persistence, lacunarity, random);
                var value = (byte)((noise + 1) * 127.5);
                
                var index = (y * width + x) * 4;
                pixels[index] = value;     // R
                pixels[index + 1] = value; // G
                pixels[index + 2] = value; // B
                pixels[index + 3] = 255;   // A
            }
        }

        return pixels;
    }

    private double GenerateFractalNoise(double x, double y, int octaves, double persistence, double lacunarity, Random random)
    {
        double amplitude = 1.0;
        double frequency = 1.0;
        double noise = 0.0;
        double maxValue = 0.0;

        for (int i = 0; i < octaves; i++)
        {
            noise += amplitude * SimpleNoise(x * frequency, y * frequency, random);
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return noise / maxValue;
    }

    private double SimpleNoise(double x, double y, Random random)
    {
        var hash = (int)(x * 73856093 + y * 19349663);
        random = new Random(hash);
        return random.NextDouble() * 2 - 1;
    }
}

// Voronoi Generator
public class VoronoiGenerator : IMapGenerator
{
    public string AlgorithmName => "voronoi";
    
    public Dictionary<string, object> DefaultParameters => new()
    {
        { "points", 50 },
        { "distanceMetric", "euclidean" }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var points = parameters.GetParameter("points", 50);
        var distanceMetric = parameters.GetParameter("distanceMetric", "euclidean");

        var random = new Random(seed);
        var pixels = new byte[width * height * 4];

        // Generate random points
        var sites = new List<Vector2>();
        for (int i = 0; i < points; i++)
        {
            sites.Add(new Vector2(
                random.Next(0, width),
                random.Next(0, height)
            ));
        }

        // Generate Voronoi diagram
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var point = new Vector2(x, y);
                var minDistance = float.MaxValue;
                var closestSite = 0;

                for (int i = 0; i < sites.Count; i++)
                {
                    var distance = CalculateDistance(point, sites[i], distanceMetric);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestSite = i;
                    }
                }

                var value = (byte)((closestSite * 255) / sites.Count);
                var index = (y * width + x) * 4;
                pixels[index] = value;     // R
                pixels[index + 1] = value; // G
                pixels[index + 2] = value; // B
                pixels[index + 3] = 255;   // A
            }
        }

        return pixels;
    }

    private float CalculateDistance(Vector2 a, Vector2 b, string metric)
    {
        return metric switch
        {
            "manhattan" => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y),
            "chebyshev" => Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y)),
            _ => Vector2.Distance(a, b) // euclidean
        };
    }
}

// Utility method for parameter extraction
public static class ParameterExtensions
{
    public static T GetParameter<T>(this Dictionary<string, object> parameters, string key, T defaultValue)
    {
        if (parameters.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }
}
