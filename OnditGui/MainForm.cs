using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Ondit;
using Ondit.IO;
using Ondit.Client;

namespace OnditGui {
    public partial class MainForm : Form {
        private Client client;
        private Thread clientThread;

        public MainForm() {
            InitializeComponent();
        }

        private void connect_Click(object sender, EventArgs e) {
            if(client != null) {
                client.Dispose();
                client = null;
            }

            if(clientThread != null) {
                clientThread.Abort();
                clientThread = null;
            }

            string host = server.Text;
            int portNumber = int.Parse(port.Text);

            Connection(host, portNumber);
        }

        private void Connection(string host, int port) {
            client = new Client(host, port);

            client.SynchronizingObject = this;

            client.RawMessageSent += MessageSent;
            client.RawMessageReceived += MessageReceived;
            client.ConversationMessageReceived += ConversationMessageReceived;

            client.Connect();
            client.WaitForConnected();

            if(client.ConnectionStatus != ConnectionStatus.Connected) {
                conversation.Text += string.Format(@"Could not connect to {0} on port {1}." + Environment.NewLine, host, port);

                return;
            }

            clientThread = new Thread(() => {
                while(client != null) {
                    client.HandleMessageBlock();
                }
            });

            clientThread.Start();
        }

        private void MessageSent(object sender, RawMessageEventArgs e) {
            sendQueue.Text += e.Message.ToString() + Environment.NewLine;
        }

        private void MessageReceived(object sender, RawMessageEventArgs e) {
            receiveQueue.Text += e.Message.ToString() + Environment.NewLine;
        }

        private void ConversationMessageReceived(object sender, ConversationMessageEventArgs e) {
            conversation.Text += string.Format(@"{0}: {1}" + Environment.NewLine, e.Sender.ToString(), e.Message);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            if(client != null) {
                clientThread.Abort();
                client.Dispose();
            }
        }

        private void sendMessage_Click(object sender, EventArgs e) {
            var m = message.Text;

            if(m.Length == 0) {
                return;
            }

            if(m[0] == '/') {
                int cmdEnd = m.IndexOf(' ');

                if(cmdEnd < 0) {
                    cmdEnd = m.Length;
                }

                string cmd = m.Substring(1, cmdEnd - 1);
                string args = m.Substring(Math.Min(cmdEnd + 1, m.Length)).TrimStart();
                string[] argWords = args.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                switch(cmd) {
                    case "join":
                        string channel = argWords[0];
                        string password = argWords.Length > 1 ? argWords[1] : null;

                        var chan = client.Channels[channel];
                        chan.Join(password);

                        break;

                    case "msg":
                        string receiver = argWords[0];
                        string msg = args.Substring(Math.Min(receiver.Length + 1, args.Length));

                        if("#&".Contains(receiver[0])) {
                            client.Channels[receiver].SendMessage(msg);
                        } else {
                            client.Users[receiver].SendMessage(msg);
                        }

                        break;
                }
            }
        }
    }
}
