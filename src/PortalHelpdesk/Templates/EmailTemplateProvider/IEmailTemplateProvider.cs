using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Messages;

namespace PortalHelpdesk.Templates.EmailTemplateProvider
{
    public interface IEmailTemplateProvider 
    {
        public string GetTicketAcknowledgementTemplate(Ticket ticket);
        public string GetTicketRequesterChangeTemplate(Ticket ticket);
        public string GetTicketAssignedForRequesterTemplate(Ticket ticket, User assignee);
        public string GetTicketAssignedForAssigneeTemplate(Ticket ticket);
        public string GetTicketResolvedTemplate(Ticket ticket, TicketResolution resolution);
        public string GetTicketCancelledTemplate(Ticket ticket);
        public string GetTicketClosedTemplate(Ticket ticket);
        public string GetTicketReopenTemplate(Ticket ticket);
        public string GetNewMessageTemplate(Ticket ticket);
        public string GetMessageTemplate(Message message);
    }
}
