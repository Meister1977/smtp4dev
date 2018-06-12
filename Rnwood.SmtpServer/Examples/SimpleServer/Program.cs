#region

using System;
using System.Text;
using System.IO;

#endregion

namespace Rnwood.SmtpServer.Example
{
    /// <summary>
    /// A simple example use of Rnwood.SmtpServer.
    /// Prints a message to the console when a session is established, completed
    /// or a message is received.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            DefaultServerBehaviour serverBehaviour = new DefaultServerBehaviour();
            serverBehaviour.SessionStarted += SessionStarted;
            serverBehaviour.SessionCompleted += SessionCompleted;
            serverBehaviour.MessageReceived += MessageReceived;

            Server server = new Server(serverBehaviour);
            server.Start();

            Console.WriteLine("Server running. Press ENTER to stop and exit");
            Console.ReadLine();
            server.Stop();
        }

        private static void SessionCompleted(object sender, SessionEventArgs e)
        {
            Console.WriteLine(
                $"SESSION END - Address:{e.Session.ClientAddress} NoOfMessages:{e.Session.Messages.Count} Error:{e.Session.SessionError}");
        }

        private static void SessionStarted(object sender, SessionEventArgs e)
        {
            Console.WriteLine($"SESSION START - Address:{e.Session.ClientAddress}");
        }

        private static void MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine(
                $"MESSAGE RECEIVED - Envelope From:{e.Message.From} Envelope To:{string.Join(", ", e.Message.To)}");

            //If you wanted to write the message out to a file, then could do this...
            //File.WriteAllBytes("myfile.eml", e.Message.Data);
        }
    }
}