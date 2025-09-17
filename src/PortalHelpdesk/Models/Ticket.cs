using PortalHelpdesk.Models.Attachments;
using PortalHelpdesk.Models.Messages;
using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ResolvedAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public DateTime CancelledAt { get; set; }
        public DateTime ReopenedAt { get; set; }
        // Foreign keys
        public int? CreatorId { get; set; }
        public int? AssigneeId { get; set; }
        public int? RequesterId { get; set; }
        public int? MessageId { get; set; }
        public int? CategoryId { get; set; }
        public int? PriorityId { get; set; }
        public int? GroupId { get; set; }
        public int? TicketStatusId { get; set; }
        // Navigation properties
        public User? Creator { get; set; }
        public User? Assignee { get; set; }
        public User? Requester { get; set; }
        public Message? Message { get; set; }
        public TicketCategory? Category { get; set; }
        public TicketPriority? Priority { get; set; }
        public Group? Group { get; set; }
        public TicketStatus? TicketStatus { get; set; }
        [JsonIgnore]
        public List<Conversation>? Conversations { get; set; }
    }
}
