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

    [HttpGet("Mock")]
    public async Task<IActionResult> GetMock()
    {
        var response = await _apiTwoService.GetMockAsync();

        if (response.IsSuccessStatusCode)
            return Ok();

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            return StatusCode(500, new { error = "Internal server error." });

        return StatusCode(503, new { error = "Exceeded retry attempts." });
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
