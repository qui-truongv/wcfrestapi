using AutoMapper;
using Microsoft.Extensions.Logging;
using QMS.Application.DTOs.Screen;
using QMS.Application.Interfaces;
using QMS.Core.Interfaces;

namespace QMS.Application.Services;

public class ScreenService : IScreenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ScreenService> _logger;

    public ScreenService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ScreenService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    #region Screen Management

    public async Task<ScreenDto?> GetScreenByIdAsync(int screenId)
    {
        try
        {
            var screen = await _unitOfWork.Screens.FirstOrDefaultAsync(s => s.ID == screenId);
            return screen != null ? _mapper.Map<ScreenDto>(screen) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting screen by ID: {ScreenId}", screenId);
            throw;
        }
    }

    public async Task<ScreenDto?> GetScreenByCodeAsync(string code)
    {
        try
        {
            var screen = await _unitOfWork.Screens.GetByCodeAsync(code);
            return screen != null ? _mapper.Map<ScreenDto>(screen) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting screen by code: {Code}", code);
            throw;
        }
    }

    public async Task<List<ScreenDto>> GetActiveScreensAsync()
    {
        try
        {
            var screens = await _unitOfWork.Screens.GetActiveScreensAsync();
            return _mapper.Map<List<ScreenDto>>(screens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active screens");
            throw;
        }
    }

    public async Task<List<ScreenDto>> GetScreensBySectionIdAsync(int sectionId)
    {
        try
        {
            var screens = await _unitOfWork.Screens.GetScreensBySectionIdAsync(sectionId);
            return _mapper.Map<List<ScreenDto>>(screens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting screens by section ID: {SectionId}", sectionId);
            throw;
        }
    }

    public async Task<string?> GetScreenNameAsync(int screenId)
    {
        try
        {
            var screen = await _unitOfWork.Screens.GetByIdAsync(screenId);
            return screen?.NAME;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting screen name: {ScreenId}", screenId);
            throw;
        }
    }

    public async Task<string?> GetScreenRemarksAsync(int screenId)
    {
        try
        {
            var screen = await _unitOfWork.Screens.GetByIdAsync(screenId);
            return screen?.REMARKS;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting screen remarks: {ScreenId}", screenId);
            throw;
        }
    }

    public async Task<int> GetDisplayRowsAsync(int screenId)
    {
        try
        {
            var screen = await _unitOfWork.Screens.GetByIdAsync(screenId);
            return screen?.DISPLAYROWS ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting display rows: {ScreenId}", screenId);
            throw;
        }
    }

    #endregion

    #region Screen Display

    public async Task<ScreenQueueDisplayDto> GetScreenQueueDisplayAsync(int screenId)
    {
        try
        {
            var screen = await _unitOfWork.Screens.FirstOrDefaultAsync(s => s.ID == screenId);

            if (screen == null)
            {
                throw new ArgumentException($"Screen with ID {screenId} not found");
            }

            var queues = await _unitOfWork.Queues.GetQueuesByScreenIdAsync(screenId);
            var today = DateTime.Today;

            var result = new ScreenQueueDisplayDto
            {
                ScreenId = screenId,
                ScreenName = screen.NAME,
                Queues = new List<QueueDisplayItemDto>()
            };

            foreach (var queue in queues)
            {
                var maxItems = queue.MAX ?? screen.DISPLAYROWS ?? 10;

                // Get queue items that should be displayed
                var items = await _unitOfWork.QueueItems.FindAsync(qi =>
                    qi.QUEUE_ID == queue.ID &&
                    qi.CREATEDATE == today &&
                    qi.STATE != 3 && // Not Miss
                    qi.STATE != 100 && // Not None
                    qi.STATE != 4 && // Not Remove
                    qi.STATE != -1 && // Not Cancel
                    qi.STATE != 2); // Not Done

                var itemsList = items
                    .OrderBy(qi => qi.STATE) // Process first (0), then Wait (1)
                    .ThenBy(qi => qi.ORDER)
                    .Take(maxItems)
                    .ToList();

                var queueDisplay = new QueueDisplayItemDto
                {
                    QueueId = queue.ID,
                    QueueName = queue.NAME,
                    Items = itemsList.Select(item => new QueueItemDisplayDto
                    {
                        ID = item.ID,
                        SEQUENCE = item.SEQUENCE,
                        DISPLAYTEXT = item.DISPLAYTEXT,
                        STATE = item.STATE,
                        StateName = GetStateName(item.STATE),
                        PRIORITY = item.PRIORITY,
                        PATIENTCODE = item.PATIENTCODE,
                        PATIENTNAME = item.PATIENTNAME,
                        PATIENTYOB = item.PATIENTYOB,
                        CLIENT_NAME = item.CLIENT_NAME,
                        CREATEDATE = item.CREATEDATE,
                        CREATETIME = item.CREATETIME,
                        SOTIEN = item.SOTIEN,
                        TENCUA = item.TENCUA,
                        TENBACSI = queue.TENBACSI,
                        TENDIEUDUONG = queue.TENDIEUDUONG
                    }).ToList()
                };

                result.Queues.Add(queueDisplay);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting screen queue display: {ScreenId}", screenId);
            throw;
        }
    }

    public async Task<List<ScreenListDto>> GetScreenListAsync()
    {
        try
        {
            var screens = await _unitOfWork.Screens.GetActiveScreensAsync();

            return screens.Select(s => new ScreenListDto
            {
                KhuVuc = s.NAME,
                Link = (s.URL ?? "") + s.ID,
                Remarks = s.REMARKS
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting screen list");
            throw;
        }
    }

    public async Task<List<ScreenListDto>> GetScreenListBySectionAsync(int sectionId)
    {
        try
        {
            var screens = await _unitOfWork.Screens.GetScreensBySectionIdAsync(sectionId);

            return screens.Select(s => new ScreenListDto
            {
                KhuVuc = s.NAME,
                Link = (s.URL ?? "") + s.ID,
                Remarks = s.REMARKS
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting screen list by section: {SectionId}", sectionId);
            throw;
        }
    }

    public async Task<List<SectionScreenDto>> GetSectionScreenListAsync()
    {
        try
        {
            var sections = await _unitOfWork.Sections.GetAllAsync();

            return sections.Select(s => new SectionScreenDto
            {
                TenKhuVuc = s.NAME ?? string.Empty,
                Link = $"kv?id={s.ID}"
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting section screen list");
            throw;
        }
    }

    #endregion

    #region Queue Display by Screen

    public async Task<int> GetQueueQuantityInScreenAsync(int screenId)
    {
        try
        {
            var queues = await _unitOfWork.Queues.FindAsync(q =>
                q.SCREEN_ID == screenId &&
                q.STATE == 1);

            return queues.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue quantity in screen: {ScreenId}", screenId);
            throw;
        }
    }

    public async Task<string?> GetQueueNameByScreenAsync(int screenId)
    {
        try
        {
            var queue = await _unitOfWork.Queues.FirstOrDefaultAsync(q =>
                q.SCREEN_ID == screenId);

            return queue?.NAME;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue name by screen: {ScreenId}", screenId);
            throw;
        }
    }

    #endregion

    #region Helper Methods

    private static string? GetStateName(int? state)
    {
        return state switch
        {
            0 => "Đang thực hiện",
            1 => "Đang chờ",
            2 => "Đã hoàn thành",
            3 => "Gọi nhỡ",
            4 => "Bỏ qua",
            -1 => "Đã hủy",
            100 => "Chưa xác định",
            _ => null
        };
    }

    #endregion
}