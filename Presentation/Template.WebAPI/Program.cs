using Serilog;
using Template.Application;
using Template.Infrastructure;
using Template.Persistence;
using Template.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices()
    .AddRateLimiterServices()
    .AddPersistenceServices(builder.Configuration)
    .AddOpenTelemetryServices()
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddSerilogServices(builder.Configuration);
builder.Host.UseSerilog();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerMiddleware();
}
app.UseRateLimiter();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
