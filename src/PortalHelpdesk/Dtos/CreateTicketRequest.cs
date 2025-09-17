using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Messages;

namespace PortalHelpdesk.Dtos
{
    public class CreateTicketRequest
    {
        public Ticket Ticket { get; set; }
        public Message Message { get; set; }
    }

}
