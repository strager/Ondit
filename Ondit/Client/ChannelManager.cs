using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public class ChannelManager {
        public IEnumerable<Channel> Channels {
            get {
                return this.channels.Values;
            }
        }

        private IDictionary<string, Channel> channels = new Dictionary<string, Channel>();

        public Channel this[string channelName] {
            get {
                if(this.channels.ContainsKey(channelName)) {
                    return channels[channelName];
                }

                var channel = new Channel(channelName);

                this.channels[channelName] = channel;

                return channel;
            }
        }
    }
}
