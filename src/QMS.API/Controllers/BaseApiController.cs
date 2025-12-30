using Microsoft.AspNetCore.Mvc;

namespace QMS.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult Success<T>(T data, string message = "Success")
    {
        return Ok(new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    protected IActionResult Created<T>(T data, string message = "Created successfully")
    {
        return StatusCode(201, new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    protected IActionResult BadRequest(string message)
    {
        return BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = message
        });
    }

    protected IActionResult NotFound(string message = "Resource not found")
    {
        return NotFound(new ApiResponse<object>
        {
            Success = false,
            Message = message
        });
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}