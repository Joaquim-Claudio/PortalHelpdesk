﻿namespace PortalHelpdesk.Configurations
{
    public class EmailConfiguration
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ImapServer { get; set; } = string.Empty;
        public int ImapPort { get; set; }
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
    }

}
