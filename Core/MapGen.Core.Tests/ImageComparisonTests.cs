using Xunit;
using MapGen.Core;
using System.Drawing;
using System.Drawing.Imaging;

namespace MapGen.Core.Tests;

public class ImageComparisonTests : IDisposable
{
    private MapGenerationService _service;

    public ImageComparisonTests()
    {
        _service = new MapGenerationService();
    }

    [Fact]
    public void TestImageComparison_ReferencePattern()
    {
        // Generate multiple variations and compare them
        var testCases = new[]
        {
            new { Name = "reference_like", Parameters = new Dictionary<string, object>
            {
                { "cavityDensity", 0.4 },
                { "cavityDepth", 0.8 },
                { "wallThickness", 0.15 },
                { "organicVariation", 0.6 },
                { "branchingFactor", 0.7 },
                { "textureDetail", 0.5 },
                { "smoothingPasses", 4 }
            }},
            new { Name = "high_contrast", Parameters = new Dictionary<string, object>
            {
                { "cavityDensity", 0.5 },
                { "cavityDepth", 0.9 },
                { "wallThickness", 0.1 },
                { "organicVariation", 0.8 },
                { "branchingFactor", 0.8 },
                { "textureDetail", 0.7 },
                { "smoothingPasses", 6 }
            }},
            new { Name = "natural_organic", Parameters = new Dictionary<string, object>
            {
                { "cavityDensity", 0.35 },
                { "cavityDepth", 0.75 },
                { "wallThickness", 0.2 },
                { "organicVariation", 0.7 },
                { "branchingFactor", 0.6 },
                { "textureDetail", 0.6 },
                { "smoothingPasses", 5 }
            }}
        };

        var width = 512;
        var height = 512;
        var seed = 12345;

        foreach (var testCase in testCases)
        {
            // Generate image
            var result = _service.GenerateMap("advanced-organic", width, height, seed, testCase.Parameters);
            
            // Analyze and save
            var analysis = AnalyzeImage(result, width, height);
            SaveTestImage(result, width, height, $"comparison_{testCase.Name}.png");
            
            Console.WriteLine($"\n{testCase.Name} Analysis:");
            Console.WriteLine($"  Cavity Percentage: {analysis.CavityPercentage:P}");
            Console.WriteLine($"  Average Depth: {analysis.AverageDepth:F1}");
            Console.WriteLine($"  Depth Variance: {analysis.DepthVariance:F1}");
            Console.WriteLine($"  Min Depth: {analysis.MinDepth}");
            Console.WriteLine($"  Max Depth: {analysis.MaxDepth}");
            
            // Quality checks for reference-like patterns
            Assert.True(analysis.CavityPercentage > 0.25 && analysis.CavityPercentage < 0.65, 
                $"Cavity percentage {analysis.CavityPercentage:P} should be between 25% and 65% for natural patterns");
            Assert.True(analysis.DepthVariance > 2000, 
                $"Depth variance {analysis.DepthVariance:F1} should be > 2000 for good contrast");
        }
    }

    [Fact]
    public void TestImageComparison_ParameterOptimization()
    {
        // Test parameter ranges to find optimal settings
        var cavityDensities = new[] { 0.3, 0.4, 0.5, 0.6 };
        var cavityDepths = new[] { 0.7, 0.8, 0.9 };
        var organicVariations = new[] { 0.5, 0.6, 0.7, 0.8 };
        
        var width = 256;
        var height = 256;
        var seed = 54321;
        var bestScore = 0.0;
        var bestParameters = new Dictionary<string, object>();

        foreach (var density in cavityDensities)
        {
            foreach (var depth in cavityDepths)
            {
                foreach (var variation in organicVariations)
                {
                    var parameters = new Dictionary<string, object>
                    {
                        { "cavityDensity", density },
                        { "cavityDepth", depth },
                        { "organicVariation", variation },
                        { "wallThickness", 0.15 },
                        { "branchingFactor", 0.7 },
                        { "textureDetail", 0.5 },
                        { "smoothingPasses", 4 }
                    };

                    var result = _service.GenerateMap("advanced-organic", width, height, seed, parameters);
                    var analysis = AnalyzeImage(result, width, height);
                    
                    // Calculate quality score based on reference image characteristics
                    var score = CalculateQualityScore(analysis);
                    
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestParameters = parameters;
                    }
                    
                    Console.WriteLine($"Density: {density}, Depth: {depth}, Variation: {variation}, Score: {score:F3}");
                }
            }
        }

        Console.WriteLine($"\nBest parameters found:");
        foreach (var param in bestParameters)
        {
            Console.WriteLine($"  {param.Key}: {param.Value}");
        }
        Console.WriteLine($"Best score: {bestScore:F3}");

        // Generate final image with best parameters
        var finalResult = _service.GenerateMap("advanced-organic", width, height, seed, bestParameters);
        SaveTestImage(finalResult, width, height, "comparison_optimized.png");
    }

    [Fact]
    public void TestImageComparison_ResolutionScaling()
    {
        // Test how well the algorithm scales to different resolutions
        var resolutions = new[] { 256, 512, 1024 };
        var parameters = new Dictionary<string, object>
        {
            { "cavityDensity", 0.4 },
            { "cavityDepth", 0.8 },
            { "wallThickness", 0.15 },
            { "organicVariation", 0.6 },
            { "branchingFactor", 0.7 },
            { "textureDetail", 0.5 },
            { "smoothingPasses", 4 }
        };
        var seed = 99999;

        foreach (var resolution in resolutions)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = _service.GenerateMap("advanced-organic", resolution, resolution, seed, parameters);
            stopwatch.Stop();

            var analysis = AnalyzeImage(result, resolution, resolution);
            SaveTestImage(result, resolution, resolution, $"comparison_resolution_{resolution}.png");

            Console.WriteLine($"\nResolution {resolution}x{resolution}:");
            Console.WriteLine($"  Generation time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"  Cavity Percentage: {analysis.CavityPercentage:P}");
            Console.WriteLine($"  Average Depth: {analysis.AverageDepth:F1}");
            Console.WriteLine($"  Depth Variance: {analysis.DepthVariance:F1}");

            // Performance should scale reasonably
            var expectedTime = resolution * resolution / 1000.0; // Rough estimate
            Assert.True(stopwatch.ElapsedMilliseconds < expectedTime * 2, 
                $"Generation time {stopwatch.ElapsedMilliseconds}ms should be reasonable for {resolution}x{resolution}");
        }
    }

    [Fact]
    public void TestImageComparison_SeedVariation()
    {
        // Test how different seeds affect the output quality
        var seeds = new[] { 11111, 22222, 33333, 44444, 55555 };
        var parameters = new Dictionary<string, object>
        {
            { "cavityDensity", 0.4 },
            { "cavityDepth", 0.8 },
            { "wallThickness", 0.15 },
            { "organicVariation", 0.6 },
            { "branchingFactor", 0.7 },
            { "textureDetail", 0.5 },
            { "smoothingPasses", 4 }
        };
        var width = 512;
        var height = 512;

        var analyses = new List<ImageAnalysis>();

        foreach (var seed in seeds)
        {
            var result = _service.GenerateMap("advanced-organic", width, height, seed, parameters);
            var analysis = AnalyzeImage(result, width, height);
            analyses.Add(analysis);
            
            SaveTestImage(result, width, height, $"comparison_seed_{seed}.png");

            Console.WriteLine($"\nSeed {seed}:");
            Console.WriteLine($"  Cavity Percentage: {analysis.CavityPercentage:P}");
            Console.WriteLine($"  Average Depth: {analysis.AverageDepth:F1}");
            Console.WriteLine($"  Depth Variance: {analysis.DepthVariance:F1}");
        }

        // Check consistency across seeds
        var cavityPercentages = analyses.Select(a => a.CavityPercentage).ToList();
        var avgCavityPercentage = cavityPercentages.Average();
        var cavityVariance = cavityPercentages.Select(p => Math.Pow(p - avgCavityPercentage, 2)).Average();

        Console.WriteLine($"\nSeed Variation Analysis:");
        Console.WriteLine($"  Average Cavity Percentage: {avgCavityPercentage:P}");
        Console.WriteLine($"  Cavity Percentage Variance: {cavityVariance:P}");

        // Variance should be reasonable (not too high, not too low)
        Assert.True(cavityVariance < 0.1, 
            $"Cavity percentage variance {cavityVariance:P} should be < 10% for consistent quality");
    }

    private double CalculateQualityScore(ImageAnalysis analysis)
    {
        // Score based on reference image characteristics
        // Reference image has:
        // - Cavity percentage around 40-50%
        // - Good contrast (high variance)
        // - Balanced depth distribution
        
        var cavityScore = 1.0 - Math.Abs(analysis.CavityPercentage - 0.45); // Target 45%
        var varianceScore = Math.Min(analysis.DepthVariance / 5000.0, 1.0); // Normalize variance
        var depthScore = 1.0 - Math.Abs(analysis.AverageDepth - 127.0) / 127.0; // Target middle gray
        
        return (cavityScore + varianceScore + depthScore) / 3.0;
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