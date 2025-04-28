using Turis.Sights;
using Microsoft.AspNetCore.Mvc;

namespace Turis.Controllers;

[ApiController]
[Route("places/[action]")]
public class SightsController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly Config _config;
    private readonly ILogger<SightsController> _logger;

    public SightsController(HttpClient httpClient, Config config, ILogger<SightsController> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    // Обработка пример GET /places/sights?radius=1000&lng=-117.000165&lat=46.732387
    [HttpGet]
    [Produces("text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Sights(
        [FromQuery] string lng,
        [FromQuery] string lat,
        [FromQuery] string radius)
    {
        if (string.IsNullOrEmpty(lng) || string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(radius))
        {
            _logger.LogWarning("Invalid parameters for sights request");
            return StatusCode(400, "lng, lat, and radius are required");
        }

        _logger.LogInformation($"Request for sights at lat: {lat}, lng: {lng}, radius: {radius}");

        try
        {
            var response = await SightsApi.GetSights(lng, lat, _httpClient, _config, radius);
            var responseString = await response.Content.ReadAsStringAsync();

            return Content(responseString, "text/plain; charset=utf-8");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing sights request");
            return StatusCode(500, "Internal server error");
        }
    }
}