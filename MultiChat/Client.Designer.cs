namespace MultiChat
{
    partial class Client
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            tb_SerRSAPubkey = new TextBox();
            tb_ClientRSApubkey = new TextBox();
            tb_ClientRSAprikey = new TextBox();
            label2 = new Label();
            label4 = new Label();
            label5 = new Label();
            tb_AESkey = new TextBox();
            label6 = new Label();
            tb_SerIP = new TextBox();
            tb_UserName = new TextBox();
            label3 = new Label();
            label7 = new Label();
            tb_Message = new TextBox();
            label8 = new Label();
            bt_Connect = new Button();
            bt_Send = new Button();
            tb_MessageLog = new RichTextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(67, 20);
            label1.TabIndex = 0;
            label1.Text = "Message";
            // 
            // tb_SerRSAPubkey
            // 
            tb_SerRSAPubkey.Location = new Point(559, 32);
            tb_SerRSAPubkey.Multiline = true;
            tb_SerRSAPubkey.Name = "tb_SerRSAPubkey";
            tb_SerRSAPubkey.ReadOnly = true;
            tb_SerRSAPubkey.ScrollBars = ScrollBars.Both;
            tb_SerRSAPubkey.Size = new Size(417, 152);
            tb_SerRSAPubkey.TabIndex = 1;
            // 
            // tb_ClientRSApubkey
            // 
            tb_ClientRSApubkey.Location = new Point(559, 190);
            tb_ClientRSApubkey.Multiline = true;
            tb_ClientRSApubkey.Name = "tb_ClientRSApubkey";
            tb_ClientRSApubkey.ReadOnly = true;
            tb_ClientRSApubkey.ScrollBars = ScrollBars.Both;
            tb_ClientRSApubkey.Size = new Size(417, 152);
            tb_ClientRSApubkey.TabIndex = 1;
            // 
            // tb_ClientRSAprikey
            // 
            tb_ClientRSAprikey.Location = new Point(559, 348);
            tb_ClientRSAprikey.Multiline = true;
            tb_ClientRSAprikey.Name = "tb_ClientRSAprikey";
            tb_ClientRSAprikey.ReadOnly = true;
            tb_ClientRSAprikey.ScrollBars = ScrollBars.Both;
            tb_ClientRSAprikey.Size = new Size(417, 152);
            tb_ClientRSAprikey.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(559, 20);
            label2.Name = "label2";
            label2.Size = new Size(161, 20);
            label2.TabIndex = 0;
            label2.Text = "Server's RSA public key";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(559, 177);
            label4.Name = "label4";
            label4.Size = new Size(158, 20);
            label4.TabIndex = 0;
            label4.Text = "Client's RSA public key";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(559, 339);
            label5.Name = "label5";
            label5.Size = new Size(163, 20);
            label5.TabIndex = 0;
            label5.Text = "Client's RSA private key";
            // 
            // tb_AESkey
            // 
            tb_AESkey.Location = new Point(12, 385);
            tb_AESkey.Multiline = true;
            tb_AESkey.Name = "tb_AESkey";
            tb_AESkey.ReadOnly = true;
            tb_AESkey.ScrollBars = ScrollBars.Both;
            tb_AESkey.Size = new Size(541, 115);
            tb_AESkey.TabIndex = 1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 374);
            label6.Name = "label6";
            label6.Size = new Size(61, 20);
            label6.TabIndex = 0;
            label6.Text = "AES key";
            // 
            // tb_SerIP
            // 
            tb_SerIP.Location = new Point(12, 539);
            tb_SerIP.Name = "tb_SerIP";
            tb_SerIP.Size = new Size(286, 27);
            tb_SerIP.TabIndex = 1;
            tb_SerIP.Text = "127.0.0.1";
            // 
            // tb_UserName
            // 
            tb_UserName.Location = new Point(12, 592);
            tb_UserName.Name = "tb_UserName";
            tb_UserName.Size = new Size(286, 27);
            tb_UserName.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 516);
            label3.Name = "label3";
            label3.Size = new Size(75, 20);
            label3.TabIndex = 0;
            label3.Text = "Server's IP";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 569);
            label7.Name = "label7";
            label7.Size = new Size(82, 20);
            label7.TabIndex = 0;
            label7.Text = "User Name";
            // 
            // tb_Message
            // 
            tb_Message.Location = new Point(454, 539);
            tb_Message.Multiline = true;
            tb_Message.Name = "tb_Message";
            tb_Message.ReadOnly = true;
            tb_Message.Size = new Size(417, 87);
            tb_Message.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(454, 516);
            label8.Name = "label8";
            label8.Size = new Size(67, 20);
            label8.TabIndex = 0;
            label8.Text = "Message";
            // 
            // bt_Connect
            // 
            bt_Connect.Location = new Point(304, 539);
            bt_Connect.Name = "bt_Connect";
            bt_Connect.Size = new Size(106, 87);
            bt_Connect.TabIndex = 2;
            bt_Connect.Text = "Connect";
            bt_Connect.UseVisualStyleBackColor = true;
            bt_Connect.Click += bt_Connect_Click;
            // 
            // bt_Send
            // 
            bt_Send.Enabled = false;
            bt_Send.Location = new Point(877, 539);
            bt_Send.Name = "bt_Send";
            bt_Send.Size = new Size(99, 87);
            bt_Send.TabIndex = 2;
            bt_Send.Text = "Send";
            bt_Send.UseVisualStyleBackColor = true;
            bt_Send.Click += bt_Send_Click;
            // 
            // tb_MessageLog
            // 
            tb_MessageLog.Location = new Point(12, 32);
            tb_MessageLog.Name = "tb_MessageLog";
            tb_MessageLog.Size = new Size(541, 339);
            tb_MessageLog.TabIndex = 3;
            tb_MessageLog.Text = "";
            // 
            // Client
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(988, 650);
            Controls.Add(tb_MessageLog);
            Controls.Add(bt_Send);
            Controls.Add(bt_Connect);
            Controls.Add(label7);
            Controls.Add(label8);
            Controls.Add(label3);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(tb_UserName);
            Controls.Add(tb_SerIP);
            Controls.Add(tb_AESkey);
            Controls.Add(tb_Message);
            Controls.Add(tb_ClientRSAprikey);
            Controls.Add(tb_ClientRSApubkey);
            Controls.Add(tb_SerRSAPubkey);
            Controls.Add(label1);
            Name = "Client";
            Text = "Client";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox tb_SerRSAPubkey;
        private TextBox tb_ClientRSApubkey;
        private TextBox tb_ClientRSAprikey;
        private Label label2;
        private Label label4;
        private Label label5;
        private TextBox tb_AESkey;
        private Label label6;
        private TextBox tb_SerIP;
        private TextBox tb_UserName;
        private Label label3;
        private Label label7;
        private TextBox tb_Message;
        private Label label8;
        private Button bt_Connect;
        private Button bt_Send;
        private RichTextBox tb_MessageLog;
    }
}