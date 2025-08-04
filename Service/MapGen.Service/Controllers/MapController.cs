using Microsoft.AspNetCore.Mvc;
using MapGen.Service.Services;
using MapGen.Core;

namespace MapGen.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MapController : ControllerBase
{
    private readonly IMapGenerationService _mapGenerationService;
    private readonly ILogger<MapController> _logger;

    public MapController(IMapGenerationService mapGenerationService, ILogger<MapController> logger)
    {
        _mapGenerationService = mapGenerationService;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateMap([FromBody] MapGenerationRequest request)
    {
        try
        {
            var result = await _mapGenerationService.GenerateMapAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                seed = result.Seed,
                format = result.MapFormat,
                generatedAt = result.GeneratedAt,
                data = Convert.ToBase64String(result.MapData!)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in map generation endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("generate/{seed:int}")]
    public async Task<IActionResult> GenerateMapWithSeed(int seed, [FromBody] MapGenerationRequest request)
    {
        try
        {
            var result = await _mapGenerationService.GenerateMapWithSeedAsync(request, seed);
            
            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                seed = result.Seed,
                format = result.MapFormat,
                generatedAt = result.GeneratedAt,
                data = Convert.ToBase64String(result.MapData!)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in map generation endpoint with seed");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    [HttpGet("algorithms")]
    public IActionResult GetAlgorithms()
    {
        try
        {
            var coreService = new Core.MapGenerationService();
            var algorithms = coreService.GetAvailableAlgorithms().ToList();
            
            var algorithmDetails = algorithms.Select(algorithm => new
            {
                name = algorithm,
                defaultParameters = coreService.GetDefaultParameters(algorithm)
            });

            return Ok(new
            {
                algorithms = algorithmDetails
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting algorithms");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
} 