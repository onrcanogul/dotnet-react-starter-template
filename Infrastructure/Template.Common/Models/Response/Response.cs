using System.Text.Json.Serialization;

namespace Template.Common.Models.Response;

public class Response<T>
{
    public T? Data { get; private set; }
    public List<string> Errors { get; set; } = new();
    [JsonIgnore]
    public int StatusCode { get; private set; }
    [JsonIgnore]
    public bool IsSuccessful { get; private set; }

    //static factory methods
    public static Response<T> Success(T data, int statusCode)
        => new() { Data = data, StatusCode = statusCode, IsSuccessful = true };

    public static Response<NoContent> Success(int statusCode)
        => new() { StatusCode = statusCode, Data = default, IsSuccessful = true };

    public static Response<T> Failure(List<string> errors, int statusCode)
        => new() { Errors = errors, StatusCode = statusCode, IsSuccessful = false };

    public static Response<T> Failure(string error, int statusCode)
        => new() { Errors = new() { error }, StatusCode = statusCode, IsSuccessful = false };
}