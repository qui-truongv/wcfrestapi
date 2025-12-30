using QMS.Core.Entities;

namespace QMS.Core.Interfaces.Repositories;

public interface IClientRepository : IGenericRepository<QMS_CLIENT>
{
    Task<QMS_CLIENT?> GetByComputerNameAsync(string computerName);
    Task<QMS_CLIENT?> GetByNameAsync(string name);
    Task<List<QMS_CLIENT>> GetClientsByQueueIdAsync(int queueId);
}