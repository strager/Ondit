using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public class ServerUser : IConversable {
        public string Nick {
            get;
            set;
        }

        public string ModeString {
            get;
            set;
        }

        public string Target {
            get {
                return Nick;
            }
        }

        public override string ToString() {
            return Nick;
        }
    }
}
