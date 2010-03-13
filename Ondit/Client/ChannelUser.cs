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

        public string Target {
            get {
                return this.channel.Target;
            }
        }

        private Channel channel;

        internal ChannelUser(Channel channel) {
            if(channel == null) {
                throw new ArgumentNullException("channel");
            }

            this.channel = channel;
        }

        public override string ToString() {
            return Nick;
        }
    }
}
