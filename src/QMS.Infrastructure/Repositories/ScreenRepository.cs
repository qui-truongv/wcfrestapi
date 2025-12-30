using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Data;

namespace QMS.Infrastructure.Repositories;

public class ScreenRepository : GenericRepository<QMS_SCREEN>, IScreenRepository
{
    public ScreenRepository(QMSDbContext context) : base(context)
    {
    }

    public async Task<List<QMS_SCREEN>> GetActiveScreensAsync()
    {
        return await _dbSet
            .Include(s => s.Section)
            .Where(s => s.STATE == 1)
            .ToListAsync();
    }

    public async Task<QMS_SCREEN?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.CODE == code);
    }

    public async Task<List<QMS_SCREEN>> GetScreensBySectionIdAsync(int sectionId)
    {
        return await _dbSet
            .Where(s => s.SECTION_ID == sectionId && s.STATE == 1)
            .ToListAsync();
    }
}