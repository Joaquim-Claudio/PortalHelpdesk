using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public DateTime ChangedAt { get; set; }
        public required string Field { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        // Foreign keys
        public int? TicketId { get; set; }
        public int? ModifierId { get; set; }
        public int? AtomicOperationId { get; set; }
        // Navigation properties
        [JsonIgnore]
        public Ticket? Ticket { get; set; }
        public User? Modifier { get; set; }
        [JsonIgnore]
        public AtomicOperation? AtomicOperation { get; set; }
    }

}
