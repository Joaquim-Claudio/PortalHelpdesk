using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Messages;

namespace PortalHelpdesk.Dtos
{
    public class CreateTicketRequest
    {
        public required Ticket Ticket { get; set; }
        public required Message Message { get; set; }
    }

}
