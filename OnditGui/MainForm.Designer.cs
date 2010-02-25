namespace OnditGui {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.server = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.port = new System.Windows.Forms.TextBox();
            this.connect = new System.Windows.Forms.Button();
            this.message = new System.Windows.Forms.TextBox();
            this.sendMessage = new System.Windows.Forms.Button();
            this.conversation = new System.Windows.Forms.TextBox();
            this.receiveQueue = new System.Windows.Forms.TextBox();
            this.sendQueue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // server
            // 
            this.server.Location = new System.Drawing.Point(68, 6);
            this.server.Name = "server";
            this.server.Size = new System.Drawing.Size(100, 22);
            this.server.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(174, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(214, 6);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(100, 22);
            this.port.TabIndex = 3;
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(320, 6);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(75, 23);
            this.connect.TabIndex = 4;
            this.connect.Text = "Connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // message
            // 
            this.message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.message.Location = new System.Drawing.Point(12, 619);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(965, 22);
            this.message.TabIndex = 5;
            // 
            // sendMessage
            // 
            this.sendMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendMessage.Location = new System.Drawing.Point(983, 619);
            this.sendMessage.Name = "sendMessage";
            this.sendMessage.Size = new System.Drawing.Size(75, 23);
            this.sendMessage.TabIndex = 6;
            this.sendMessage.Text = "Send";
            this.sendMessage.UseVisualStyleBackColor = true;
            this.sendMessage.Click += new System.EventHandler(this.sendMessage_Click);
            // 
            // conversation
            // 
            this.conversation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.conversation.Location = new System.Drawing.Point(15, 35);
            this.conversation.Multiline = true;
            this.conversation.Name = "conversation";
            this.conversation.ReadOnly = true;
            this.conversation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.conversation.Size = new System.Drawing.Size(595, 578);
            this.conversation.TabIndex = 7;
            // 
            // receiveQueue
            // 
            this.receiveQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.receiveQueue.Location = new System.Drawing.Point(616, 35);
            this.receiveQueue.Multiline = true;
            this.receiveQueue.Name = "receiveQueue";
            this.receiveQueue.ReadOnly = true;
            this.receiveQueue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.receiveQueue.Size = new System.Drawing.Size(442, 300);
            this.receiveQueue.TabIndex = 8;
            // 
            // sendQueue
            // 
            this.sendQueue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sendQueue.Location = new System.Drawing.Point(616, 341);
            this.sendQueue.Multiline = true;
            this.sendQueue.Name = "sendQueue";
            this.sendQueue.ReadOnly = true;
            this.sendQueue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sendQueue.Size = new System.Drawing.Size(442, 272);
            this.sendQueue.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 653);
            this.Controls.Add(this.sendQueue);
            this.Controls.Add(this.receiveQueue);
            this.Controls.Add(this.conversation);
            this.Controls.Add(this.sendMessage);
            this.Controls.Add(this.message);
            this.Controls.Add(this.connect);
            this.Controls.Add(this.port);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.server);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "Ondit GUI client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox server;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.TextBox message;
        private System.Windows.Forms.Button sendMessage;
        private System.Windows.Forms.TextBox conversation;
        private System.Windows.Forms.TextBox receiveQueue;
        private System.Windows.Forms.TextBox sendQueue;
    }
}

