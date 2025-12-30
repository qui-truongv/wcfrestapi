using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace QMS.API.Controllers.V1;

[ApiVersion("1.0")]
public class HealthController : BaseApiController
{
    [HttpGet]
    public IActionResult Get()
    {
        return Success(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0"
        }, "API is running");
    }
}