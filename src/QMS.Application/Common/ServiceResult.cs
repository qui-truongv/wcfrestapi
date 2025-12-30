namespace QMS.Application.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ServiceResult<T> SuccessResult(T data, string message = "Success")
    {
        return new ServiceResult<T>
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = 200
        };
    }

    public static ServiceResult<T> FailureResult(string message, int statusCode = 400)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> FailureResult(List<string> errors, string message = "Operation failed", int statusCode = 400)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}