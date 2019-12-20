using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmailCheckerService
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;
        public EmailService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }
        public List<EmailMessage> ReceiveEmailAsync(int maxCount = 10)
        {
            using (var emailClient = new Pop3Client())
            {

                emailClient.Connect("mail.pixel.com.lb", 110, SecureSocketOptions.StartTls);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                //emailClient.AuthenticationMechanisms.Remove("PLAIN");
                emailClient.Authenticate("support@pixel.com.lb", "P!xel123!");
                List<EmailMessage> emails = new List<EmailMessage>();
                for (int i = 0; i < emailClient.Count && i < maxCount; i++)
                {
                    var message = emailClient.GetMessage(i);
                    var emailMessage = new EmailMessage
                    {
                        Content = !string.IsNullOrEmpty(message.HtmlBody) ? message.HtmlBody : message.TextBody,
                        Subject = message.Subject
                    };
                    emailMessage.ToAddresses.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                    emailMessage.FromAddresses.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                }

                return emails;
            }
        }



        public void CheckEmailGmail()
        {
            using (var client = new ImapClient(new ProtocolLogger(Console.OpenStandardError())))
            {

                client.AuthenticationMechanisms.Remove("PLAIN");

                client.Connect("imap.gmail.com", 993, SecureSocketOptions.StartTls);

                client.Authenticate("restlessops@gmail.com", "passwordHere");

                client.Inbox.Open(FolderAccess.ReadOnly);

                // Get the summary information of all of the messages (suitable for displaying in a message list).
                var messages = client.Inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId).ToList();
            }
        }

        public void CheckEmail()
        {
            using (var client = new ImapClient(new ProtocolLogger("imap.log")))
            {
                client.ServerCertificateValidationCallback = (s, c, ch, e) => true;
                //client.Connect("imap-mail.outlook.com", 993, true);
                client.Connect(_emailConfiguration.ImapOutlookServer, _emailConfiguration.ImapOutlookPort, true);
                client.AuthenticationMechanisms.Remove("XOAUTH");
                if (client.IsConnected)
                    client.Authenticate(_emailConfiguration.ImapOutlookUsername, _emailConfiguration.ImapOutlookPassword);
                if (client.IsAuthenticated)
                {

                    client.Inbox.Open(FolderAccess.ReadWrite);
                    // Get the summary information of all of the messages (suitable for displaying in a message list).
                    //var messages = client.Inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId).ToList();
                    //var messages = client.Inbox.Fetch();

                    var mailIds = client.Inbox.Search(SearchQuery.NotSeen);
                    foreach (var id in mailIds)
                    {
                        var mail = client.Inbox.GetMessage(id);
                        // Do Stuff With Email
                        // ...
                        // ...
                        //
                        client.Inbox.AddFlags(id, MessageFlags.Seen, true);
                    }

                    //var inbox = client.Inbox;
                    //inbox.Open(FolderAccess.ReadOnly);

                    //var x = inbox.Count;
                    //var xx = inbox.Recent;

                    //for (int i = 0; i < inbox.Count; i++)
                    //{
                    //    var message = inbox.GetMessage(i);
                    //    var messageSub = message.Subject;
                    //    var messageTextBody = message.TextBody;

                    //    //Console.WriteLine("Subject: {0}", message.Subject);
                    //}
                }
                client.Disconnect(true);
            }
        }

        public async Task TestCheckEmailAsync()
        {
            try
            {
                using (var client = new ImapClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync(_emailConfiguration.ImapOutlookServer, _emailConfiguration.ImapOutlookPort, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    if (client.IsConnected)
                        client.Authenticate(_emailConfiguration.ImapOutlookUsername, _emailConfiguration.ImapOutlookPassword);
                    if (client.IsAuthenticated)
                    {
                        var inbox = client.Inbox;
                        inbox.Open(FolderAccess.ReadOnly);
                        inbox.CountChanged += async (s, e) =>
                        {
                            using (var mailFetch = new ImapClient())
                            {
                                mailFetch.ServerCertificateValidationCallback = (g, c, h, k) => true;
                                await mailFetch.ConnectAsync(_emailConfiguration.ImapOutlookServer, _emailConfiguration.ImapOutlookPort, true);
                                mailFetch.AuthenticationMechanisms.Remove("XOAUTH2");
                                if (mailFetch.IsConnected)
                                    mailFetch.Authenticate(_emailConfiguration.ImapOutlookUsername, _emailConfiguration.ImapOutlookPassword);
                                if (mailFetch.IsAuthenticated)
                                {
                                    mailFetch.Inbox.Open(FolderAccess.ReadOnly);
                                    var mailIds = mailFetch.Inbox.Search(SearchQuery.NotSeen);
                                    foreach (var id in mailIds)
                                    {
                                        var mail = mailFetch.Inbox.GetMessage(id);
                                        //var htmlDoc = new HtmlDocument();
                                        //htmlDoc.LoadHtml(mail.HtmlBody);
                                        //var context = htmlDoc.GetElementbyId("context")?.InnerText;
                                        //if (context != null)
                                        //{
                                        //var entity = context;
                                        //}
                                    }
                                }
                            };
                        };
                    }
                    using (var done = new CancellationTokenSource())
                    {
                        var task = client.IdleAsync(done.Token);
                        int timeout = client.Timeout;
                        //while (true)
                        //{
                        //    Thread.Sleep(10000);
                        //    if (client.IsIdle)
                        //    {
                        //        if (!client.Inbox.IsOpen)
                        //            client.Inbox.Open(FolderAccess.ReadOnly);
                        //        client.Idle(done.Token);
                        //    }
                        //}

                        while (true)
                        {
                            if (!client.Inbox.IsOpen)
                                client.Inbox.Open(FolderAccess.ReadOnly);
                            var task2 = client.IdleAsync(done.Token);
                            Thread.Sleep(100000);
                            done.Cancel();
                            task2.Wait();
                        }
                        // done.Cancel();
                        // task.Wait();
                    }
                    //client.Disconnect(true);
                };
            }
            catch (Exception ex)
            {
                string exception = ex.Message;
                string innerexception = ex.InnerException.ToString();
            }
        }

        public void Send(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            message.Subject = emailMessage.Subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = emailMessage.Content
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL (Which you should!)
                emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true);

                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                emailClient.Send(message);

                emailClient.Disconnect(true);
            }
        }
    }
}
