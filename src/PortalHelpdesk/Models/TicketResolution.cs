using PortalHelpdesk.Models.Attachments;
using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models
{
    public class TicketResolution
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Foreign keys
        public int? TicketId { get; set; }
        public int? ResolverId { get; set; }
        // Navigation properties
        [JsonIgnore]
        public Ticket? Ticket { get; set; }
        public User? Resolver { get; set; }
        public List<ResolutionAttachment>? Attachments { get; set; }
    }
}
