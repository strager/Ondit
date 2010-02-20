using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit {
    public class RawMessage {
        public string Host {
            get;
            set;
        }

        public string Command {
            get;
            set;
        }

        public string[] Arguments {
            get;
            set;
        }

        public RawMessage() :
            this(null) {
        }

        public RawMessage(string command) :
            this(command, null) {
        }

        public RawMessage(string command, string[] arguments) :
            this(null, command, arguments) {
        }

        public RawMessage(string host, string command, string[] arguments) {
            Host = host;
            Command = command;
            Arguments = arguments;
        }

        public static RawMessage FromRaw(string raw) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            string output = "";

            if(Host != null) {
                output += ":" + Host + " ";
            }

            output += Command;

            for(int i = 0; i < Arguments.Length - 1; ++i) {
                output += " " + Arguments[i];
            }

            if(Arguments.Length - 1 >= 0) {
                output += " :" + Arguments[Arguments.Length - 1];
            }

            return output;
        }
    }
}
