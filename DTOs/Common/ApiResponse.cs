using System.Text.Json.Serialization;

namespace TalentShowCase.API.DTOs.Common;

public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { get; set; }

    [JsonPropertyName("Code")]
    public int Code { get; set; }

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Metadata { get; set; }

    public static ApiResponse<T> Succeed(T data, int code = 200, object? metadata = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Code = code,
            Data = data,
            Metadata = metadata
        };
    }

    public static ApiResponse<T> Fail(string message, int errorCode = 500, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Code = errorCode,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

