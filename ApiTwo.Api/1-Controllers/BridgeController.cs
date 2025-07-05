using ApiTwo.Api._2_Application.Services.UseCases;
using ApiTwo.Api._3_Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ApiTwo.Api._1_Controllers;

[ApiController]
[Route("[controller]")]
public class BridgeController : ControllerBase
{
    private readonly IBridgeService _bridgeService;
    public BridgeController(IBridgeService bridgeService)
    {
        _bridgeService = bridgeService;
    }

    [HttpGet("Mock")]
    public IActionResult MockEndpoint()
    {
        var responses = new List<Func<IActionResult>>
        {
            () => Ok(),
            () => StatusCode(503, "Service Unavailable"),
            () => StatusCode(504, "Timeout error"),
            () => StatusCode(500, "Internal Server Error") // not transient error
        };

        var rnd = new Random();
        var response = responses[rnd.Next(responses.Count)];

        return response();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBridges()
    {
        var cities = await _bridgeService.GetAllBridgesAsync();
        return Ok(cities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBridgeById(Guid id)
    {
        var bridge = await _bridgeService.GetBridgeByIdAsync(id);
        if (bridge == null)
        {
            return NotFound();
        }
        return Ok(bridge);
    }

    [HttpPost]
    public async Task<IActionResult> AddBridge([FromBody] BridgeDto bridge)
    {
        var result = await _bridgeService.AddBridgeAsync(bridge);
        return CreatedAtAction(nameof(GetBridgeById), new { id = result.Id }, bridge);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBridge(Guid id, [FromBody] BridgeWithIdDto bridge)
    {
        bridge.Id = id;
        await _bridgeService.UpdateBridgeAsync(bridge);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBridge(Guid id)
    {
        await _bridgeService.DeleteBridgeAsync(id);
        return NoContent();
    }
}
