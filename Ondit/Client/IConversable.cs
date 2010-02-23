using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public interface IConversable {
        string ConversationTarget {
            get;
        }
    }
}
