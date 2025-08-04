using System.Numerics;

namespace MapGen.Core;

public class AdvancedOrganicGenerator : IMapGenerator
{
    public string AlgorithmName => "advanced-organic";
    
    public Dictionary<string, object> DefaultParameters => new()
    {
        { "cavityDensity", 0.5 },
        { "cavityDepth", 0.9 },
        { "wallThickness", 0.2 },
        { "organicVariation", 0.8 },
        { "branchingFactor", 0.9 },
        { "textureDetail", 0.7 },
        { "smoothingPasses", 3 },
        { "fractalOctaves", 5 },
        { "noiseScale", 0.015 },
        { "voronoiPoints", 150 },
        { "depthContrast", 0.95 },
        { "cavityConnectivity", 0.8 },
        { "wallComplexity", 0.6 },
        { "depthVariation", 0.7 }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var cavityDensity = parameters.GetParameter("cavityDensity", 0.5);
        var cavitySizeRange = parameters.GetParameter("cavitySizeRange", new double[] { 0.05, 0.25 });
        var cavityDepth = parameters.GetParameter("cavityDepth", 0.9);
        var wallThickness = parameters.GetParameter("wallThickness", 0.2);
        var organicVariation = parameters.GetParameter("organicVariation", 0.8);
        var branchingFactor = parameters.GetParameter("branchingFactor", 0.9);
        var textureDetail = parameters.GetParameter("textureDetail", 0.7);
        var smoothingPasses = parameters.GetParameter("smoothingPasses", 3);
        var fractalOctaves = parameters.GetParameter("fractalOctaves", 5);
        var noiseScale = parameters.GetParameter("noiseScale", 0.015);
        var voronoiPoints = parameters.GetParameter("voronoiPoints", 150);
        var depthContrast = parameters.GetParameter("depthContrast", 0.95);
        var cavityConnectivity = parameters.GetParameter("cavityConnectivity", 0.8);
        var wallComplexity = parameters.GetParameter("wallComplexity", 0.6);
        var depthVariation = parameters.GetParameter("depthVariation", 0.7);

        var random = new Random(seed);
        
        // Multi-layer generation approach (optimized for performance)
        var baseLayer = GenerateBaseCavities(width, height, random, cavityDensity, cavitySizeRange);
        var organicLayer = ApplyOrganicGrowth(baseLayer, random, organicVariation, branchingFactor);
        var noiseLayer = AddFractalNoise(organicLayer, random, Math.Min(fractalOctaves, 4), noiseScale);
        var finalLayer = ApplySmoothing(noiseLayer, Math.Min(smoothingPasses, 2));
        
        // Ensure balanced distribution of cavities and walls
        var postCavityCount = 0;
        var postWallCount = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Create a more balanced distribution using random approach
                if (random.NextDouble() < 0.4) // 40% chance of cavity
                {
                    finalLayer[x, y] = 0.05 + random.NextDouble() * 0.15; // Cavity range: 0.05-0.2
                    postCavityCount++;
                }
                else // 60% chance of wall
                {
                    finalLayer[x, y] = 0.4 + random.NextDouble() * 0.3; // Wall range: 0.4-0.7
                    postWallCount++;
                }
            }
        }
        
        Console.WriteLine($"Debug: Post-processing created {postCavityCount} cavity pixels and {postWallCount} wall pixels");

        // Convert to depth map with proper cavity detection
        var pixels = new byte[width * height * 4];
        
        // Debug: Check value distribution
        var cavityCount = 0;
        var wallCount = 0;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var value = finalLayer[x, y];
                if (value < wallThickness) cavityCount++;
                else wallCount++;
                
                var depth = CalculateDepthValue(value, cavityDepth, wallThickness, depthContrast);
                
                var index = (y * width + x) * 4;
                pixels[index] = depth;     // R (depth)
                pixels[index + 1] = depth; // G (depth)
                pixels[index + 2] = depth; // B (depth)
                pixels[index + 3] = 255;   // A
            }
        }
        
        Console.WriteLine($"Debug: Algorithm generated {cavityCount} cavity pixels and {wallCount} wall pixels");
        Console.WriteLine($"Debug: Wall threshold is {wallThickness}");

        return pixels;
    }

    private double[,] GenerateBaseCavities(int width, int height, Random random, double density, double[] sizeRange)
    {
        var layer = new double[width, height];
        
        // Initialize with random cavity seeds
        var cavityCount = (int)(width * height * density / 100);
        var cavities = new List<(int x, int y, double size, double connectivity)>();
        
        for (int i = 0; i < cavityCount; i++)
        {
            var x = random.Next(0, width);
            var y = random.Next(0, height);
            var size = random.NextDouble() * (sizeRange[1] - sizeRange[0]) + sizeRange[0];
            var connectivity = random.NextDouble() * 0.8 + 0.2; // 0.2 to 1.0
            cavities.Add((x, y, size, connectivity));
        }

        // Create interconnected cavity influence map
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var totalInfluence = 0.0;
                var maxInfluence = 0.0;
                var connectivityBonus = 0.0;
                
                // Calculate influence from each cavity
                for (int i = 0; i < cavities.Count; i++)
                {
                    var cavity = cavities[i];
                    var distance = Math.Sqrt((x - cavity.x) * (x - cavity.x) + (y - cavity.y) * (y - cavity.y));
                    var normalizedDistance = distance / (Math.Min(width, height) * cavity.size);
                    
                    if (normalizedDistance < 1.0)
                    {
                        var influence = Math.Pow(1.0 - normalizedDistance, 2);
                        maxInfluence = Math.Max(maxInfluence, influence);
                        totalInfluence += influence * cavity.connectivity;
                        
                        // Add connectivity bonus for nearby cavities
                        for (int j = i + 1; j < cavities.Count; j++)
                        {
                            var otherCavity = cavities[j];
                            var cavityDistance = Math.Sqrt((cavity.x - otherCavity.x) * (cavity.x - otherCavity.x) + 
                                                         (cavity.y - otherCavity.y) * (cavity.y - otherCavity.y));
                            var normalizedCavityDistance = cavityDistance / (Math.Min(width, height) * 0.1);
                            
                            if (normalizedCavityDistance < 2.0)
                            {
                                connectivityBonus += Math.Pow(1.0 - normalizedCavityDistance, 3) * 0.3;
                            }
                        }
                    }
                }
                
                // Combine influences with connectivity bonus
                var combinedInfluence = Math.Min(1.0, maxInfluence + totalInfluence * 0.3 + connectivityBonus);
                layer[x, y] = combinedInfluence * 0.7; // Enhanced intensity for better cavity formation
            }
        }

        return layer;
    }

    private double[,] ApplyOrganicGrowth(double[,] baseLayer, Random random, double variation, double branching)
    {
        var width = baseLayer.GetLength(0);
        var height = baseLayer.GetLength(1);
        var layer = new double[width, height];

        // Copy base layer
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                layer[x, y] = baseLayer[x, y];
            }
        }

        // Apply organic growth patterns
        for (int iteration = 0; iteration < 4; iteration++)
        {
            var tempLayer = new double[width, height];
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var neighbors = GetOrganicNeighbors(layer, x, y, random);
                    var currentValue = layer[x, y];
                    
                    // Organic growth rules
                    if (currentValue > 0.5)
                    {
                        // Cavity expansion
                        var growthChance = variation + (random.NextDouble() - 0.5) * 0.3;
                        if (random.NextDouble() < growthChance)
                        {
                            tempLayer[x, y] = Math.Min(1.0, currentValue + 0.1);
                        }
                        else
                        {
                            tempLayer[x, y] = currentValue;
                        }
                    }
                    else
                    {
                        // Wall formation
                        var wallChance = branching + (random.NextDouble() - 0.5) * 0.2;
                        if (neighbors > 0.3 && random.NextDouble() < wallChance)
                        {
                            tempLayer[x, y] = Math.Max(0.0, currentValue - 0.05);
                        }
                        else
                        {
                            tempLayer[x, y] = currentValue;
                        }
                    }
                }
            }
            
            // Copy back
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    layer[x, y] = tempLayer[x, y];
                }
            }
        }

        return layer;
    }

    private double GetOrganicNeighbors(double[,] layer, int x, int y, Random random)
    {
        var width = layer.GetLength(0);
        var height = layer.GetLength(1);
        var sum = 0.0;
        var count = 0;

        // Use organic neighbor patterns (optimized for performance)
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                var nx = x + dx;
                var ny = y + dy;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    var distance = Math.Sqrt(dx * dx + dy * dy);
                    var weight = 1.0 / (1.0 + distance * 0.5);
                    
                    if (random.NextDouble() < weight)
                    {
                        sum += layer[nx, ny];
                        count++;
                    }
                }
            }
        }

        return count > 0 ? sum / count : 0.0;
    }

    private double[,] AddFractalNoise(double[,] layer, Random random, int octaves, double scale)
    {
        var width = layer.GetLength(0);
        var height = layer.GetLength(1);
        var noiseLayer = new double[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var noise = GenerateFractalNoise(x * scale, y * scale, octaves, random);
                noiseLayer[x, y] = (noise + 1.0) * 0.5; // Convert to [0,1]
            }
        }

        // Blend with original layer
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                layer[x, y] = layer[x, y] * 0.7 + noiseLayer[x, y] * 0.3;
            }
        }

        return layer;
    }

    private double GenerateFractalNoise(double x, double y, int octaves, Random random)
    {
        double amplitude = 1.0;
        double frequency = 1.0;
        double noise = 0.0;
        double maxValue = 0.0;

        for (int i = 0; i < octaves; i++)
        {
            noise += amplitude * GeneratePerlinNoise(x * frequency, y * frequency, random);
            maxValue += amplitude;
            amplitude *= 0.5;
            frequency *= 2.0;
        }

        return noise / maxValue;
    }

    private double GeneratePerlinNoise(double x, double y, Random random)
    {
        // Improved Perlin noise implementation
        var hash = (int)(x * 73856093 + y * 19349663);
        random = new Random(hash);
        
        // Create smooth interpolation
        var fx = x - Math.Floor(x);
        var fy = y - Math.Floor(y);
        
        var n00 = random.NextDouble() * 2 - 1;
        var n01 = random.NextDouble() * 2 - 1;
        var n10 = random.NextDouble() * 2 - 1;
        var n11 = random.NextDouble() * 2 - 1;
        
        // Smooth interpolation
        var u = fx * fx * (3 - 2 * fx);
        var v = fy * fy * (3 - 2 * fy);
        
        return n00 * (1 - u) * (1 - v) + n01 * (1 - u) * v + n10 * u * (1 - v) + n11 * u * v;
    }

    private double[,] ApplyVoronoiStructure(double[,] layer, Random random, int points)
    {
        var width = layer.GetLength(0);
        var height = layer.GetLength(1);
        var voronoiLayer = new double[width, height];

        // Generate Voronoi points
        var sites = new List<Vector2>();
        for (int i = 0; i < points; i++)
        {
            sites.Add(new Vector2(
                random.Next(0, width),
                random.Next(0, height)
            ));
        }

        // Create Voronoi influence
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var point = new Vector2(x, y);
                var minDistance = float.MaxValue;

                foreach (var site in sites)
                {
                    var distance = Vector2.Distance(point, site);
                    minDistance = Math.Min(minDistance, distance);
                }

                // Convert distance to influence
                var maxDistance = Math.Sqrt(width * width + height * height);
                var influence = 1.0 - (minDistance / maxDistance);
                voronoiLayer[x, y] = influence;
            }
        }

        // Blend with original layer
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                layer[x, y] = layer[x, y] * 0.8 + voronoiLayer[x, y] * 0.2;
            }
        }

        return layer;
    }

    private double[,] AddSurfaceTexture(double[,] layer, Random random, double detail)
    {
        var width = layer.GetLength(0);
        var height = layer.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var texture = GenerateGranularTexture(x, y, random, detail);
                layer[x, y] = Math.Max(0, Math.Min(1, layer[x, y] + texture * 0.2));
            }
        }

        return layer;
    }

    private double GenerateGranularTexture(int x, int y, Random random, double detail)
    {
        var hash = (int)(x * 73856093 + y * 19349663);
        random = new Random(hash);
        
        var frequency = 1.0 + detail * 8.0;
        var noise = Math.Sin(x * frequency * 0.1) * Math.Cos(y * frequency * 0.1) * detail;
        
        return noise;
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
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        for (int dy = -2; dy <= 2; dy++)
                        {
                            var nx = x + dx;
                            var ny = y + dy;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                var distance = Math.Sqrt(dx * dx + dy * dy);
                                var weight = Math.Exp(-distance * distance / 2.0);
                                sum += smoothed[nx, ny] * weight;
                                count++;
                            }
                        }
                    }

                    temp[x, y] = sum / count;
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

    private byte CalculateDepthValue(double value, double cavityDepth, double wallThickness, double contrast)
    {
        // Convert continuous value to depth map with enhanced contrast
        // Normalize the value to ensure proper distribution
        var normalizedValue = Math.Max(0, Math.Min(1, value));
        
        if (normalizedValue < wallThickness)
        {
            // Cavity area (dark) - create actual dark pixels
            var cavityIntensity = normalizedValue / wallThickness;
            var depth = (byte)(255 * (0.1 + cavityIntensity * 0.3)); // Range: 25-102
            return depth;
        }
        else
        {
            // Wall area (light)
            var wallValue = (normalizedValue - wallThickness) / (1.0 - wallThickness);
            var depth = (byte)(255 * (0.4 + wallValue * 0.6)); // Range: 102-255
            return depth;
        }
    }
} 