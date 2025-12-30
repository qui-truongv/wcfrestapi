using QMS.Core.Entities;

namespace QMS.Core.Interfaces.Repositories;

public interface IScreenRepository : IGenericRepository<QMS_SCREEN>
{
    Task<List<QMS_SCREEN>> GetActiveScreensAsync();
    Task<QMS_SCREEN?> GetByCodeAsync(string code);
    Task<List<QMS_SCREEN>> GetScreensBySectionIdAsync(int sectionId);
}