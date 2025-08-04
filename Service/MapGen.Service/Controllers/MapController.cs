using Microsoft.AspNetCore.Mvc;
using MapGen.Service.Services;
using MapGen.Service.Models;
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
            // Validate resolution limits
            if (request.Width > 2048 || request.Height > 2048)
            {
                return BadRequest(new { error = "Maximum resolution is 2048x2048" });
            }

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
                data = Convert.ToBase64String(result.MapData!),
                width = request.Width,
                height = request.Height,
                algorithm = request.Algorithm
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
            // Validate resolution limits
            if (request.Width > 2048 || request.Height > 2048)
            {
                return BadRequest(new { error = "Maximum resolution is 2048x2048" });
            }

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
                data = Convert.ToBase64String(result.MapData!),
                width = request.Width,
                height = request.Height,
                algorithm = request.Algorithm
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

    [HttpPost("generate/high-res")]
    public async Task<IActionResult> GenerateHighResolutionMap([FromBody] MapGenerationRequest request)
    {
        try
        {
            // Validate high-resolution limits
            if (request.Width > 2048 || request.Height > 2048)
            {
                return BadRequest(new { error = "Maximum resolution is 2048x2048" });
            }

            if (request.Width < 512 || request.Height < 512)
            {
                return BadRequest(new { error = "Minimum resolution for high-res is 512x512" });
            }

            _logger.LogInformation("Generating high-resolution map: {Width}x{Height}, Algorithm: {Algorithm}", 
                request.Width, request.Height, request.Algorithm);

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
                data = Convert.ToBase64String(result.MapData!),
                width = request.Width,
                height = request.Height,
                algorithm = request.Algorithm,
                resolution = $"{request.Width}x{request.Height}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in high-resolution map generation endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("generate/batch")]
    public async Task<IActionResult> GenerateBatchMaps([FromBody] BatchMapGenerationRequest request)
    {
        try
        {
            if (request.Count > 10)
            {
                return BadRequest(new { error = "Maximum batch size is 10" });
            }

            var results = new List<object>();
            
            for (int i = 0; i < request.Count; i++)
            {
                var seed = request.BaseSeed + i;
                var result = await _mapGenerationService.GenerateMapWithSeedAsync(request.Request, seed);
                
                if (result.Success)
                {
                    results.Add(new
                    {
                        index = i,
                        seed = result.Seed,
                        format = result.MapFormat,
                        generatedAt = result.GeneratedAt,
                        data = Convert.ToBase64String(result.MapData!),
                        width = request.Request.Width,
                        height = request.Request.Height,
                        algorithm = request.Request.Algorithm
                    });
                }
            }

            return Ok(new
            {
                success = true,
                count = results.Count,
                results = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch map generation endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
} 