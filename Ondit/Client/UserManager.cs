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

        private ServerUser UserOrNew(string nick) {
            var existing = users.ContainsKey(nick) ? users[nick] : null;

            if(existing != null) {
                return existing;
            }

            var user = new ServerUser {
                Nick = nick
            };

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

        private void CheckMessage(object sender, RawMessageEventArgs e) {
            CheckNickChange(e.Message);
        }

        private void CheckNickChange(RawMessage message) {
            if(message.Command != "NICK") {
                return;
            }

            if(message.Prefix == null || message.Prefix.Nick == null) {
                return;
            }

            if(message.Arguments.Length < 1) {
                return;
            }

            // Not a user we aren't tracking?  We don't care, then.
            if(!users.ContainsKey(message.Arguments[0])) {
                return;
            }

            ChangeUserNick(message.Prefix.Nick, message.Arguments[0]);
        }
    }
}
