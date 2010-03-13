using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
    public interface IConversable {
        void SendMessage(string message);
        void SendNotice(string notice);

        string ToString();
    }
}
