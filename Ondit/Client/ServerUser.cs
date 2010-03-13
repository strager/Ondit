using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public class ServerUser : IConversable {
        public string Nick {
            get;
            internal set;
        }

        public string ModeString {
            get;
            internal set;
        }

        internal ClientBase Client {
            get;
            private set;
        }

        internal ServerUser(ClientBase client) {
            Client = client;
        }

        public void SendMessage(string message) {
            Client.SendMessage(new RawMessage("PRIVMSG", Nick, message));
        }

        public void SendNotice(string notice) {
            Client.SendMessage(new RawMessage("NOTICE", Nick, notice));
        }

        public override string ToString() {
            return Nick;
        }
    }
}
