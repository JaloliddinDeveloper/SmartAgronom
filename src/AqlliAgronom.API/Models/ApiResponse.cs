namespace AqlliAgronom.API.Models;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public string? TraceId { get; init; }

    public static ApiResponse<T> Success(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };
}

public class ApiErrorResponse
{
    public bool Success => false;
    public string Message { get; init; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; init; }
    public string? TraceId { get; init; }
}
