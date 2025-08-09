namespace MapGen.Service.Services;

public interface IMapGenerationService
{
    Task<MapGenerationResult> GenerateMapAsync(MapGenerationRequest request);
    Task<MapGenerationResult> GenerateMapWithSeedAsync(MapGenerationRequest request, int seed);
}

public record MapGenerationRequest
{
    public int Width { get; init; } = 100;
    public int Height { get; init; } = 100;
    public string Algorithm { get; init; } = "perlin";
    public Dictionary<string, object> Parameters { get; init; } = new();
}

public record MapGenerationResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public byte[]? MapData { get; init; }
    public string? MapFormat { get; init; }
    public int Seed { get; init; }
    public string? Algorithm { get; init; }
    public string? AlgorithmVersion { get; init; }
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
} 