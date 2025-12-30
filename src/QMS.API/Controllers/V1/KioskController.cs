using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using QMS.Application.DTOs.Kiosk;
using QMS.Application.DTOs.Screen;
using QMS.Application.Interfaces;

namespace QMS.API.Controllers.V1;

/// <summary>
/// Kiosk Management API Controller - FIXED
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/kiosk")]
public class KioskController : BaseApiController
{
    private readonly IKioskService _kioskService;
    private readonly ILogger<KioskController> _logger;

    public KioskController(
        IKioskService kioskService,
        ILogger<KioskController> logger)
    {
        _kioskService = kioskService;
        _logger = logger;
    }

    /// <summary>
    /// Get kiosk by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var kiosk = await _kioskService.GetKioskByIdAsync(id);
            if (kiosk == null)
                return NotFound($"Kiosk {id} not found");

            return Success(kiosk, "Kiosk retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk {KioskId}", id);
            return InternalServerError("Failed to retrieve kiosk");
        }
    }

    /// <summary>
    /// Get kiosk by code
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            var kiosk = await _kioskService.GetKioskByCodeAsync(code);
            if (kiosk == null)
                return NotFound($"Kiosk with code {code} not found");

            return Success(kiosk, "Kiosk retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk by code {Code}", code);
            return InternalServerError("Failed to retrieve kiosk");
        }
    }

    /// <summary>
    /// Get all kiosks
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var kiosks = await _kioskService.GetAllKiosksAsync();
            return Success(kiosks, $"{kiosks.Count} kiosks retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all kiosks");
            return InternalServerError("Failed to retrieve kiosks");
        }
    }

    /// <summary>
    /// Get kiosk information with queues
    /// </summary>
    [HttpGet("info/{nameOrIp}")]
    public async Task<IActionResult> GetKioskInformation(string nameOrIp)
    {
        try
        {
            var info = await _kioskService.GetKioskInformationAsync(nameOrIp);
            if (info == null)
                return NotFound($"Kiosk {nameOrIp} not found");

            return Success(info, "Kiosk information retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk information {NameOrIp}", nameOrIp);
            return InternalServerError("Failed to retrieve kiosk information");
        }
    }

    /// <summary>
    /// Get kiosk queues by kiosk ID
    /// </summary>
    [HttpGet("{kioskId}/queues")]
    public async Task<IActionResult> GetKioskQueues(int kioskId)
    {
        try
        {
            var queues = await _kioskService.GetKioskQueuesAsync(kioskId);
            return Success(queues, $"{queues.Count} kiosk queues retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk queues {KioskId}", kioskId);
            return InternalServerError("Failed to retrieve kiosk queues");
        }
    }
}
