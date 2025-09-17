using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public List<Ticket>? Tickets { get; set; }
    }
}
