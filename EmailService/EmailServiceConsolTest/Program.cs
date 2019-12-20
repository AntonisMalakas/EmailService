using EmailCheckerService;
using System;

namespace EmailServiceConsolTest
{
    class Program

    {
        private IEmailService _emailService;
        public Program(IEmailService emailService)
        {
            this._emailService = emailService;
        }
        static void Main(string[] args)
        {

            var test = Program._emailService.ReceiveEmail();

            Console.WriteLine("Hello World!");

        }
    }
}
