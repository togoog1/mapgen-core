using MapGen.Service.Services;

namespace MapGen.Service.Models;

public class BatchMapGenerationRequest
{
    public MapGenerationRequest Request { get; set; } = new();
    public int Count { get; set; } = 1;
    public int BaseSeed { get; set; } = 0;
} 