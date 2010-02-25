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

    /// <summary>
    /// A bare IRC client.  Does not do anything meaningful with messages sent and received.
    /// </summary>
    public class ClientBase : IDisposable {
        private IRawMessageReader reader;
        private IRawMessageWriter writer;

        private IList<IDisposable> objectsToDispose = new List<IDisposable>();

        private Socket MakeSocket(IPEndPoint endPoint) {
            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(endPoint);

            return socket;
        }

        /// <summary>
        /// Creates an IRC client by connecting to a socket on <paramref name="host"/> on port <paramref name="port"/>.
        /// </summary>
        /// <param name="host">Name of the host to connect to (IP or domain name).</param>
        /// <param name="port">Port number to connect using.</param>
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

        /// <summary>
        /// Creates an IRC client by using the given <see cref="RawMessage"/> reader and writer.
        /// </summary>
        /// <param name="reader">Client input reader.</param>
        /// <param name="writer">Client output writer.</param>
        public ClientBase(IRawMessageReader reader, IRawMessageWriter writer) {
            this.reader = reader;
            this.writer = writer;
        }

        /// <summary>
        /// Reads one queued message if there are any messages on the queue and handles it.
        /// </summary>
        /// <returns>Message handled.</returns>
        /// <exception cref="ObjectDisposedException"/>
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

        /// <summary>
        /// Reads one queued message if there are any messages on the queue and handles it.  If there are no messages on the queue, waits for one and handles it.
        /// </summary>
        /// <returns>Message handled.</returns>
        /// <exception cref="ObjectDisposedException"/>
        public RawMessage HandleMessageBlock() {
            if(disposed) {
                throw new ObjectDisposedException("this");
            }

            var message = reader.ReadBlock();

            HandleMessage(message);

            return message;
        }

        /// <summary>
        /// Sends a message to the send queue.
        /// </summary>
        /// <param name="message">Message to queue.</param>
        /// <exception cref="ObjectDisposedException"/>
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

        /// <summary>
        /// Fired when a message is received from the server to the client.
        /// </summary>
        public event EventHandler<RawMessageEventArgs> RawMessageReceived;

        /// <summary>
        /// Fired when a message is sent from the client to the server.
        /// </summary>
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

        /// <summary>
        /// Disposes the IRC client.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
    }
}
