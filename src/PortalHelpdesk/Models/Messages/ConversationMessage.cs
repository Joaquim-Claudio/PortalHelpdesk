using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models.Messages
{
    public class ConversationMessage
    {
        // (Primary) Foreign keys
        public int ConversationId { get; set; }
        public int MessageId { get; set; }
        // Navigation properties
        [JsonIgnore]
        public Conversation? Conversation { get; set; }
        public required Message Message { get; set; }
    }
}
