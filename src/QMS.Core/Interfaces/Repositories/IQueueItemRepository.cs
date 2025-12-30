using QMS.Core.Entities;

namespace QMS.Core.Interfaces.Repositories;

public interface IQueueItemRepository : IGenericRepository<QMS_QUEUE_ITEM>
{
    Task<QMS_QUEUE_ITEM?> GetByPatientCodeAsync(int queueId, string patientCode, DateTime date);
    Task<List<QMS_QUEUE_ITEM>> GetByQueueIdAsync(int queueId, DateTime date, int? state = null);
    Task<QMS_QUEUE_ITEM?> GetProcessingItemAsync(int queueId, int? clientId = null);
    Task<QMS_QUEUE_ITEM?> GetNextWaitingItemAsync(int queueId);
    Task<List<QMS_QUEUE_ITEM>> GetItemsByStateAsync(int queueId, int state, DateTime date);
    Task<int> CountByStateAsync(int queueId, int state, DateTime date);
    Task<QMS_QUEUE_ITEM?> GetByDisplayTextAsync(int queueId, string displayText, DateTime date);
}