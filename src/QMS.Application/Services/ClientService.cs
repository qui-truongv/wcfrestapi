using AutoMapper;
using Microsoft.Extensions.Logging;
using QMS.Application.DTOs.Client;
using QMS.Application.Interfaces;
using QMS.Core.Interfaces;

namespace QMS.Application.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ClientService> _logger;

    public ClientService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ClientService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ClientDto?> GetClientByIdAsync(int clientId)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            return client != null ? _mapper.Map<ClientDto>(client) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client by ID: {ClientId}", clientId);
            throw;
        }
    }

    public async Task<ClientDto?> GetClientByComputerNameAsync(string computerName)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByComputerNameAsync(computerName);
            return client != null ? _mapper.Map<ClientDto>(client) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client by computer name: {ComputerName}", computerName);
            throw;
        }
    }

    public async Task<ClientDto?> GetClientByNameAsync(string name)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByNameAsync(name);
            return client != null ? _mapper.Map<ClientDto>(client) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client by name: {Name}", name);
            throw;
        }
    }

    public async Task<List<ClientDto>> GetClientsByQueueIdAsync(int queueId)
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetClientsByQueueIdAsync(queueId);
            return _mapper.Map<List<ClientDto>>(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients by queue ID: {QueueId}", queueId);
            throw;
        }
    }
}