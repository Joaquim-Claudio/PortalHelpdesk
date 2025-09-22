using PortalHelpdesk.Models.Messages;
using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models.Attachments
{
    public class MessageAttachment
    {
        // (Primary) Foreign keys
        public int MessageId { get; set; }
        public int AttachmentId { get; set; }
        // Navigation properties
        [JsonIgnore]
        public Message? Message { get; set; }
        public required Attachment Attachment { get; set; }
    }
}