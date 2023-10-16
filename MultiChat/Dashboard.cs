using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiChat
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void tb_Server_Click(object sender, EventArgs e)
        {
            Server server = new Server();
            server.Show();
        }

        private void bt_Client_Click(object sender, EventArgs e)
        {
            Client client = new Client();
            client.Show();
        }
    }
}
