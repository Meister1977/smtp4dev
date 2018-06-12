namespace Rnwood.SmtpServer.Extensions.Auth
{
    public interface IAuthMechanismProcessor
    {
        IAuthenticationRequest Credentials { get; }

        AuthMechanismProcessorStatus ProcessResponse(string data);
    }
}