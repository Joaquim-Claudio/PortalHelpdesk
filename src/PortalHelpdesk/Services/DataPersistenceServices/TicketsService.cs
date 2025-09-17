using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Messages;
using PortalHelpdesk.Services.AutomationServices.Notifications;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class TicketsService
    {
        private readonly HelpdeskContext _context;
        private readonly TicketHistoryService _ticketHistoryService;
        private readonly EmailNotificationService _emailNotificationService;
        private readonly UsersService _usersService;

        public TicketsService(HelpdeskContext context, TicketHistoryService ticketHistoryService,
            EmailNotificationService emailNotificationService, UsersService usersService)
        {
            _context = context;
            _ticketHistoryService = ticketHistoryService;
            _emailNotificationService = emailNotificationService;
            _usersService = usersService;
        }
        public async Task<Ticket?> GetTicketById(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .FirstOrDefaultAsync(t => t.Id == id);
            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetAllTickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByGroup(int groupId)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return Enumerable.Empty<Ticket>();
            }

            var tickets = await _context.Tickets
                .Where(t => t.GroupId == group.Id && t.TicketStatus != null
                            && t.TicketStatus.Status != "Resolvido" && t.TicketStatus.Status != "Cancelado"
                            && t.TicketStatus.Status != "Encerrado")
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetUnassignedTickets()
        {
            var tickets = await _context.Tickets
                .Where(t => t.Assignee == null)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetPendingTickets()
        {
            var tickets = await _context.Tickets
                .Where(t => t.TicketStatus != null
                            && (t.TicketStatus.Status == "Aberto" || t.TicketStatus.Status == "Em Andamento"))
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetCompletedTickets()
        {
            var tickets = await _context.Tickets
                .Where(t => t.TicketStatus != null && t.TicketStatus.Status == "Resolvido")
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetCancelledTickets()
        {
            var tickets = await _context.Tickets
                .Where(t => t.TicketStatus != null && t.TicketStatus.Status == "Cancelado")
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();
            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetClosedTickets()
        {
            var tickets = await _context.Tickets
                .Where(t => t.TicketStatus != null && t.TicketStatus.Status == "Encerrado")
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();
            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAssignedToUser(User user)
        {
            var tickets = await _context.Tickets
                .Where(t => t.Assignee != null && t.Assignee.Id == user.Id)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetOpenTicketsAssignedToUser(User user)
        {
            var tickets = await _context.Tickets
                .Where(t => t.Assignee != null && t.Assignee.Id == user.Id && t.TicketStatus != null
                            && (t.TicketStatus.Status == "Open" || t.TicketStatus.Status == "Em Andamento"))
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsResolvedBeforeAsync(DateOnly date)
        {
            var tickets = await _context.Tickets
                .Where(t => t.TicketStatus != null && t.TicketStatus.Status == "Resolvido"
                            && t.ResolvedAt != default(DateTime) && DateOnly.FromDateTime(t.ResolvedAt) <= date)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .Include(t => t.Group)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Message)
                .OrderBy(t => t.Id)
                .ToListAsync();
            return tickets;
        }

        public async Task<Ticket> CreateTicket(Ticket ticket, Message message, User creator)
        {
            var defaultTicketStatus = await _context.TicketStatus
                .FirstOrDefaultAsync(ts => ts.Status == "Aberto")
                ?? throw new InvalidOperationException("Default ticket status 'Aberto' not found.");

            var defaultPriority = await _context.TicketPriorities
                .FirstOrDefaultAsync(p => p.Level == "Standard")
                ?? throw new InvalidOperationException("Default ticket priority 'Standard' not found.");

            var requester = await _context.Users.FindAsync(ticket.RequesterId)
                ?? throw new KeyNotFoundException("Requester not found.");

            message.SentAt = DateTime.UtcNow;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            ticket.CreatedAt = DateTime.UtcNow;
            ticket.Creator = creator;
            ticket.Requester = requester;
            ticket.TicketStatus = defaultTicketStatus;
            ticket.Priority = defaultPriority;
            ticket.MessageId = message.Id;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            await _emailNotificationService.SendTicketAcknowledgement(ticket);

            return ticket;
        }

        public async Task<Ticket> UpdateTicket(Ticket updatedTicket, User modifier)
        {
            var ticket = await GetTicketById(updatedTicket.Id)
                ?? throw new KeyNotFoundException("Ticket not found");

            await _ticketHistoryService.RecordTicketChanges(updatedTicket, modifier);
            await SendNotificationIfRequired(ticket, updatedTicket);

            ticket.RequesterId = updatedTicket.RequesterId;
            ticket.AssigneeId = updatedTicket.AssigneeId;
            ticket.CategoryId = updatedTicket.CategoryId;
            ticket.PriorityId = updatedTicket.PriorityId;
            ticket.GroupId = updatedTicket.GroupId;
            ticket.TicketStatusId = updatedTicket.TicketStatusId;
            ticket.UpdatedAt = DateTime.UtcNow;

            if (IsTicketResolved(updatedTicket))
                ticket.ResolvedAt = DateTime.UtcNow;
            if (IsTicketClosed(updatedTicket))
                ticket.ClosedAt = DateTime.UtcNow;
            if (IsTicketCancelled(updatedTicket))
                ticket.CancelledAt = DateTime.UtcNow;
            if (IsTicketReopened(updatedTicket, ticket))
                ticket.ReopenedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ticket;
        }

        public async Task ReopenTicket(Ticket ticket)
        {
            var ticketStatus = await _context.TicketStatus
                .FirstOrDefaultAsync(ts => ts.Status == "Aberto")
                ?? throw new InvalidOperationException("Ticket status 'Aberto' not found.");

            var updatedTicket = new Ticket
            {
                Id = ticket.Id,
                RequesterId = ticket.RequesterId,
                AssigneeId = ticket.AssigneeId,
                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId,
                GroupId = ticket.GroupId,
                TicketStatusId = ticketStatus.Id
            };

            var modifier = await _usersService.GetSystemUser();
            await UpdateTicket(updatedTicket, modifier!);
        }

        public async Task CloseTicket(Ticket ticket)
        {
            var ticketStatus = await _context.TicketStatus
                .FirstOrDefaultAsync(ts => ts.Status == "Encerrado")
                ?? throw new InvalidOperationException("Ticket status 'Encerrado' not found.");

            var updatedTicket = new Ticket
            {
                Id = ticket.Id,
                RequesterId = ticket.RequesterId,
                AssigneeId = ticket.AssigneeId,
                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId,
                GroupId = ticket.GroupId,
                TicketStatusId = ticketStatus.Id
            };

            var modifier = await _usersService.GetSystemUser();
            await UpdateTicket(updatedTicket, modifier!);
        }

        public async Task DeleteTicket(Ticket ticket)
        {
            await _ticketHistoryService.DeleteTicketHistory(ticket);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public bool IsTicketClosed(Ticket updatedTicket)
        {
            return updatedTicket.TicketStatus != null && updatedTicket.TicketStatus.Status == "Encerrado";
        }
        public bool IsTicketCancelled(Ticket updatedTicket)
        {
            return updatedTicket.TicketStatus != null && updatedTicket.TicketStatus.Status == "Cancelado";
        }
        public bool IsTicketResolved(Ticket updatedTicket)
        {
            return updatedTicket.TicketStatus != null && updatedTicket.TicketStatus.Status == "Resolvido";
        }
        public bool IsTicketReopened(Ticket updatedTicket, Ticket existingTicket)
        {
            return existingTicket.TicketStatus != null && updatedTicket.TicketStatus != null
                && existingTicket.TicketStatus.Status == "Resolvido" && updatedTicket.TicketStatus.Status == "Aberto";
        }

        public async Task SendNotificationIfRequired(Ticket existingTicket, Ticket updatedTicket)
        {

            var category = await _context.TicketCategories.FindAsync(updatedTicket.CategoryId);
            var priority = await _context.TicketPriorities.FindAsync(updatedTicket.PriorityId);
            var ticketStatus = await _context.TicketStatus.FindAsync(updatedTicket.TicketStatusId);
            var creator = await _context.Users.FindAsync(existingTicket.CreatorId);
            var requester = await _context.Users.FindAsync(updatedTicket.RequesterId);
            var message = await _context.Messages.FindAsync(existingTicket.MessageId);

            var fullUpdatedTicket = updatedTicket;
            fullUpdatedTicket.Category = category;
            fullUpdatedTicket.Priority = priority;
            fullUpdatedTicket.TicketStatus = ticketStatus;
            fullUpdatedTicket.Creator = creator;
            fullUpdatedTicket.Requester = requester;
            fullUpdatedTicket.Message = message;

            if (existingTicket.AssigneeId != fullUpdatedTicket.AssigneeId && fullUpdatedTicket.AssigneeId != null)
            {
                var assignee = await _context.Users.FindAsync(fullUpdatedTicket.AssigneeId);
                if (assignee != null)
                {
                    await _emailNotificationService.SendTicketAssignedAcknowledgement(fullUpdatedTicket, assignee);
                }
            }

            if (existingTicket.TicketStatusId != fullUpdatedTicket.TicketStatusId)
            {

                await SendStatusChangeNotification(existingTicket, fullUpdatedTicket);
            }

            if (existingTicket.RequesterId != fullUpdatedTicket.RequesterId && fullUpdatedTicket.RequesterId != null)
            {
                await _emailNotificationService.SendTicketRequesterChangeAcknowledgement(fullUpdatedTicket);
            }
            await Task.CompletedTask;
        }


        public async Task SendStatusChangeNotification(Ticket existingTicket, Ticket updatedTicket)
        {
            if (IsTicketResolved(updatedTicket))
            {
                var resolution = await _context.TicketResolutions
                    .Where(r => r.TicketId == updatedTicket.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .FirstOrDefaultAsync();

                if (resolution != null)
                {
                    await _emailNotificationService.SendTicketResolvedAcknowledgement(updatedTicket, resolution);
                }
            }

            else if (IsTicketCancelled(updatedTicket))
            {
                await _emailNotificationService.SendTicketCancelledAcknowledgement(updatedTicket);
            }

            else if (IsTicketClosed(updatedTicket))
            {
                await _emailNotificationService.SendTicketClosedAcknowledgement(updatedTicket);
            }

            else if (IsTicketReopened(updatedTicket, existingTicket))
            {
                await _emailNotificationService.SendTicketReopenAcknowledgement(updatedTicket);
            }
        }
    }
}
