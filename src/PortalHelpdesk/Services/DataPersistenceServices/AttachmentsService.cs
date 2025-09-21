using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Attachments;
using DbAttachment = PortalHelpdesk.Models.Attachments.Attachment;
using DbMessage = PortalHelpdesk.Models.Messages.Message;
using MsMessage = Microsoft.Graph.Models.Message;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class AttachmentsService
    {
        public readonly HelpdeskContext _context;
        private readonly FileUploadOptions _options;
        private readonly string _uploadsRoot;

        public AttachmentsService(HelpdeskContext context, IOptions<FileUploadOptions> options, IWebHostEnvironment env)
        {
            _context = context;
            _options = options.Value;

            _uploadsRoot = Path.Combine(env.ContentRootPath, "Uploads");
            if (!Directory.Exists(_uploadsRoot))
            {
                Directory.CreateDirectory(_uploadsRoot);
            }
        }

        public async Task<DbAttachment?> GetAttachmentById(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);

            return attachment;
        }

        public async Task<List<MessageAttachment>?> GetMessageAttachments(int messageId)
        {
            var attachments = await _context.MessageAttachments
                .Where(ma => ma.MessageId == messageId)
                .Include(ma => ma.Attachment)
                .OrderBy(ma => ma.MessageId)
                .ToListAsync();

            return attachments;
        }

        public async Task<List<ResolutionAttachment>?> GetResolutionAttachments(int resolutionId)
        {
            var attachments = await _context.ResolutionAttachments
                .Where(ma => ma.ResolutionId == resolutionId)
                .Include(ma => ma.Attachment)
                .OrderBy(ma => ma.ResolutionId)
                .ToListAsync();

            return attachments;
        }

        public async Task<List<MessageAttachment>> SaveEmailAttachments(DbMessage dbMessage, MsMessage emailMessage, 
            GraphServiceClient _graphClient)
        {
            var savedAttachments = new List<MessageAttachment>();

            if (emailMessage.Attachments == null || emailMessage.Attachments.Count == 0)
                return savedAttachments;

            var folderPath = Path.Combine(_uploadsRoot, $"Msg-{dbMessage.Id}");
            Directory.CreateDirectory(folderPath);

            foreach (var attachment in emailMessage.Attachments)
            {
                if (attachment is FileAttachment fileAttachment)
                {
                    var fileName = fileAttachment.Name ?? $"{Guid.NewGuid()}.dat";
                    var extension = Path.GetExtension(fileName).ToLowerInvariant();

                    if (!GetAllowedFileTypes().Contains(extension))
                        continue;

                    var uniqueName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(folderPath, uniqueName);

                    byte[] contentBytes = fileAttachment.ContentBytes ?? [];
                    if (contentBytes == null || contentBytes.Length == 0)
                    {

                        var attachmentFromGraph = await _graphClient.Users[emailMessage.From?.EmailAddress?.Address]
                            .Messages[emailMessage.Id]
                            .Attachments[fileAttachment.Id]
                            .GetAsync() as FileAttachment;

                        contentBytes = attachmentFromGraph?.ContentBytes ?? [];
                    }

                    await File.WriteAllBytesAsync(filePath, contentBytes);

                    var attachmentEntity = new DbAttachment
                    {
                        FileName = fileName,
                        FileType = fileAttachment.ContentType ?? "application/octet-stream",
                        FileLocation = filePath,
                        UploadedAt = DateTime.UtcNow
                    };

                    var msgAttachment = new MessageAttachment
                    {
                        MessageId = dbMessage.Id,
                        Attachment = attachmentEntity
                    };

                    _context.MessageAttachments.Add(msgAttachment);
                    savedAttachments.Add(msgAttachment);
                }
            }

            if (savedAttachments.Count > 0)
                await _context.SaveChangesAsync();

            return savedAttachments;
        }


        public async Task<MessageAttachment?> SaveMessageAttachment(DbMessage message, IFormFile file)
        {
            var folderPath = Path.Combine(_uploadsRoot, $"Msg-{message.Id}");
            var attachment = await SaveAttachment(file, folderPath);

            var messageAttachment = new MessageAttachment
            {
                MessageId = message.Id,
                Attachment = attachment
            };

            _context.MessageAttachments.Add(messageAttachment);
            await _context.SaveChangesAsync();

            return messageAttachment;
        }

        public async Task<ResolutionAttachment?> SaveResolutionAttachment(TicketResolution resolution, IFormFile file)
        {
            var folderPath = Path.Combine(_uploadsRoot, $"Res-{resolution.Id}");
            var attachment = await SaveAttachment(file, folderPath);

            var resolutionAttachment = new ResolutionAttachment
            {
                ResolutionId = resolution.Id,
                Attachment = attachment
            };

            _context.ResolutionAttachments.Add(resolutionAttachment);
            await _context.SaveChangesAsync();

            return resolutionAttachment;
        }

        public async Task<DbAttachment> SaveAttachment(IFormFile file, string folderPath)
        {
            Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new DbAttachment
            {
                FileName = file.FileName,
                FileType = file.ContentType,
                FileLocation = filePath,
                UploadedAt = DateTime.UtcNow
            };

            return attachment;
        }
        public List<string> GetAllowedFileTypes()
        {
            return [.. _options.AllowedExtensions];
        }

        public bool IsValidFileType(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _options.AllowedExtensions.Contains(extension);
        }

    }
}
