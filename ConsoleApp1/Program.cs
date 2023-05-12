using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

//ReadGmailService service = new ReadGmailService("anhphong987654321@gmail.com", "zqvnxvxmxiheobzi");
namespace ConsoleApp1
{
    class Program
    {
        // Connection-related properties
        public const SecureSocketOptions SslOptions = SecureSocketOptions.Auto;
        public const string Host = "imap.gmail.com";
        public const int Port = 993;

        // Authentication-related properties
        public const string Username = "anhphong987654321@gmail.com";
        public const string Password = "zqvnxvxmxiheobzi";

        public static void Main(string[] args)
        {
            using (var client = new IdleClient(Host, Port, Username, Password, SslOptions))
            {
                Console.WriteLine("Hit any key to end the demo.");

                var idleTask = client.RunAsync();

                Task.Run(() =>
                {
                    Console.ReadKey(true);
                }).Wait();

                client.Exit();

                idleTask.GetAwaiter().GetResult();
            }
        }
    }
}