using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Filters
{
    public class UserResolverFilter : IAsyncActionFilter
    {
        private readonly HelpdeskContext _context;
        private readonly ILogger<UserResolverFilter> _logger;
        private readonly AppDefaults _userDefaults;
        private readonly UsersService _usersService;

        public UserResolverFilter(HelpdeskContext context, ILogger<UserResolverFilter> logger, 
            IOptions<AppDefaults> userDefaults, UsersService usersService)
        {
            _context = context;
            _logger = logger;
            _userDefaults = userDefaults.Value;
            _usersService = usersService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                string username = context.HttpContext.User.Identity?.Name ?? throw new UnauthorizedAccessException();
                var user = await _usersService.GetUserByEmail(username);

                if (user == null)
                {
                    _logger.LogInformation("Creating new user: {User}", username);

                    var newUser = new User
                    {
                        Name = FormatNameFromUsername(username),
                        Email = username,
                        IsActive = true,
                        Role = _userDefaults.DefaultRole,
                    };

                    await _usersService.CreateUser(newUser);

                }
                else
                {
                    user.IsActive = true;
                    user.LastActivityAt = DateTime.UtcNow;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }

                await next();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resolving the user.");
            }
        }

        private static string FormatNameFromUsername(string username)
        {
            var name = username.Split('@').First();
            var parts = name.Split('.');
            string firstName = parts.Length > 0 ? Capitalize(parts[0]) : name;
            string lastName = parts.Length > 1 ? Capitalize(parts[^1]) : "";
            return $"{firstName} {lastName}".Trim();
        }

        private static string Capitalize(string s) => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);

    }

}