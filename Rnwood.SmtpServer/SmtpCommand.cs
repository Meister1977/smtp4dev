#region

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace Rnwood.SmtpServer
{
    public class SmtpCommand
    {
        public static Regex COMMANDREGEX = new Regex("(?'verb'[^ :]+)[ :]*(?'arguments'.*)");

        public SmtpCommand(string text)
        {
            Text = text;

            if (!string.IsNullOrEmpty(text))
            {
                var match = COMMANDREGEX.Match(text);

                if (match.Success)
                {
                    Verb = match.Groups["verb"].Value;
                    ArgumentsText = match.Groups["arguments"].Value ?? "";
                    Arguments = ParseArguments(ArgumentsText);
                    IsValid = true;
                    return;
                }
            }

            IsValid = false;
            IsEmpty = true;
        }

        public string Text { get; }

        public string ArgumentsText { get; }

        public string[] Arguments { get; }

        public string Verb { get; }

        public bool IsValid { get; }
        public bool IsEmpty { get; }

        private string[] ParseArguments(string argumentsText)
        {
            var ltCount = 0;
            var arguments = new List<string>();
            var currentArgument = new StringBuilder();
            foreach (var character in argumentsText)
                switch (character)
                {
                    case '<':
                        ltCount++;
                        goto default;
                    case '>':
                        ltCount--;
                        goto default;
                    case ' ':
                    case ':':
                        if (ltCount == 0)
                        {
                            arguments.Add(currentArgument.ToString());
                            currentArgument = new StringBuilder();
                        }
                        else
                        {
                            goto default;
                        }

                        break;
                    default:
                        currentArgument.Append(character);
                        break;
                }

            if (currentArgument.Length != 0) arguments.Add(currentArgument.ToString());
            return arguments.ToArray();
        }
    }
}