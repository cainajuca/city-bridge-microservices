using ApiOne.Api.Application.Services.UseCases;
using ApiOne.Api.Infra.ApiTwo;
using ApiOne.Api.Infra.Database;
using ApiOne.Api.Domain.Interfaces;
using ApiOne.Infra.ApiTwo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiTwoOptions>(builder.Configuration.GetSection("ExternalApi"));
builder.Services
    .AddHttpClient<IApiTwoService, ApiTwoService>((serviceProvider, client) =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<ApiTwoOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl);
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
