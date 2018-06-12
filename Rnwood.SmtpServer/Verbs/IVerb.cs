namespace Rnwood.SmtpServer.Verbs
{
    public interface IVerb
    {
        void Process(IConnection connection, SmtpCommand command);
    }
}