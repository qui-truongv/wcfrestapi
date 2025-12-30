using QMS.Core.Entities;
using QMS.Core.Interfaces.Repositories;

namespace QMS.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repositories
    IQueueRepository Queues { get; }
    IQueueItemRepository QueueItems { get; }
    IScreenRepository Screens { get; }
    IKioskRepository Kiosks { get; }
    IParameterRepository Parameters { get; }
    IClientRepository Clients { get; }
    IGenericRepository<QMS_SECTION> Sections { get; }
    IGenericRepository<QMS_QUEUE_TYPE> QueueTypes { get; }
    IGenericRepository<QMS_QUEUE_GROUP> QueueGroups { get; }
    IGenericRepository<QMS_QUEUE_ITEM_REPORT> QueueItemReports { get; }
    IGenericRepository<QMS_KIOSK_QUEUE> KioskQueues { get; }

    // Methods
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}