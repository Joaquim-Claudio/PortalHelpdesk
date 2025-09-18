using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using PortalHelpdesk.Models;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    [RequiredScope("Access.AsUser")]
    public class GroupsController : ControllerBase
    {
        private readonly ILogger<GroupsController> _logger;
        private readonly GroupsService _groupsService;

        public GroupsController(ILogger<GroupsController> logger, GroupsService groupsService)
        {
            _logger = logger;
            _groupsService = groupsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] Group newGroup)
        {
            try
            {
                var group = await _groupsService.GetGroupByName(newGroup.Name);

                if (group != null)
                {
                    _logger.LogInformation($"Group '{newGroup.Name}' already exists.");
                    return Conflict($"Group '{newGroup.Name}' already exists.");
                }

                group = await _groupsService.CreateGroup(newGroup);
                _logger.LogInformation("OK");

                return CreatedAtAction(nameof(CreateGroup), new { id = group.Id }, group);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup([FromBody] Group updatedGroup, int groupId)
        {
            try
            {
                var group = await _groupsService.GetGroupById(groupId);

                if (group == null)
                {
                    _logger.LogInformation("Group not found");
                    return NotFound("Group not found");
                }

                group = await _groupsService.UpdateGroup(updatedGroup);
                _logger.LogInformation("OK");

                return Ok(group);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }

        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            try
            {
                var group = await _groupsService.GetGroupById(groupId);

                if (group == null)
                {
                    _logger.LogInformation("Group not found");
                    return NotFound("Group not found");
                }

                await _groupsService.DeleteGroup(group);
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
