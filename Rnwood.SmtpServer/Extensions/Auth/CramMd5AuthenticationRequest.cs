namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class CramMd5AuthenticationRequest : IAuthenticationRequest
    {
        public CramMd5AuthenticationRequest(string username, string challenge, string challengeResponse)
        {
            Username = username;
            ChallengeResponse = challengeResponse;
            Challenge = challenge;
        }

        public string Username { get; }

        public string ChallengeResponse { get; }

        public string Challenge { get; }
    }
}