using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QMS.Application.DTOs.Client;
using QMS.Application.DTOs.Queue;
using QMS.Application.DTOs.Screen;
using QMS.Application.Interfaces;
using QMS.Core.Enums;
using QMS.Core.Interfaces;
using System.Collections.Concurrent;

namespace QMS.Application.Services;

public class QMSCacheService : IQMSCacheService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<QMSCacheService> _logger;

    // Concurrent collections for thread-safe operations
    private readonly ConcurrentDictionary<int, List<QueueItemDto>> _queueItemsCache;
    private readonly ConcurrentDictionary<int, QueueDto> _queuesCache;
    private readonly ConcurrentDictionary<int, ScreenDto> _screensCache;
    private readonly ConcurrentDictionary<int, ClientDto> _clientsCache;
    private readonly ConcurrentDictionary<string, string> _parametersCache;

    private DateTime _lastReloadTime;
    private bool _isLoading;
    private readonly SemaphoreSlim _reloadLock = new(1, 1);

    // Cache keys
    private const string CACHE_KEY_QUEUES = "qms_queues";
    private const string CACHE_KEY_SCREENS = "qms_screens";
    private const string CACHE_KEY_CLIENTS = "qms_clients";
    private const string CACHE_KEY_PARAMETERS = "qms_parameters";
    private const string CACHE_KEY_QUEUE_ITEMS = "qms_queue_items_{0}";

    public QMSCacheService(
        IUnitOfWork unitOfWork,
        IMemoryCache memoryCache,
        ILogger<QMSCacheService> logger)
    {
        _unitOfWork = unitOfWork;
        _memoryCache = memoryCache;
        _logger = logger;

        _queueItemsCache = new ConcurrentDictionary<int, List<QueueItemDto>>();
        _queuesCache = new ConcurrentDictionary<int, QueueDto>();
        _screensCache = new ConcurrentDictionary<int, ScreenDto>();
        _clientsCache = new ConcurrentDictionary<int, ClientDto>();
        _parametersCache = new ConcurrentDictionary<string, string>();

        _lastReloadTime = DateTime.MinValue;
    }

    #region Cache Management

    public async Task InitializeCacheAsync()
    {
        if (_isLoading)
        {
            _logger.LogWarning("Cache is currently loading. Initialization skipped.");
            return;
        }

        await ReloadCacheAsync();
    }

    public async Task ReloadCacheAsync()
    {
        await _reloadLock.WaitAsync();

        try
        {
            if (_isLoading)
            {
                _logger.LogInformation("Cache reload already in progress");
                return;
            }

            _isLoading = true;
            _logger.LogInformation("Starting cache reload...");

            // 1. Load all base data
            await LoadQueuesAsync();
            await LoadScreensAsync();
            await LoadClientsAsync();
            await LoadParametersAsync();

            // 2. Load today's queue items
            await LoadTodayQueueItemsAsync();

            _lastReloadTime = DateTime.Now;
            _logger.LogInformation("Cache reload completed successfully. Items: {Count}, Time: {Time}",
                GetCachedItemsCount(), _lastReloadTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reloading cache");
            throw;
        }
        finally
        {
            _isLoading = false;
            _reloadLock.Release();
        }
    }

    public Task<bool> IsCacheLoadedAsync()
    {
        return Task.FromResult(_queuesCache.Count > 0 && _screensCache.Count > 0);
    }

    #endregion

    #region Queue Items Cache

    public List<QueueItemDto> GetCachedQueueItems(int queueId)
    {
        if (_queueItemsCache.TryGetValue(queueId, out var items))
        {
            return items.OrderBy(i => i.ORDER).ToList();
        }
        return new List<QueueItemDto>();
    }

    public void AddQueueItemToCache(QueueItemDto item)
    {
        if (item.QUEUE_ID == null) return;

        var queueId = item.QUEUE_ID.Value;

        _queueItemsCache.AddOrUpdate(
            queueId,
            new List<QueueItemDto> { item },
            (key, existingList) =>
            {
                existingList.Add(item);
                return existingList;
            });
    }

    public void RemoveQueueItemFromCache(int queueId, int itemId)
    {
        if (_queueItemsCache.TryGetValue(queueId, out var items))
        {
            items.RemoveAll(i => i.ID == itemId);
        }
    }

    public void ClearQueueItemsCache()
    {
        _queueItemsCache.Clear();
        _logger.LogInformation("Queue items cache cleared");
    }

    #endregion

    #region Queue Cache

    public List<QueueDto> GetCachedQueues()
    {
        return _queuesCache.Values.ToList();
    }

    public QueueDto? GetCachedQueue(int queueId)
    {
        _queuesCache.TryGetValue(queueId, out var queue);
        return queue;
    }

    #endregion

    #region Screen Cache

    public List<ScreenDto> GetCachedScreens()
    {
        return _screensCache.Values.ToList();
    }

    public ScreenDto? GetCachedScreen(int screenId)
    {
        _screensCache.TryGetValue(screenId, out var screen);
        return screen;
    }

    #endregion

    #region Client Cache

    public List<ClientDto> GetCachedClients()
    {
        return _clientsCache.Values.ToList();
    }

    public ClientDto? GetCachedClient(int clientId)
    {
        _clientsCache.TryGetValue(clientId, out var client);
        return client;
    }

    #endregion

    #region Parameter Cache

    public Task<string?> GetCachedParameterAsync(string code)
    {
        _parametersCache.TryGetValue(code, out var value);
        return Task.FromResult(value);
    }

    public Task<Dictionary<string, string>> GetAllCachedParametersAsync()
    {
        return Task.FromResult(new Dictionary<string, string>(_parametersCache));
    }

    #endregion

    #region Statistics

    public int GetCachedItemsCount()
    {
        return _queueItemsCache.Values.Sum(list => list.Count);
    }

    public DateTime GetLastReloadTime()
    {
        return _lastReloadTime;
    }

    #endregion

    #region Private Helper Methods

    private async Task LoadQueuesAsync()
    {
        try
        {
            var queues = await _unitOfWork.Queues.GetActiveQueuesAsync();

            _queuesCache.Clear();
            foreach (var queue in queues)
            {
                var queueDto = new QueueDto
                {
                    ID = queue.ID,
                    NAME = queue.NAME,
                    STATE = queue.STATE,
                    SCREEN_ID = queue.SCREEN_ID,
                    DEPARTMENT_ID = queue.DEPARTMENT_ID,
                    MAX = queue.MAX,
                    QUEUETYPE_ID = queue.QUEUETYPE_ID,
                    SECTION_ID = queue.SECTION_ID,
                    REMARKS = queue.REMARKS,
                    TENBACSI = queue.TENBACSI,
                    TENDIEUDUONG = queue.TENDIEUDUONG,
                    IS_MANUAL = queue.IS_MANUAL,
                    IDX = queue.IDX,
                    CODESCREEN = queue.CODESCREEN
                };

                _queuesCache.TryAdd(queue.ID, queueDto);
            }

            _logger.LogDebug("Loaded {Count} queues into cache", _queuesCache.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading queues into cache");
            throw;
        }
    }

    private async Task LoadScreensAsync()
    {
        try
        {
            var screens = await _unitOfWork.Screens.GetActiveScreensAsync();

            _screensCache.Clear();
            foreach (var screen in screens)
            {
                var screenDto = new ScreenDto
                {
                    ID = screen.ID,
                    CODE = screen.CODE,
                    NAME = screen.NAME,
                    SECTION_ID = screen.SECTION_ID,
                    STATE = screen.STATE,
                    DISPLAYROWS = screen.DISPLAYROWS,
                    URL = screen.URL,
                    REMARKS = screen.REMARKS,
                    NUMSCREEN = screen.NUMSCREEN,
                    SectionName = screen.Section?.NAME
                };

                _screensCache.TryAdd(screen.ID, screenDto);
            }

            _logger.LogDebug("Loaded {Count} screens into cache", _screensCache.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading screens into cache");
            throw;
        }
    }

    private async Task LoadClientsAsync()
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();

            _clientsCache.Clear();
            foreach (var client in clients)
            {
                var clientDto = new ClientDto
                {
                    ID = client.ID,
                    NAME = client.NAME,
                    COMPUTERNAME = client.COMPUTERNAME,
                    IPADDRESS = client.IPADDRESS,
                    STATE = client.STATE,
                    PROCESSDURATION = client.PROCESSDURATION,
                    QUEUE_ID = client.QUEUE_ID,
                    REMARKS = client.REMARKS,
                    IPSERVER = client.IPSERVER,
                    NAMEAUDIO = client.NAMEAUDIO
                };

                _clientsCache.TryAdd(client.ID, clientDto);
            }

            _logger.LogDebug("Loaded {Count} clients into cache", _clientsCache.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading clients into cache");
            throw;
        }
    }

    private async Task LoadParametersAsync()
    {
        try
        {
            var parameters = await _unitOfWork.Parameters.GetAllParametersAsync();

            _parametersCache.Clear();
            foreach (var param in parameters)
            {
                _parametersCache.TryAdd(param.Key, param.Value);
            }

            _logger.LogDebug("Loaded {Count} parameters into cache", _parametersCache.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading parameters into cache");
            throw;
        }
    }

    private async Task LoadTodayQueueItemsAsync()
    {
        try
        {
            _queueItemsCache.Clear();

            var today = DateTime.Today;
            var allItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.CREATEDATE == today &&
                qi.STATE != -1); // Not cancelled

            var itemsList = allItems.OrderBy(qi => qi.QUEUE_ID).ThenBy(qi => qi.ORDER).ToList();

            foreach (var item in itemsList)
            {
                if (!item.QUEUE_ID.HasValue || !item.SEQUENCE.HasValue)
                    continue;

                var queueDto = GetCachedQueue(item.QUEUE_ID.Value);

                var itemDto = new QueueItemDto
                {
                    ID = item.ID,
                    QUEUE_ID = item.QUEUE_ID,
                    SEQUENCE = item.SEQUENCE,
                    PREFIX = item.PREFIX,
                    DISPLAYTEXT = item.DISPLAYTEXT,
                    SUFFIX = item.SUFFIX,
                    ORDER = item.ORDER,
                    STATE = item.STATE,
                    PRIORITY = item.PRIORITY,
                    PREVIOUS = item.PREVIOUS,
                    PREVIOUSDEPT_ID = item.PREVIOUSDEPT_ID,
                    PATIENTCODE = item.PATIENTCODE,
                    PATIENTNAME = item.PATIENTNAME,
                    PATIENTYOB = item.PATIENTYOB,
                    CREATEDATE = item.CREATEDATE,
                    CREATETIME = item.CREATETIME,
                    ESTIMATETIME = item.ESTIMATETIME,
                    PROCESSTIME = item.PROCESSTIME,
                    FINISHTIME = item.FINISHTIME,
                    CLIENT_ID = item.CLIENT_ID,
                    CLIENT_NAME = item.CLIENT_NAME,
                    REMARKS = item.REMARKS,
                    ISMATOATHUOC = item.ISMATOATHUOC,
                    SOTIEN = item.SOTIEN,
                    TENCUA = item.TENCUA,
                    QueueName = queueDto?.NAME
                };

                AddQueueItemToCache(itemDto);
            }

            _logger.LogDebug("Loaded {Count} queue items into cache", GetCachedItemsCount());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading queue items into cache");
            throw;
        }
    }

    public object GetCacheStatistics(int type)
    {
        return type switch
        {
            1 => new { Type = "QueueItems", Count = GetCachedItemsCount() },
            2 => new { Type = "Queues", Count = _queuesCache.Count },
            3 => new { Type = "Screens", Count = _screensCache.Count },
            4 => new { Type = "Parameters", Count = _parametersCache.Count },
            _ => new
            {
                QueueItems = GetCachedItemsCount(),
                Queues = _queuesCache.Count,
                Screens = _screensCache.Count,
                Clients = _clientsCache.Count,
                Parameters = _parametersCache.Count,
                LastReload = _lastReloadTime
            }
        };
    }

    public string GetValueOfParameter(string code, string defaultValue = "")
    {
        // Kiểm tra cache đã được nạp chưa
        if (_parametersCache.IsEmpty)
        {
            _logger.LogWarning("Parameter cache is empty when accessing key: {Key}", code);
            return defaultValue;
        }

        // TryGetValue giúp tránh exception và không cần dùng Find (duyệt list)
        if (_parametersCache.TryGetValue(code, out var value))
        {
            return value;
        }

        _logger.LogDebug("Parameter key {Key} not found in cache. Returning default: {Default}", code, defaultValue);

        // Thay vì trả về "9" cứng nhắc như code cũ, chúng ta cho phép truyền defaultValue
        // Nếu bạn muốn giữ nguyên logic cũ của hệ thống, hãy truyền "9" vào parameter defaultValue
        return defaultValue;
    }

    public bool ClearCacheByType(CacheType type)
    {
        try
        {
            _logger.LogWarning("Yêu cầu xóa Cache được kích hoạt cho loại: {Type}", type);

            switch (type)
            {
                case CacheType.QueueItem:
                    _queueItemsCache.Clear();
                    break;
                case CacheType.Queue:
                    _queuesCache.Clear();
                    break;
                case CacheType.Screen:
                    _screensCache.Clear();
                    break;
                case CacheType.Parameter:
                    _parametersCache.Clear();
                    break;
                case CacheType.All:
                    _queueItemsCache.Clear();
                    _queuesCache.Clear();
                    _screensCache.Clear();
                    _clientsCache.Clear();
                    _parametersCache.Clear();
                    break;
                default:
                    _logger.LogError("Loại Cache không hợp lệ để xóa: {Type}", type);
                    return false;
            }

            _logger.LogInformation("Đã xóa trắng Cache loại {Type} thành công.", type);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi xảy ra khi cố gắng xóa Cache loại {Type}", type);
            return false;
        }
    }

    public List<QueueItemDto> GetQueueItemsByQueue(int queueId, int? take = null)
    {
        try
        {
            // 1. Lấy danh sách từ Cache theo QueueId (Đã được phân loại trong ConcurrentDictionary)
            if (!_queueItemsCache.TryGetValue(queueId, out var items))
            {
                return new List<QueueItemDto>();
            }

            // 2. Lọc và Sắp xếp
            // Code cũ dùng q.State != Status.None, giả sử Status.None = 0 hoặc -1 (đã hủy)
            var query = items
                .Where(q => q.STATE != -1)
                .OrderBy(q => q.ORDER);

            // 3. Xử lý lấy số lượng giới hạn (nếu có)
            if (take.HasValue && take.Value > 0)
            {
                return query.Take(take.Value).ToList();
            }

            return query.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách QueueItem cho Queue: {QueueId}", queueId);
            return new List<QueueItemDto>();
        }
    }
    #endregion
}