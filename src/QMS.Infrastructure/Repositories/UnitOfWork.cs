using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QMS.Core.Entities;
using QMS.Core.Interfaces;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Data;
using QMS.Infrastructure.Repositories;

namespace QMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly QMSDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IQueueRepository? _queues;
    private IQueueItemRepository? _queueItems;
    private IScreenRepository? _screens;
    private IKioskRepository? _kiosks;
    private IParameterRepository? _parameters;
    private IClientRepository? _clients;
    private IGenericRepository<QMS_SECTION>? _sections;
    private IGenericRepository<QMS_QUEUE_TYPE>? _queueTypes;
    private IGenericRepository<QMS_QUEUE_GROUP>? _queueGroups;
    private IGenericRepository<QMS_QUEUE_ITEM_REPORT>? _queueItemReports;
    private IGenericRepository<QMS_KIOSK_QUEUE>? _kioskQueues;

    public UnitOfWork(QMSDbContext context)
    {
        _context = context;
    }

    // Lazy initialization of repositories
    public IQueueRepository Queues
    {
        get { return _queues ??= new QueueRepository(_context); }
    }

    public IQueueItemRepository QueueItems
    {
        get { return _queueItems ??= new QueueItemRepository(_context); }
    }

    public IScreenRepository Screens
    {
        get { return _screens ??= new ScreenRepository(_context); }
    }

    public IKioskRepository Kiosks
    {
        get { return _kiosks ??= new KioskRepository(_context); }
    }

    public IParameterRepository Parameters
    {
        get { return _parameters ??= new ParameterRepository(_context); }
    }

    public IClientRepository Clients
    {
        get { return _clients ??= new ClientRepository(_context); }
    }

    public IGenericRepository<QMS_SECTION> Sections
    {
        get { return _sections ??= new GenericRepository<QMS_SECTION>(_context); }
    }

    public IGenericRepository<QMS_QUEUE_TYPE> QueueTypes
    {
        get { return _queueTypes ??= new GenericRepository<QMS_QUEUE_TYPE>(_context); }
    }

    public IGenericRepository<QMS_QUEUE_GROUP> QueueGroups
    {
        get { return _queueGroups ??= new GenericRepository<QMS_QUEUE_GROUP>(_context); }
    }

    public IGenericRepository<QMS_QUEUE_ITEM_REPORT> QueueItemReports
    {
        get { return _queueItemReports ??= new GenericRepository<QMS_QUEUE_ITEM_REPORT>(_context); }
    }

    public IGenericRepository<QMS_KIOSK_QUEUE> KioskQueues
    {
        get { return _kioskQueues ??= new GenericRepository<QMS_KIOSK_QUEUE>(_context); }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();

            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}