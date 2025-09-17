using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;

namespace PortalHelpdesk.Services
{
    public class UserResolverFilter : IAsyncActionFilter
    {
        private readonly HelpdeskContext _context;
        private readonly ILogger<UserResolverFilter> _logger;
        private readonly UserDefaults _userDefaults;

        public UserResolverFilter(HelpdeskContext context, ILogger<UserResolverFilter> logger, IOptions<UserDefaults> userDefaults)
        {
            _context = context;
            _logger = logger;
            _userDefaults = userDefaults.Value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                string ADUsername = context.HttpContext.User.Identity?.Name ?? throw new UnauthorizedAccessException();

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.ADUsername == ADUsername);

                if (user == null)
                {
                    _logger.LogInformation("Creating new user: {User}", ADUsername);

                    string username = ADUsername.Split('\\').Last();

                    var newUser = new User
                    {
                        ADUsername = ADUsername,
                        Name = FormatNameFromUsername(username),
                        Email = $"{username}@{_userDefaults.DefaultDomain}",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        LastActivityAt = DateTime.UtcNow,
                        Role = _userDefaults.DefaultRole,
                    };

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                }
                else
                {
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

        private string FormatNameFromUsername(string username)
        {
            var parts = username.Split('.');
            string firstName = parts.Length > 0 ? Capitalize(parts[0]) : username;
            string lastName = parts.Length > 1 ? Capitalize(parts[^1]) : "";
            return $"{firstName} {lastName}".Trim();
        }

        private string Capitalize(string s) => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);

    }

}