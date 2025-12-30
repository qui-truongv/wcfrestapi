using QMS.Core.Entities;

namespace QMS.Core.Interfaces.Repositories;

public interface IParameterRepository : IGenericRepository<QMS_PARAMETER>
{
    Task<QMS_PARAMETER?> GetByCodeAsync(string code);
    Task<string?> GetValueAsync(string code);
    Task<Dictionary<string, string>> GetAllParametersAsync();
}