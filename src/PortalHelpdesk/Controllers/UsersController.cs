using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using PortalHelpdesk.Dtos;
using PortalHelpdesk.Models;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    [RequiredScope("Access.AsUser")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UsersService _usersService;

        public UsersController(ILogger<UsersController> logger, UsersService usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        [HttpGet("byemail")]
        public async Task<IActionResult> GetByEmail([FromQuery (Name = "email")] string userEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogInformation("User email cannot empty");
                    return BadRequest("User email cannot empty");
                }

                var users = await _usersService.GetUserByEmail(userEmail);
                _logger.LogInformation("OK");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _usersService.GetAllUsers();
                _logger.LogInformation("OK");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            try
            {
                var users = await _usersService.GetAllActiveUsers();
                _logger.LogInformation("OK");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("requesters")]
        public async Task<IActionResult> GetRequesters()
        {
            try
            {
                var users = await _usersService.GetAllRequesters();
                _logger.LogInformation("OK");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpGet("bygroup")]
        public async Task<IActionResult> GetByGroup([FromQuery(Name = "group")] int groupId)
        {
            try
            {
                var users = await _usersService.GetUserByGroup(groupId);

                if (users == null)
                {
                    _logger.LogInformation("OK");
                    return Ok(Enumerable.Empty<Ticket>());
                }

                _logger.LogInformation("OK");

                return Ok(users);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser([FromBody] User updatedUser, int userId)
        {
            try
            {
                var user = await _usersService.GetUserById(userId);

                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    return NotFound("User not found");
                }

                user = await _usersService.UpdateUser(updatedUser);
                _logger.LogInformation("OK");

                return Ok(user);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _usersService.GetUserById(userId);

                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    return NotFound("User not found");
                }

                await _usersService.DeleteUser(user);
                _logger.LogInformation("OK");

                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }
    }
}