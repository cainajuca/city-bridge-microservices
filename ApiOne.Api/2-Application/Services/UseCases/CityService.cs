using ApiOne.Api.Domain.Dtos;
using ApiOne.Api.Domain.Entities;
using ApiOne.Api.Infra.Database;

namespace ApiOne.Api.Application.Services.UseCases;

public interface ICityService
{
    Task<IEnumerable<City>> GetAllCitiesAsync();
    Task<City> GetCityByIdAsync(Guid id);
    Task<City> AddCityAsync(CityDto city);
    Task UpdateCityAsync(CityDto city);
    Task DeleteCityAsync(Guid id);
}

public class CityService : ICityService
{
    private readonly AppDbContext _db;

    public CityService(AppDbContext db)
    {
        _db = db; // TODO: implement Repository pattern
    }

    public Task<IEnumerable<City>> GetAllCitiesAsync()
    {
        return Task.FromResult(_db.Cities.AsEnumerable());
    }

    public Task<City> GetCityByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<City> AddCityAsync(CityDto city)
    {
        var newCity = new City
        {
            Name = city.Name,
        };

        _db.Cities.Add(newCity);

        await _db.SaveChangesAsync();

        return newCity;
    }
    
    public Task UpdateCityAsync(CityDto city)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCityAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
