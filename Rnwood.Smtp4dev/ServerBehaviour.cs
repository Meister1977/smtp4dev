#region

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Rnwood.Smtp4dev.Properties;
using Rnwood.SmtpServer;
using Rnwood.SmtpServer.Extensions;
using Rnwood.SmtpServer.Extensions.Auth;

#endregion

namespace Rnwood.Smtp4dev
{
    public class ServerBehaviour : IServerBehaviour
    {
        private readonly AuthExtension _authExtension = new AuthExtension();
        private readonly EightBitMimeExtension _eightBitMimeExtension = new EightBitMimeExtension();
        private readonly SizeExtension _sizeExtension = new SizeExtension();
        private readonly StartTlsExtension _startTLSExtension = new StartTlsExtension();

        public event EventHandler<MessageEventArgs> MessageReceived;

        public event EventHandler<SessionEventArgs> SessionCompleted;

        #region IServerBehaviour Members

        public void OnMessageCompleted(IConnection connection)
        {
        }

        public void OnMessageReceived(IConnection connection, Message message)
        {
            if (MessageReceived != null) MessageReceived(this, new MessageEventArgs(message));
        }

        public void OnMessageRecipientAdding(IConnection connection, Message message, string recipient)
        {
        }

        public void OnSessionStarted(IConnection connection, ISession session)
        {
        }

        public void OnCommandReceived(IConnection connection, SmtpCommand command)
        {
        }

        public string DomainName => Settings.Default.DomainName;

        public IPAddress IpAddress => IPAddress.Parse(Settings.Default.IPAddress);

        public int PortNumber => Settings.Default.PortNumber;

        public bool IsSSLEnabled(IConnection connection)
        {
            return Settings.Default.EnableSSL;
        }

        public bool IsSessionLoggingEnabled(IConnection connection)
        {
            return true;
        }

        public X509Certificate GetSSLCertificate(IConnection connection)
        {
            if (string.IsNullOrEmpty(Settings.Default.SSLCertificatePath)) return null;

            if (string.IsNullOrEmpty(Settings.Default.SSLCertificatePassword))
                return new X509Certificate(Settings.Default.SSLCertificatePath);

            return new X509Certificate(Settings.Default.SSLCertificatePath, Settings.Default.SSLCertificatePassword);
        }

        public IEnumerable<IExtension> GetExtensions(IConnection connection)
        {
            var extensions = new List<IExtension>();

            if (Settings.Default.Enable8BITMIME) extensions.Add(_eightBitMimeExtension);

            if (Settings.Default.EnableSTARTTLS) extensions.Add(_startTLSExtension);

            if (Settings.Default.EnableAUTH) extensions.Add(_authExtension);

            if (Settings.Default.EnableSIZE) extensions.Add(_sizeExtension);

            return extensions;
        }

        public long? GetMaximumMessageSize(IConnection connection)
        {
            var value = Settings.Default.MaximumMessageSize;
            return value != 0 ? value : (long?) null;
        }

        public void OnSessionCompleted(IConnection connection, ISession Session)
        {
            if (SessionCompleted != null) SessionCompleted(this, new SessionEventArgs(Session));
        }

        public int GetReceiveTimeout(IConnection connection)
        {
            return Settings.Default.ReceiveTimeout;
        }

        public AuthenticationResult ValidateAuthenticationCredentials(IConnection connection,
            IAuthenticationRequest authenticationRequest)
        {
            return AuthenticationResult.Success;
        }

        public void OnMessageStart(IConnection connection, string from)
        {
            if (Settings.Default.RequireAuthentication && !connection.Session.Authenticated)
                throw new SmtpServerException(new SmtpResponse(StandardSmtpResponseCode.AuthenticationRequired,
                    "Must authenticate before sending mail"));

            if (Settings.Default.RequireSecureConnection && !connection.Session.SecureConnection)
                throw new SmtpServerException(new SmtpResponse(StandardSmtpResponseCode.BadSequenceOfCommands,
                    "Mail must be sent over secure connection"));
        }

        public bool IsAuthMechanismEnabled(IConnection connection, IAuthMechanism authMechanism)
        {
            if (Settings.Default.OnlyAllowClearTextAuthOverSecureConnection)
                return !authMechanism.IsPlainText || connection.Session.SecureConnection;

            return true;
        }

        public IMessage CreateMessage(IConnection connection)
        {
            return new Message(connection.Session);
        }

        #endregion
    }
}