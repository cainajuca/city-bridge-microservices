using ApiTwo.Api._2_Application.Services.UseCases;
using ApiTwo.Api._3_Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using System.Diagnostics;

namespace ApiTwo.Api._1_Controllers;

[ApiController]
[Route("[controller]")]
public class BridgeController : ControllerBase
{
    private readonly IBridgeService _bridgeService;
    private readonly ILogger<BridgeController> _logger;

    public BridgeController(
        IBridgeService bridgeService,
        ILogger<BridgeController> logger)
    {
        _bridgeService = bridgeService;
        _logger = logger;
    }

    [HttpGet("Mock")]
    public IActionResult MockEndpoint()
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? "no-trace";

        _logger.LogInformation("Received Mock request – TraceId: {TraceId}", traceId);

        var (code, message, level) = _responses[Random.Shared.Next(_responses.Length)];

        switch (level)
        {
            case LogEventLevel.Information:
                _logger.LogInformation("Responding with {StatusCode} — {Message}", code, message);
                break;
            case LogEventLevel.Warning:
                _logger.LogWarning("Responding with {StatusCode} — {Message}", code, message);
                break;
            case LogEventLevel.Error:
                _logger.LogError("Responding with {StatusCode} — {Message}", code, message);
                break;
        }

        return StatusCode(code, message);
    }

    private static readonly (int Code, string Message, LogEventLevel Level)[] _responses =
    [
        (200, "OK", LogEventLevel.Information),
        (503, "Service Unavailable", LogEventLevel.Warning),
        (504, "Timeout error", LogEventLevel.Warning),
        (500, "Internal Server Error", LogEventLevel.Error)
    ];

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
