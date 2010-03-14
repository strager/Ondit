using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ondit.Client;

namespace OnditBot {
    public interface IPlugin {
        string Name {
            get;
        }

        Client Client {
            get;
            set;
        }
    }
}
