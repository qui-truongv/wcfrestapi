using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Data;

namespace QMS.Infrastructure.Repositories;

public class ClientRepository : GenericRepository<QMS_CLIENT>, IClientRepository
{
    public ClientRepository(QMSDbContext context) : base(context)
    {
    }

    public async Task<QMS_CLIENT?> GetByComputerNameAsync(string computerName)
    {
        return await _dbSet
            .Include(c => c.Queue)
            .FirstOrDefaultAsync(c => c.COMPUTERNAME == computerName);
    }

    public async Task<QMS_CLIENT?> GetByNameAsync(string name)
    {
        return await _dbSet
            .Include(c => c.Queue)
            .FirstOrDefaultAsync(c => c.NAME == name);
    }

    public async Task<List<QMS_CLIENT>> GetClientsByQueueIdAsync(int queueId)
    {
        return await _dbSet
            .Where(c => c.QUEUE_ID == queueId)
            .ToListAsync();
    }
}