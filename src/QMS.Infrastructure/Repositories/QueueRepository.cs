using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Data;

namespace QMS.Infrastructure.Repositories;

public class QueueRepository : GenericRepository<QMS_QUEUE>, IQueueRepository
{
    public QueueRepository(QMSDbContext context) : base(context)
    {
    }

    public async Task<QMS_QUEUE?> GetByDepartmentIdAsync(int departmentId)
    {
        return await _dbSet
            .Include(q => q.QueueType)
            .Include(q => q.Section)
            .Include(q => q.Screen)
            .FirstOrDefaultAsync(q => q.DEPARTMENT_ID == departmentId);
    }

    public async Task<List<QMS_QUEUE>> GetActiveQueuesAsync()
    {
        return await _dbSet
            .Include(q => q.QueueType)
            .Include(q => q.Section)
            .Where(q => q.STATE == 1)
            .ToListAsync();
    }

    public async Task<List<QMS_QUEUE>> GetQueuesByScreenIdAsync(int screenId)
    {
        return await _dbSet
            .Include(q => q.QueueType)
            .Include(q => q.Section)
            .Where(q => q.SCREEN_ID == screenId && q.STATE == 1)
            .ToListAsync();
    }

    public async Task<List<QMS_QUEUE>> GetQueuesBySectionIdAsync(int sectionId)
    {
        return await _dbSet
            .Include(q => q.QueueType)
            .Where(q => q.SECTION_ID == sectionId && q.STATE == 1)
            .ToListAsync();
    }

    public async Task<int> GetMaxSequenceAsync(int queueId, DateTime date)
    {
        var maxSequence = await _context.QMS_QUEUE_ITEM
            .Where(qi => qi.QUEUE_ID == queueId && qi.CREATEDATE == date.Date)
            .MaxAsync(qi => (int?)qi.SEQUENCE);

        return maxSequence ?? 0;
    }

    public async Task<QMS_QUEUE?> GetQueueWithDetailsAsync(int queueId)
    {
        return await _dbSet
            .Include(q => q.QueueType)
            .Include(q => q.Section)
            .Include(q => q.Screen)
            .Include(q => q.QueueGroup)
            .FirstOrDefaultAsync(q => q.ID == queueId);
    }
}