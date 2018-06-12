#region

using System;
using System.IO;
using Rnwood.SmtpServer.Verbs;

#endregion

namespace Rnwood.SmtpServer
{
    public class DataVerb : IVerb
    {
        public virtual void Process(IConnection connection, SmtpCommand command)
        {
            if (connection.CurrentMessage == null)
            {
                connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.BadSequenceOfCommands,
                    "Bad sequence of commands"));
                return;
            }

            connection.CurrentMessage.SecureConnection = connection.Session.SecureConnection;
            connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.StartMailInputEndWithDot,
                "End message with period"));

            using (var writer = new StreamWriter(connection.CurrentMessage.GetData(true), connection.ReaderEncoding))
            {
                var firstLine = true;

                do
                {
                    var line = connection.ReadLine();

                    if (line != ".")
                    {
                        line = ProcessLine(line);

                        if (!firstLine)
                            writer.Write(Environment.NewLine);

                        writer.Write(line);
                    }
                    else
                    {
                        break;
                    }

                    firstLine = false;
                } while (true);

                writer.Flush();
                var maxMessageSize =
                    connection.Server.Behaviour.GetMaximumMessageSize(connection);

                if (maxMessageSize.HasValue && writer.BaseStream.Length > maxMessageSize.Value)
                {
                    connection.WriteResponse(
                        new SmtpResponse(StandardSmtpResponseCode.ExceededStorageAllocation,
                            "Message exceeds fixed size limit"));
                }
                else
                {
                    writer.Close();
                    connection.Server.Behaviour.OnMessageCompleted(connection);
                    connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.OK, "Mail accepted"));
                    connection.CommitMessage();
                }
            }
        }

        protected virtual string ProcessLine(string line)
        {
            //Remove escaping of end of message character
            if (line.StartsWith(".")) line = line.Remove(0, 1);
            return line;
        }
    }
}