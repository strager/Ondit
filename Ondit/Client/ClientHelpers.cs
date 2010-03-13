using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ondit;

namespace Ondit.Client {
    public static class ClientHelpers {
        public static void SendMessage(this ClientBase client, IConversable receiver, string message) {
            client.SendMessage(new RawMessage("PRIVMSG", receiver.Target, message));
        }

        public static void SendMessage(this ClientBase client, string receiver, string message) {
            client.SendMessage(new RawMessage("PRIVMSG", receiver, message));
        }

        public static void SendNotice(this ClientBase client, IConversable receiver, string message) {
            client.SendMessage(new RawMessage("NOTICE", receiver.Target, message));
        }

        public static void SendNotice(this ClientBase client, string receiver, string message) {
            client.SendMessage(new RawMessage("NOTICE", receiver, message));
        }

        public static void JoinChannel(this ClientBase client, string channel, string key = null) {
            client.SendMessage(new RawMessage("JOIN", channel, key));
        }

        public static void JoinChannel(this ClientBase client, Channel channel, string key = null) {
            client.SendMessage(new RawMessage("JOIN", channel.Name, key));
        }

        public static void PartChannel(this ClientBase client, string channel) {
            client.SendMessage(new RawMessage("PART", channel));
        }

        public static void PartChannel(this ClientBase client, Channel channel) {
            client.SendMessage(new RawMessage("PART", channel.Name));
        }
    }
}
