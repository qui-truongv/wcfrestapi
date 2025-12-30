using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using QMS.Application.Interfaces;

namespace QMS.API.Controllers.V1
{
    /// <summary>
    /// Client Management API Controller - FIXED
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/client")]
    public class ClientController : BaseApiController
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IClientService clientService,
            ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        /// <summary>
        /// Get client by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                    return NotFound($"Client {id} not found");

                return Success(client, "Client retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client {ClientId}", id);
                return InternalServerError("Failed to retrieve client");
            }
        }

        /// <summary>
        /// Get client by computer name
        /// </summary>
        [HttpGet("computer/{computerName}")]
        public async Task<IActionResult> GetByComputerName(string computerName)
        {
            try
            {
                var client = await _clientService.GetClientByComputerNameAsync(computerName);
                if (client == null)
                    return NotFound($"Client with computer name {computerName} not found");

                return Success(client, "Client retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client by computer name {ComputerName}", computerName);
                return InternalServerError("Failed to retrieve client");
            }
        }

        /// <summary>
        /// Get clients by queue ID
        /// </summary>
        [HttpGet("queue/{queueId}")]
        public async Task<IActionResult> GetByQueueId(int queueId)
        {
            try
            {
                var clients = await _clientService.GetClientsByQueueIdAsync(queueId);
                return Success(clients, $"{clients.Count} clients retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients by queue {QueueId}", queueId);
                return InternalServerError("Failed to retrieve clients");
            }
        }
    }
}
