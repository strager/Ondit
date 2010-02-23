using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public class ChannelUser : IConversable {
        public string ConversationTarget {
            get {
                return channel.Name;
            }
        }

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
    }
}
