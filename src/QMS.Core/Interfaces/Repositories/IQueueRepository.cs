using QMS.Core.Entities;
using System.Linq.Expressions;

namespace QMS.Core.Interfaces.Repositories;

public interface IQueueRepository : IGenericRepository<QMS_QUEUE>
{
    Task<QMS_QUEUE?> GetByDepartmentIdAsync(int departmentId);
    Task<List<QMS_QUEUE>> GetActiveQueuesAsync();
    Task<List<QMS_QUEUE>> GetQueuesByScreenIdAsync(int screenId);
    Task<List<QMS_QUEUE>> GetQueuesBySectionIdAsync(int sectionId);
    Task<int> GetMaxSequenceAsync(int queueId, DateTime date);
    Task<QMS_QUEUE?> GetQueueWithDetailsAsync(int queueId);
}