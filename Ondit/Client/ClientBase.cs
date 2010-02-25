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

    public class ClientBase : IDisposable {
        private IRawMessageReader reader;
        private IRawMessageWriter writer;

        private IList<IDisposable> objectsToDispose = new List<IDisposable>();

        private Socket MakeSocket(IPEndPoint endPoint) {
            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(endPoint);

            return socket;
        }

        public ClientBase(string host, int port) {
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
        }

        public ClientBase(IRawMessageReader reader, IRawMessageWriter writer) {
            this.reader = reader;
            this.writer = writer;
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

        protected bool Disposed {
            get {
                return disposed;
            }
        }

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
