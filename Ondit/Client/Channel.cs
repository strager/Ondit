using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public class Channel : IConversable {
        public string ConversationTarget {
            get {
                return Name;
            }
        }

        private readonly string name;

        public string Name {
            get {
                return this.name;
            }
        }

        public string Topic {
            get;
            internal set;
        }

        public IEnumerable<ChannelUser> Users {
            get {
                return UserCollection;
            }
        }

        internal Client Client {
            get;
            private set;
        }

        public void Join() {
            Join(null);
        }

        public void Join(string key) {
            Client.SendMessage(new RawMessage("JOIN", Name, key));
        }

        public void Part() {
            Client.SendMessage(new RawMessage("PART", Name));
        }

        public void SendMessage(string message) {
            Client.SendMessage(new RawMessage("PRIVMSG", Name, message));
        }

        public void SendNotice(string notice) {
            Client.SendMessage(new RawMessage("NOTICE", Name, notice));
        }

        internal ICollection<ChannelUser> UserCollection {
            get;
            private set;
        }

        internal Channel(Client client, string name) {
            this.name = name;

            this.UserCollection = new List<ChannelUser>();
        }
    }
}
