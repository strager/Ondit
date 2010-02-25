using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ondit;
using Ondit.IO;

namespace Ondit.Client {
    public enum ConnectionStatus {
        NotConnected,
        Connecting,
        Connected
    };

    public class Client : ClientBase {
        public ChannelManager Channels {
            get;
            private set;
        }

        public UserManager Users {
            get;
            private set;
        }

        public ConnectionStatus ConnectionStatus {
            get;
            protected set;
        }

        private void Init() {
            Channels = new ChannelManager(this);
            Users = new UserManager(this);

            RawMessageReceived += CheckMessage;
        }

        public Client(string host, int port) :
            base(host, port) {
            Init();
        }

        public Client(IRawMessageReader reader, IRawMessageWriter writer) :
            base(reader, writer) {
            Init();
        }

        public void Connect() {
            Connect(null);
        }

        public void Connect(string password) {
            if(Disposed) {
                throw new ObjectDisposedException("this");
            }

            if(ConnectionStatus != ConnectionStatus.NotConnected) {
                throw new InvalidOperationException("Cannot attempt to connect when not unconnected");
            }

            ConnectionStatus = ConnectionStatus.Connecting;

            if(password != null) {
                SendMessage(new RawMessage("PASS", password));
            }

            SendMessage(new RawMessage("NICK", "nickhere"));
            SendMessage(new RawMessage("USER", "username", "host", "server", "Real Name Here"));
        }

        public void WaitForConnected() {
            while(ConnectionStatus == ConnectionStatus.Connecting) {
                HandleMessageBlock();
            }
        }

        private void CheckMessage(object sender, RawMessageEventArgs e) {
            CheckWelcomeMessage(e.Message);
            CheckPing(e.Message);
        }

        private void CheckWelcomeMessage(RawMessage message) {
            if(message.Command == "001") {
                ConnectionStatus = ConnectionStatus.Connected;
            }
        }

        private void CheckPing(RawMessage message) {
            if(message.Command == "PING") {
                SendMessage(new RawMessage("PONG", message.Arguments));
            }
        }
    }
}
