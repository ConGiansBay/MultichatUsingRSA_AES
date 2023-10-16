namespace MultiChat
{
    partial class Server
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tb_MessageLog = new RichTextBox();
            label4 = new Label();
            label2 = new Label();
            tb_SerRSAprikey = new TextBox();
            tb_SerRSAPubkey = new TextBox();
            label1 = new Label();
            bt_Listen = new Button();
            bt_StopListen = new Button();
            SuspendLayout();
            // 
            // tb_MessageLog
            // 
            tb_MessageLog.Location = new Point(12, 32);
            tb_MessageLog.Name = "tb_MessageLog";
            tb_MessageLog.Size = new Size(824, 418);
            tb_MessageLog.TabIndex = 21;
            tb_MessageLog.Text = "";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(499, 453);
            label4.Name = "label4";
            label4.Size = new Size(175, 19);
            label4.TabIndex = 4;
            label4.Text = "Server's RSA private key";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 453);
            label2.Name = "label2";
            label2.Size = new Size(171, 19);
            label2.TabIndex = 5;
            label2.Text = "Server's RSA public key";
            // 
            // tb_SerRSAprikey
            // 
            tb_SerRSAprikey.Location = new Point(499, 466);
            tb_SerRSAprikey.Multiline = true;
            tb_SerRSAprikey.Name = "tb_SerRSAprikey";
            tb_SerRSAprikey.ReadOnly = true;
            tb_SerRSAprikey.Size = new Size(481, 152);
            tb_SerRSAprikey.TabIndex = 18;
            // 
            // tb_SerRSAPubkey
            // 
            tb_SerRSAPubkey.Location = new Point(12, 466);
            tb_SerRSAPubkey.Multiline = true;
            tb_SerRSAPubkey.Name = "tb_SerRSAPubkey";
            tb_SerRSAPubkey.ReadOnly = true;
            tb_SerRSAPubkey.Size = new Size(481, 152);
            tb_SerRSAPubkey.TabIndex = 12;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(69, 19);
            label1.TabIndex = 9;
            label1.Text = "Message";
            // 
            // bt_Listen
            // 
            bt_Listen.Location = new Point(842, 32);
            bt_Listen.Name = "bt_Listen";
            bt_Listen.Size = new Size(132, 46);
            bt_Listen.TabIndex = 22;
            bt_Listen.Text = "Start";
            bt_Listen.UseVisualStyleBackColor = true;
            bt_Listen.Click += tb_Listen_Click;
            // 
            // bt_StopListen
            // 
            bt_StopListen.Enabled = false;
            bt_StopListen.Location = new Point(842, 84);
            bt_StopListen.Name = "bt_StopListen";
            bt_StopListen.Size = new Size(132, 46);
            bt_StopListen.TabIndex = 22;
            bt_StopListen.Text = "Stop";
            bt_StopListen.UseVisualStyleBackColor = true;
            bt_StopListen.Click += bt_StopListen_Click;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(986, 630);
            Controls.Add(bt_StopListen);
            Controls.Add(bt_Listen);
            Controls.Add(tb_MessageLog);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(tb_SerRSAprikey);
            Controls.Add(tb_SerRSAPubkey);
            Controls.Add(label1);
            Font = new Font("Times New Roman", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "Server";
            Text = "Server";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox tb_MessageLog;
        private Label label4;
        private Label label2;
        private TextBox tb_SerRSAprikey;
        private TextBox tb_SerRSAPubkey;
        private Label label1;
        private Button bt_Listen;
        private Button bt_StopListen;
    }
}