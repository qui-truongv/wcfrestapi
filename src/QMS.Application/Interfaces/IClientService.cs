using QMS.Application.DTOs.Client;

namespace QMS.Application.Interfaces;

public interface IClientService
{
    Task<ClientDto?> GetClientByIdAsync(int clientId);
    Task<ClientDto?> GetClientByComputerNameAsync(string computerName);
    Task<ClientDto?> GetClientByNameAsync(string name);
    Task<List<ClientDto>> GetClientsByQueueIdAsync(int queueId);
}