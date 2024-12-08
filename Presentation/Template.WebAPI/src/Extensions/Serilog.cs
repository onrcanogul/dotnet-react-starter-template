using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Template.WebAPI.Extensions;

public static class Serilog
{
    public static IServiceCollection AddSerilogServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LoggingDb");
        var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter() },  // log message
            { "level", new LevelColumnWriter(true, NpgsqlTypes.NpgsqlDbType.Varchar) }, // log level
            { "timestamp", new TimestampColumnWriter(NpgsqlTypes.NpgsqlDbType.TimestampTz) }, // time
            { "exception", new ExceptionColumnWriter() }, // error message
            { "properties", new PropertiesColumnWriter(NpgsqlTypes.NpgsqlDbType.Jsonb) } // props
        };
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console() // write to console
            .WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: "logs", // table name
                columnOptions: columnWriters, // columns
                needAutoCreateTable: true // auto create
            ).CreateLogger();
        return services;
    }
}