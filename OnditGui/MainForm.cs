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
        private ClientThreader threader = new ClientThreader();
        private Client client;

        public MainForm() {
            threader.SynchronizingObject = this;

            InitializeComponent();
        }

        private void connect_Click(object sender, EventArgs e) {
            if(client != null) {
                client.Dispose();
            }

            string host = server.Text;
            int portNumber = int.Parse(port.Text);

            Connection(host, portNumber);
        }

        private void Connection(string host, int port) {
            client = new Client(host, port);

            threader.Client = client;

            threader.RawMessageSent += MessageSent;
            threader.RawMessageReceived += MessageReceived;

            client.Connect();
            client.WaitForConnected();

            if(!client.IsConnected) {
                conversation.Text += "Could not connect to " + host + " on port " + port + "." + Environment.NewLine;

                return;
            }

            threader.Thread.Start();
        }

        private void MessageSent(object sender, RawMessageEventArgs e) {
            sendQueue.Text += e.Message.ToString() + Environment.NewLine;
        }

        private void MessageReceived(object sender, RawMessageEventArgs e) {
            receiveQueue.Text += e.Message.ToString() + Environment.NewLine;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            if(client != null) {
                client.Dispose();
            }
        }
    }
}
