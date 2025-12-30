using QMS.Core.Entities;

namespace QMS.Core.Interfaces.Repositories;

public interface IKioskRepository : IGenericRepository<QMS_KIOSK>
{
    Task<QMS_KIOSK?> GetByCodeAsync(string code);
    Task<QMS_KIOSK?> GetByNameOrIpAsync(string nameOrIp);
    Task<List<QMS_KIOSK_QUEUE>> GetKioskQueuesAsync(int kioskId);
}