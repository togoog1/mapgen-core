using Xunit;
using MapGen.Core;
using System.Drawing;
using System.Drawing.Imaging;

namespace MapGen.Core.Tests;

public class OrganicCavityTests : IDisposable
{
    private MapGenerationService _service;

    public OrganicCavityTests()
    {
        _service = new MapGenerationService();
    }

    [Fact]
    public void TestAdvancedOrganicGenerator_DefaultParameters()
    {
        // Arrange
        var parameters = new Dictionary<string, object>();
        var width = 512;
        var height = 512;
        var seed = 12345;

        // Act
        var result = _service.GenerateMap("advanced-organic", width, height, seed, parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(width * height * 4, result.Length);
        
        // Save test image for visual inspection
        SaveTestImage(result, width, height, "test_advanced_organic_default.png");
    }

    [Fact]
    public void TestAdvancedOrganicGenerator_HighResolution()
    {
        // Arrange
        var parameters = new Dictionary<string, object>
        {
            { "resolution", 1024 },
            { "cavityDensity", 0.5 },
            { "cavityDepth", 0.9 },
            { "wallThickness", 0.1 },
            { "organicVariation", 0.8 },
            { "branchingFactor", 0.8 },
            { "textureDetail", 0.7 },
            { "smoothingPasses", 6 }
        };
        var width = 1024;
        var height = 1024;
        var seed = 54321;

        // Act
        var result = _service.GenerateMap("advanced-organic", width, height, seed, parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(width * height * 4, result.Length);
        
        // Save test image for visual inspection
        SaveTestImage(result, width, height, "test_advanced_organic_highres.png");
    }

    [Fact]
    public void TestAdvancedOrganicGenerator_SeedConsistency()
    {
        // Arrange
        var parameters = new Dictionary<string, object>();
        var width = 256;
        var height = 256;
        var seed = 99999;

        // Act
        var result1 = _service.GenerateMap("advanced-organic", width, height, seed, parameters);
        var result2 = _service.GenerateMap("advanced-organic", width, height, seed, parameters);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Length, result2.Length);
        
        // Results should be identical with same seed
        Assert.Equal(result1, result2);
        
        // Save test image for visual inspection
        SaveTestImage(result1, width, height, "test_advanced_organic_seed_consistency.png");
    }

    [Fact]
    public void TestAdvancedOrganicGenerator_ParameterSweep()
    {
        // Test different parameter combinations
        var testCases = new[]
        {
            new { Name = "high_density", Parameters = new Dictionary<string, object> { { "cavityDensity", 0.7 } } },
            new { Name = "low_density", Parameters = new Dictionary<string, object> { { "cavityDensity", 0.2 } } },
            new { Name = "deep_cavities", Parameters = new Dictionary<string, object> { { "cavityDepth", 0.95 } } },
            new { Name = "shallow_cavities", Parameters = new Dictionary<string, object> { { "cavityDepth", 0.5 } } },
            new { Name = "high_variation", Parameters = new Dictionary<string, object> { { "organicVariation", 0.9 } } },
            new { Name = "low_variation", Parameters = new Dictionary<string, object> { { "organicVariation", 0.2 } } },
            new { Name = "high_branching", Parameters = new Dictionary<string, object> { { "branchingFactor", 0.9 } } },
            new { Name = "low_branching", Parameters = new Dictionary<string, object> { { "branchingFactor", 0.3 } } }
        };

        var width = 512;
        var height = 512;
        var seed = 11111;

        foreach (var testCase in testCases)
        {
            // Act
            var result = _service.GenerateMap("advanced-organic", width, height, seed, testCase.Parameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(width * height * 4, result.Length);
            
            // Save test image for visual inspection
            SaveTestImage(result, width, height, $"test_advanced_organic_{testCase.Name}.png");
        }
    }

    [Fact]
    public void TestAdvancedOrganicGenerator_Performance()
    {
        // Arrange
        var parameters = new Dictionary<string, object>();
        var width = 1024;
        var height = 1024;
        var seed = 77777;

        // Act & Measure
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _service.GenerateMap("advanced-organic", width, height, seed, parameters);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(width * height * 4, result.Length);
        
        // Performance should be reasonable (less than 30 seconds for 1024x1024)
        Assert.True(stopwatch.ElapsedMilliseconds < 30000, 
            $"Generation took {stopwatch.ElapsedMilliseconds}ms, expected < 30000ms");
        
        Console.WriteLine($"Generated {width}x{height} map in {stopwatch.ElapsedMilliseconds}ms");
        
        // Save test image for visual inspection
        SaveTestImage(result, width, height, "test_advanced_organic_performance.png");
    }

    [Fact]
    public void TestAdvancedOrganicGenerator_ImageAnalysis()
    {
        // Arrange
        var parameters = new Dictionary<string, object>();
        var width = 512;
        var height = 512;
        var seed = 55555;

        // Act
        var result = _service.GenerateMap("advanced-organic", width, height, seed, parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(width * height * 4, result.Length);

        // Debug: Check first few pixel values
        Console.WriteLine($"Debug: First 10 pixel values:");
        for (int i = 0; i < Math.Min(40, result.Length); i += 4)
        {
            Console.WriteLine($"  Pixel {i/4}: R={result[i]}, G={result[i+1]}, B={result[i+2]}, A={result[i+3]}");
        }

        // Analyze the generated image
        var analysis = AnalyzeImage(result, width, height);
        
        // Basic quality checks
        Assert.True(analysis.CavityPercentage > 0.1 && analysis.CavityPercentage < 0.9, 
            $"Cavity percentage {analysis.CavityPercentage:P} should be between 10% and 90%");
        Assert.True(analysis.AverageDepth > 50 && analysis.AverageDepth < 200, 
            $"Average depth {analysis.AverageDepth} should be between 50 and 200");
        Assert.True(analysis.DepthVariance > 1000, 
            $"Depth variance {analysis.DepthVariance} should be > 1000 for good contrast");
        
        Console.WriteLine($"Image Analysis:");
        Console.WriteLine($"  Cavity Percentage: {analysis.CavityPercentage:P}");
        Console.WriteLine($"  Average Depth: {analysis.AverageDepth:F1}");
        Console.WriteLine($"  Depth Variance: {analysis.DepthVariance:F1}");
        Console.WriteLine($"  Min Depth: {analysis.MinDepth}");
        Console.WriteLine($"  Max Depth: {analysis.MaxDepth}");
        
        // Debug: Show depth distribution
        var depthRanges = new Dictionary<string, int>
        {
            { "0-50", 0 }, { "51-100", 0 }, { "101-150", 0 }, 
            { "151-200", 0 }, { "201-255", 0 }
        };
        
        for (int i = 0; i < result.Length; i += 4)
        {
            var depth = result[i];
            if (depth <= 50) depthRanges["0-50"]++;
            else if (depth <= 100) depthRanges["51-100"]++;
            else if (depth <= 150) depthRanges["101-150"]++;
            else if (depth <= 200) depthRanges["151-200"]++;
            else depthRanges["201-255"]++;
        }
        
        Console.WriteLine($"  Depth Distribution:");
        foreach (var range in depthRanges)
        {
            Console.WriteLine($"    {range.Key}: {range.Value} pixels");
        }
        
        // Save test image for visual inspection
        SaveTestImage(result, width, height, "test_advanced_organic_analysis.png");
    }

    private void SaveTestImage(byte[] pixelData, int width, int height, string filename)
    {
        try
        {
            using var bitmap = new Bitmap(width, height);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var index = (y * width + x) * 4;
                    var grayValue = pixelData[index];
                    var color = Color.FromArgb(grayValue, grayValue, grayValue);
                    bitmap.SetPixel(x, y, color);
                }
            }
            
            var testOutputDir = Path.Combine(Environment.CurrentDirectory, "TestOutput");
            Directory.CreateDirectory(testOutputDir);
            var filePath = Path.Combine(testOutputDir, filename);
            bitmap.Save(filePath, ImageFormat.Png);
            
            Console.WriteLine($"Test image saved: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not save test image {filename}: {ex.Message}");
        }
    }

    private ImageAnalysis AnalyzeImage(byte[] pixelData, int width, int height)
    {
        var depths = new List<int>();
        var cavityPixels = 0;
        var totalPixels = width * height;

        for (int i = 0; i < pixelData.Length; i += 4)
        {
            var depth = pixelData[i];
            depths.Add(depth);
            
            // Consider pixels with depth < 100 as cavities
            if (depth < 100)
            {
                cavityPixels++;
            }
        }

        var cavityPercentage = (double)cavityPixels / totalPixels;
        var averageDepth = depths.Average();
        var depthVariance = depths.Select(d => Math.Pow(d - averageDepth, 2)).Average();
        var minDepth = depths.Min();
        var maxDepth = depths.Max();

        return new ImageAnalysis
        {
            CavityPercentage = cavityPercentage,
            AverageDepth = averageDepth,
            DepthVariance = depthVariance,
            MinDepth = minDepth,
            MaxDepth = maxDepth
        };
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}

public class ImageAnalysis
{
    public double CavityPercentage { get; set; }
    public double AverageDepth { get; set; }
    public double DepthVariance { get; set; }
    public int MinDepth { get; set; }
    public int MaxDepth { get; set; }
} 