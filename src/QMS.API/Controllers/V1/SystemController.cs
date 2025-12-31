using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using QMS.Application.Interfaces;

namespace QMS.API.Controllers.V1;

/// <summary>
/// Cache Management API Controller - FIXED
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/system")]
public class SystemController : BaseApiController
{
}
