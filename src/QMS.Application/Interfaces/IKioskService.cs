using QMS.Application.DTOs.Kiosk;

namespace QMS.Application.Interfaces;

public interface IKioskService
{
    // Kiosk Management
    Task<KioskDto?> GetKioskByIdAsync(int kioskId);
    Task<KioskDto?> GetKioskByCodeAsync(string code);
    Task<KioskDto?> GetKioskByNameOrIpAsync(string nameOrIp);
    Task<List<KioskDto>> GetAllKiosksAsync();

    // Kiosk Information
    Task<KioskInformationDto?> GetKioskInformationAsync(string kioskNameOrIp);
    Task<List<KioskQueueDto>> GetKioskQueuesAsync(int kioskId);
    Task<List<KioskQueueDto>> GetKioskQueuesByNameOrIpAsync(string nameOrIp);
}