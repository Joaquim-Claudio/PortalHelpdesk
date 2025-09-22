using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models.Attachments
{
    public class ResolutionAttachment
    {
        // (Primary) Foreign keys
        public int ResolutionId { get; set; }
        public int AttachmentId { get; set; }
        // Navigation properties
        [JsonIgnore]
        public TicketResolution? TicketResolution { get; set; }
        public required Attachment Attachment { get; set; }
    }
}
