using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using QMS.Application.DTOs.Screen;
using QMS.Application.Interfaces;

namespace QMS.API.Controllers.V1
{
    /// <summary>
    /// Screen Display API Controller - FIXED
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/screen")]
    public class ScreenController : BaseApiController
    {
        private readonly IScreenService _screenService;
        private readonly ILogger<ScreenController> _logger;

        public ScreenController(
            IScreenService screenService,
            ILogger<ScreenController> logger)
        {
            _screenService = screenService;
            _logger = logger;
        }

        #region Screen Management

        /// <summary>
        /// Get screen by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var screen = await _screenService.GetScreenByIdAsync(id);
                if (screen == null)
                    return NotFound($"Screen {id} not found");

                return Success(screen, "Screen retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting screen {ScreenId}", id);
                return InternalServerError("Failed to retrieve screen");
            }
        }

        /// <summary>
        /// Get screen by code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var screen = await _screenService.GetScreenByCodeAsync(code);
                if (screen == null)
                    return NotFound($"Screen with code {code} not found");

                return Success(screen, "Screen retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting screen by code {Code}", code);
                return InternalServerError("Failed to retrieve screen");
            }
        }

        /// <summary>
        /// Get all active screens
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveScreens()
        {
            try
            {
                var screens = await _screenService.GetActiveScreensAsync();
                return Success(screens, $"{screens.Count} active screens retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active screens");
                return InternalServerError("Failed to retrieve active screens");
            }
        }

        /// <summary>
        /// Get screens by section ID
        /// </summary>
        [HttpGet("section/{sectionId}")]
        public async Task<IActionResult> GetScreensBySection(int sectionId)
        {
            try
            {
                var screens = await _screenService.GetScreensBySectionIdAsync(sectionId);
                return Success(screens, $"{screens.Count} screens retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting screens by section {SectionId}", sectionId);
                return InternalServerError("Failed to retrieve screens");
            }
        }

        #endregion

        #region Screen Display Data

        /// <summary>
        /// Get queue display data for a screen
        /// </summary>
        [HttpGet("{screenId}/display")]
        public async Task<IActionResult> GetScreenDisplay(int screenId)
        {
            try
            {
                var displayData = await _screenService.GetScreenQueueDisplayAsync(screenId);
                return Success(displayData, "Screen display data retrieved successfully");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting screen display {ScreenId}", screenId);
                return InternalServerError("Failed to retrieve screen display data");
            }
        }

        /// <summary>
        /// Get screen list
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetScreenList()
        {
            try
            {
                var screens = await _screenService.GetScreenListAsync();
                return Success(screens, $"{screens.Count} screens in list");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting screen list");
                return InternalServerError("Failed to retrieve screen list");
            }
        }

        /// <summary>
        /// Get section screen list
        /// </summary>
        [HttpGet("sections")]
        public async Task<IActionResult> GetSectionScreenList()
        {
            try
            {
                var sections = await _screenService.GetSectionScreenListAsync();
                return Success(sections, $"{sections.Count} sections retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting section screen list");
                return InternalServerError("Failed to retrieve section screen list");
            }
        }

        #endregion
    }
}
