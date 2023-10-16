namespace MultiChat
{
    partial class Dashboard
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
            tb_Server = new Button();
            bt_Client = new Button();
            SuspendLayout();
            // 
            // tb_Server
            // 
            tb_Server.Location = new Point(29, 24);
            tb_Server.Name = "tb_Server";
            tb_Server.Size = new Size(116, 48);
            tb_Server.TabIndex = 0;
            tb_Server.Text = "Server";
            tb_Server.UseVisualStyleBackColor = true;
            tb_Server.Click += tb_Server_Click;
            // 
            // bt_Client
            // 
            bt_Client.Location = new Point(206, 24);
            bt_Client.Name = "bt_Client";
            bt_Client.Size = new Size(116, 48);
            bt_Client.TabIndex = 0;
            bt_Client.Text = "Client";
            bt_Client.UseVisualStyleBackColor = true;
            bt_Client.Click += bt_Client_Click;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(11F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(346, 106);
            Controls.Add(bt_Client);
            Controls.Add(tb_Server);
            Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Dashboard";
            Text = "Dashboard";
            ResumeLayout(false);
        }

        #endregion

        private Button tb_Server;
        private Button bt_Client;
    }
}