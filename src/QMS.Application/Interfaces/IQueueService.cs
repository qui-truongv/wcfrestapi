using QMS.Application.DTOs.Queue;

namespace QMS.Application.Interfaces;

public interface IQueueService
{
    // Queue Management
    Task<QueueDto?> GetQueueByIdAsync(int queueId);
    Task<QueueDto?> GetQueueByDepartmentIdAsync(int departmentId);
    Task<List<QueueDto>> GetActiveQueuesAsync();
    Task<List<QueueDto>> GetQueuesByScreenIdAsync(int screenId);
    Task<QueueStatisticsDto> GetQueueStatisticsAsync(int queueId, DateTime? date = null);

    // Queue Item Management
    Task<QueueItemDto?> GetQueueItemByIdAsync(int itemId);
    Task<List<QueueItemDto>> GetQueueItemsAsync(int queueId, DateTime? date = null, int? state = null, int? limit = null);
    Task<QueueItemDto?> FindQueueItemByPatientCodeAsync(int queueId, string patientCode, DateTime? date = null);
    Task<QueueItemDto?> FindQueueItemByDisplayTextAsync(int queueId, string displayText, DateTime? date = null);

    // Queue Operations
    Task<QueueItemDto> AddNewQueueItemAsync(CreateQueueItemDto dto);
    Task<QueueItemDto?> GetNextQueueItemAsync(int queueId, int clientId, string clientName);
    Task<bool> UpdateQueueItemStateAsync(int queueId, string displayText, int state, string? clientName = null, int? isMaToaThuoc = null);
    Task<bool> UpdateQueueItemStateByPatientCodeAsync(int queueId, string patientCode, int state, string? clientName = null, bool recall = false);
    Task<bool> ClearMissedItemsAsync(int queueId);
    Task<bool> MoveQueueItemAsync(int fromQueueId, string order, int toQueueId, string patientCode, int? departmentId = null);

    // Parameter Management
    Task<string?> GetParameterValueAsync(string code);
    Task<Dictionary<string, string>> GetAllParametersAsync();
}