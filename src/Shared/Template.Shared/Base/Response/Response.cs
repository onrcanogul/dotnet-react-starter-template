using System.Text.Json.Serialization;

namespace Template.Shared.Base.Response;

/// <summary>
/// The single envelope every service operation returns. Controllers hand it to
/// <c>BaseController.ApiResult(...)</c>, which unwraps <see cref="ServiceResponse.StatusCode"/>.
/// Failures normally travel as exceptions (see <c>Template.Shared.Exceptions</c>) and are
/// turned into a failed response by the global exception handler.
/// </summary>
public class ServiceResponse<T> : ServiceResponse
{
    public T? Data { get; private set; }

    private ServiceResponse() { }

    public static ServiceResponse<T> Success(T data, int statusCode)
        => new() { Data = data, StatusCode = statusCode, IsSuccessful = true };

    /// <summary>Success with no payload - use <c>ServiceResponse&lt;NoContent&gt;</c> as the declared type.</summary>
    public new static ServiceResponse<T> Success(int statusCode)
        => new() { StatusCode = statusCode, IsSuccessful = true };

    public new static ServiceResponse<T> Failure(List<string> errors, int statusCode)
        => new() { Errors = errors, StatusCode = statusCode, IsSuccessful = false };

    public new static ServiceResponse<T> Failure(string error, int statusCode)
        => new() { Errors = [error], StatusCode = statusCode, IsSuccessful = false };
}
public class ServiceResponse
{
    public List<string> Errors { get; set; } = new();

    [JsonIgnore]
    public int StatusCode { get; protected set; }

    [JsonIgnore]
    public bool IsSuccessful { get; protected set; }

    protected ServiceResponse() { }

    public static ServiceResponse Success(int statusCode)
        => new() { StatusCode = statusCode, IsSuccessful = true };

    public static ServiceResponse Failure(List<string> errors, int statusCode)
        => new() { Errors = errors, StatusCode = statusCode, IsSuccessful = false };

    public static ServiceResponse Failure(string error, int statusCode)
        => new() { Errors = [error], StatusCode = statusCode, IsSuccessful = false };
}