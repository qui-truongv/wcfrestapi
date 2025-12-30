using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Data;

namespace QMS.Infrastructure.Repositories;

public class KioskRepository : GenericRepository<QMS_KIOSK>, IKioskRepository
{
    public KioskRepository(QMSDbContext context) : base(context)
    {
    }

    public async Task<QMS_KIOSK?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(k => k.CODE == code);
    }

    public async Task<QMS_KIOSK?> GetByNameOrIpAsync(string nameOrIp)
    {
        return await _dbSet
            .FirstOrDefaultAsync(k => k.NAME == nameOrIp || k.IPADDRESS == nameOrIp);
    }

    public async Task<List<QMS_KIOSK_QUEUE>> GetKioskQueuesAsync(int kioskId)
    {
        return await _context.QMS_KIOSK_QUEUE
            .Include(kq => kq.Queue)
            .Where(kq => kq.KIOSK_ID == kioskId && kq.IsActive)
            .OrderBy(kq => kq.DisplayOrder)
            .ToListAsync();
    }
}