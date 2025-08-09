using MapGen.Core;

namespace MapGen.Service.Services;

public class MapGenerationService : IMapGenerationService
{
    private readonly ILogger<MapGenerationService> _logger;
    private readonly Core.MapGenerationService _coreService;

    public MapGenerationService(ILogger<MapGenerationService> logger)
    {
        _logger = logger;
        _coreService = new Core.MapGenerationService();
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

            // Convert parameters to the format expected by the core service
            var parameters = request.Parameters.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
            );

            // Use the core service to generate the map
            var mapData = await Task.Run(() => 
                _coreService.GenerateMap(request.Algorithm, request.Width, request.Height, seed, parameters)
            );

            return new MapGenerationResult
            {
                Success = true,
                MapData = mapData,
                MapFormat = "raw", // Raw RGBA data
                Seed = seed,
                Algorithm = request.Algorithm,
                AlgorithmVersion = _coreService.GetAlgorithmVersion(request.Algorithm)
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
} 