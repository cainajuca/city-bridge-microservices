using ApiTwo.Api._2_Application.Services.UseCases;
using ApiTwo.Api._4_Infra.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

ConfigureLogging(builder.Configuration);
builder.Host.UseSerilog();

builder.Services.AddScoped<IBridgeService, BridgeService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

ConfigurePipeline(app);

app.Run();

static void ConfigureLogging(IConfiguration configuration)
{
    string env = configuration["ASPNETCORE_ENVIRONMENT"]!;

    var elasticUri = new Uri(configuration["ElasticConfiguration:Uri"]!);

    string indexFormat = $"apitwo-logs-{env.ToLower()}-{DateTime.UtcNow:yyyy-MM}";

    var elasticsearchConfiguration = new ElasticsearchSinkOptions(elasticUri)
    {
        AutoRegisterTemplate = true,
        IndexFormat = indexFormat,
        NumberOfReplicas = 1,
        NumberOfShards = 2,
    };

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .Enrich.WithProperty("ServiceName", "apitwo")
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(elasticsearchConfiguration)
        .Enrich.WithProperty("Environment", env)
        .CreateLogger();
}

static void ConfigurePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.MapControllers();
}
