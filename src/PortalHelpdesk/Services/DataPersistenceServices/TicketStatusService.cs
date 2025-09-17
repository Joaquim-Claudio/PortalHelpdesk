using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class TicketStatusService
    {
        private readonly HelpdeskContext _context;

        public TicketStatusService(HelpdeskContext context)
        {
            _context = context;
        }

        public async Task<bool> TicketStatusCanBeUpdated(Ticket ticket)
        {
            var ticketStatus = _context.TicketStatus
                .FirstOrDefault(ts => ts.Id == ticket.TicketStatusId);

            if (ticketStatus != null && ticketStatus.Status == "Resolvido")
            {
                var hasResolution = await _context.TicketResolutions
                    .AnyAsync(r => r.TicketId == ticket.Id);
                
                var hasWorklogs = await _context.Worklogs
                    .AnyAsync(wl => wl.TicketId == ticket.Id);

                if(!hasResolution || !hasWorklogs) return false;
            }

            return true;
        }
    }
}
