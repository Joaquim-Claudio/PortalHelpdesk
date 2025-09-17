using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Controllers
{
    [ApiController]
    [Authorize(Roles = "BUILTIN\\Users")]
    [Route("/api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly ILogger<PropertiesController> _logger;

        private readonly PorpertiesService _propertiesService;
        public PropertiesController(ILogger<PropertiesController> logger, PorpertiesService propertiesService)
        {
            _logger = logger;
            _propertiesService = propertiesService;
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            try
            {
                var groups = await _propertiesService.GetAllGroups();
                _logger.LogInformation("OK");

                return Ok(groups);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("ticket_status")]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                var status = await _propertiesService.GetAllTicketStatus();
                _logger.LogInformation("OK");

                return Ok(status);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _propertiesService.GetAllCategories();
                _logger.LogInformation("OK");

                return Ok(categories);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("priorities")]
        public async Task<IActionResult> GetPriorities()
        {
            try
            {
                var priorities = await _propertiesService.GetAllPriorities();
                _logger.LogInformation("OK");

                return Ok(priorities);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }
    }
}
