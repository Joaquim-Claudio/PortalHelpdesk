using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalHelpdesk.Dtos;
using PortalHelpdesk.Models;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Controllers
{
    [ApiController]
    [Authorize(Roles = "BUILTIN\\Users")]
    [Route("/api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ILogger<TicketsController> _logger;
        private readonly TicketsService _ticketsService;
        private readonly TicketStatusService _ticketStatusService;
        private readonly UsersService _usersService;

        public TicketsController(ILogger<TicketsController> logger, TicketsService ticketsService, UsersService usersService,
            TicketStatusService ticketStatusService)
        {
            _logger = logger;
            _ticketsService = ticketsService;
            _usersService = usersService;
            _ticketStatusService = ticketStatusService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tickets = await _ticketsService.GetAllTickets();

                if (tickets == null)
                {
                    _logger.LogInformation("OK");
                    return Ok(Enumerable.Empty<Ticket>());
                }

                _logger.LogInformation("OK");

                return Ok(tickets);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("bygroup")]
        public async Task<IActionResult> GetByGroup([FromQuery(Name = "group")] int groupId)
        {
            try
            {
                var tickets = await _ticketsService.GetTicketsByGroup(groupId);

                if (tickets == null)
                {
                    _logger.LogInformation("OK");
                    return Ok(Enumerable.Empty<Ticket>());
                }

                _logger.LogInformation("OK");

                return Ok(tickets);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("unassigned")]
        public async Task<IActionResult> GetUnassigned()
        {
            try
            {
                var tickets = await _ticketsService.GetUnassignedTickets();

                if (tickets == null)
                {
                    _logger.LogInformation("OK");
                    return Ok(Enumerable.Empty<Ticket>());
                }

                _logger.LogInformation("OK");

                return Ok(tickets);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var tickets = await _ticketsService.GetPendingTickets();

                if (tickets == null)
                {
                    _logger.LogInformation("OK");
                    return Ok(Enumerable.Empty<Ticket>());
                }

                _logger.LogInformation("OK");

                return Ok(tickets);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompleted()
        {
            try
            {
                var tickets = await _ticketsService.GetCompletedTickets();

                if (tickets == null)
                {
                    _logger.LogInformation("OK");
                    return Ok(Enumerable.Empty<Ticket>());
                }

                _logger.LogInformation("OK");

                return Ok(tickets);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("cancelled")]
        public async Task<IActionResult> GetCancelled()
        {
            try
            {
                var tickets = await _ticketsService.GetCancelledTickets();
                if (tickets == null)
                {
                    _logger.LogInformation("OK");
                    return Ok(Enumerable.Empty<Ticket>());
                }
                _logger.LogInformation("OK");
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("closed")]
        public async Task<IActionResult> GetClosed()
        {
            try
            {
                var tickets = await _ticketsService.GetClosedTickets();
                if (tickets == null)
                {
                    _logger.LogInformation("OK");
                    return Ok(Enumerable.Empty<Ticket>());
                }
                _logger.LogInformation("OK");
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("my_all")]
        public async Task<IActionResult> GetAllAssignedToUser()
        {
            try
            {
                string ADUsername = User.Identity?.Name ?? throw new UnauthorizedAccessException();
                var user = await _usersService.GetUserByADUsername(ADUsername);

                if (user == null)
                {
                    _logger.LogInformation("OK");
                    return NotFound("User not found");
                }

                var tickets = await _ticketsService.GetAllTicketsAssignedToUser(user);

                _logger.LogInformation("OK");

                return Ok(tickets);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("my_open")]
        public async Task<IActionResult> GetOpenAssignedToUser()
        {
            try
            {
                string ADUsername = User.Identity?.Name ?? throw new UnauthorizedAccessException();
                var user = await _usersService.GetUserByADUsername(ADUsername);

                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    return NotFound("User not found");
                }

                var tickets = await _ticketsService.GetOpenTicketsAssignedToUser(user);

                _logger.LogInformation("OK");

                return Ok(tickets);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> FormCreateTicket([FromBody] CreateTicketRequest createTicketRequest)
        {
            try
            {
                string ADUsername = User.Identity?.Name ?? throw new UnauthorizedAccessException();
                var user = await _usersService.GetUserByADUsername(ADUsername);

                if (user == null)
                {
                    _logger.LogInformation("User not found.");
                    return NotFound("User not found.");
                }

                var ticket = createTicketRequest.Ticket;
                var message = createTicketRequest.Message;

                var newTicket = await _ticketsService.CreateTicket(ticket, message, user);

                _logger.LogInformation("OK");

                return CreatedAtAction(nameof(FormCreateTicket), new { id = newTicket.Id }, newTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpPut("{ticketId}")]
        public async Task<IActionResult> UpdateTicket([FromBody] Ticket updatedTicket, int ticketId)
        {
            try
            {
                var ticket = await _ticketsService.GetTicketById(ticketId);
                if (ticket == null)
                {
                    _logger.LogInformation("Ticket not found.");
                    return NotFound("Ticket not found.");
                }

                string ADUsername = User.Identity?.Name ?? throw new UnauthorizedAccessException();
                var user = await _usersService.GetUserByADUsername(ADUsername);

                if (user == null)
                {
                    _logger.LogInformation("User not found.");
                    return NotFound("User not found.");
                }

                if (updatedTicket.TicketStatusId != null && updatedTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    var canUpdateStatus = await _ticketStatusService.TicketStatusCanBeUpdated(updatedTicket);
                    if (!canUpdateStatus)
                    {
                        _logger.LogInformation("Cannot update ticket status to 'Resolvido' without a resolution and worklog.");
                        return BadRequest("Cannot update ticket status to 'Resolvido' without a resolution and worklog.");
                    }
                }

                ticket = await _ticketsService.UpdateTicket(updatedTicket, user);

                _logger.LogInformation("OK");

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpDelete("{ticketId}")]
        public async Task<IActionResult> DeleteTicket(int ticketId)
        {
            try
            {
                var ticket = await _ticketsService.GetTicketById(ticketId);

                if (ticket == null)
                {
                    _logger.LogInformation($"Ticket {ticketId} not found");
                    return NotFound($"Ticket {ticketId} not found");
                }

                await _ticketsService.DeleteTicket(ticket);

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
