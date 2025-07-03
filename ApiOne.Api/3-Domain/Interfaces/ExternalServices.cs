using ApiOne.Api.Domain.Dtos;

namespace ApiOne.Api.Domain.Interfaces;
public interface IApiTwoService
{
    Task<HttpResponseMessage> GetMockAsync();
    Task<string> GetBridgeAsync();
    Task<string> PostBridgeAsync(BridgeDto input);
}