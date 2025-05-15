using Microsoft.AspNetCore.Mvc;
using Turis.Location;

namespace Turis.Controllers;

[ApiController]
[Route("[controller]")]
public class PlacesController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly Config _config;
    private readonly ILogger<PlacesController> _logger;

    public PlacesController(HttpClient httpClient, Config config, ILogger<PlacesController> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }
    
    // Обработка пример GET /places?location=Москва
    [HttpGet]
    [Produces("text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPlaces([FromQuery] string location)
    {
        if (string.IsNullOrEmpty(location))
        {
            _logger.LogWarning("Empty location parameter received");
            return StatusCode(400, "Location parameter is required");
        }

        _logger.LogInformation($"Request for location: {location}");

        try
        {
            var response = await LocationApi.GetPlaces(location, _httpClient, _config);
            var responseString = await response.Content.ReadAsStringAsync();

            return Content(responseString, "text/plain; charset=utf-8");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing location request for {location}");
            return StatusCode(500, "Internal server error");
        }
    }
}