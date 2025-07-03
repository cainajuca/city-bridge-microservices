using ApiOne.Api.Application.Services.UseCases;
using ApiOne.Api.Domain.Dtos;
using ApiOne.Api.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiOne.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CityController : ControllerBase
{
    private readonly ILogger<CityController> _logger;
    private readonly ICityService _cityService;
    private readonly IApiTwoService _apiTwoService;

    public CityController(
        ILogger<CityController> logger,
        ICityService cityService,
        IApiTwoService apiTwoService)
    {
        _logger = logger;
        _cityService = cityService;
        _apiTwoService = apiTwoService;
    }

    /*
    Next steps:
    - Consolidate this API into a single project and ensure it runs (done)
    - Git init and push this code (done)
    - Apply the same changes to ApiTwo and get it 100% working
    - Move ApiTwo into a second project within this same repository and push second commit
    - Run both in parallel using docker-compose
        - Check the docker-compose.yml file already in the repository (keep it inside ApiOne)
    - Implement a Circuit Breaker using Polly
    */

    [HttpGet("Mock")]
    public async Task<IActionResult> GetMock()
    {
        for (int i = 0; i < 3; i++)
        {
            var result = await _apiTwoService.GetMockAsync();
            
            if (result.IsSuccessStatusCode)
                return Ok();

            if (result.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                _logger.LogError("Gateway timeout occurred while calling the Catalog endpoint. Retrying attempt {Attempt}.", i + 1);

                // Exponential back-off: wait 2^i seconds before retrying to give the downstream service time to recover
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));

                continue;
            }

            if (result.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogError("Service unavailable when calling the Catalog endpoint. Retrying attempt {Attempt}.", i + 1);

                // Exponential back-off: wait 2^i seconds before retrying to give the downstream service time to recover
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
                
                continue;
            }

            if (result.StatusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError("Internal server error received from the Catalog endpoint. Circuit breaker may be activated. Retrying attempt {Attempt}.", i + 1);
                return StatusCode(500, new { error = "Internal server error." });
            }
        }

        return StatusCode(500, new { error = "Exceeded retry attempts." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCities()
    {
        var cities = await _cityService.GetAllCitiesAsync();
        return Ok(cities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCityById(Guid id)
    {
        var city = await _cityService.GetCityByIdAsync(id);
        if (city == null)
        {
            return NotFound();
        }
        return Ok(city);
    }

    [HttpPost]
    public async Task<IActionResult> AddCity([FromBody] CityDto city)
    {
        var result = await _cityService.AddCityAsync(city);
        return CreatedAtAction(nameof(GetCityById), new { id = result.Id }, city);
    }

    [HttpPost("{id}/Bridge")]
    public async Task<IActionResult> AddBridge([FromBody] BridgeDto input)
    {
        await _apiTwoService.PostBridgeAsync(input);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCity(Guid id, [FromBody] CityDto city)
    {
        // city.Id = id;
        await _cityService.UpdateCityAsync(city);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        await _cityService.DeleteCityAsync(id);
        return NoContent();
    }
}
