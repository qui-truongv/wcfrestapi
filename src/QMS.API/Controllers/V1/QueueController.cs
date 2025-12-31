using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using QMS.Application.DTOs.Queue;
using QMS.Application.Interfaces;

namespace QMS.API.Controllers.V1;

/// <summary>
/// Queue Management API Controller - FIXED
/// </summary>
[ApiVersion("1.0")]
public class QueueController : BaseApiController
{
    private readonly IQueueService _queueService;
    private readonly ILogger<QueueController> _logger;

    public QueueController(
        IQueueService queueService,
        ILogger<QueueController> logger)
    {
        _queueService = queueService;
        _logger = logger;
    }

    #region Queue Management

    /// <summary>
    /// Get queue by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<QueueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQueueById(int id)
    {
        try
        {
            var queue = await _queueService.GetQueueByIdAsync(id);

            if (queue == null)
                return NotFound($"Queue with ID {id} not found");

            return Success(queue, "Queue retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue {QueueId}", id);
            return InternalServerError("Failed to retrieve queue");
        }
    }

    /// <summary>
    /// Get queue by department ID
    /// </summary>
    [HttpGet("department/{departmentId}")]
    [ProducesResponseType(typeof(ApiResponse<QueueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQueueByDepartment(int departmentId)
    {
        try
        {
            var queue = await _queueService.GetQueueByDepartmentIdAsync(departmentId);

            if (queue == null)
                return NotFound($"Queue for department {departmentId} not found");

            return Success(queue, "Queue retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue for department {DepartmentId}", departmentId);
            return InternalServerError("Failed to retrieve queue");
        }
    }

    /// <summary>
    /// Get all active queues
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<List<QueueDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveQueues()
    {
        try
        {
            var queues = await _queueService.GetActiveQueuesAsync();
            return Success(queues, $"{queues.Count} active queues retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active queues");
            return InternalServerError("Failed to retrieve active queues");
        }
    }

    /// <summary>
    /// Get queues by screen ID
    /// </summary>
    [HttpGet("screen/{screenId}")]
    [ProducesResponseType(typeof(ApiResponse<List<QueueDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQueuesByScreen(int screenId)
    {
        try
        {
            var queues = await _queueService.GetQueuesByScreenIdAsync(screenId);
            return Success(queues, $"{queues.Count} queues retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queues for screen {ScreenId}", screenId);
            return InternalServerError("Failed to retrieve queues");
        }
    }

    /// <summary>
    /// Get queue statistics
    /// </summary>
    [HttpGet("{id}/statistics")]
    [ProducesResponseType(typeof(ApiResponse<QueueStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQueueStatistics(
        int id,
        [FromQuery] DateTime? date = null)
    {
        try
        {
            var statistics = await _queueService.GetQueueStatisticsAsync(id, date);
            return Success(statistics, "Statistics retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for queue {QueueId}", id);
            return InternalServerError("Failed to retrieve statistics");
        }
    }

    #endregion

    #region Queue Items - Read Operations

    /// <summary>
    /// Get queue item by ID
    /// </summary>
    [HttpGet("items/{itemId}")]
    [ProducesResponseType(typeof(ApiResponse<QueueItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQueueItem(int itemId)
    {
        try
        {
            var item = await _queueService.GetQueueItemByIdAsync(itemId);

            if (item == null)
                return NotFound($"Queue item {itemId} not found");

            return Success(item, "Queue item retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue item {ItemId}", itemId);
            return InternalServerError("Failed to retrieve queue item");
        }
    }

    /// <summary>
    /// Get queue items for a queue
    /// </summary>
    [HttpGet("{queueId}/items")]
    [ProducesResponseType(typeof(ApiResponse<List<QueueItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQueueItems(
        int queueId,
        [FromQuery] DateTime? date = null,
        [FromQuery] int? state = null,
        [FromQuery] int? limit = null)
    {
        try
        {
            var items = await _queueService.GetQueueItemsAsync(queueId, date, state, limit);
            return Success(items, $"{items.Count} queue items retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue items for queue {QueueId}", queueId);
            return InternalServerError("Failed to retrieve queue items");
        }
    }

    /// <summary>
    /// Find queue item by patient code
    /// </summary>
    [HttpGet("{queueId}/items/patient/{patientCode}")]
    [ProducesResponseType(typeof(ApiResponse<QueueItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindByPatientCode(
        int queueId,
        string patientCode,
        [FromQuery] DateTime? date = null)
    {
        try
        {
            var item = await _queueService.FindQueueItemByPatientCodeAsync(
                queueId, patientCode, date);

            if (item == null)
                return NotFound($"No queue item found for patient {patientCode}");

            return Success(item, "Queue item found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding queue item for patient {PatientCode}", patientCode);
            return InternalServerError("Failed to find queue item");
        }
    }

    /// <summary>
    /// Find queue item by display text
    /// </summary>
    [HttpGet("{queueId}/items/display/{displayText}")]
    [ProducesResponseType(typeof(ApiResponse<QueueItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindByDisplayText(
        int queueId,
        string displayText,
        [FromQuery] DateTime? date = null)
    {
        try
        {
            var item = await _queueService.FindQueueItemByDisplayTextAsync(
                queueId, displayText, date);

            if (item == null)
                return NotFound($"No queue item found with display text {displayText}");

            return Success(item, "Queue item found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding queue item by display text {DisplayText}", displayText);
            return InternalServerError("Failed to find queue item");
        }
    }

    #endregion

    #region Queue Items - Create Operations

    /// <summary>
    /// Create new queue item
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<QueueItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateQueueItem([FromBody] CreateQueueItemDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return base.BadRequest(ModelState);

            var queueItem = await _queueService.AddNewQueueItemAsync(dto);

            return Created(
                $"/api/v1/queue/items/{queueItem.ID}",
                queueItem,
                "Queue item created successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating queue item");
            return InternalServerError("Failed to create queue item");
        }
    }

    /// <summary>
    /// Create queue item for reception
    /// </summary>
    [HttpPost("tiepnhan")]
    [ProducesResponseType(typeof(ApiResponse<QueueItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTiepNhanItem([FromBody] TiepNhanRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return base.BadRequest(ModelState);

            var queueItem = await _queueService.AddNewTiepNhanAsync(
                request.QueueId,
                request.Priority,
                request.PatientCode);

            return Created(
                $"/api/v1/queue/items/{queueItem.ID}",
                queueItem,
                "Tiep nhan item created successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tiep nhan item");
            return InternalServerError("Failed to create tiep nhan item");
        }
    }

    #endregion

    #region Queue Operations

    /// <summary>
    /// Get next queue item for a client
    /// </summary>
    [HttpPost("next")]
    [ProducesResponseType(typeof(ApiResponse<QueueItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNextQueueItem([FromBody] NextQueueItemRequest request)
    {
        try
        {
            var queueItem = await _queueService.GetNextQueueItemAsync(
                request.QueueId,
                request.ClientId,
                request.ClientName);

            if (queueItem == null)
                return NotFound("No waiting items in queue");

            return Success(queueItem, "Next queue item retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next queue item");
            return InternalServerError("Failed to get next queue item");
        }
    }

    /// <summary>
    /// Update queue item state
    /// </summary>
    [HttpPut("items/state")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQueueItemState([FromBody] UpdateStateRequest request)
    {
        try
        {
            var success = await _queueService.UpdateQueueItemStateAsync(
                request.QueueId,
                request.DisplayText,
                request.State,
                request.ClientName,
                request.IsMaToaThuoc);

            if (!success)
                return NotFound("Queue item not found");

            return Success("Queue item state updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating queue item state");
            return InternalServerError("Failed to update queue item state");
        }
    }

    /// <summary>
    /// Update queue item state by patient code
    /// </summary>
    [HttpPut("items/state/patient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStateByPatientCode(
        [FromBody] UpdateStateByPatientRequest request)
    {
        try
        {
            var success = await _queueService.UpdateQueueItemStateByPatientCodeAsync(
                request.QueueId,
                request.PatientCode,
                request.State,
                request.ClientName,
                request.Recall);

            if (!success)
                return NotFound("No queue items found for patient");

            return Success("Queue item(s) updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating queue item state by patient code");
            return InternalServerError("Failed to update queue item state");
        }
    }

    /// <summary>
    /// Clear missed items
    /// </summary>
    [HttpPost("{queueId}/clear-missed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearMissedItems(int queueId)
    {
        try
        {
            await _queueService.ClearMissedItemsAsync(queueId);
            return Success("Missed items cleared successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing missed items for queue {QueueId}", queueId);
            return InternalServerError("Failed to clear missed items");
        }
    }

    /// <summary>
    /// Move queue item to another queue
    /// </summary>
    [HttpPost("items/move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveQueueItem([FromBody] MoveQueueItemRequest request)
    {
        try
        {
            var success = await _queueService.MoveQueueItemAsync(
                request.FromQueueId,
                request.Order,
                request.ToQueueId,
                request.PatientCode,
                request.DepartmentId);

            if (!success)
                return NotFound("Original queue item not found");

            return Success("Queue item moved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving queue item");
            return InternalServerError("Failed to move queue item");
        }
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Get parameter value
    /// </summary>
    [HttpGet("parameters/{code}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetParameter(string code)
    {
        try
        {
            var value = await _queueService.GetParameterValueAsync(code);

            if (string.IsNullOrEmpty(value))
                return NotFound($"Parameter {code} not found");

            return Success(new { code, value }, "Parameter retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting parameter {Code}", code);
            return InternalServerError("Failed to retrieve parameter");
        }
    }

    /// <summary>
    /// Get all parameters
    /// </summary>
    [HttpGet("parameters")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllParameters()
    {
        try
        {
            var parameters = await _queueService.GetAllParametersAsync();
            return Success(parameters, $"{parameters.Count} parameters retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all parameters");
            return InternalServerError("Failed to retrieve parameters");
        }
    }


    /// <summary>
    /// Get all parameters
    /// </summary>
    [HttpGet("Items-Queue")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQueueItemsByQueue(int queueId, int? take)
    {
        try
        {
            var rtn = await _queueService.GetQueueItemsByQueue(queueId, take);
            return Success(rtn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all parameters");
            return InternalServerError("Failed to retrieve parameters");
        }
    }
    #endregion

}

#region Request DTOs

public class TiepNhanRequest
{
    public int QueueId { get; set; }
    public int Priority { get; set; }
    public string PatientCode { get; set; } = string.Empty;
}

public class NextQueueItemRequest
{
    public int QueueId { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
}

public class UpdateStateRequest
{
    public int QueueId { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public int State { get; set; }
    public string? ClientName { get; set; }
    public int? IsMaToaThuoc { get; set; }
}

public class UpdateStateByPatientRequest
{
    public int QueueId { get; set; }
    public string PatientCode { get; set; } = string.Empty;
    public int State { get; set; }
    public string? ClientName { get; set; }
    public bool Recall { get; set; }
}

public class MoveQueueItemRequest
{
    public int FromQueueId { get; set; }
    public string Order { get; set; } = string.Empty;
    public int ToQueueId { get; set; }
    public string PatientCode { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
}

#endregion