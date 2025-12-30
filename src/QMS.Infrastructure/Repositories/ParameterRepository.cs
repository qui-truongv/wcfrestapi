using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Data;

namespace QMS.Infrastructure.Repositories;

public class ParameterRepository : GenericRepository<QMS_PARAMETER>, IParameterRepository
{
    public ParameterRepository(QMSDbContext context) : base(context)
    {
    }

    public async Task<QMS_PARAMETER?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.CODE == code);
    }

    public async Task<string?> GetValueAsync(string code)
    {
        var parameter = await GetByCodeAsync(code);
        return parameter?.VALUE;
    }

    public async Task<Dictionary<string, string>> GetAllParametersAsync()
    {
        return await _dbSet
            .Where(p => !string.IsNullOrEmpty(p.VALUE))
            .ToDictionaryAsync(p => p.CODE, p => p.VALUE!);
    }
}