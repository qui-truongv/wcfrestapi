using Microsoft.AspNetCore.Mvc;

namespace QMS.API.Controllers;

/// <summary>
/// Base API Controller with common response methods
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Return success response with data
    /// </summary>
    protected IActionResult Success<T>(T data, string message = "Success")
    {
        return Ok(new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    /// <summary>
    /// Return success response without data
    /// </summary>
    protected IActionResult Success(string message = "Success")
    {
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = message,
            Data = null
        });
    }

    /// <summary>
    /// Return created response
    /// </summary>
    protected IActionResult Created<T>(T data, string message = "Created successfully")
    {
        return StatusCode(201, new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    /// <summary>
    /// Return created response with location
    /// </summary>
    protected IActionResult Created<T>(string location, T data, string message = "Created successfully")
    {
        Response.Headers.Add("Location", location);
        return StatusCode(201, new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    /// <summary>
    /// Return bad request with message
    /// </summary>
    protected new IActionResult BadRequest(string message)
    {
        return BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Data = null
        });
    }

    /// <summary>
    /// Return not found with message
    /// </summary>
    protected new IActionResult NotFound(string message = "Resource not found")
    {
        return NotFound(new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Data = null
        });
    }

    /// <summary>
    /// Return internal server error
    /// </summary>
    protected IActionResult InternalServerError(string message = "Internal server error")
    {
        return StatusCode(500, new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Data = null
        });
    }
}

/// <summary>
/// Generic API Response
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}