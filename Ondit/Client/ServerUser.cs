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

        public string Target {
            get {
                return Nick;
            }
        }

        internal ServerUser() {
        }

        public override string ToString() {
            return Nick;
        }
    }
}
