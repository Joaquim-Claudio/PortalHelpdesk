using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class PorpertiesService
    {
        private readonly HelpdeskContext _context;

        public PorpertiesService(HelpdeskContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            return await _context.Groups.OrderBy(g => g.Id).ToListAsync();
        }

        public async Task<IEnumerable<TicketStatus>> GetAllTicketStatus()
        {
            return await _context.TicketStatus.OrderBy(t => t.Id).ToListAsync();
        }

        public async Task<IEnumerable<TicketCategory>> GetAllCategories()
        {
            return await _context.TicketCategories.OrderBy(c => c.Id).ToListAsync();
        }

        public async Task<IEnumerable<TicketPriority>> GetAllPriorities()
        {
            return await _context.TicketPriorities.OrderBy(p => p.Id).ToListAsync();
        }
    }
}
