using System;
using System.Collections.Generic;
using System.Text;

namespace EmailCheckerService
{
    public interface IEmailConfiguration
    {
        string SmtpServer { get; }
        int SmtpPort { get; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }

        string PopServer { get; }
        int PopPort { get; }
        string PopUsername { get; }
        string PopPassword { get; }

        string ImapServer { get; }
        int ImapPort { get; }
        string ImapUsername { get; }
        string ImapPassword { get; }

        string ImapOutlookServer { get; }
        int ImapOutlookPort { get; }
        string ImapOutlookUsername { get; }
        string ImapOutlookPassword { get; }
    }
}
