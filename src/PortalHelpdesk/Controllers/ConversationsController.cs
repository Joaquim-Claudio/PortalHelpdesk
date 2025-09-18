using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using PortalHelpdesk.Models.Messages;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    [RequiredScope("Access.AsUser")]
    public class ConversationsController : ControllerBase
    {
        private readonly ILogger<ConversationsController> _logger;
        private readonly ConversationsService _conversationsService;
        private readonly TicketsService _ticketsService;
        public ConversationsController(ILogger<ConversationsController> logger, ConversationsService conversationsService,
            TicketsService ticketsService)
        {
            _logger = logger;
            _conversationsService = conversationsService;
            _ticketsService = ticketsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByTickedId([FromQuery(Name = "ticket")] int ticketId)
        {
            try
            {
                var ticket = await _ticketsService.GetTicketById(ticketId);

                if (ticket == null)
                {
                    _logger.LogInformation("Ticket not found.");
                    return NotFound("Ticket not found.");
                }

                var conversation = await _conversationsService.GetConversationByTicketId(ticket);

                if (conversation == null)
                {
                    _logger.LogInformation($"Conversation not found for the specified ticket.");
                    return NotFound($"Conversation not found for ticket {ticketId}");
                }

                _logger.LogInformation("OK");

                return Ok(conversation);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpPost("messages")]
        public async Task<IActionResult> AddMessage([FromBody] Message newMessage, [FromQuery(Name = "ticket")] int ticketId)
        {
            try
            {
                var ticket = await _ticketsService.GetTicketById(ticketId);

                if (ticket == null)
                {
                    _logger.LogInformation("Ticket not found.");
                    return NotFound("Ticket not found.");
                }

                var conversation = await _conversationsService.AddMessageToConversation(newMessage, ticket);
                _logger.LogInformation("OK");

                return CreatedAtAction(nameof(AddMessage), new { id = conversation?.Id }, conversation);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }
    }
}
