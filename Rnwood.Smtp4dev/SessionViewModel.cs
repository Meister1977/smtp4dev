#region

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Text;
using Rnwood.SmtpServer;

#endregion

namespace Rnwood.Smtp4dev
{
    public class SessionViewModel
    {
        public SessionViewModel(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }

        public bool SecureConnection => Session.SecureConnection;

        public string Client => Session.ClientAddress.ToString();

        public int NumberOfMessages => Session.Messages.Count;

        public DateTime StartDate => Session.StartDate;

        public void ViewLog()
        {
            var tempFiles = new TempFileCollection();
            var msgFile = new FileInfo(tempFiles.AddExtension("txt"));
            File.WriteAllText(msgFile.FullName, Session.Log, Encoding.ASCII);
            Process.Start(msgFile.FullName);
        }
    }
}