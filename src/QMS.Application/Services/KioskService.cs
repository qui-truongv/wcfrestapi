using AutoMapper;
using Microsoft.Extensions.Logging;
using QMS.Application.DTOs.Kiosk;
using QMS.Application.Interfaces;
using QMS.Core.Interfaces;

namespace QMS.Application.Services;

public class KioskService : IKioskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<KioskService> _logger;

    public KioskService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<KioskService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    #region Kiosk Management

    public async Task<KioskDto?> GetKioskByIdAsync(int kioskId)
    {
        try
        {
            var kiosk = await _unitOfWork.Kiosks.GetByIdAsync(kioskId);
            return kiosk != null ? _mapper.Map<KioskDto>(kiosk) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk by ID: {KioskId}", kioskId);
            throw;
        }
    }

    public async Task<KioskDto?> GetKioskByCodeAsync(string code)
    {
        try
        {
            var kiosk = await _unitOfWork.Kiosks.GetByCodeAsync(code);
            return kiosk != null ? _mapper.Map<KioskDto>(kiosk) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk by code: {Code}", code);
            throw;
        }
    }

    public async Task<KioskDto?> GetKioskByNameOrIpAsync(string nameOrIp)
    {
        try
        {
            var kiosk = await _unitOfWork.Kiosks.GetByNameOrIpAsync(nameOrIp);
            return kiosk != null ? _mapper.Map<KioskDto>(kiosk) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk by name or IP: {NameOrIp}", nameOrIp);
            throw;
        }
    }

    public async Task<List<KioskDto>> GetAllKiosksAsync()
    {
        try
        {
            var kiosks = await _unitOfWork.Kiosks.GetAllAsync();
            return _mapper.Map<List<KioskDto>>(kiosks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all kiosks");
            throw;
        }
    }

    #endregion

    #region Kiosk Information

    public async Task<KioskInformationDto?> GetKioskInformationAsync(string kioskNameOrIp)
    {
        try
        {
            var kiosk = await _unitOfWork.Kiosks.GetByNameOrIpAsync(kioskNameOrIp);

            if (kiosk == null)
            {
                _logger.LogWarning("Kiosk not found: {KioskNameOrIp}", kioskNameOrIp);
                return null;
            }

            var kioskQueues = await _unitOfWork.Kiosks.GetKioskQueuesAsync(kiosk.ID);

            var result = new KioskInformationDto
            {
                KioskName = kiosk.NAME ?? string.Empty,
                IpAddress = kiosk.IPADDRESS,
                Queues = kioskQueues.Select(kq => new KioskQueueDto
                {
                    ID = kq.Queue?.ID ?? 0,
                    Name = kq.DISPLAYTEXT ?? string.Empty,
                    QueueName = kq.Queue?.NAME ?? string.Empty,
                    ScreenId = kq.Queue?.SCREEN_ID ?? 0,
                    Priority = kq.PRIORITY ?? 0,
                    DisplayOrder = Convert.ToInt32(kq.DisplayOrder)
                }).ToList()
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk information: {KioskNameOrIp}", kioskNameOrIp);
            throw;
        }
    }

    public async Task<List<KioskQueueDto>> GetKioskQueuesAsync(int kioskId)
    {
        try
        {
            var kioskQueues = await _unitOfWork.Kiosks.GetKioskQueuesAsync(kioskId);

            return kioskQueues.Select(kq => new KioskQueueDto
            {
                ID = kq.Queue?.ID ?? 0,
                Name = kq.DISPLAYTEXT ?? string.Empty,
                QueueName = kq.Queue?.NAME ?? string.Empty,
                ScreenId = kq.Queue?.SCREEN_ID ?? 0,
                Priority = kq.PRIORITY ?? 0,
                DisplayOrder = Convert.ToInt32(kq.DisplayOrder)
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk queues: {KioskId}", kioskId);
            throw;
        }
    }

    public async Task<List<KioskQueueDto>> GetKioskQueuesByNameOrIpAsync(string nameOrIp)
    {
        try
        {
            var kiosk = await _unitOfWork.Kiosks.GetByNameOrIpAsync(nameOrIp);

            if (kiosk == null)
            {
                _logger.LogWarning("Kiosk not found: {NameOrIp}", nameOrIp);
                return new List<KioskQueueDto>();
            }

            return await GetKioskQueuesAsync(kiosk.ID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kiosk queues by name or IP: {NameOrIp}", nameOrIp);
            throw;
        }
    }

    #endregion
}