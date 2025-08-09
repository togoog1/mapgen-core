using System.Numerics;

namespace MapGen.Core;

public interface IMapGenerator
{
    byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters);
    string AlgorithmName { get; }
    Dictionary<string, object> DefaultParameters { get; }
    string Version { get; }
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
            { "cellular", new OrganicCavityGenerator() },
            { "diamond-square", new DiamondSquareGenerator() },
            { "fractal", new FractalNoiseGenerator() },
            { "voronoi", new VoronoiGenerator() },
            { "organic-cavity", new OrganicCavityGenerator() },
            { "advanced-organic", new AdvancedOrganicGenerator() },
            { "tunnel-lattice", new TunnelLatticeGenerator() }
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
 
    public string GetAlgorithmVersion(string algorithm)
    {
        if (!_generators.TryGetValue(algorithm.ToLower(), out var generator))
        {
            throw new ArgumentException($"Unknown algorithm: {algorithm}");
        }
        return generator.Version;
    }
}

// Perlin Noise Generator
public class PerlinNoiseGenerator : IMapGenerator
{
    public string AlgorithmName => "perlin";
    public string Version => "1.0.0";
    
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
    public string Version => "1.0.0";
    
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

// Enhanced Cellular Automata Generator for Organic Cavities
public class OrganicCavityGenerator : IMapGenerator
{
    public string AlgorithmName => "organic-cavity";
    public string Version => "1.0.0";
    
    public Dictionary<string, object> DefaultParameters => new()
    {
        { "iterations", 8 },
        { "birthThreshold", 3 },
        { "survivalThreshold", 2 },
        { "initialDensity", 0.4 },
        { "organicGrowth", 0.6 },
        { "cavityDepth", 0.8 },
        { "wallThickness", 0.2 },
        { "textureDetail", 0.4 },
        { "smoothingPasses", 3 }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var iterations = parameters.GetParameter("iterations", 8);
        var birthThreshold = parameters.GetParameter("birthThreshold", 3);
        var survivalThreshold = parameters.GetParameter("survivalThreshold", 2);
        var initialDensity = parameters.GetParameter("initialDensity", 0.4);
        var organicGrowth = parameters.GetParameter("organicGrowth", 0.6);
        var cavityDepth = parameters.GetParameter("cavityDepth", 0.8);
        var wallThickness = parameters.GetParameter("wallThickness", 0.2);
        var textureDetail = parameters.GetParameter("textureDetail", 0.4);
        var smoothingPasses = parameters.GetParameter("smoothingPasses", 3);

        var random = new Random(seed);
        
        // Multi-layer approach for organic cavities
        var baseLayer = GenerateBaseLayer(width, height, random, initialDensity);
        var growthLayer = GenerateGrowthLayer(baseLayer, random, organicGrowth);
        var detailLayer = GenerateDetailLayer(growthLayer, random, textureDetail);
        var finalLayer = ApplySmoothing(detailLayer, smoothingPasses);

        // Convert to pixel data with depth mapping
        var pixels = new byte[width * height * 4];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var value = finalLayer[x, y];
                var depth = CalculateDepth(value, cavityDepth, wallThickness);
                
                var index = (y * width + x) * 4;
                pixels[index] = depth;     // R (depth)
                pixels[index + 1] = depth; // G (depth)
                pixels[index + 2] = depth; // B (depth)
                pixels[index + 3] = 255;   // A
            }
        }

        return pixels;
    }

    private bool[,] GenerateBaseLayer(int width, int height, Random random, double density)
    {
        var grid = new bool[width, height];
        
        // Create organic initial pattern with clusters
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Create clustered seeding for more organic patterns
                var clusterFactor = Math.Sin(x * 0.1) * Math.Cos(y * 0.1) * 0.2;
                var adjustedDensity = density + clusterFactor;
                
                grid[x, y] = random.NextDouble() < adjustedDensity;
            }
        }

        return grid;
    }

    private bool[,] GenerateGrowthLayer(bool[,] baseLayer, Random random, double organicGrowth)
    {
        var width = baseLayer.GetLength(0);
        var height = baseLayer.GetLength(1);
        var grid = new bool[width, height];

        // Copy base layer
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = baseLayer[x, y];
            }
        }

        // Apply organic growth patterns
        for (int i = 0; i < 5; i++)
        {
            grid = RunOrganicGrowthStep(grid, random, organicGrowth);
        }

        return grid;
    }

    private bool[,] RunOrganicGrowthStep(bool[,] grid, Random random, double organicGrowth)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);
        var newGrid = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var neighbors = CountOrganicNeighbors(grid, x, y, random);
                var isAlive = grid[x, y];

                if (isAlive)
                {
                    // Survival with organic variation
                    var survivalChance = organicGrowth + (random.NextDouble() - 0.5) * 0.2;
                    newGrid[x, y] = neighbors >= 2 && random.NextDouble() < survivalChance;
                }
                else
                {
                    // Birth with organic variation
                    var birthChance = organicGrowth + (random.NextDouble() - 0.5) * 0.3;
                    newGrid[x, y] = neighbors >= 3 && random.NextDouble() < birthChance;
                }
            }
        }

        return newGrid;
    }

    private int CountOrganicNeighbors(bool[,] grid, int x, int y, Random random)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);
        var count = 0;

        // Use organic neighbor patterns (not just 8-connected)
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                var nx = x + dx;
                var ny = y + dy;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (grid[nx, ny])
                    {
                        // Weight by distance for more organic patterns
                        var distance = Math.Sqrt(dx * dx + dy * dy);
                        var weight = distance <= 1 ? 1.0 : 1.0 / distance;
                        
                        if (random.NextDouble() < weight)
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }

    private double[,] GenerateDetailLayer(bool[,] growthLayer, Random random, double textureDetail)
    {
        var width = growthLayer.GetLength(0);
        var height = growthLayer.GetLength(1);
        var detailLayer = new double[width, height];

        // Convert boolean grid to continuous values
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                detailLayer[x, y] = growthLayer[x, y] ? 1.0 : 0.0;
            }
        }

        // Add granular texture detail
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var noise = GenerateGranularNoise(x, y, random, textureDetail);
                detailLayer[x, y] = Math.Max(0, Math.Min(1, detailLayer[x, y] + noise * 0.3));
            }
        }

        return detailLayer;
    }

    private double GenerateGranularNoise(int x, int y, Random random, double detail)
    {
        // Create fine granular texture
        var hash = (int)(x * 73856093 + y * 19349663);
        random = new Random(hash);
        
        var noise = random.NextDouble() * 2 - 1;
        var frequency = 1.0 + detail * 4.0;
        
        return Math.Sin(x * frequency * 0.1) * Math.Cos(y * frequency * 0.1) * detail;
    }

    private double[,] ApplySmoothing(double[,] layer, int passes)
    {
        var width = layer.GetLength(0);
        var height = layer.GetLength(1);
        var smoothed = new double[width, height];

        // Copy original
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                smoothed[x, y] = layer[x, y];
            }
        }

        // Apply multiple smoothing passes
        for (int pass = 0; pass < passes; pass++)
        {
            var temp = new double[width, height];
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var sum = 0.0;
                    var count = 0;

                    // Gaussian-like smoothing
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            var nx = x + dx;
                            var ny = y + dy;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                var weight = dx == 0 && dy == 0 ? 0.5 : 0.0625;
                                sum += smoothed[nx, ny] * weight;
                                count++;
                            }
                        }
                    }

                    temp[x, y] = sum;
                }
            }

            // Copy back
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    smoothed[x, y] = temp[x, y];
                }
            }
        }

        return smoothed;
    }

    private byte CalculateDepth(double value, double cavityDepth, double wallThickness)
    {
        // Convert continuous value to depth map
        if (value < wallThickness)
        {
            // Cavity area (dark)
            var depth = (byte)(255 * (1.0 - cavityDepth));
            return depth;
        }
        else
        {
            // Wall area (light)
            var wallValue = (value - wallThickness) / (1.0 - wallThickness);
            var depth = (byte)(255 * (0.3 + wallValue * 0.7));
            return depth;
        }
    }
}

// Diamond-Square Generator
public class DiamondSquareGenerator : IMapGenerator
{
    public string AlgorithmName => "diamond-square";
    public string Version => "1.0.0";
    
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
    public string Version => "1.0.0";
    
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
    public string Version => "1.0.0";
    
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
        if (!parameters.TryGetValue(key, out var value) || value == null)
        {
            return defaultValue;
        }

        // Direct type match
        if (value is T typedValue)
        {
            return typedValue;
        }

        // Type conversion for common cases
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (InvalidCastException)
        {
            // Handle specific conversions that Convert.ChangeType might not handle
            if (typeof(T) == typeof(byte) && value is int intValue)
            {
                return (T)(object)(byte)Math.Clamp(intValue, byte.MinValue, byte.MaxValue);
            }
            if (typeof(T) == typeof(byte) && value is double doubleValue)
            {
                return (T)(object)(byte)Math.Clamp((int)doubleValue, byte.MinValue, byte.MaxValue);
            }
            
            return defaultValue;
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }
}

