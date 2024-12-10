using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RateLimiter.API.Middleware;
using RateLimiter.Application;
using Domain.Interfaces;
using RateLimiter.Domain;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Start();

void Start()
{
    try
    {
        Log.Information("Starting web application...");
        Log.Information("Building the app:");
        var app = Build();
        Log.Information("Configuring the app:");
        Configure(app);
        app.MapControllers();

        app.Run();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
    }
    finally
    {   
        Log.CloseAndFlush();
    }
}

WebApplication Build()
{
    var builder = WebApplication.CreateBuilder(args).AddLoggingMiddleware();
    var environmentName = builder.Environment.EnvironmentName;
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    builder.Configuration.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
    builder.Configuration.AddEnvironmentVariables();

    // Register DistributedMemoryCache
    builder.Services.AddDistributedMemoryCache();

    builder.Services.AddEndpointsApiExplorer();

    if (Environment.GetEnvironmentVariable("SwaggerGen") is not "true" && environmentName != "IntegrationTests")
    {
        // Add logging
        AddLogging(builder);
    }

    // To inject List<IRateLimitRule>
    builder.Services.AddScoped(provider =>
    {
        return provider.GetServices<IRateLimitRule>().ToList();
    });

    // Add services to the container and configure urls to use lowercase with slugs instead of camelcase.
    builder.Services.AddScoped<RateLimiterService>();
    builder.Services.AddControllers();

    // Add the swagger generator
    AddSwaggerGen(builder);
    return builder.Build();
}

// Setup app insights and any other logging
void AddLogging(WebApplicationBuilder builder)
{
    builder.Logging.ClearProviders();
    if (builder.Environment.IsDevelopment())
    {
        builder.Logging.AddConsole();
    }
    else
    {
        builder.Logging.AddJsonConsole();
    }

    builder.Logging.AddSerilog(Log.Logger);
}

void Configure(WebApplication app)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Rate Limiter API V1");
    });
    // Add swagger
    RegisterSwaggerMiddleware(app);
    // Configure the HTTP request pipeline.
    app.UseAuthorization();
    app.UseHttpsRedirection();
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseHsts();
    }
}

void RegisterSwaggerMiddleware(WebApplication app)
{
    // Register the Swagger Middleware
    app.UseSwagger();

    // Register the Swagger UI Middleware
    app.UseSwaggerUI(options =>
    {
    options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Rate Limiter V1");
    options.DefaultModelRendering(ModelRendering.Model);
    options.DefaultModelExpandDepth(1);
    });
}

void AddSwaggerGen(WebApplicationBuilder builder)
{
    // Register the Swagger generator to the service collection
    builder.Services.AddSwaggerGen(swaggerGenOptions =>
    {
        swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown version",
            Title = "RateLimiter - /rate-limiter/request",
            Description =
                "This service provides rate limiting capability"
        });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        swaggerGenOptions.OperationFilter<SwaggerCallSourceFilter>();
        swaggerGenOptions.IncludeXmlComments(xmlPath);
        // We're replacing '+' with '.' because inner classes are generated with a plus
        // and that doesn't play nice with the gateway promotion tool.
        swaggerGenOptions.CustomSchemaIds(selector => selector?.FullName?.Replace('+', '.'));
    });
}

namespace API
{
    /// <summary>
    /// Program Class used for e2e testing
    /// </summary>
    public partial class Program
    {
    }
}
