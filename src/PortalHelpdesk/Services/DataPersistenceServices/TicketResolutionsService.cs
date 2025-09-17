using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class TicketResolutionsService
    {
        private readonly HelpdeskContext _context;

        public TicketResolutionsService(HelpdeskContext context)
        {
            _context = context;
        }

        public async Task<TicketResolution?> GetResolutionById(int id)
        {
            var resolution = await _context.TicketResolutions
                .Include(r => r.Resolver)
                .Include(r => r.Attachments!)
                    .ThenInclude(ra => ra.Attachment)
                .FirstOrDefaultAsync(r => r.Id == id);

            return resolution;
        }

        public async Task<TicketResolution?> GetResolutionByTicketId(Ticket ticket)
        {
            var resolution = await _context.TicketResolutions
                .Include(r => r.Resolver)
                .Include(r => r.Attachments!)
                    .ThenInclude(ra => ra.Attachment)
                .FirstOrDefaultAsync(r => r.Ticket != null && r.Ticket.Id == ticket.Id);

            return resolution;
        }

        public async Task<TicketResolution> AddResolution(TicketResolution resolution, Ticket ticket, User resolver)
        {
            resolution.CreatedAt = DateTime.UtcNow;
            resolution.Resolver = resolver;
            resolution.Ticket = ticket;
            _context.TicketResolutions.Add(resolution);
            await _context.SaveChangesAsync();

            return resolution;
        }

        public async Task<TicketResolution> UpdateResolution(TicketResolution updatedResolution, User modifier)
        {
            var resolution = await GetResolutionById(updatedResolution.Id)
                ?? throw new KeyNotFoundException("Resolution not found");

            resolution.Resolver = modifier;
            resolution.Description = updatedResolution.Description;
            resolution.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return resolution;
        }

        public async Task<bool> HasResolution(Ticket ticket)
        {
            return await _context.TicketResolutions.AnyAsync(r => r.TicketId == ticket.Id);
        }
    }
}
