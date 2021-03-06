﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public class Channel : IConversable {
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

        public string Target {
            get {
                return Name;
            }
        }

        public IEnumerable<ChannelUser> Users {
            get {
                return UserCollection;
            }
        }

        public override string ToString() {
            return Name;
        }

        internal ICollection<ChannelUser> UserCollection {
            get;
            private set;
        }

        public Channel(string name) {
            this.name = name;

            UserCollection = new List<ChannelUser>();
        }

        public ChannelUser this[string nick] {
            get {
                var user = Users.SingleOrDefault((u) => u.Nick == nick)
                    ?? new ChannelUser(this) { Nick = nick };

                return user;
            }
        }

        public ChannelUser GetUserFromPrefix(RawMessagePrefix prefix) {
            var user = this[prefix.Nick];

            // TODO Assign parts.

            return user;
        }

        public static bool IsChannelName(string name) {
            if(name.Length < 1) {
                return false;
            }

            return name[0] == '#' || name[0] == '&';
        }
    }
}
