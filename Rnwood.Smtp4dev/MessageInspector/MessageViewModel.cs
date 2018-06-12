#region

using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using anmar.SharpMimeTools;
using Microsoft.Win32;

#endregion

namespace Rnwood.Smtp4dev.MessageInspector
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public MessageViewModel(SharpMimeMessage message)
        {
            Message = message;
        }

        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }


        public SharpMimeMessage Message { get; }

        public MessageViewModel[] Children
        {
            get { return Message.Cast<SharpMimeMessage>().Select(part => new MessageViewModel(part)).ToArray(); }
        }

        public HeaderViewModel[] Headers
        {
            get
            {
                return Message.Header.Cast<DictionaryEntry>()
                    .Select(de => new HeaderViewModel((string) de.Key, (string) de.Value)).ToArray();
            }
        }

        public string Data => Message.Header.RawHeaders + "\r\n\r\n" + Message.Body;

        public string Body => Message.BodyDecoded;

        public string Type => Message.Header.TopLevelMediaType + "/" + Message.Header.SubType;

        public long Size => Message.Size;

        public string Disposition => Message.Header.ContentDisposition;

        public string Encoding => Message.Header.ContentTransferEncoding;

        public string Name => Message.Name ?? "Unnamed" + ": " + MimeType + " (" + Message.Size + " bytes)";

        protected string MimeType => Message.Header.TopLevelMediaType + "/" + Message.Header.SubType;

        public string Subject => Message.Header.Subject;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void Save()
        {
            var dialog = new SaveFileDialog();

            var filename = Message.Name ?? "Unnamed";

            if (string.IsNullOrEmpty(Path.GetExtension(Message.Name)))
                filename = filename + MIMEDatabase.GetExtension(MimeType);

            dialog.FileName = filename;
            dialog.Filter = "File (*.*)|*.*";

            if (dialog.ShowDialog() == true)
                using (var stream = File.OpenWrite(dialog.FileName))
                {
                    Message.DumpBody(stream);
                }
        }

        public void View()
        {
            var extn = Path.GetExtension(Message.Name ?? "Unnamed");

            if (string.IsNullOrEmpty(extn)) extn = MIMEDatabase.GetExtension(MimeType) ?? ".part";

            var tempFiles = new TempFileCollection();
            var msgFile = new FileInfo(tempFiles.AddExtension(extn.TrimStart('.')));

            using (var stream = msgFile.OpenWrite())
            {
                Message.DumpBody(stream);
            }

            Process.Start(msgFile.FullName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class HeaderViewModel
    {
        public HeaderViewModel(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public string Value { get; }
    }
}