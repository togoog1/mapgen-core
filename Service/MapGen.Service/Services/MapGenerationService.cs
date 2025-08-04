using System.Security.Cryptography;

namespace MapGen.Service.Services;

public class MapGenerationService : IMapGenerationService
{
    private readonly ILogger<MapGenerationService> _logger;

    public MapGenerationService(ILogger<MapGenerationService> logger)
    {
        _logger = logger;
    }

    public async Task<MapGenerationResult> GenerateMapAsync(MapGenerationRequest request)
    {
        var seed = Random.Shared.Next();
        return await GenerateMapWithSeedAsync(request, seed);
    }

    public async Task<MapGenerationResult> GenerateMapWithSeedAsync(MapGenerationRequest request, int seed)
    {
        try
        {
            _logger.LogInformation("Generating map with algorithm: {Algorithm}, size: {Width}x{Height}, seed: {Seed}", 
                request.Algorithm, request.Width, request.Height, seed);

            // TODO: Implement actual map generation logic
            // For now, we'll create a simple placeholder map
            var mapData = await GeneratePlaceholderMapAsync(request.Width, request.Height, seed);

            return new MapGenerationResult
            {
                Success = true,
                MapData = mapData,
                MapFormat = "png",
                Seed = seed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating map");
            return new MapGenerationResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private Task<byte[]> GeneratePlaceholderMapAsync(int width, int height, int seed)
    {
        // This is a placeholder implementation
        // In a real implementation, you would integrate with your mapgen-core library
        var random = new Random(seed);
        var pixels = new byte[width * height * 4]; // RGBA format

        for (int i = 0; i < pixels.Length; i += 4)
        {
            var value = random.Next(0, 256);
            pixels[i] = (byte)value;     // R
            pixels[i + 1] = (byte)value; // G
            pixels[i + 2] = (byte)value; // B
            pixels[i + 3] = 255;         // A
        }

        // TODO: Convert to actual PNG format
        // For now, just return the raw pixel data
        return Task.FromResult(pixels);
    }
} 