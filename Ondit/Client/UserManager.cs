using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public class UserManager {
        public IEnumerable<ServerUser> Users {
            get {
                return this.users.Values;
            }
        }

        // TODO Lock to prevent funny business.
        private IDictionary<string, ServerUser> users = new Dictionary<string, ServerUser>();   // nick => user

        public ClientBase Client {
            get;
            private set;
        }

        public UserManager(ClientBase client) {
            Client = client;
        }

        private ServerUser UserOrNew(string nick) {
            var existing = users.ContainsKey(nick) ? users[nick] : null;

            if(existing != null) {
                return existing;
            }

            var user = new ServerUser(Client);

            user.Nick = nick;

            return user;
        }

        private void ChangeUserNick(string oldNick, string newNick) {
            var user = UserOrNew(oldNick);

            users.Remove(oldNick);

            user.Nick = newNick;

            users[newNick] = user;
        }

        public ServerUser this[string nick] {
            get {
                return UserOrNew(nick);
            }
        }
    }
}
