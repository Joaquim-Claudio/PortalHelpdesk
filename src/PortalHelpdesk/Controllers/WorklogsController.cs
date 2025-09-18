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
    public class WorklogsController : ControllerBase
    {
        private readonly ILogger<WorklogsController> _logger;
        private readonly WorklogsService _worklogsService;
        private readonly TicketsService _ticketsService;
        private readonly UsersService _usersService;

        public WorklogsController(ILogger<WorklogsController> logger, WorklogsService worklogsService,
            TicketsService ticketsService, UsersService usersService)
        {
            _logger = logger;
            _worklogsService = worklogsService;
            _ticketsService = ticketsService;
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByTickedId([FromQuery(Name = "ticket")] int ticketId)
        {
            try
            {
                var ticket = await _ticketsService.GetTicketById(ticketId);

                if (ticket == null)
                {
                    _logger.LogInformation("Ticket not found.");
                    return NotFound("Ticket not found.");
                }

                var worklogs = await _worklogsService.GetWorklogsByTicketId(ticket);
                _logger.LogInformation("OK");

                return Ok(worklogs);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddWorklog([FromBody] Worklog newWorklog, [FromQuery(Name = "ticket")] int ticketId)
        {
            try
            {
                var ticket = await _ticketsService.GetTicketById(ticketId);

                string ADUsername = User.Identity?.Name ?? throw new UnauthorizedAccessException();
                var user = await _usersService.GetUserByADUsername(ADUsername);

                if (user == null || ticket == null)
                {
                    _logger.LogInformation(user == null ? "User" : "Ticket" + " not found.");
                    return NotFound(user == null ? "User" : "Ticket" + " not found.");
                }

                var worklog = await _worklogsService.AddWorklog(newWorklog, ticket, user);
                _logger.LogInformation("OK");

                return CreatedAtAction(nameof(AddWorklog), new { id = worklog.Id }, worklog);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpPut("{worklogId}")]
        public async Task<IActionResult> UpdateWorklog([FromBody] Worklog updatedWorklog, int worklogId)
        {
            try
            {
                var worklog = await _worklogsService.GetWorklogById(worklogId);

                if (worklog == null)
                {
                    _logger.LogInformation("Worklog not found");
                    return NotFound("Worklog not found");
                }

                worklog = await _worklogsService.UpdateWorklog(updatedWorklog);
                _logger.LogInformation("OK");

                return Ok(worklog);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpDelete("{worklogId}")]
        public async Task<IActionResult> DeleteWorklog(int worklogId)
        {
            try
            {
                var worklog = await _worklogsService.GetWorklogById(worklogId);

                if (worklog == null)
                {
                    _logger.LogInformation("Worklog not found");
                    return NotFound("Worklog not found");
                }

                await _worklogsService.DeleteWorklog(worklog);
                _logger.LogInformation("OK");

                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }
    }
}
