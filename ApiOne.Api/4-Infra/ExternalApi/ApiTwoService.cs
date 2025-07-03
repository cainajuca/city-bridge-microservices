using ApiOne.Api.Domain.Dtos;
using ApiOne.Api.Domain.Interfaces;

namespace ApiOne.Infra.ApiTwo;
public class ApiTwoService : IApiTwoService
{
    private readonly HttpClient _httpClient;

    public ApiTwoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetMockAsync() => await _httpClient.GetAsync("Bridge/Mock");

    public async Task<string> GetBridgeAsync()
    {
        var response = await _httpClient.GetAsync("Bridge");

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostBridgeAsync(BridgeDto input)
    {
        var bridge = new
        {
            input.Name,
            input.CityId,
        };

        var response = await _httpClient.PostAsJsonAsync("Bridge", bridge);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
