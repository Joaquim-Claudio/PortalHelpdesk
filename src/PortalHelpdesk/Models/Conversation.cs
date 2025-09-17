using PortalHelpdesk.Models.Messages;
using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        // Foreign keys
        public int TicketId { get; set; }
        // Navigation properties
        [JsonIgnore]
        public Ticket? Ticket { get; set; }
        public List<ConversationMessage>? Messages { get; set; }
    }
}
