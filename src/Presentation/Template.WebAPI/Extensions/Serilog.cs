using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Template.WebAPI.Extensions;

public static class Serilog
{
    /// <summary>
    /// Levels and overrides come from the <c>Serilog</c> section of
    /// configuration, so verbosity is an appsettings/environment change rather
    /// than a code change. The database sink is optional: without a
    /// <c>LoggingDb</c> connection string it logs to the console alone, which
    /// is what tests and a bare `dotnet run` want.
    /// </summary>
    public static IServiceCollection AddSerilogServices(this IServiceCollection services, IConfiguration configuration)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console();

        var connectionString = configuration.GetConnectionString("LoggingDb");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            var columnWriters = new Dictionary<string, ColumnWriterBase>
            {
                { "message", new RenderedMessageColumnWriter() },
                { "level", new LevelColumnWriter(true, NpgsqlTypes.NpgsqlDbType.Varchar) },
                { "timestamp", new TimestampColumnWriter(NpgsqlTypes.NpgsqlDbType.TimestampTz) },
                { "exception", new ExceptionColumnWriter() },
                { "properties", new PropertiesColumnWriter(NpgsqlTypes.NpgsqlDbType.Jsonb) }
            };

            loggerConfiguration.WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: "logs",
                columnOptions: columnWriters,
                needAutoCreateTable: true);
        }

        Log.Logger = loggerConfiguration.CreateLogger();
        return services;
    }
}
