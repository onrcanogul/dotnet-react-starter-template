using Microsoft.Extensions.Configuration;

namespace Template.Shared.Configuration;

/// <summary>
/// Fail-fast accessors for required configuration.
///
/// A missing connection string or secret must stop the app at startup with a
/// message naming the key - never surface later as a null reference somewhere
/// deep in the request pipeline.
/// </summary>
public static class ConfigurationGuard
{
    /// <summary>Reads a required setting, or throws naming the missing key.</summary>
    public static string GetRequired(this IConfiguration configuration, string key)
        => configuration[key] is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException(
                $"Required configuration '{key}' is missing or empty. " +
                $"Set it via user-secrets, environment variables or appsettings.");

    /// <summary>Reads a required connection string, or throws naming the missing key.</summary>
    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
        => configuration.GetConnectionString(name) is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException(
                $"Required connection string 'ConnectionStrings:{name}' is missing or empty. " +
                $"Set it via user-secrets, environment variables or appsettings.");
}
