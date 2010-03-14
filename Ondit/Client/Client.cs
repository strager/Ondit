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

    /// <summary>
    /// A useful IRC client.  Handles authentication, channels, messages, etc. and provides a useful interface for them.
    /// </summary>
    public partial class Client : ClientBase {
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

            InitMessageHandlers();

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

        private delegate void RawMessageHandler(RawMessage message);

        private IDictionary<string, RawMessageHandler> messageHandlers = new Dictionary<string, RawMessageHandler>();

        private void InitMessageHandlers() {
            messageHandlers["001"] = (message) => {
                ConnectionStatus = ConnectionStatus.Connected;
            };

            messageHandlers["PING"] = (message) => {
                SendMessage(new RawMessage("PONG", message.Arguments));
            };

            messageHandlers["PRIVMSG"] = (message) => {
                string senderString = message.Prefix == null ? null : message.Prefix.Nick;
                string destinationString = message.Arguments[0];
                string messageString = string.Join(" ", message.Arguments.Skip(1).ToArray());

                IConversable sender;

                if(Channel.IsChannelName(destinationString)) {
                    sender = Channels[destinationString][senderString];
                } else {
                    sender = Users[senderString];
                }

                OnConversationMessageReceived(new ConversationMessageEventArgs(sender, messageString));
            };

            messageHandlers["331"] = (message) => {
                string channel = message.Arguments[0];

                Channels[channel].Topic = "";
            };

            messageHandlers["332"] = (message) => {
                string channel = message.Arguments[0];
                string topic = message.Arguments[1];

                Channels[channel].Topic = topic;
            };

            messageHandlers["TOPIC"] = (message) => {
                string channel = message.Arguments[0];
                string topic = message.Arguments[1];

                Channels[channel].Topic = topic;
            };
        }

        private void CheckRawMessage(object sender, RawMessageEventArgs e) {
            if(messageHandlers.ContainsKey(e.Message.Command)) {
                messageHandlers[e.Message.Command](e.Message);
            }
        }
    }
}
