using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models.Attachments
{
    public class Attachment
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public required string FileType { get; set; }
        [JsonIgnore]
        public string FileLocation { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
