using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class WorklogsService
    {
        private readonly HelpdeskContext _context;

        public WorklogsService(HelpdeskContext context)
        {
            _context = context;
        }

        public async Task<Worklog?> GetWorklogById(int id)
        {
            var worklog = await _context.Worklogs
                .FirstOrDefaultAsync(w => w.Id == id);
            return worklog;
        }

        public async Task<IEnumerable<Worklog>> GetWorklogsByTicketId(Ticket ticket)
        {
            var worklogs = await _context.Worklogs
                .Where(w => w.Ticket != null && w.Ticket.Id == ticket.Id)
                .OrderBy(w => w.Id)
                .ToListAsync();

            return worklogs;
        }

        public async Task<Worklog> AddWorklog(Worklog worklog, Ticket ticket, User user)
        {
            worklog.CreatedAt = DateTime.UtcNow;
            worklog.User = user;
            worklog.Ticket = ticket;
            _context.Worklogs.Add(worklog);
            await _context.SaveChangesAsync();

            return worklog;
        }

        public async Task<Worklog> UpdateWorklog(Worklog updatedWorklog)
        {
            var worklog = await GetWorklogById(updatedWorklog.Id);

            if (worklog == null)
            {
                throw new KeyNotFoundException("Worklog not found");
            }

            worklog.Description = updatedWorklog.Description;
            worklog.TimeTaken = updatedWorklog.TimeTaken;
            worklog.StartTime = updatedWorklog.StartTime;
            worklog.EndTime = updatedWorklog.EndTime;
            worklog.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return worklog;
        }

        public async Task DeleteWorklog(Worklog worklog)
        {
            _context.Worklogs.Remove(worklog);
            await _context.SaveChangesAsync();
        }

    }
}
