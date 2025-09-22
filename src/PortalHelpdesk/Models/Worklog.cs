using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models
{
    public class Worklog
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public TimeOnly TimeTaken { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Foreign keys
        public int TicketId { get; set; }
        public int? UserId { get; set; }
        // Navigation properties
        [JsonIgnore]
        public Ticket? Ticket { get; set; }
        public User? User { get; set; }
    }
}
