using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MultiChat
{
    public partial class Client : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private bool isConnectedToServer = false;
        private string userName;
        private bool isLogin;
        private bool allIsDisposed;
        public Client()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void ReceiveMessage()
        {
            if (!isConnectedToServer)
            {
                // Đưa ra thông báo không tìm thấy máy chủ trên RichTextBox
                AppendMessage("SERVER NOT FOUND");
                return;
            }
            //lặp với điều kiện có kết nối tới Server rồi
            while (client.Connected)
            {
                string receivedData = ReadMessage(stream);
                //string fileName = Path.GetFileName(attachmentFilePath);
                if (receivedData != null)
                {
                    AppendMessage(receivedData);
                }
                else
                {
                    AppendMessage("Server disconnected.");
                    break;
                }
            }
        }

        private void bt_Connect_Click(object sender, EventArgs e)
        {
            if (bt_Connect.Text == "Connect")
            {
                //Kiểm tra địa chỉ IP
                IPAddress ip;
                string IPadd = tb_SerIP.Text.Trim();
                if (!IPAddress.TryParse(IPadd, out ip))
                {
                    MessageBox.Show("Wrong IP address!");
                    return;
                }
                if (!string.IsNullOrEmpty(tb_UserName.Text))
                {
                    try
                    {
                        //Tạo kết nối tới server
                        client = new TcpClient(IPadd, 8080);
                        //dùng để ghi và đọc dữ liệu từ kết nối
                        stream = client.GetStream();
                        //Tạo luồng
                        //Sử dụng ReceiveMessage để có thể đọc message được gửi từ server và hiển thị
                        receiveThread = new Thread(ReceiveMessage);
                        //Thực thi luồng
                        receiveThread.Start();
                        // Thiết lập trạng thái kết nối
                        isConnectedToServer = true;
                        // lấy tên người dùng ra và gửi tới Server
                        userName = tb_UserName.Text;
                        tb_UserName.Font = new Font(tb_UserName.Font, FontStyle.Bold);
                        // Gửi tên người dùng tới server
                        SendMessage($"{userName}");

                        // Vô hiệu hóa nút "Join" và cho phép gửi tin sau khi kết nối thành công
                        bt_Connect.Text = "Disconnect";
                        bt_Send.Enabled = true;
                        tb_Message.ReadOnly = false;
                        tb_UserName.ReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        AppendMessage("[ERROR] " + ex.Message);
                        // Đưa ra thông báo không tìm thấy máy chủ trên RichTextBox
                        AppendMessage("SERVER NOT FOUND");
                        isConnectedToServer = false;
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a username.");
                }
                isLogin = true;
            }
            else
            {
                isLogin = false;
                Task.Run(() =>
                {
                    SendMessage($"{tb_UserName.Text} is disconected");
                    this.Invoke((Action)(() =>
                    {
                        tb_UserName.Enabled = true;
                        tb_UserName.ReadOnly = true;
                        tb_UserName.Clear();
                        tb_Message.ReadOnly = false;
                        
                        bt_Send.Enabled = false;
                        bt_Connect.Text = "Connect";

                    }));
                    if (stream != null && stream.CanRead)
                    {
                        ReadMessage(stream);
                    }
                    if (client != null && client.Connected)
                    {
                        client.Close();
                    }
                    if (stream != null)
                    {
                        stream.Close();
                    }
                });
            }
        }

        private void bt_Send_Click(object sender, EventArgs e)
        {
            string message = tb_Message.Text;
            userName = tb_UserName.Text;
            tb_UserName.Font = new Font(tb_UserName.Font, FontStyle.Bold);


            SendMessage($"{userName}: {message}");

        }
        private void SendMessage(string message)
        {
            if (client != null && client.Connected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }
        private string ReadMessage(NetworkStream stream)
        {
            if (!stream.CanRead || !client.Connected)
            {
                return null;
            }

            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
            }
            catch (IOException)
            {
                // Xử lý ngoại lệ IOException khi kết nối bị đóng
                return null;
            }

            if (bytesRead > 0)
            {
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            else
            {
                return null;
            }
        }
        private void AppendMessage(string message)
        {
            //check coi đúng luồng không, nếu ở luồng khác, sử dụng Invoke để gọi lại AppendMessage trên luồng chính
            if (tb_MessageLog.InvokeRequired)
            {
                tb_MessageLog.Invoke(new Action<string>(AppendMessage), message);
            }
            else
            {
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
        private void CloseForm(object sender, FormClosingEventArgs e)
        {
            if (isLogin)
            {
                MessageBox.Show("Vui lòng đăng xuất trước khi đóng form!");
                e.Cancel = true;
            }
            else
            {
                if (client != null && client.Connected)
                {
                    client.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
            allIsDisposed = true;
        }

    }
}