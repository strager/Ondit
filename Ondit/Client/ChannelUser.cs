using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public class ChannelUser : IConversable {
        public string Nick {
            get;
            set;
        }

        public char ModeChar {
            get;
            set;
        }

        public string Target {
            get {
                return Channel.Target;
            }
        }

        public Channel Channel {
            get;
            private set;
        }

        public ChannelUser(Channel channel) {
            if(channel == null) {
                throw new ArgumentNullException("channel");
            }

            Channel = channel;
        }

        public override string ToString() {
            return Nick;
        }
    }
}
