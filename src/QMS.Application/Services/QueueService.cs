using AutoMapper;
using Microsoft.Extensions.Logging;
using QMS.Application.DTOs.Queue;
using QMS.Application.Interfaces;
using QMS.Core.Entities;
using QMS.Core.Interfaces;

namespace QMS.Application.Services;

/// <summary>
/// Complete Queue Service with all business logic from BVBase
/// </summary>
public class QueueService : IQueueService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<QueueService> _logger;
    private readonly IQMSHelperService _helper;
    private readonly IQMSCacheService _cacheService;

    public QueueService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<QueueService> logger,
        IQMSHelperService helper,
        IQMSCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _helper = helper;
        _cacheService = cacheService;
    }

    #region Queue Management

    public async Task<QueueDto?> GetQueueByIdAsync(int queueId)
    {
        try
        {
            var queue = await _unitOfWork.Queues.GetQueueWithDetailsAsync(queueId);
            return queue != null ? _mapper.Map<QueueDto>(queue) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue by ID: {QueueId}", queueId);
            throw;
        }
    }

    public async Task<QueueDto?> GetQueueByDepartmentIdAsync(int departmentId)
    {
        try
        {
            var queue = await _unitOfWork.Queues.GetByDepartmentIdAsync(departmentId);
            return queue != null ? _mapper.Map<QueueDto>(queue) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue by department ID: {DepartmentId}", departmentId);
            throw;
        }
    }

    public async Task<List<QueueDto>> GetActiveQueuesAsync()
    {
        try
        {
            var queues = await _unitOfWork.Queues.GetActiveQueuesAsync();
            return _mapper.Map<List<QueueDto>>(queues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active queues");
            throw;
        }
    }

    public async Task<List<QueueDto>> GetQueuesByScreenIdAsync(int screenId)
    {
        try
        {
            var queues = await _unitOfWork.Queues.GetQueuesByScreenIdAsync(screenId);
            return _mapper.Map<List<QueueDto>>(queues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queues by screen ID: {ScreenId}", screenId);
            throw;
        }
    }

    public async Task<QueueStatisticsDto> GetQueueStatisticsAsync(int queueId, DateTime? date = null)
    {
        try
        {
            var targetDate = date?.Date ?? DateTime.Today;

            var queue = await _unitOfWork.Queues.GetByIdAsync(queueId);
            if (queue == null)
            {
                throw new ArgumentException($"Queue with ID {queueId} not found");
            }

            var statistics = new QueueStatisticsDto
            {
                Queue_ID = queueId,
                QueueName = queue.NAME,
                Wait = await _unitOfWork.QueueItems.CountByStateAsync(queueId, 1, targetDate),
                Process = await _unitOfWork.QueueItems.CountByStateAsync(queueId, 0, targetDate),
                Miss = await _unitOfWork.QueueItems.CountByStateAsync(queueId, 3, targetDate)
            };

            var completedItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.CREATEDATE == targetDate &&
                (qi.STATE == 2 || qi.STATE == -1));

            statistics.Done = completedItems.Count(qi => qi.STATE == 2);
            statistics.Cancel = completedItems.Count(qi => qi.STATE == -1);
            statistics.Total = statistics.Wait + statistics.Process + statistics.Done +
                              statistics.Miss + statistics.Cancel;

            var processingItem = await _unitOfWork.QueueItems.GetProcessingItemAsync(queueId);
            statistics.ProcessingSequence = processingItem?.DISPLAYTEXT;

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue statistics for queue ID: {QueueId}", queueId);
            throw;
        }
    }

    #endregion

    #region Queue Item Queries

    public async Task<QueueItemDto?> GetQueueItemByIdAsync(int itemId)
    {
        try
        {
            var item = await _unitOfWork.QueueItems.GetByIdAsync(itemId);
            return item != null ? _mapper.Map<QueueItemDto>(item) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue item by ID: {ItemId}", itemId);
            throw;
        }
    }

    public async Task<List<QueueItemDto>> GetQueueItemsAsync(
        int queueId,
        DateTime? date = null,
        int? state = null,
        int? limit = null)
    {
        try
        {
            var targetDate = date?.Date ?? DateTime.Today;
            var items = await _unitOfWork.QueueItems.GetByQueueIdAsync(queueId, targetDate, state);

            if (limit.HasValue && limit.Value > 0)
            {
                items = items.Take(limit.Value).ToList();
            }

            return _mapper.Map<List<QueueItemDto>>(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue items for queue ID: {QueueId}", queueId);
            throw;
        }
    }

    public async Task<QueueItemDto?> FindQueueItemByPatientCodeAsync(
        int queueId,
        string patientCode,
        DateTime? date = null)
    {
        try
        {
            var targetDate = date?.Date ?? DateTime.Today;
            var item = await _unitOfWork.QueueItems.GetByPatientCodeAsync(queueId, patientCode, targetDate);
            return item != null ? _mapper.Map<QueueItemDto>(item) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding queue item by patient code: {PatientCode}", patientCode);
            throw;
        }
    }

    public async Task<QueueItemDto?> FindQueueItemByDisplayTextAsync(
        int queueId,
        string displayText,
        DateTime? date = null)
    {
        try
        {
            var targetDate = date?.Date ?? DateTime.Today;
            var item = await _unitOfWork.QueueItems.GetByDisplayTextAsync(queueId, displayText, targetDate);
            return item != null ? _mapper.Map<QueueItemDto>(item) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding queue item by display text: {DisplayText}", displayText);
            throw;
        }
    }

    #endregion

    #region Parameter Management

    public async Task<string?> GetParameterValueAsync(string code)
    {
        try
        {
            return await _helper.GetParameterAsync(code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting parameter value: {Code}", code);
            throw;
        }
    }

    public async Task<Dictionary<string, string>> GetAllParametersAsync()
    {
        try
        {
            return await _unitOfWork.Parameters.GetAllParametersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all parameters");
            throw;
        }
    }

    #endregion

    #region AddNew Logic - Converted from BVBase

    /// <summary>
    /// Add new queue item - Logic từ BVBase.AddNew()
    /// </summary>
    public async Task<QueueItemDto> AddNewQueueItemAsync(CreateQueueItemDto dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var createDate = dto.NGAYCAPSTT?.Date ?? DateTime.Now;
            if (createDate.Date == DateTime.Today)
            {
                createDate = DateTime.Now; // Include time for today
            }

            // 1. Get Queue Information
            int queueId;
            string queueName;
            string? tenBacSi = null;

            if (dto.DEPARTMENT_ID > 0)
            {
                var queue = await _unitOfWork.Queues.GetByDepartmentIdAsync(dto.DEPARTMENT_ID);
                if (queue == null)
                {
                    throw new InvalidOperationException(
                        $"Queue not found for department ID: {dto.DEPARTMENT_ID}");
                }
                queueId = queue.ID;
                queueName = queue.NAME;
                tenBacSi = queue.TENBACSI;
            }
            else if (dto.QUEUE_ID.HasValue && dto.QUEUE_ID.Value > 0)
            {
                var queue = await _unitOfWork.Queues.GetQueueWithDetailsAsync(dto.QUEUE_ID.Value);
                if (queue == null)
                {
                    throw new InvalidOperationException($"Queue not found with ID: {dto.QUEUE_ID}");
                }
                queueId = queue.ID;
                queueName = queue.NAME;
                tenBacSi = queue.TENBACSI;
            }
            else
            {
                throw new ArgumentException("Either DEPARTMENT_ID or QUEUE_ID must be provided");
            }

            // 2. Check if patient already has a ticket
            if (!string.IsNullOrEmpty(dto.PATIENTCODE))
            {
                var exists = await _helper.CheckPatientExistsInQueueAsync(
                    queueId, dto.PATIENTCODE, createDate.Date);

                if (exists)
                {
                    var existingItem = await _unitOfWork.QueueItems.GetByPatientCodeAsync(
                        queueId, dto.PATIENTCODE, createDate.Date);

                    _logger.LogWarning(
                        "Patient {PatientCode} already has ticket in queue {QueueId}",
                        dto.PATIENTCODE, queueId);

                    var existingDto = _mapper.Map<QueueItemDto>(existingItem);
                    existingDto.QueueName = queueName;
                    return existingDto;
                }
            }

            // 3. Generate Sequence Number
            int sequence = await _helper.GetMaxSequenceAsync(queueId, createDate);
            sequence++;

            // Check cache for max sequence
            var cachedItems = _cacheService.GetCachedQueueItems(queueId);
            if (cachedItems.Any())
            {
                var maxCached = cachedItems.Max(qi => qi.SEQUENCE ?? 0);
                if (maxCached >= sequence)
                {
                    sequence = maxCached + 1;
                }
            }

            // 4. Get Display Parameters
            var length = await _helper.GetIntParameterAsync("DoDaiSTT", 4);
            var prefix = await _helper.GetParameterAsync("Prefix", "False");
            var suffix = await _helper.GetParameterAsync("Suffix", "False");
            var remarks = await _helper.GetParameterAsync("REMARKS", "");

            string displayText = sequence.ToString().PadLeft(length, '0');
            string order = displayText;
            int previous = 0;

            // 5. Handle Priority Queue
            if (dto.PRIORITY > 0)
            {
                // Calculate priority sequence
                var prioritySeq = await _helper.CalculatePrioritySequenceKhamBenhAsync(
                    queueId, createDate);

                if (prioritySeq > 0)
                {
                    sequence = prioritySeq;
                    displayText = sequence.ToString().PadLeft(length, '0');
                }

                // Calculate order for insertion
                order = await _helper.CalculateOrderForPriorityAsync(
                    queueId, displayText, createDate);

                // Add priority prefix/suffix
                var priorityText = await _helper.GetParameterAsync("BNUT", "UT");
                if (prefix == "True")
                {
                    displayText = priorityText + displayText;
                }
                else if (suffix == "True")
                {
                    displayText = displayText + priorityText;
                }
            }

            // 6. Calculate Estimate Time
            var estimateTime = await _helper.CalculateEstimateTimeAsync(queueId, createDate);

            // 7. Get BenhVien_ID
            var benhVienId = await _helper.GetParameterAsync("BenhVien_ID", "");

            // 8. Create Queue Item
            var newItem = new QMS_QUEUE_ITEM
            {
                QUEUE_ID = queueId,
                PATIENTCODE = dto.PATIENTCODE,
                PATIENTNAME = dto.PATIENTNAME,
                PATIENTYOB = dto.PATIENTYOB,
                CREATEDATE = createDate.Date,
                CREATETIME = createDate,
                ORDER = order,
                SEQUENCE = sequence,
                PREFIX = prefix,
                DISPLAYTEXT = displayText,
                SUFFIX = suffix,
                PREVIOUS = previous,
                ESTIMATETIME = estimateTime,
                PRIORITY = dto.PRIORITY,
                STATE = dto.STATE,
                CLIENT_ID = dto.CLIENT_ID,
                CLIENT_NAME = dto.CLIENT_NAME,
                BENHVIEN_ID = benhVienId,
                NGAYTAO = DateTime.Now,
                REMARKS = remarks,
                ISMATOATHUOC = dto.ISMATOATHUOC ?? 0,
                SOTIEN = dto.SOTIEN,
                TENCUA = dto.TENCUA
            };

            await _unitOfWork.QueueItems.AddAsync(newItem);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Created queue item for patient {PatientCode} in queue {QueueId}, Sequence: {Sequence}",
                dto.PATIENTCODE, queueId, sequence);

            // 9. Add to Cache
            var cacheItem = new QueueItemDto
            {
                ID = newItem.ID,
                QUEUE_ID = queueId,
                QueueName = queueName,
                SEQUENCE = sequence,
                DISPLAYTEXT = displayText,
                ORDER = order,
                PATIENTCODE = dto.PATIENTCODE,
                PATIENTNAME = dto.PATIENTNAME,
                PATIENTYOB = dto.PATIENTYOB,
                PRIORITY = dto.PRIORITY,
                STATE = dto.STATE,
                CREATEDATE = createDate.Date,
                CREATETIME = createDate,
                ESTIMATETIME = estimateTime,
                CLIENT_ID = dto.CLIENT_ID,
                CLIENT_NAME = dto.CLIENT_NAME,
                ISMATOATHUOC = dto.ISMATOATHUOC,
                SOTIEN = dto.SOTIEN,
                TENCUA = dto.TENCUA
            };

            _cacheService.AddQueueItemToCache(cacheItem);

            return cacheItem;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error adding new queue item");
            throw;
        }
    }

    /// <summary>
    /// Add new for Tiep Nhan (Reception) - Logic từ BVBase.AddNewTiepNhan()
    /// </summary>
    public async Task<QueueItemDto> AddNewTiepNhanAsync(
        int queueId,
        int priority,
        string patientCode)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var createDate = DateTime.Now;

            // 1. Get Queue
            var queue = await _unitOfWork.Queues.GetByIdAsync(queueId);
            if (queue == null)
            {
                throw new ArgumentException($"Queue with ID {queueId} not found");
            }

            // 2. Generate Sequence
            int sequence = await _helper.GetMaxSequenceAsync(queueId, createDate);
            sequence++;

            // 3. Get Parameters
            var numStart = await _helper.GetIntParameterAsync("NUMSTART", 0);
            var length = await _helper.GetIntParameterAsync("DoDaiSTT", 4);
            var prefix = await _helper.GetParameterAsync("Prefix", "False");
            var suffix = await _helper.GetParameterAsync("Suffix", "False");
            var remarks = await _helper.GetParameterAsync("REMARKS", "");

            string displayText = (numStart + sequence).ToString().PadLeft(length, '0');
            string order = displayText;
            int previous = 0;

            // 4. Handle Priority
            if (priority > 0)
            {
                order = await _helper.CalculateOrderForPriorityAsync(
                    queueId, displayText, createDate);

                if (previous > 0)
                {
                    displayText = previous.ToString().PadLeft(length, '0');
                }

                var priorityText = await _helper.GetParameterAsync("priorityKios", "UT");
                if (prefix == "True")
                {
                    displayText = priorityText + displayText;
                }
                else if (suffix == "True")
                {
                    displayText = displayText + priorityText;
                }
            }

            // 5. Calculate Estimate Time
            var estimateTime = await _helper.CalculateEstimateTimeAsync(queueId, createDate);

            // 6. Get BenhVien_ID
            var benhVienId = await _helper.GetParameterAsync("BenhVien_ID", "");

            // 7. Create Item
            var newItem = new QMS_QUEUE_ITEM
            {
                QUEUE_ID = queueId,
                CREATEDATE = createDate.Date,
                CREATETIME = createDate,
                ORDER = order,
                SEQUENCE = sequence,
                PATIENTCODE = patientCode,
                PREFIX = prefix,
                DISPLAYTEXT = displayText,
                SUFFIX = suffix,
                PREVIOUS = previous,
                PRIORITY = priority,
                STATE = 1, // Wait
                ESTIMATETIME = estimateTime,
                BENHVIEN_ID = benhVienId,
                NGAYTAO = createDate,
                REMARKS = remarks,
                ISMATOATHUOC = 0
            };

            await _unitOfWork.QueueItems.AddAsync(newItem);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Created Tiep Nhan item in queue {QueueId}, Sequence: {Sequence}",
                queueId, sequence);

            // 8. Return DTO
            var result = new QueueItemDto
            {
                ID = newItem.ID,
                QUEUE_ID = queueId,
                QueueName = queue.NAME,
                SEQUENCE = sequence,
                DISPLAYTEXT = displayText,
                ORDER = order,
                PATIENTCODE = patientCode,
                CREATEDATE = createDate.Date,
                CREATETIME = createDate,
                ESTIMATETIME = estimateTime,
                STATE = 1,
                PRIORITY = priority,
                REMARKS = remarks
            };

            _cacheService.AddQueueItemToCache(result);

            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error adding Tiep Nhan item");
            throw;
        }
    }

    #endregion

    #region Queue Operations

    public async Task<QueueItemDto?> GetNextQueueItemAsync(int queueId, int clientId, string clientName)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var today = DateTime.Today;

            // Check if there's already a processing item
            var processingItem = await _unitOfWork.QueueItems.FirstOrDefaultAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.STATE == 0 &&
                (qi.CLIENT_ID == 0 || qi.CLIENT_ID == clientId) &&
                qi.CREATEDATE == today);

            if (processingItem != null)
            {
                _logger.LogInformation(
                    "Found existing processing item for queue {QueueId}, client {ClientId}",
                    queueId, clientId);
                return _mapper.Map<QueueItemDto>(processingItem);
            }

            // Get next waiting item
            var nextItem = await _unitOfWork.QueueItems.GetNextWaitingItemAsync(queueId);

            if (nextItem == null)
            {
                _logger.LogInformation("No waiting items found for queue {QueueId}", queueId);
                return null;
            }

            // Update to processing
            nextItem.STATE = 0;
            nextItem.PROCESSTIME = DateTime.Now;
            nextItem.CLIENT_ID = clientId;
            nextItem.CLIENT_NAME = clientName;

            _unitOfWork.QueueItems.Update(nextItem);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Assigned queue item {ItemId} to client {ClientId}",
                nextItem.ID, clientId);

            return _mapper.Map<QueueItemDto>(nextItem);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error getting next queue item for queue {QueueId}", queueId);
            throw;
        }
    }

    public async Task<bool> UpdateQueueItemStateAsync(
        int queueId,
        string displayText,
        int state,
        string? clientName = null,
        int? isMaToaThuoc = null)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var today = DateTime.Today;

            var item = await _unitOfWork.QueueItems.FirstOrDefaultAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.DISPLAYTEXT == displayText &&
                qi.CREATEDATE == today &&
                (isMaToaThuoc == null || qi.ISMATOATHUOC == isMaToaThuoc));

            if (item == null)
            {
                _logger.LogWarning(
                    "Queue item not found: Queue {QueueId}, Display {DisplayText}",
                    queueId, displayText);
                return false;
            }

            if (item.STATE == 2)
            {
                _logger.LogInformation("Queue item {ItemId} is already done", item.ID);
                return true;
            }

            item.STATE = state;
            item.CLIENT_NAME = clientName;

            if (state == 0)
            {
                item.PROCESSTIME = DateTime.Now;
            }
            else if (state == 2)
            {
                item.FINISHTIME = DateTime.Now;
            }

            _unitOfWork.QueueItems.Update(item);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Updated queue item {ItemId} state to {State}", item.ID, state);

            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating queue item state");
            throw;
        }
    }

    public async Task<bool> UpdateQueueItemStateByPatientCodeAsync(
        int queueId,
        string patientCode,
        int state,
        string? clientName = null,
        bool recall = false)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var today = DateTime.Today;

            var recallShowDisplay = await _helper.GetBoolParameterAsync("RecallShowDisplay", false);

            IEnumerable<QMS_QUEUE_ITEM> items;

            if (recall && recallShowDisplay)
            {
                items = await _unitOfWork.QueueItems.FindAsync(qi =>
                    qi.QUEUE_ID == queueId &&
                    qi.PATIENTCODE == patientCode &&
                    qi.CREATEDATE == today);
            }
            else
            {
                items = await _unitOfWork.QueueItems.FindAsync(qi =>
                    qi.QUEUE_ID == queueId &&
                    qi.PATIENTCODE == patientCode &&
                    qi.CREATEDATE == today &&
                    qi.STATE != 2);
            }

            var itemsList = items.ToList();
            if (!itemsList.Any())
            {
                _logger.LogWarning(
                    "No queue items found for patient {PatientCode} in queue {QueueId}",
                    patientCode, queueId);
                return false;
            }

            foreach (var item in itemsList)
            {
                item.STATE = state;
                item.CLIENT_NAME = clientName;

                if (state == 0)
                {
                    item.PROCESSTIME = DateTime.Now;
                }
                else if (state == 2)
                {
                    item.FINISHTIME = DateTime.Now;
                }

                _unitOfWork.QueueItems.Update(item);
            }

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Updated {Count} queue items for patient {PatientCode} to state {State}",
                itemsList.Count, patientCode, state);

            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating queue item state by patient code");
            throw;
        }
    }

    public async Task<bool> ClearMissedItemsAsync(int queueId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var today = DateTime.Today;

            var missedItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.STATE == 3 &&
                qi.CREATEDATE == today);

            var missedItemsList = missedItems.ToList();

            if (!missedItemsList.Any())
            {
                _logger.LogInformation("No missed items found for queue {QueueId}", queueId);
                return true;
            }

            foreach (var item in missedItemsList)
            {
                item.STATE = 1;
                item.CLIENT_ID = 0;
                item.CLIENT_NAME = null;
                _unitOfWork.QueueItems.Update(item);
            }

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Cleared {Count} missed items for queue {QueueId}",
                missedItemsList.Count, queueId);

            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error clearing missed items for queue {QueueId}", queueId);
            throw;
        }
    }

    public async Task<bool> MoveQueueItemAsync(
        int fromQueueId,
        string order,
        int toQueueId,
        string patientCode,
        int? departmentId = null)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var today = DateTime.Today;

            var originalItem = await _unitOfWork.QueueItems.FirstOrDefaultAsync(qi =>
                qi.QUEUE_ID == fromQueueId &&
                qi.ORDER == order &&
                qi.PATIENTCODE == patientCode &&
                qi.CREATEDATE == today);

            if (originalItem == null)
            {
                _logger.LogWarning("Original queue item not found for move operation");
                return false;
            }

            var estimateTime = await _helper.CalculateEstimateTimeAsync(toQueueId, DateTime.Now);

            var newItem = new QMS_QUEUE_ITEM
            {
                QUEUE_ID = toQueueId,
                SEQUENCE = originalItem.SEQUENCE,
                DISPLAYTEXT = originalItem.DISPLAYTEXT,
                ORDER = originalItem.ORDER,
                PATIENTCODE = originalItem.PATIENTCODE,
                PATIENTNAME = originalItem.PATIENTNAME,
                PATIENTYOB = originalItem.PATIENTYOB,
                CREATEDATE = today,
                CREATETIME = DateTime.Now,
                PRIORITY = originalItem.PRIORITY,
                PREVIOUS = originalItem.PREVIOUS,
                STATE = 1, // Wait
                ESTIMATETIME = estimateTime,
                BENHVIEN_ID = originalItem.BENHVIEN_ID,
                NGAYTAO = DateTime.Now,
                PREFIX = originalItem.PREFIX,
                SUFFIX = originalItem.SUFFIX,
                ISMATOATHUOC = originalItem.ISMATOATHUOC
            };

            await _unitOfWork.QueueItems.AddAsync(newItem);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Moved queue item from queue {FromQueue} to {ToQueue} for patient {PatientCode}",
                fromQueueId, toQueueId, patientCode);

            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error moving queue item");
            throw;
        }
    }

    #endregion
}