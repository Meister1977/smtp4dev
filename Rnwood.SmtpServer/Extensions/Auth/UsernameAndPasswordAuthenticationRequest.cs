namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class UsernameAndPasswordAuthenticationRequest : IAuthenticationRequest
    {
        public UsernameAndPasswordAuthenticationRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }
    }
}