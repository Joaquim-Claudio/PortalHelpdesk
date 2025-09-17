using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class GroupsService
    {
        private readonly HelpdeskContext _context;

        public GroupsService(HelpdeskContext context)
        {
            _context = context;
        }

        public async Task<Group?> GetGroupById(int id)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == id);
            return group;
        }
        public async Task<Group?> GetGroupByName(string groupName)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Name == groupName);
            return group;
        }

        public async Task<Group> CreateGroup(Group group)
        {
            group.CreatedAt = DateTime.UtcNow;
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return group;
        }

        public async Task<Group> UpdateGroup(Group updatedGroup)
        {
            var group = await GetGroupById(updatedGroup.Id);

            if (group == null)
            {
                throw new KeyNotFoundException("Group not found");
            }

            group.Name = updatedGroup.Name;
            group.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return group;
        }

        public async Task DeleteGroup(Group group)
        {
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
        }

    }
}
