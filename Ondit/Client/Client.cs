using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Ondit;
using Ondit.IO;

namespace Ondit.Client {
    public class RawMessageEventArgs : EventArgs {
        public RawMessage Message {
            get;
            set;
        }

        public RawMessageEventArgs(RawMessage message) {
            Message = message;
        }
    }

    public class Client : IDisposable {
        private IRawMessageReader reader;
        private IRawMessageWriter writer;

        private IList<IDisposable> objectsToDispose = new List<IDisposable>();

        private Socket MakeSocket(IPEndPoint endPoint) {
            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(endPoint);

            return socket;
        }

        public Client(string host, int port) {
            foreach(var address in Dns.GetHostEntry(host).AddressList) {
                var socket = MakeSocket(new IPEndPoint(address, port));

                if(!socket.Connected) {
                    continue;
                }

                objectsToDispose.Add(socket);

                var stream = new NetworkStream(socket, false);

                objectsToDispose.Add(stream);

                var streamReader = new StreamReader(stream);
                var streamWriter = new StreamWriter(stream);

                objectsToDispose.Add(streamReader);
                objectsToDispose.Add(streamWriter);

                var reader = new RawMessageTextReader(streamReader);
                var writer = new RawMessageTextWriter(streamWriter);

                objectsToDispose.Add(reader);
                objectsToDispose.Add(writer);

                this.reader = reader;
                this.writer = writer;

                break;
            }

            // TODO More error handling.

            RawMessageReceived += CheckWelcomeMessage;
            RawMessageReceived += CheckPing;
        }

        public Client(IRawMessageReader reader, IRawMessageWriter writer) {
            this.reader = reader;
            this.writer = writer;
        }

        public bool IsConnected {
            get;
            protected set;
        }

        public bool IsConnecting {
            get;
            protected set;
        }

        public void Connect() {
            Connect(null);
        }

        public void Connect(string password) {
            if(disposed) {
                throw new ObjectDisposedException("this");
            }

            IsConnecting = true;

            if(password != null) {
                SendMessage(new RawMessage("PASS", password));
            }

            SendMessage(new RawMessage("NICK", "nickhere"));
            SendMessage(new RawMessage("USER", "username", "host", "server", "Real Name Here"));
        }

        public void WaitForConnected() {
            while(IsConnecting) {
                HandleMessageBlock();
            }
        }

        public RawMessage HandleMessage() {
            if(disposed) {
                throw new ObjectDisposedException("this");
            }

            var message = reader.Read();

            if(message != null) {
                HandleMessage(message);
            }

            return message;
        }

        public RawMessage HandleMessageBlock() {
            if(disposed) {
                throw new ObjectDisposedException("this");
            }

            var message = reader.ReadBlock();

            HandleMessage(message);

            return message;
        }

        public void SendMessage(RawMessage message) {
            if(disposed) {
                throw new ObjectDisposedException("this");
            }

            writer.Write(message);

            OnRawMessageSent(new RawMessageEventArgs(message));
        }

        private void HandleMessage(RawMessage message) {
            OnRawMessageReceived(new RawMessageEventArgs(message));
        }

        private void CheckWelcomeMessage(object sender, RawMessageEventArgs e) {
            if(e.Message.Command == "001") {
                IsConnected = true;
                IsConnecting = false;
            }
        }

        private void CheckPing(object sender, RawMessageEventArgs e) {
            if(e.Message.Command == "PING") {
                SendMessage(new RawMessage("PONG", e.Message.Arguments));
            }
        }

        public event EventHandler<RawMessageEventArgs> RawMessageReceived;
        public event EventHandler<RawMessageEventArgs> RawMessageSent;

        protected virtual void OnRawMessageReceived(RawMessageEventArgs e) {
            var handler = RawMessageReceived;

            if(handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnRawMessageSent(RawMessageEventArgs e) {
            var handler = RawMessageSent;

            if(handler != null) {
                handler(this, e);
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {
            if(!disposed) {
                if(disposing) {
                    foreach(var obj in objectsToDispose.Reverse()) {
                        obj.Dispose();
                    }
                }

                disposed = true;
            }
        }

        public void Dispose() {
            Dispose(true);
        }
    }
}
