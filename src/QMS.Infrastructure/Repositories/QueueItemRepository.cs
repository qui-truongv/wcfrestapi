using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Data;

namespace QMS.Infrastructure.Repositories;

public class QueueItemRepository : GenericRepository<QMS_QUEUE_ITEM>, IQueueItemRepository
{
    public QueueItemRepository(QMSDbContext context) : base(context)
    {
    }

    public async Task<QMS_QUEUE_ITEM?> GetByPatientCodeAsync(int queueId, string patientCode, DateTime date)
    {
        return await _dbSet
            .Include(qi => qi.Queue)
            .FirstOrDefaultAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.PATIENTCODE == patientCode &&
                qi.CREATEDATE == date.Date &&
                qi.STATE != -1); // Not cancelled
    }

    public async Task<List<QMS_QUEUE_ITEM>> GetByQueueIdAsync(int queueId, DateTime date, int? state = null)
    {
        var query = _dbSet
            .Include(qi => qi.Queue)
            .Where(qi => qi.QUEUE_ID == queueId && qi.CREATEDATE == date.Date);

        if (state.HasValue)
        {
            query = query.Where(qi => qi.STATE == state.Value);
        }

        return await query
            .OrderBy(qi => qi.ORDER)
            .ToListAsync();
    }

    public async Task<QMS_QUEUE_ITEM?> GetProcessingItemAsync(int queueId, int? clientId = null)
    {
        var query = _dbSet
            .Include(qi => qi.Queue)
            .Where(qi =>
                qi.QUEUE_ID == queueId &&
                qi.STATE == 0 && // Process status
                qi.CREATEDATE == DateTime.Today);

        if (clientId.HasValue)
        {
            query = query.Where(qi => qi.CLIENT_ID == 0 || qi.CLIENT_ID == clientId.Value);
        }

        return await query
            .OrderBy(qi => qi.ORDER)
            .FirstOrDefaultAsync();
    }

    public async Task<QMS_QUEUE_ITEM?> GetNextWaitingItemAsync(int queueId)
    {
        return await _dbSet
            .Include(qi => qi.Queue)
            .Where(qi =>
                qi.QUEUE_ID == queueId &&
                qi.CREATEDATE == DateTime.Today &&
                qi.STATE == 1) // Wait status
            .OrderBy(qi => qi.ORDER)
            .FirstOrDefaultAsync();
    }

    public async Task<List<QMS_QUEUE_ITEM>> GetItemsByStateAsync(int queueId, int state, DateTime date)
    {
        return await _dbSet
            .Include(qi => qi.Queue)
            .Where(qi =>
                qi.QUEUE_ID == queueId &&
                qi.STATE == state &&
                qi.CREATEDATE == date.Date)
            .OrderBy(qi => qi.ORDER)
            .ToListAsync();
    }

    public async Task<int> CountByStateAsync(int queueId, int state, DateTime date)
    {
        return await _dbSet
            .CountAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.STATE == state &&
                qi.CREATEDATE == date.Date);
    }

    public async Task<QMS_QUEUE_ITEM?> GetByDisplayTextAsync(int queueId, string displayText, DateTime date)
    {
        return await _dbSet
            .Include(qi => qi.Queue)
            .Where(qi =>
                qi.QUEUE_ID == queueId &&
                qi.DISPLAYTEXT == displayText &&
                qi.CREATEDATE == date.Date)
            .OrderBy(qi => qi.STATE)
            .FirstOrDefaultAsync();
    }
}