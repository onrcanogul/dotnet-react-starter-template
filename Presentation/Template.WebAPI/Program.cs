using Serilog;
using Template.Application;
using Template.Infrastructure;
using Template.Persistence;
using Template.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerServices()
    .AddLocalizationServices()
    .AddCorsServices()
    .AddJsonSerializerServices()
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
    app.UseSwaggerMiddleware();
}
app.UseLocalizationServices();
app.UseInfrastructureServices();
app.UseCorsServices();
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();
