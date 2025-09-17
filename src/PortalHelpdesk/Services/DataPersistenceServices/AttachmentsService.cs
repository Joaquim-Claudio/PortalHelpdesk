using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Attachments;
using PortalHelpdesk.Models.Messages;

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

        public async Task<Attachment?> GetAttachmentById(int id)
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

        public async Task<List<MessageAttachment>> SaveEmailAttachments(Message dbMessage, MimeMessage emailMessage)
        {
            var savedAttachments = new List<MessageAttachment>();

            foreach (var attachment in emailMessage.Attachments)
            {
                if (attachment is MimePart mimePart)
                {
                    var folderPath = Path.Combine(_uploadsRoot, $"Msg {dbMessage.Id}");
                    Directory.CreateDirectory(folderPath);

                    var fileName = mimePart.FileName ?? $"{Guid.NewGuid()}.dat";
                    var extension = Path.GetExtension(fileName).ToLowerInvariant();

                    if(!GetAllowedFileTypes().Contains(extension))
                    {
                        continue;
                    }

                    var uniqueName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(folderPath, uniqueName);

                    using (var stream = File.Create(filePath))
                    {
                        await mimePart.Content.DecodeToAsync(stream);
                    }

                    var attachmentEntity = new Attachment
                    {
                        FileName = fileName,
                        FileType = mimePart.ContentType.MimeType,
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

            if (savedAttachments.Count != 0)
                await _context.SaveChangesAsync();

            return savedAttachments;
        }


        public async Task<MessageAttachment?> SaveMessageAttachment(Message message, IFormFile file)
        {
            var folderPath = Path.Combine(_uploadsRoot, $"Msg {message.Id}");
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
            var folderPath = Path.Combine(_uploadsRoot, $"Res {resolution.Id}");
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

        public async Task<Attachment> SaveAttachment(IFormFile file, string folderPath)
        {
            Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
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
