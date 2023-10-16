using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiChat
{
    public partial class Server : Form
    {
        private TcpListener server;
        private Thread listenThread;
        private List<TcpClient> connectedClients = new List<TcpClient>();

        //Danh sách lưu tên người dùng với mỗi client được kết nối
        private ConcurrentDictionary<TcpClient, string> userNames = new ConcurrentDictionary<TcpClient, string>();
        //Dùng để kiểm tra xem Server có đang chạy hay không
        private bool isServerRunning;

        public Server()
        {
            InitializeComponent();
        }
        private void AcceptClients()
        {
            while (isServerRunning)
            {
                try
                {
                    //Chấp nhận kết nối từ Client
                    TcpClient client = server.AcceptTcpClient();

                    // Thêm client vào danh sách
                    lock (connectedClients)
                    {
                        connectedClients.Add(client);
                    }
                    //Đọc tên người dùng
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string userName = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    userNames[client] = userName;
                    Log($"[NEW USER] - {userName} connected from: {client.Client.RemoteEndPoint}.");
                    SendMessageToAllClients(client, $"[NEW USER] - {userName} is connected.");

                    //Tạo thread mới để nhận tin nhắn từ Client vừa kết nối
                    Thread receiveMessagesThread = new Thread(() => ReceiveMessage(client));
                    receiveMessagesThread.Start();
                }
                catch (Exception ex)
                {
                    if (!isServerRunning)
                    {
                        break;
                    }
                    Log($"Error accepting client: {ex.Message}");
                    continue;
                }
            }
        }
        private void ReceiveMessage(TcpClient client)
        {
            //Tạo 1 stream để đọc và ghi tin nhắn gửi từ Client
            using (NetworkStream stream = client.GetStream())
            {
                //Mảng này để đọc dữ liệu từ stream đó
                byte[] buffer = new byte[1024];

                //Nếu vẫn còn kết nối thì vẫn nhận tin nhắn
                while (client.Connected && client != null && stream != null)
                {
                    if (isServerRunning == false)
                    {
                        break;
                    }
                    if (stream.DataAvailable)
                    {
                        //Đọc dữ liệu từ stream vào mảng byte
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        //Chuyển dữ liệu đọc thành chuỗi
                        string userName = userNames[client];
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        
                         //In ra màn hình
                         Log($"{receivedData}");
                         //Gửi tới các client khác nữa
                         SendMessageToAllClients(client, $"{receivedData}");
                    }
                }

                //Xoá Client khỏi ds
                lock (connectedClients)
                {
                    connectedClients.Remove(client);
                }
                if (client != null && client.Client != null)
                {
                    Log($"Client disconnected: {client.Client.RemoteEndPoint}\n");
                    stream.Close();
                }
            }

        }
        private async Task SendMessageToAllClients(TcpClient senderClient, string message)
        {
            //Chuyển tin nhắn thành mảng byte
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            //Duyệt qua tất cả các Client trong list để gửi tin nhắn
            foreach (TcpClient client in userNames.Keys)
            {
                NetworkStream stream = client.GetStream();
                 await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
        private void Log(string message)
        {
            if (tb_MessageLog.InvokeRequired)
            {
                tb_MessageLog.Invoke(new Action<string>(Log), message);
            }
            else
            {
                //serverTextBox.AppendText(message);
                string[] messageParts = message.Split(':');
                if (messageParts.Length == 2)
                {
                    Font boldFont = new Font(tb_MessageLog.Font, FontStyle.Bold);
                    tb_MessageLog.SelectionFont = boldFont;
                    tb_MessageLog.AppendText(messageParts[0]);

                    Font ItalicFont = new Font(tb_MessageLog.Font, FontStyle.Italic);
                    tb_MessageLog.SelectionFont = ItalicFont;
                    tb_MessageLog.AppendText($": ({DateTime.Now})");

                    tb_MessageLog.SelectionFont = tb_MessageLog.Font; // Đặt lại font gốc
                    tb_MessageLog.AppendText(": " + messageParts[1] + "\n");
                }
                else
                {
                    tb_MessageLog.AppendText(message + "\n");
                }
            }
        }

        private void tb_Listen_Click(object sender, EventArgs e)
        {
            int port = 8080;
            server = new TcpListener(IPAddress.Any, port);
            //Server bắt đầu lắng nghe
            server.Start();
            isServerRunning = true;

            //Lấy địa chỉ ip hiện tại của pc để làm máy chủ
            string ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                     .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                     ?.ToString();

            Log($"Server started on {ipAddress}:{port}\n");

            //Tạo thread để chấp nhận kết nối từ các Client
            listenThread = new Thread(AcceptClients);
            listenThread.Start();

            //Ẩn/hiện các button
            bt_Listen.Enabled = false;
            bt_StopListen.Enabled = true;
        }

        private void bt_StopListen_Click(object sender, EventArgs e)
        {
            bt_Listen.Enabled = true;
            bt_StopListen.Enabled = false;

            isServerRunning = false;
            //Dừng máy chủ
            server.Stop();
            //Xoá danh sách các Clients đang kết nối
            connectedClients.Clear();


            //test
            // Ngắt kết nối với tất cả các client
            foreach (TcpClient client in userNames.Keys)
            {
                if (isServerRunning)
                {
                    //client.Close();
                    NetworkStream stream = client.GetStream();
                    stream.Close();
                    client.Close();
                }
            }

            Log("Server stopped\n");
        }
        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hiển thị cảnh báo yes/no
            DialogResult result = MessageBox.Show("Are you sure you want to close the server?");

            // Nếu người dùng chọn Yes
            if (result == DialogResult.Yes)
            {
                // Ngắt kết nối với tất cả các client
                if (connectedClients.Count > 0)
                {
                    lock (connectedClients)
                    {
                        List<TcpClient> clientsCopy = new List<TcpClient>(connectedClients);
                        // Duyệt qua danh sách các client và ngắt kết nối với mỗi client
                        foreach (TcpClient client in connectedClients)
                        {
                            client.Close();
                        }
                    }
                }
                if (isServerRunning)
                {
                    MessageBox.Show("Vui lòng dừng lắng nghe trước khi đóng Server!");
                    e.Cancel = true;
                }
            }
            else
            {
                // Hủy đóng form
                e.Cancel = true;
            }
        }
    }
}
