#region

using System;
using System.IO;
using anmar.SharpMimeTools;
using Rnwood.SmtpServer;

#endregion

namespace Rnwood.Smtp4dev
{
    public class MessageViewModel
    {
        private SharpMimeMessage _contents;

        public MessageViewModel(Message message)
        {
            Message = message;
        }

        public Message Message { get; }

        public string From => Message.From;

        public string To => string.Join(", ", Message.To);

        public DateTime ReceivedDate => Message.ReceivedDate;

        public string Subject => SharpMimeTools.rfc2047decode(Parts.Header.Subject);

        public SharpMimeMessage Parts
        {
            get
            {
                if (_contents == null) _contents = new SharpMimeMessage(Message.GetData());

                return _contents;
            }
        }

        public bool HasBeenViewed { get; private set; }

        public void SaveToFile(FileInfo file)
        {
            HasBeenViewed = true;

            var data = new byte[64 * 1024];
            int bytesRead;

            using (var dataStream = Message.GetData(false))
            {
                using (var fileStream = file.OpenWrite())
                {
                    while ((bytesRead = dataStream.Read(data, 0, data.Length)) > 0)
                        fileStream.Write(data, 0, bytesRead);
                }
            }
        }

        public void MarkAsViewed()
        {
            HasBeenViewed = true;
        }
    }
}