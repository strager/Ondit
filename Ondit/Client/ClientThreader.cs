using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Ondit.Client {
    public class ClientThreader {
        private Client client;

        public Client Client {
            get {
                return this.client;
            }

            set {
                if(this.client != null) {
                    this.client.RawMessageReceived -= RedirectRawMessageReceived;
                    this.client.RawMessageSent -= RedirectRawMessageSent;
                }

                this.client = value;

                if(this.client != null) {
                    this.client.RawMessageReceived += RedirectRawMessageReceived;
                    this.client.RawMessageSent += RedirectRawMessageSent;
                }
            }
        }

        public Thread Thread {
            get;
            private set;
        }

        public ISynchronizeInvoke SynchronizingObject {
            get;
            set;
        }

        public ClientThreader() :
            this(null) {
        }

        public ClientThreader(Client client) {
            Client = client;
            Thread = new Thread(MessageLoop);
        }

        public event EventHandler<RawMessageEventArgs> RawMessageReceived;
        public event EventHandler<RawMessageEventArgs> RawMessageSent;

        private void MessageLoop() {
            while(true) {
                Client.HandleMessageBlock();
            }
        }

        private void RedirectRawMessageReceived(object sender, RawMessageEventArgs e) {
            var handler = RawMessageReceived;

            if(handler != null) {
                if(SynchronizingObject != null) {
                    SynchronizingObject.Invoke(handler, new object[] { sender, e });
                } else {
                    handler.Invoke(sender, e);
                }
            }
        }

        private void RedirectRawMessageSent(object sender, RawMessageEventArgs e) {
            var handler = RawMessageSent;

            if(handler != null) {
                if(SynchronizingObject != null) {
                    SynchronizingObject.Invoke(handler, new object[] { sender, e });
                } else {
                    handler.Invoke(sender, e);
                }
            }
        }
    }
}
