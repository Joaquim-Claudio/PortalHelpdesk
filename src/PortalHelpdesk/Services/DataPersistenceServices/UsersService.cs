using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class UsersService
    {
        private readonly HelpdeskContext _context;

        public UsersService(HelpdeskContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserById(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User?> GetUserByADUsername(string adUsername)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ADUsername == adUsername);
            return user;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<User?> GetSystemUser()
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == "System");
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Group)
                .OrderBy(u => u.Id)
                .ToListAsync();
            return users;
        }

        public async Task<IEnumerable<User>> GetAllActiveUsers()
        {
            var users = await _context.Users
                .Where(u => u.IsActive == true)
                .Include(u => u.Group)
                .OrderBy(u => u.Id)
                .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<User>> GetAllRequesters()
        {
            var users = await _context.Users
                .Where(u => u.Role == "Requester")
                .Include(u => u.Group)
                .OrderBy(u => u.Id)
                .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<User>> GetUserByGroup(int groupId)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return Enumerable.Empty<User>();
            }

            var users = await _context.Users
                .Where(u => u.GroupId == group.Id && u.IsActive == true)
                .Include(u => u.Group)
                .OrderBy(u => u.Id)
                .ToListAsync();

            return users;
        }

        public async Task<User> CreateUser(User newUser)
        {
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.LastActivityAt = DateTime.UtcNow;
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<User> UpdateUser(User updatedUser)
        {
            var user = await GetUserById(updatedUser.Id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Role = updatedUser.Role;
            user.GroupId = updatedUser.GroupId;
            user.IsActive = updatedUser.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task DeleteUser(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

    }
}
