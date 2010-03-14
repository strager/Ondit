using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ondit.Client;

namespace OnditBot.Plugins {
    public abstract class CommandPlugin : IPlugin {
        private readonly Random random = new Random();
        private const string CommandFormat = "!$trigger$ $message$";

        public abstract string Name {
            get;
        }

        private Client client;

        public Client Client {
            get {
                return this.client;
            }

            set {
                if(this.client != null) {
                    client.ConversationMessageReceived -= HandleConversationMessage;
                }

                this.client = value;

                if(this.client != null) {
                    client.ConversationMessageReceived += HandleConversationMessage;
                }
            }
        }

        public abstract IEnumerable<string> Triggers {
            get;
        }

        protected abstract void MessageReceived(IConversable sender, string message);

        private void HandleConversationMessage(object sender, ConversationMessageEventArgs e) {
            foreach(var trigger in Triggers) {
                var match = GetTriggerRegex(trigger).Match(e.Message);

                if(!match.Success) {
                    continue;
                }

                MessageReceived(e.Sender, match.Groups["message"].Value);

                break;
            }
        }

        private static Regex GetTriggerRegex(string trigger) {
            // TODO More sturdy.
            var s = CommandFormat.Replace(@"$trigger$", trigger);
            s = s.Replace("$message$", @"\b(?<message>.*)\b");

            return new Regex(s, RegexOptions.IgnorePatternWhitespace);
        }
    }
}
