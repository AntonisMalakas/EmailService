using System;
using System.Collections.Generic;
using System.Text;

namespace EmailCheckerService
{
    public class EmailConfiguration: IEmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }

        public string PopServer { get; set; }
        public int PopPort { get; set; }
        public string PopUsername { get; set; }
        public string PopPassword { get; set; }

        public string ImapServer { get; set; }
        public int ImapPort { get; set; }
        public string ImapUsername { get; set; }
        public string ImapPassword { get; set; }

        public string ImapOutlookServer { get; set; }
        public int ImapOutlookPort { get; set; }
        public string ImapOutlookUsername { get; set; }
        public string ImapOutlookPassword { get; set; }
    }
}
