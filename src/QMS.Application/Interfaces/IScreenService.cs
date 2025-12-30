using QMS.Application.DTOs.Screen;

namespace QMS.Application.Interfaces;

public interface IScreenService
{
    // Screen Management
    Task<ScreenDto?> GetScreenByIdAsync(int screenId);
    Task<ScreenDto?> GetScreenByCodeAsync(string code);
    Task<List<ScreenDto>> GetActiveScreensAsync();
    Task<List<ScreenDto>> GetScreensBySectionIdAsync(int sectionId);
    Task<string?> GetScreenNameAsync(int screenId);
    Task<string?> GetScreenRemarksAsync(int screenId);
    Task<int> GetDisplayRowsAsync(int screenId);

    // Screen Display
    Task<ScreenQueueDisplayDto> GetScreenQueueDisplayAsync(int screenId);
    Task<List<ScreenListDto>> GetScreenListAsync();
    Task<List<ScreenListDto>> GetScreenListBySectionAsync(int sectionId);
    Task<List<SectionScreenDto>> GetSectionScreenListAsync();

    // Queue Display by Screen
    Task<int> GetQueueQuantityInScreenAsync(int screenId);
    Task<string?> GetQueueNameByScreenAsync(int screenId);
}