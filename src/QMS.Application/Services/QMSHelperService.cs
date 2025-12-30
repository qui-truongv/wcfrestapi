using Microsoft.Extensions.Logging;
using QMS.Application.Interfaces;
using QMS.Core.Interfaces;

namespace QMS.Application.Services;

/// <summary>
/// Helper service for QMS calculations and validations
/// Replaces static methods from BVBase.cs
/// </summary>
public class QMSHelperService : IQMSHelperService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQMSCacheService _cacheService;
    private readonly ILogger<QMSHelperService> _logger;

    public QMSHelperService(
        IUnitOfWork unitOfWork,
        IQMSCacheService cacheService,
        ILogger<QMSHelperService> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _logger = logger;
    }

    #region Sequence Number Generation

    /// <summary>
    /// Get max sequence number for a queue on a specific date
    /// </summary>
    public async Task<int> GetMaxSequenceAsync(int queueId, DateTime date)
    {
        try
        {
            var maxSequence = await _unitOfWork.Queues.GetMaxSequenceAsync(queueId, date);
            return maxSequence;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting max sequence for queue {QueueId}", queueId);
            return 0;
        }
    }

    /// <summary>
    /// Calculate priority sequence using insertion algorithm
    /// Logic từ QMS_STTUuTien trong BVBase
    /// </summary>
    public async Task<int> CalculatePrioritySequenceAsync(int queueId, DateTime date)
    {
        try
        {
            var slChenUT = await GetIntParameterAsync("SLChenUT", 5);

            // Get all items for the day
            var allItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.CREATEDATE == date.Date);

            var itemsList = allItems.OrderByDescending(qi => qi.SEQUENCE).ToList();

            if (!itemsList.Any())
                return 1;

            // Current calling number
            var callingItem = itemsList.FirstOrDefault(qi => qi.STATE == 0);
            int sttCalling = callingItem?.SEQUENCE ?? 0;

            // Last priority number
            var priorityItems = itemsList.Where(qi => qi.PRIORITY == 1).ToList();
            int sttPriorityLast = priorityItems.Any()
                ? priorityItems.First().SEQUENCE ?? 0
                : 0;

            // Current max sequence
            int sttCurrent = itemsList.First().SEQUENCE ?? 0;

            // Current max normal sequence
            var normalItems = itemsList.Where(qi => qi.PRIORITY == 0).ToList();
            int sttNormalCurrent = normalItems.Any()
                ? normalItems.First().SEQUENCE ?? 0
                : 0;

            // Calculate insertion position
            if (sttPriorityLast == 0) // No priority yet
            {
                if (sttNormalCurrent < slChenUT + 1)
                {
                    return sttNormalCurrent + 1;
                }

                if (sttCalling == 0)
                {
                    return slChenUT + 1;
                }

                if (sttCalling + slChenUT + 1 == sttCurrent)
                {
                    return sttCurrent;
                }

                if (sttCalling + slChenUT + 1 > sttCurrent)
                {
                    return sttCurrent + 1;
                }

                if (sttCalling + slChenUT + 1 < sttCurrent)
                {
                    return sttCalling + slChenUT + 1;
                }
            }
            else // Has priority
            {
                if (sttPriorityLast + slChenUT + 1 == sttCurrent)
                {
                    return sttCurrent;
                }

                if (sttPriorityLast + slChenUT + 1 > sttCurrent)
                {
                    return sttCurrent + 1;
                }

                if (sttPriorityLast + slChenUT + 1 < sttCurrent)
                {
                    if (sttCalling > sttPriorityLast)
                    {
                        if (sttCalling + slChenUT + 1 >= sttCurrent)
                        {
                            return sttCurrent + 1;
                        }
                        return sttCalling + slChenUT + 1;
                    }
                    else
                    {
                        if (sttPriorityLast + slChenUT + 1 > sttCurrent)
                        {
                            return sttCurrent + 1;
                        }
                        return sttPriorityLast + slChenUT + 1;
                    }
                }
            }

            return sttCurrent + 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating priority sequence for queue {QueueId}", queueId);
            return -1;
        }
    }

    /// <summary>
    /// Calculate priority sequence for Kham Benh specifically
    /// Logic từ QMS_STTUuTien_KHAMBENH trong BVBase
    /// </summary>
    public async Task<int> CalculatePrioritySequenceKhamBenhAsync(int queueId, DateTime date)
    {
        try
        {
            var slChenUT = await GetIntParameterAsync("SLChenUT", 5);

            var allItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.CREATEDATE == date.Date);

            var itemsList = allItems.OrderByDescending(qi => qi.SEQUENCE).ToList();

            if (!itemsList.Any())
                return 1;

            // Get key sequences
            int sttCalling = itemsList
                .Where(qi => qi.STATE == 0)
                .Select(qi => qi.SEQUENCE ?? 0)
                .FirstOrDefault();

            int sttPriorityLast = itemsList
                .Where(qi => qi.PREVIOUS == 1)
                .Select(qi => qi.SEQUENCE ?? 0)
                .FirstOrDefault();

            int sttCurrent = itemsList
                .Select(qi => qi.SEQUENCE ?? 0)
                .First();

            int sttNormalCurrent = itemsList
                .Where(qi => qi.PRIORITY == 0)
                .OrderByDescending(qi => qi.SEQUENCE)
                .Select(qi => qi.SEQUENCE ?? 0)
                .FirstOrDefault();

            int sttDoneLast = itemsList
                .Where(qi => qi.STATE == 2)
                .Select(qi => qi.SEQUENCE ?? 0)
                .FirstOrDefault();

            // Logic: Find insertion position
            // Case 1: First number
            if (sttCurrent == 0)
                return 1;

            // Case 2: Has priority but no normal
            if (sttPriorityLast != 0 && sttNormalCurrent == 0)
                return sttPriorityLast + 1;

            // Use done as calling if no active calling
            if (sttCalling == 0)
                sttCalling = sttDoneLast;

            // Case 3: No priority yet
            if (sttPriorityLast == 0)
            {
                for (int i = 1; i < int.MaxValue; i++)
                {
                    var insertPos = (slChenUT * i) + i;

                    if (!itemsList.Any(qi => qi.SEQUENCE == insertPos))
                    {
                        if (sttCalling < insertPos)
                        {
                            return sttCurrent > insertPos ? insertPos : sttCurrent + 1;
                        }
                    }
                }
            }
            // Case 4: Has priority
            else
            {
                for (int i = 1; i < int.MaxValue; i++)
                {
                    var insertPos = sttPriorityLast + (slChenUT * i) + i;

                    if (!itemsList.Any(qi => qi.SEQUENCE == insertPos))
                    {
                        if (sttCalling < insertPos)
                        {
                            return sttCurrent > insertPos ? insertPos : sttCurrent + 1;
                        }
                    }
                }
            }

            return sttCurrent + 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating priority sequence (Kham Benh) for queue {QueueId}", queueId);
            return -1;
        }
    }

    #endregion

    #region Display Text Generation

    /// <summary>
    /// Generate display text with prefix/suffix
    /// </summary>
    public async Task<string> GenerateDisplayTextAsync(int sequence, int? priority = null)
    {
        try
        {
            var length = await GetIntParameterAsync("DoDaiSTT", 4);
            var displayText = sequence.ToString().PadLeft(length, '0');

            if (priority.HasValue && priority.Value > 0)
            {
                var priorityText = await GetParameterAsync("BNUT", "UT");
                var prefix = await GetParameterAsync("Prefix", "False");
                var suffix = await GetParameterAsync("Suffix", "False");

                if (prefix == "True")
                {
                    displayText = priorityText + displayText;
                }
                else if (suffix == "True")
                {
                    displayText = displayText + priorityText;
                }
            }

            return displayText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating display text for sequence {Sequence}", sequence);
            return sequence.ToString();
        }
    }

    /// <summary>
    /// Calculate ORDER field for priority insertion
    /// Logic từ QMS_GetOrderNext trong BVBase
    /// </summary>
    public async Task<string> CalculateOrderForPriorityAsync(int queueId, string displayText, DateTime date)
    {
        try
        {
            var step = await GetIntParameterAsync("numpriority", 3);
            var length = await GetIntParameterAsync("DoDaiSTT", 4);

            var waitingItems = await _unitOfWork.QueueItems.FindAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.CREATEDATE == date.Date &&
                (qi.STATE == 1 || qi.STATE == 100)); // Wait or None

            var normalItems = waitingItems
                .Where(qi => qi.PRIORITY == 0)
                .OrderByDescending(qi => qi.ORDER)
                .ToList();

            if (!normalItems.Any())
                return displayText;

            var priorityItems = waitingItems
                .Where(qi => qi.PRIORITY != 0)
                .OrderByDescending(qi => qi.ORDER)
                .ToList();

            var maxPriorityItem = priorityItems.FirstOrDefault();

            if (maxPriorityItem != null)
            {
                if (maxPriorityItem.PREVIOUS.HasValue && maxPriorityItem.PREVIOUS.Value > 0)
                {
                    var previousSeq = maxPriorityItem.PREVIOUS.Value;
                    var nextItems = normalItems
                        .Where(qi => qi.SEQUENCE > previousSeq)
                        .OrderBy(qi => qi.SEQUENCE)
                        .Take(step)
                        .ToList();

                    if (nextItems.Any())
                    {
                        var lastItem = nextItems.Last();
                        return lastItem.ORDER + "c";
                    }
                    else
                    {
                        var nextPriority = priorityItems
                            .OrderBy(qi => qi.SEQUENCE)
                            .LastOrDefault();

                        if (nextPriority != null)
                        {
                            var previous = nextPriority.SEQUENCE ?? 0;
                            return previous.ToString().PadLeft(length, '0') + "d";
                        }
                    }

                    return displayText;
                }
                else
                {
                    var previousSeq = maxPriorityItem.SEQUENCE ?? 0;
                    var nextItems = normalItems
                        .Where(qi => qi.SEQUENCE > previousSeq)
                        .OrderBy(qi => qi.SEQUENCE)
                        .Take(step)
                        .ToList();

                    if (nextItems.Any())
                    {
                        var lastItem = nextItems.Last();
                        return lastItem.ORDER + "b";
                    }

                    return displayText;
                }
            }
            else
            {
                var minItem = normalItems.Last();
                var nextItem = normalItems
                    .OrderBy(qi => qi.SEQUENCE)
                    .Take(step)
                    .LastOrDefault();

                if (nextItem != null)
                {
                    return nextItem.ORDER + "a";
                }

                return displayText;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating order for priority");
            return displayText;
        }
    }

    #endregion

    #region Time Calculations

    /// <summary>
    /// Calculate estimate time for new queue item
    /// Logic từ QMS_TinhTGCho_AddNew
    /// </summary>
    public async Task<DateTime> CalculateEstimateTimeAsync(int queueId, DateTime createDate)
    {
        try
        {
            // Get start time parameter
            var startTimeParam = await GetParameterAsync("GIOBATDAU", "7:30");
            var timeParts = startTimeParam.Split(':');
            var startTime = new TimeSpan(
                int.Parse(timeParts[0]),
                int.Parse(timeParts[1]),
                0);

            var estimateTime = createDate.Date.Add(startTime);

            // If start time is in the past, use current time
            if (estimateTime < createDate)
            {
                estimateTime = createDate;
            }

            // Add waiting time
            var waitingMinutes = await CalculateWaitingTimeAsync(queueId);
            estimateTime = estimateTime.AddMinutes(waitingMinutes);

            return estimateTime;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating estimate time for queue {QueueId}", queueId);
            return createDate;
        }
    }

    /// <summary>
    /// Calculate waiting time based on queue status
    /// </summary>
    public async Task<int> CalculateWaitingTimeAsync(int queueId)
    {
        try
        {
            var defaultProcessTime = await GetIntParameterAsync("TGChoTB", 5);
            var fluctuation = await GetIntParameterAsync("TGDaoDong", 0);

            // Get active clients for this queue
            var clients = await _unitOfWork.Clients.GetClientsByQueueIdAsync(queueId);
            var activeClients = clients.Where(c => c.STATE == 1).ToList();

            int processTime = defaultProcessTime;

            if (activeClients.Any())
            {
                var client = activeClients.First();
                processTime = client.PROCESSDURATION ?? defaultProcessTime;
            }

            // Calculate based on waiting items
            var waitingCount = await _unitOfWork.QueueItems.CountByStateAsync(
                queueId,
                1, // Wait state
                DateTime.Today);

            var totalTime = processTime + fluctuation;

            return totalTime > 0 ? totalTime : 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating waiting time for queue {QueueId}", queueId);
            return 5; // Default 5 minutes
        }
    }

    #endregion

    #region Validation

    /// <summary>
    /// Check if patient already has a ticket in queue
    /// </summary>
    public async Task<bool> CheckPatientExistsInQueueAsync(int queueId, string patientCode, DateTime date)
    {
        try
        {
            if (string.IsNullOrEmpty(patientCode))
                return false;

            var existingItem = await _unitOfWork.QueueItems.FirstOrDefaultAsync(qi =>
                qi.QUEUE_ID == queueId &&
                qi.PATIENTCODE == patientCode &&
                qi.CREATEDATE == date.Date &&
                (qi.STATE == 0 || qi.STATE == 1 || qi.STATE == 3 ||
                 qi.STATE == null || qi.STATE == 100));

            return existingItem != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking patient exists in queue");
            return false;
        }
    }

    /// <summary>
    /// Check if queue is manual
    /// </summary>
    public async Task<bool> IsQueueManualAsync(int queueId)
    {
        try
        {
            var queue = await _unitOfWork.Queues.GetByIdAsync(queueId);
            return queue?.IS_MANUAL == 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if queue is manual");
            return false;
        }
    }

    #endregion

    #region Parameter Helpers

    /// <summary>
    /// Get parameter value from cache or database
    /// </summary>
    public async Task<string> GetParameterAsync(string code, string defaultValue = "")
    {
        try
        {
            // Try cache first
            var value = await _cacheService.GetCachedParameterAsync(code);

            if (!string.IsNullOrEmpty(value))
                return value;

            // Fallback to database
            value = await _unitOfWork.Parameters.GetValueAsync(code);

            return value ?? defaultValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting parameter {Code}", code);
            return defaultValue;
        }
    }

    /// <summary>
    /// Get integer parameter
    /// </summary>
    public async Task<int> GetIntParameterAsync(string code, int defaultValue = 0)
    {
        try
        {
            var value = await GetParameterAsync(code);

            if (int.TryParse(value, out int result))
                return result;

            return defaultValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting int parameter {Code}", code);
            return defaultValue;
        }
    }

    /// <summary>
    /// Get boolean parameter
    /// </summary>
    public async Task<bool> GetBoolParameterAsync(string code, bool defaultValue = false)
    {
        try
        {
            var value = await GetParameterAsync(code);

            if (bool.TryParse(value, out bool result))
                return result;

            return value?.ToLower() == "true" || defaultValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bool parameter {Code}", code);
            return defaultValue;
        }
    }

    #endregion
}