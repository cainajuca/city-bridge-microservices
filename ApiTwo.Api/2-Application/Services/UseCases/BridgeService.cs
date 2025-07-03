using ApiTwo.Api._3_Domain.Dtos;
using ApiTwo.Api._3_Domain.Entities;
using ApiTwo.Api._4_Infra.Database;

namespace ApiTwo.Api._2_Application.Services.UseCases;
public interface IBridgeService
{
    Task<IEnumerable<Bridge>> GetAllBridgesAsync();
    Task<Bridge> GetBridgeByIdAsync(Guid id);
    Task<Bridge> AddBridgeAsync(BridgeDto bridge);
    Task UpdateBridgeAsync(BridgeWithIdDto bridge);
    Task DeleteBridgeAsync(Guid id);
}

public class BridgeService : IBridgeService
{
    private readonly AppDbContext _db;

    public BridgeService(AppDbContext db)
    {
        _db = db; // TODO: implement Repository pattern
    }

    public Task<IEnumerable<Bridge>> GetAllBridgesAsync()
    {
        return Task.FromResult(_db.Bridges.AsEnumerable());
    }

    public Task<Bridge> GetBridgeByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Bridge> AddBridgeAsync(BridgeDto bridge)
    {
        var newBridge = new Bridge
        {
            Name = bridge.Name,
            CityId = bridge.CityId,
        };

        _db.Bridges.Add(newBridge);

        await _db.SaveChangesAsync();

        return newBridge;
    }

    public Task UpdateBridgeAsync(BridgeWithIdDto bridge)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBridgeAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}

