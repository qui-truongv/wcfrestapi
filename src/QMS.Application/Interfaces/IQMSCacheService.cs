using QMS.Application.DTOs.Client;
using QMS.Application.DTOs.Queue;
using QMS.Application.DTOs.Screen;
using QMS.Core.Entities;

namespace QMS.Application.Interfaces;

public interface IQMSCacheService
{
    // Cache Management
    Task InitializeCacheAsync();
    Task ReloadCacheAsync();
    Task<bool> IsCacheLoadedAsync();

    // Queue Items Cache
    List<QueueItemDto> GetCachedQueueItems(int queueId);
    void AddQueueItemToCache(QueueItemDto item);
    void RemoveQueueItemFromCache(int queueId, int itemId);
    void ClearQueueItemsCache();

    // Queue Cache
    List<QueueDto> GetCachedQueues();
    QueueDto? GetCachedQueue(int queueId);

    // Screen Cache
    List<ScreenDto> GetCachedScreens();
    ScreenDto? GetCachedScreen(int screenId);

    // Client Cache
    List<ClientDto> GetCachedClients();
    ClientDto? GetCachedClient(int clientId);

    // Parameter Cache
    Task<string?> GetCachedParameterAsync(string code);
    Task<Dictionary<string, string>> GetAllCachedParametersAsync();

    // Statistics
    int GetCachedItemsCount();
    DateTime GetLastReloadTime();
}