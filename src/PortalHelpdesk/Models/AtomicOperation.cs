namespace PortalHelpdesk.Models
{
    public class AtomicOperation
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
