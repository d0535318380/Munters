using HealthChecks.ApplicationStatus.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Munters.Giphy;
using Munters.Host.Api.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var redisOptions = builder.Configuration
    .GetSection(nameof(RedisCacheOptions));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddProblemDetails()
    .AddOpenApi()
    .AddGiphyServices();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisOptions.GetValue<string>("ConnectionString");
    options.InstanceName = redisOptions.GetValue<string>("InstanceName");
});
builder.Services
    .AddHealthChecks()
    .AddApplicationStatus()
    .AddRedis(redisOptions.GetValue<string>("ConnectionString")!, "redis")
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGiphyEndpoints();

await app.RunAsync().ConfigureAwait(false);