using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using PortalHelpdesk.Models;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    [RequiredScope("Access.AsUser")]
    public class ResolutionsController: ControllerBase
    {
        private readonly ILogger<ResolutionsController> _logger;
        private readonly TicketResolutionsService _resolutionsService;
        private readonly TicketsService _ticketsService;
        private readonly UsersService _usersService;

        public ResolutionsController(ILogger<ResolutionsController> logger, TicketResolutionsService resolutionsService,
            TicketsService ticketsService, UsersService usersService)
        {
            _logger = logger;
            _resolutionsService = resolutionsService;
            _ticketsService = ticketsService;
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetResolutionByTicketId([FromQuery(Name = "ticket")] int ticketId)
        {
            try
            {
                var ticket = await _ticketsService.GetTicketById(ticketId);

                if (ticket == null)
                {
                    _logger.LogInformation("Ticket not found.");
                    return NotFound("Ticket not found.");
                }

                var resolution = await _resolutionsService.GetResolutionByTicketId(ticket);

                if (resolution == null)
                {
                    _logger.LogInformation($"Resolution not found for the specified ticket.");
                    return NotFound($"Resolution not found for ticket {ticketId}");
                }

                _logger.LogInformation("OK");

                return Ok(resolution);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddResolution([FromBody] TicketResolution newResolution, [FromQuery(Name = "ticket")] int ticketId)
        {
            try
            {
                var ticket = await _ticketsService.GetTicketById(ticketId);

                string username = User.Identity?.Name ?? throw new UnauthorizedAccessException();
                var user = await _usersService.GetUserByEmail(username);

                if (user == null || ticket == null)
                {
                    _logger.LogInformation(user == null ? "User" : "Ticket" + " not found.");
                    return NotFound(user == null ? "User" : "Ticket" + " not found.");
                }

                var hasResolution = await _resolutionsService.HasResolution(ticket);

                if(hasResolution)
                {
                    _logger.LogInformation("Resolution already exists for the specified ticket.");
                    return Conflict("Resolution already exists for the specified ticket.");
                }

                var resolution = await _resolutionsService.AddResolution(newResolution, ticket, user);
                _logger.LogInformation("OK");

                return CreatedAtAction(nameof(AddResolution), new { id = resolution.Id }, resolution);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpPut("{resolutionId}")]
        public async Task<IActionResult> UpdateResolution([FromBody] TicketResolution updatedResolution, int resolutionId)
        {
            try
            {
                string username = User.Identity?.Name ?? throw new UnauthorizedAccessException();
                var user = await _usersService.GetUserByEmail(username);

                if (user == null)
                {
                    _logger.LogInformation("User not found.");
                    return NotFound("User not found.");
                }

                var resolution = await _resolutionsService.GetResolutionById(resolutionId);

                if (resolution == null)
                {
                    _logger.LogInformation("Resolution not found");
                    return NotFound("Resolution not found");
                }

                resolution = await _resolutionsService.UpdateResolution(updatedResolution, user);
                _logger.LogInformation("OK");

                return Ok(resolution);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }
    }
}
