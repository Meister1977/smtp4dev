#region

using Rnwood.SmtpServer.Verbs;

#endregion

namespace Rnwood.SmtpServer
{
    public class MailVerb : IVerb
    {
        public MailVerb()
        {
            SubVerbMap = new VerbMap();
            SubVerbMap.SetVerbProcessor("FROM", new MailFromVerb());
        }

        public VerbMap SubVerbMap { get; }

        public MailFromVerb FromSubVerb => (MailFromVerb) SubVerbMap.GetVerbProcessor("FROM");


        public void Process(IConnection connection, SmtpCommand command)
        {
            var subrequest = new SmtpCommand(command.ArgumentsText);
            var verbProcessor = SubVerbMap.GetVerbProcessor(subrequest.Verb);

            if (verbProcessor != null)
                verbProcessor.Process(connection, subrequest);
            else
                connection.WriteResponse(
                    new SmtpResponse(StandardSmtpResponseCode.CommandParameterNotImplemented,
                        "Subcommand {0} not implemented", subrequest.Verb));
        }
    }
}