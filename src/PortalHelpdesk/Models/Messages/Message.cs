using PortalHelpdesk.Models.Attachments;

namespace PortalHelpdesk.Models.Messages
{
    public class Message
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string? Cc { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public string? InReplyTo { get; set; }
        public string? MessageId { get; set; }
        public List<string>? References { get; set; }

        // Navigation properties
        public List<MessageAttachment>? Attachments { get; set; }
    }
}
