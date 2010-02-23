using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public class ChannelUser : IConversable {
        public string Nick {
            get;
            internal set;
        }

        public char ModeChar {
            get;
            internal set;
        }

        private Channel channel;

        internal ChannelUser(Channel channel) {
            this.channel = channel;
        }

        public void SendMessage(string message) {
            channel.Client.SendMessage(new RawMessage("PRIVMSG", channel.Name, message));
        }

        public void SendNotice(string notice) {
            channel.Client.SendMessage(new RawMessage("NOTICE", channel.Name, notice));
        }
    }
}
