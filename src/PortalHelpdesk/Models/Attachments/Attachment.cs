using System.Text.Json.Serialization;

namespace PortalHelpdesk.Models.Attachments
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        [JsonIgnore]
        public string FileLocation { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
