namespace ONGES.Campaign.Application.DTOs;

/// <summary>
/// Standard API response wrapper.
/// </summary>
public class Result<T> where T : class
{
    public Result(T data)
    {
        Data = data;
        Success = true;
    }

    public Result()
    {
        Success = true;
    }

    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? MessageDetail { get; set; }
}
