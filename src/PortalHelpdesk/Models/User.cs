using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models
{
    public class User
    {
        public int Id { get; set; }
        public string ADUsername { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }

        // Foreign keys
        public int? GroupId { get; set; }

        // Navigation properties
        public Group? Group { get; set; }
        [JsonIgnore]
        public List<Ticket>? TicketsCreated { get; set; }
        [JsonIgnore]
        public List<Ticket>? TicketsAssigned { get; set; }
        [JsonIgnore]
        public List<Ticket>? TicketsRequested { get; set; }
        [JsonIgnore]
        public List<TicketHistory>? TicketChanges { get; set; }
        [JsonIgnore]
        public List<TicketResolution>? TicketsResolved { get; set; }
    }
}
