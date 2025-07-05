using ApiOne.Api.Application.Services.UseCases;
using ApiOne.Api.Infra.ApiTwo;
using ApiOne.Api.Infra.Database;
using ApiOne.Api.Domain.Interfaces;
using ApiOne.Infra.ApiTwo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiTwoOptions>(builder.Configuration.GetSection("ExternalApi"));
builder.Services
    .AddHttpClient<IApiTwoService, ApiTwoService>((serviceProvider, client) =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<ApiTwoOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl);
    })
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.ShouldHandle = args => new ValueTask<bool>(
            // Retry on network exceptions or timeouts
            args.Outcome.Exception != null
            // Retry only for transient status codes: 408, 429, 502, 503, 504
            || (args.Outcome.Result is HttpResponseMessage resp
                && (resp.StatusCode == HttpStatusCode.RequestTimeout     // 408
                 || resp.StatusCode == (HttpStatusCode)429               // Too Many Requests
                 || resp.StatusCode == HttpStatusCode.BadGateway         // 502
                 || resp.StatusCode == HttpStatusCode.ServiceUnavailable // 503
                 || resp.StatusCode == HttpStatusCode.GatewayTimeout)    // 504
            )
        );
    });

builder.Services.AddScoped<ICityService, CityService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
