using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Controllers
{
    [ApiController]
    [Authorize(Roles = "BUILTIN\\Users")]
    [Route("/api/[controller]")]
    public class AttachmentsController : ControllerBase
    {
        private readonly HelpdeskContext _context;
        private readonly ILogger<AttachmentsController> _logger;
        private readonly AttachmentsService _attachmentsService;
        private readonly ConversationsService _conversationsService;
        private readonly TicketResolutionsService _resolutionsService;

        public AttachmentsController(HelpdeskContext context, ILogger<AttachmentsController> logger,
            AttachmentsService attachmentsService, ConversationsService conversationsService, TicketResolutionsService resolutionsService)
        {
            _context = context;
            _logger = logger;
            _attachmentsService = attachmentsService;
            _conversationsService = conversationsService;
            _resolutionsService = resolutionsService;
        }

        [HttpPost("message/{messageId}")]
        public async Task<IActionResult> UploadMessageAttachment(int messageId, IFormFile file)
        {
            try
            {
                var message = await _conversationsService.GetMessageById(messageId);

                if (message == null)
                {
                    _logger.LogInformation($"Message not found.");
                    return NotFound($"Message {messageId} not found.");
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest("A file is required to be uploaded.");
                }

                if (!_attachmentsService.IsValidFileType(file))
                {
                    return BadRequest($"File extension not allowed. Allowed extensios: " +
                        $"{string.Join(", ", _attachmentsService.GetAllowedFileTypes())}");
                }

                var attachment = await _attachmentsService.SaveMessageAttachment(message, file);
                _logger.LogInformation("OK");

                return Ok(attachment);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading attachment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("resolution/{resolutionId}")]
        public async Task<IActionResult> UploadResolutionAttachment(int resolutionId, IFormFile file)
        {
            try
            {
                var resolution = await _resolutionsService.GetResolutionById(resolutionId);

                if (resolution == null)
                {
                    _logger.LogInformation($"Resolution not found.");
                    return NotFound($"Resolution {resolutionId} not found.");
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest("A file is required to be uploaded.");
                }

                if (!_attachmentsService.IsValidFileType(file))
                {
                    return BadRequest($"File extension not allowed. Allowed extensios: " +
                        $"{string.Join(", ", _attachmentsService.GetAllowedFileTypes())}");
                }

                var attachment = await _attachmentsService.SaveResolutionAttachment(resolution, file);
                _logger.LogInformation("OK");

                return Ok(attachment);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading attachment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{attachmentId}/download")]
        public async Task<IActionResult> DownloadAttachment(int attachmentId)
        {
            try
            {
                var attachment = await _attachmentsService.GetAttachmentById(attachmentId);
                if (attachment == null)
                {
                    _logger.LogInformation($"Attachment not found.");
                    return NotFound("Attachment not found.");
                }

                if (!System.IO.File.Exists(attachment.FileLocation))
                {
                    _logger.LogInformation("File not found.");
                    return NotFound("File not found.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(attachment.FileLocation);
                return File(fileBytes, attachment.FileType, attachment.FileName);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading attachment");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
