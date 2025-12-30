using AutoMapper;
using Microsoft.Extensions.Logging;
using QMS.Application.Common;
using QMS.Application.DTOs.Queue;
using QMS.Application.Interfaces;
using QMS.Core.Entities;
using QMS.Core.Interfaces;

namespace QMS.Application.Services;

public class QueueService : IQueueService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<QueueService> _logger;

    public QueueService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<QueueService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
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

            // Get done and cancelled from database (not in cache)
            var completedItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.CREATEDATE == targetDate &&
                (qi.STATE == 2 || qi.STATE == -1));

            statistics.Done = completedItems.Count(qi => qi.STATE == 2);
            statistics.Cancel = completedItems.Count(qi => qi.STATE == -1);
            statistics.Total = statistics.Wait + statistics.Process + statistics.Done + statistics.Miss + statistics.Cancel;

            // Get processing item
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

    public async Task<List<QueueItemDto>> GetQueueItemsAsync(int queueId, DateTime? date = null, int? state = null, int? limit = null)
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

    public async Task<QueueItemDto?> FindQueueItemByPatientCodeAsync(int queueId, string patientCode, DateTime? date = null)
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

    public async Task<QueueItemDto?> FindQueueItemByDisplayTextAsync(int queueId, string displayText, DateTime? date = null)
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
            return await _unitOfWork.Parameters.GetValueAsync(code);
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
            int? departmentId = null;

            if (dto.DEPARTMENT_ID > 0)
            {
                var queue = await _unitOfWork.Queues.GetByDepartmentIdAsync(dto.DEPARTMENT_ID);
                if (queue == null)
                {
                    throw new InvalidOperationException($"Queue not found for department ID: {dto.DEPARTMENT_ID}");
                }
                queueId = queue.ID;
                queueName = queue.NAME;
                departmentId = queue.DEPARTMENT_ID;
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
                departmentId = queue.DEPARTMENT_ID;
            }
            else
            {
                throw new ArgumentException("Either DEPARTMENT_ID or QUEUE_ID must be provided");
            }

            // 2. Check if patient already has a ticket
            if (!string.IsNullOrEmpty(dto.PATIENTCODE))
            {
                var existingItem = await _unitOfWork.QueueItems.FirstOrDefaultAsync(qi =>
                    qi.QUEUE_ID == queueId &&
                    qi.PATIENTCODE == dto.PATIENTCODE &&
                    qi.CREATEDATE == createDate.Date &&
                    (qi.STATE == 1 || qi.STATE == 0 || qi.STATE == 3 || qi.STATE == null || qi.STATE == 100));

                if (existingItem != null)
                {
                    _logger.LogWarning("Patient {PatientCode} already has a ticket in queue {QueueId}",
                        dto.PATIENTCODE, queueId);
                    return _mapper.Map<QueueItemDto>(existingItem);
                }
            }

            // 3. Generate Sequence Number
            int sequence = await _unitOfWork.Queues.GetMaxSequenceAsync(queueId, createDate);
            sequence++;

            // 4. Get Parameters
            var prefix = await GetParameterValueAsync("Prefix") ?? "";
            var suffix = await GetParameterValueAsync("Suffix") ?? "";
            var lengthStr = await GetParameterValueAsync("DoDaiSTT") ?? "4";
            int length = int.Parse(lengthStr);

            string displayText = sequence.ToString().PadLeft(length, '0');
            string order = displayText;
            int previous = 0;

            // 5. Handle Priority Queue (Insert logic)
            if (dto.PRIORITY > 0)
            {
                order = await CalculatePriorityOrderAsync(queueId, displayText, createDate);

                // Extract previous sequence from order if needed
                var priorityParam = await GetParameterValueAsync("BNUT");
                if (!string.IsNullOrEmpty(priorityParam))
                {
                    if (prefix == "True")
                    {
                        displayText = priorityParam + displayText;
                    }
                    else if (suffix == "True")
                    {
                        displayText = displayText + priorityParam;
                    }
                }
            }

            // 6. Calculate Estimate Time
            var estimateTime = await CalculateEstimateTimeAsync(queueId, createDate);

            // 7. Create Queue Item
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
                BENHVIEN_ID = await GetParameterValueAsync("BenhVienID"),
                NGAYTAO = DateTime.Now,
                ISMATOATHUOC = dto.ISMATOATHUOC ?? 0,
                SOTIEN = dto.SOTIEN,
                TENCUA = dto.TENCUA
            };

            await _unitOfWork.QueueItems.AddAsync(newItem);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Created queue item for patient {PatientCode} in queue {QueueId}, Sequence: {Sequence}",
                dto.PATIENTCODE, queueId, sequence);

            var result = _mapper.Map<QueueItemDto>(newItem);
            result.QueueName = queueName;

            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error adding new queue item");
            throw;
        }
    }

    // Helper method for priority order calculation
    private async Task<string> CalculatePriorityOrderAsync(int queueId, string displayText, DateTime date)
    {
        try
        {
            var numPriorityStr = await GetParameterValueAsync("numpriority") ?? "3";
            int step = int.Parse(numPriorityStr);

            // Get waiting items (normal priority)
            var normalItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.CREATEDATE == date.Date &&
                qi.PRIORITY == 0 &&
                (qi.STATE == 1 || qi.STATE == 100));

            var normalItemsList = normalItems.OrderByDescending(qi => qi.ORDER).ToList();

            if (!normalItemsList.Any())
            {
                return displayText;
            }

            // Get priority items
            var priorityItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.CREATEDATE == date.Date &&
                qi.PRIORITY != 0 &&
                (qi.STATE == 1 || qi.STATE == 100));

            var priorityItemsList = priorityItems.OrderByDescending(qi => qi.ORDER).ToList();
            var maxPriorityItem = priorityItemsList.FirstOrDefault();

            if (maxPriorityItem != null && maxPriorityItem.PREVIOUS.HasValue && maxPriorityItem.PREVIOUS.Value > 0)
            {
                int previousSequence = maxPriorityItem.PREVIOUS.Value;
                var nextItems = normalItemsList
                    .Where(qi => qi.SEQUENCE > previousSequence)
                    .OrderBy(qi => qi.SEQUENCE)
                    .Take(step)
                    .ToList();

                if (nextItems.Any())
                {
                    var lastItem = nextItems.Last();
                    return lastItem.ORDER + "c";
                }
            }
            else if (maxPriorityItem != null)
            {
                int previousSequence = maxPriorityItem.SEQUENCE ?? 0;
                var nextItems = normalItemsList
                    .Where(qi => qi.SEQUENCE > previousSequence)
                    .OrderBy(qi => qi.SEQUENCE)
                    .Take(step)
                    .ToList();

                if (nextItems.Any())
                {
                    var lastItem = nextItems.Last();
                    return lastItem.ORDER + "b";
                }
            }
            else
            {
                var minItem = normalItemsList.Last();
                int previousSequence = minItem.SEQUENCE ?? 0;
                var nextItem = normalItemsList
                    .Where(qi => qi.SEQUENCE < previousSequence + step)
                    .FirstOrDefault();

                if (nextItem != null)
                {
                    return nextItem.ORDER + "a";
                }
            }

            return displayText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating priority order");
            return displayText;
        }
    }

    // Helper method for estimate time calculation
    private async Task<DateTime> CalculateEstimateTimeAsync(int queueId, DateTime createDate)
    {
        try
        {
            // Get time parameters
            var startTimeParam = await GetParameterValueAsync("GIOBATDAU") ?? "7:30";
            var startTimeParts = startTimeParam.Split(':');
            var startTime = new TimeSpan(
                int.Parse(startTimeParts[0]),
                int.Parse(startTimeParts[1]),
                0);

            var estimationTime = createDate.Date.Add(startTime);

            if (estimationTime < createDate)
            {
                estimationTime = createDate;
            }

            // Calculate waiting time based on queue
            int waitingMinutes = await CalculateWaitingTimeAsync(queueId);

            return estimationTime.AddMinutes(waitingMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating estimate time");
            return createDate;
        }
    }

    // Helper method for waiting time calculation
    private async Task<int> CalculateWaitingTimeAsync(int queueId)
    {
        try
        {
            var waitingCount = await _unitOfWork.QueueItems.CountByStateAsync(queueId, 1, DateTime.Today);

            // Get average processing time from parameter or client
            var clients = await _unitOfWork.Clients.GetClientsByQueueIdAsync(queueId);
            int processDuration = clients.FirstOrDefault()?.PROCESSDURATION ?? 5; // Default 5 minutes

            return waitingCount * processDuration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating waiting time");
            return 0;
        }
    }

    public async Task<QueueItemDto?> GetNextQueueItemAsync(int queueId, int clientId, string clientName)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var today = DateTime.Today;

            // 1. Check if there's already a processing item
            var processingItem = await _unitOfWork.QueueItems.FirstOrDefaultAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.STATE == 0 && // Process
                (qi.CLIENT_ID == 0 || qi.CLIENT_ID == clientId) &&
                qi.CREATEDATE == today);

            if (processingItem != null)
            {
                _logger.LogInformation("Found existing processing item for queue {QueueId}, client {ClientId}",
                    queueId, clientId);
                return _mapper.Map<QueueItemDto>(processingItem);
            }

            // 2. Get next waiting item
            var nextItem = await _unitOfWork.QueueItems.FirstOrDefaultAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.CREATEDATE == today &&
                qi.STATE != 3 && // Not Miss
                qi.STATE != 100 && // Not None
                qi.STATE != 0 && // Not Process
                qi.STATE != 4 && // Not Remove
                qi.STATE != 2); // Not Done

            if (nextItem == null)
            {
                _logger.LogInformation("No waiting items found for queue {QueueId}", queueId);
                return null;
            }

            // 3. Update item to processing state
            nextItem.STATE = 0; // Process
            nextItem.PROCESSTIME = DateTime.Now;
            nextItem.CLIENT_ID = clientId;
            nextItem.CLIENT_NAME = clientName;

            _unitOfWork.QueueItems.Update(nextItem);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Assigned queue item {ItemId} to client {ClientId}",
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

            // Find the item
            var item = await _unitOfWork.QueueItems.FirstOrDefaultAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.DISPLAYTEXT == displayText &&
                qi.CREATEDATE == today &&
                (isMaToaThuoc == null || qi.ISMATOATHUOC == isMaToaThuoc));

            if (item == null)
            {
                _logger.LogWarning("Queue item not found: Queue {QueueId}, Display {DisplayText}",
                    queueId, displayText);
                return false;
            }

            // Check if already done
            if (item.STATE == 2)
            {
                _logger.LogInformation("Queue item {ItemId} is already done", item.ID);
                return true;
            }

            // Update state
            item.STATE = state;
            item.CLIENT_NAME = clientName;

            if (state == 0) // Process
            {
                item.PROCESSTIME = DateTime.Now;
            }
            else if (state == 2) // Done
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

            // Get recall parameter
            bool recallShowDisplay = false;
            var recallParam = await GetParameterValueAsync("RecallShowDisplay");
            if (!string.IsNullOrEmpty(recallParam))
            {
                bool.TryParse(recallParam, out recallShowDisplay);
            }

            // Find items based on recall setting
            IEnumerable<QMS_QUEUE_ITEM> items;

            if (recall && recallShowDisplay)
            {
                // Get all items including done
                items = await _unitOfWork.QueueItems.FindAsync(qi =>
                    qi.QUEUE_ID == queueId &&
                    qi.PATIENTCODE == patientCode &&
                    qi.CREATEDATE == today);
            }
            else
            {
                // Get only non-done items
                items = await _unitOfWork.QueueItems.FindAsync(qi =>
                    qi.QUEUE_ID == queueId &&
                    qi.PATIENTCODE == patientCode &&
                    qi.CREATEDATE == today &&
                    qi.STATE != 2); // Not Done
            }

            var itemsList = items.ToList();
            if (!itemsList.Any())
            {
                _logger.LogWarning("No queue items found for patient {PatientCode} in queue {QueueId}",
                    patientCode, queueId);
                return false;
            }

            // Update all found items
            foreach (var item in itemsList)
            {
                item.STATE = state;
                item.CLIENT_NAME = clientName;

                if (state == 0) // Process
                {
                    item.PROCESSTIME = DateTime.Now;
                }
                else if (state == 2) // Done
                {
                    item.FINISHTIME = DateTime.Now;
                }

                _unitOfWork.QueueItems.Update(item);
            }

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Updated {Count} queue items for patient {PatientCode} to state {State}",
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

            // Find all missed items
            var missedItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.STATE == 3 && // Miss
                qi.CREATEDATE == today);

            var missedItemsList = missedItems.ToList();

            if (!missedItemsList.Any())
            {
                _logger.LogInformation("No missed items found for queue {QueueId}", queueId);
                return true;
            }

            // Reset missed items to waiting
            foreach (var item in missedItemsList)
            {
                item.STATE = 1; // Wait
                item.CLIENT_ID = 0;
                item.CLIENT_NAME = null;
                _unitOfWork.QueueItems.Update(item);
            }

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Cleared {Count} missed items for queue {QueueId}",
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

            // Find the original item
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

            // Calculate waiting time for new queue
            int waitingMinutes = await CalculateWaitingTimeAsync(toQueueId);
            var estimateTime = await CalculateEstimateTimeAsync(toQueueId, DateTime.Now);

            // Create new item in target queue
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

            _logger.LogInformation("Moved queue item from queue {FromQueue} to {ToQueue} for patient {PatientCode}",
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