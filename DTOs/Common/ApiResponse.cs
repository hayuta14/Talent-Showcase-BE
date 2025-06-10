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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

    [JsonPropertyName("Code")]
    public int Code { get; set; }

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Succeed(T data, int code = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Code = code,
            Data = data,
        };
    }

    public static ApiResponse<T> Fail(string message, int errorCode = 500, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Code = errorCode,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

