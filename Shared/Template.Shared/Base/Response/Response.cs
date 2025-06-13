using System.Text.Json.Serialization;

namespace Template.Shared.Base.Response;

public class ServiceResponse<T> : ServiceResponse
{
    public T? Data { get; private set; }

    private ServiceResponse() { }

    public static ServiceResponse<T> Success(T data, int statusCode)
        => new() { Data = data, StatusCode = statusCode, IsSuccessful = true };

    public static ServiceResponse<NoContent> Success(int statusCode)
        => new() { StatusCode = statusCode, IsSuccessful = true };

    public static ServiceResponse<T> Failure(List<string> errors, int statusCode)
        => new() { Errors = errors, StatusCode = statusCode, IsSuccessful = false };

    public static ServiceResponse<T> Failure(string error, int statusCode)
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