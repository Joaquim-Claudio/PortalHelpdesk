namespace PortalHelpdesk.Configurations
{
    public class FileUploadOptions
    {
        public string[] AllowedExtensions { get; set; } = [];
        public int MaxSizeMB { get; set; }
    }

}
