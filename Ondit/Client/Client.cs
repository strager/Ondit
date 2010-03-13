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

    public class ConnectionStatusEventArgs : EventArgs {
        public ConnectionStatus OldStatus {
            get;
            set;
        }

        public ConnectionStatus NewStatus {
            get;
            set;
        }

        public ConnectionStatusEventArgs(ConnectionStatus oldStatus, ConnectionStatus newStatus) {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }

    public class ConversationMessageEventArgs : EventArgs {
        public IConversable Sender {
            get;
            set;
        }

        public string Message {
            get;
            set;
        }

        public ConversationMessageEventArgs(IConversable sender, string message) {
            Sender = sender;
            Message = message;
        }
    }

    /// <summary>
    /// A useful IRC client.  Handles authentication, channels, messages, etc. and provides a useful interface for them.
    /// </summary>
    public class Client : ClientBase {
        /// <summary>
        /// Manager for the active channels on the IRC server.
        /// </summary>
        public ChannelManager Channels {
            get;
            private set;
        }

        /// <summary>
        /// Manager for the active users on the IRC server.
        /// </summary>
        public UserManager Users {
            get;
            private set;
        }

        /// <summary>
        /// Current state of the IRC connection.
        /// </summary>
        public ConnectionStatus ConnectionStatus {
            get {
                return connectionStatus;
            }

            protected set {
                if(value == connectionStatus) {
                    return;
                }

                var old = connectionStatus;

                connectionStatus = value;

                OnConnectionStatusChanged(new ConnectionStatusEventArgs(old, value));
            }
        }

        private ConnectionStatus connectionStatus = ConnectionStatus.NotConnected;

        private void Init() {
            Channels = new ChannelManager();
            Users = new UserManager();

            RawMessageReceived += CheckRawMessage;
        }

        /// <summary>
        /// Creates an IRC client by connecting to a socket on <paramref name="host"/> on port <paramref name="port"/>.
        /// </summary>
        /// <param name="host">Name of the host to connect to (IP or domain name).</param>
        /// <param name="port">Port number to connect using.</param>
        public Client(string host, int port) :
            base(host, port) {
            Init();
        }

        /// <summary>
        /// Creates an IRC client by using the given <see cref="RawMessage"/> reader and writer.
        /// </summary>
        /// <param name="reader">Client input reader.</param>
        /// <param name="writer">Client output writer.</param>
        public Client(IRawMessageReader reader, IRawMessageWriter writer) :
            base(reader, writer) {
            Init();
        }

        /// <summary>
        /// Connects and authenticates to the IRC server.
        /// </summary>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="InvalidOperationException">Cannot attempt to connect when not unconnected</exception>
        public void Connect() {
            Connect(null);
        }

        /// <summary>
        /// Connects and authenticates to the IRC server.
        /// </summary>
        /// <param name="password">IRC server password.</param>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="InvalidOperationException">Cannot attempt to connect when not unconnected</exception>
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

        /// <summary>
        /// Blocks the thread waiting for the connection and authentication to succeed or fail.
        /// </summary>
        public void WaitForConnected() {
            while(ConnectionStatus == ConnectionStatus.Connecting) {
                HandleMessageBlock();
            }
        }

        public void SendMessage(IConversable receiver, string message) {
            SendMessage(new RawMessage("PRIVMSG", receiver.Target, message));
        }

        public void SendMessage(string receiver, string message) {
            SendMessage(new RawMessage("PRIVMSG", receiver, message));
        }

        public void SendNotice(IConversable receiver, string message) {
            SendMessage(new RawMessage("NOTICE", receiver.Target, message));
        }

        public void SendNotice(string receiver, string message) {
            SendMessage(new RawMessage("NOTICE", receiver, message));
        }

        public void JoinChannel(string channel, string key = null) {
            SendMessage(new RawMessage("JOIN", channel, key));
        }

        public void JoinChannel(Channel channel, string key = null) {
            SendMessage(new RawMessage("JOIN", channel.Name, key));
        }

        public void PartChannel(string channel) {
            SendMessage(new RawMessage("PART", channel));
        }

        public void PartChannel(Channel channel) {
            SendMessage(new RawMessage("PART", channel.Name));
        }

        /// <summary>
        /// Fired when the state of the connection changes.
        /// </summary>
        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;

        protected virtual void OnConnectionStatusChanged(ConnectionStatusEventArgs e) {
            FireEvent(ConnectionStatusChanged, e);
        }

        public event EventHandler<ConversationMessageEventArgs> ConversationMessageReceived;

        protected virtual void OnConversationMessageReceived(ConversationMessageEventArgs e) {
            FireEvent(ConversationMessageReceived, e);
        }

        private void CheckRawMessage(object sender, RawMessageEventArgs e) {
            CheckWelcomeMessage(e.Message);
            CheckPing(e.Message);
            CheckPrivateMessage(e.Message);
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

        private void CheckPrivateMessage(RawMessage message) {
            if(message.Command != "PRIVMSG") {
                return;
            }

            string senderString = message.Arguments[0];
            string messageString = string.Join(" ", message.Arguments.Skip(1).ToArray());

            IConversable sender;

            if(Channel.IsChannelName(senderString)) {
                sender = Channels[senderString];
            } else {
                sender = Users[senderString];
            }

            OnConversationMessageReceived(new ConversationMessageEventArgs(sender, messageString));
        }
    }
}
