using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class TicketHistoryService
    {
        private readonly HelpdeskContext _context;

        public TicketHistoryService(HelpdeskContext context)
        {
            _context = context;
        }

        public async Task RecordTicketChanges(Ticket updatedTicket, User modifier)
        {
            var existingTicket = await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == updatedTicket.Id);

            var atomicOperation = new AtomicOperation { CreatedAt = DateTime.UtcNow };
            _context.AtomicOperations.Add(atomicOperation);
            await _context.SaveChangesAsync();

            if (existingTicket != null)
            {
                var changes = GetPropsChanged(existingTicket, updatedTicket);
                foreach (var (propName, oldValue, newValue) in changes)
                {
                    var ticketHistory = new TicketHistory
                    {
                        ChangedAt = DateTime.UtcNow,
                        Field = propName,
                        OldValue = oldValue,
                        NewValue = newValue,
                        TicketId = updatedTicket.Id,
                        ModifierId = modifier.Id,
                        AtomicOperationId = atomicOperation.Id
                    };
                    _context.TicketHistories.Add(ticketHistory);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTicketHistory(Ticket ticket)
        {
            var histories = await _context.TicketHistories
                .Where(th => th.TicketId == ticket.Id)
                .ToListAsync();

            _context.TicketHistories.RemoveRange(histories);
            await _context.SaveChangesAsync();
        }

        public List<(string propName, string oldValue, string newValue)> GetPropsChanged(Ticket existingTicket, Ticket updatedTicket)
        {
            var changedProps = new List<(string propName, string oldValue, string newValue)>();

            if (existingTicket.AssigneeId != updatedTicket.AssigneeId)
                changedProps.Add(("Assignee", existingTicket.AssigneeId.ToString() ?? "null", updatedTicket.AssigneeId.ToString() ?? "null"));

            if (existingTicket.RequesterId != updatedTicket.RequesterId)
                changedProps.Add(("Requester", existingTicket.RequesterId.ToString() ?? "null", updatedTicket.RequesterId.ToString() ?? "null"));

            if (existingTicket.CategoryId != updatedTicket.CategoryId)
                changedProps.Add(("Category", existingTicket.CategoryId.ToString() ?? "null", updatedTicket.CategoryId.ToString() ?? "null"));

            if (existingTicket.PriorityId != updatedTicket.PriorityId)
                changedProps.Add(("Priority", existingTicket.PriorityId.ToString() ?? "null", updatedTicket.PriorityId.ToString() ?? "null"));

            if (existingTicket.GroupId != updatedTicket.GroupId)
                changedProps.Add(("Group", existingTicket.GroupId.ToString() ?? "null", updatedTicket.GroupId.ToString() ?? "null"));

            if (existingTicket.TicketStatusId != updatedTicket.TicketStatusId)
                changedProps.Add(("TicketStatus", existingTicket.TicketStatusId.ToString() ?? "null", updatedTicket.TicketStatusId.ToString() ?? "null"));

            return changedProps;
        }
    }
}
