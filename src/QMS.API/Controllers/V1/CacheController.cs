using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using QMS.Application.Interfaces;
using QMS.Core.Enums;

namespace QMS.API.Controllers.V1;

/// <summary>
/// Cache Management API Controller - FIXED
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cache")]
public class CacheController : BaseApiController
{
    private readonly IQMSCacheService _cacheService;
    private readonly ILogger<CacheController> _logger;

    public CacheController(
        IQMSCacheService cacheService,
        ILogger<CacheController> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Reload QMS cache
    /// </summary>
    [HttpPost("reload")]
    public async Task<IActionResult> ReloadCache()
    {
        try
        {
            _logger.LogInformation("Cache reload requested");

            await _cacheService.ReloadCacheAsync();

            var statistics = new
            {
                ItemsCount = _cacheService.GetCachedItemsCount(),
                QueuesCount = _cacheService.GetCachedQueues().Count,
                ScreensCount = _cacheService.GetCachedScreens().Count,
                ClientsCount = _cacheService.GetCachedClients().Count,
                LastReloadTime = _cacheService.GetLastReloadTime()
            };

            return Success(statistics, "Cache reloaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reloading cache");
            return InternalServerError("Cache reload failed: " + ex.Message);
        }
    }

    /// <summary>
    /// Get cache status
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> GetCacheStatus()
    {
        try
        {
            var isLoaded = await _cacheService.IsCacheLoadedAsync();

            var status = new
            {
                IsLoaded = isLoaded,
                ItemsCount = _cacheService.GetCachedItemsCount(),
                QueuesCount = _cacheService.GetCachedQueues().Count,
                ScreensCount = _cacheService.GetCachedScreens().Count,
                ClientsCount = _cacheService.GetCachedClients().Count,
                LastReloadTime = _cacheService.GetLastReloadTime()
            };

            return Success(status, "Cache status retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache status");
            return InternalServerError("Failed to retrieve cache status");
        }
    }

    /// <summary>
    /// Clear queue items cache
    /// </summary>
    [HttpDelete("queue-items")]
    public IActionResult ClearQueueItemsCache()
    {
        try
        {
            _cacheService.ClearQueueItemsCache();
            return Success("Queue items cache cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing queue items cache");
            return InternalServerError("Failed to clear cache");
        }
    }

    /// <summary>
    /// Clear queue items cache
    /// </summary>
    [HttpGet("cache-count")]
    public IActionResult GetCacheStatistics(int type)
    {
        try
        {
            var rtn = _cacheService.GetCacheStatistics(type);
            return Success(rtn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing queue items cache");
            return InternalServerError("Failed to clear cache");
        }
    }

    /// <summary>
    /// Clear queue items cache
    /// </summary>
    [HttpGet("value")]
    public IActionResult GetValueOfParameter(string code)
    {
        try
        {
            var rtn = _cacheService.GetValueOfParameter(code);
            return Success(rtn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing queue items cache");
            return InternalServerError("Failed to clear cache");
        }
    }

    /// <summary>
    /// Clear queue items cache
    /// </summary>
    [HttpGet("clear-queue")]
    public IActionResult ClearCacheByType(CacheType type)
    {
        try
        {
            var rtn = _cacheService.ClearCacheByType(type);
            return Success(rtn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing queue items cache");
            return InternalServerError("Failed to clear cache");
        }
    }
}