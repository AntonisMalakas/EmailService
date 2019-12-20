using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailCheckerService
{
    public interface IEmailService
    {
        void Send(EmailMessage emailMessage);
        List<EmailMessage> ReceiveEmailAsync(int maxCount = 10);
        void CheckEmail();
        Task TestCheckEmailAsync();
    }
}
