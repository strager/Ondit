﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.Client {
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

    public partial class Client {
        /// <summary>
        /// Fired when the state of the connection changes.
        /// </summary>
        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;

        public event EventHandler<ConversationMessageEventArgs> ConversationMessageReceived;

        protected virtual void OnConnectionStatusChanged(ConnectionStatusEventArgs e) {
            FireEvent(ConnectionStatusChanged, e);
        }

        protected virtual void OnConversationMessageReceived(ConversationMessageEventArgs e) {
            FireEvent(ConversationMessageReceived, e);
        }
    }
}
