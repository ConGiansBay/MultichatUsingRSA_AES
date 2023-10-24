using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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

        //Danh sách khóa công khai với mỗi client được kết nối
        private ConcurrentDictionary<TcpClient, string> RSAPubKeys = new ConcurrentDictionary<TcpClient, string>();
        private ConcurrentDictionary<TcpClient, string> AESKeys = new ConcurrentDictionary<TcpClient, string>();
        private void CreateNewRSAKeys()
        {
            //lets take a new CSP with a new 2048 bit rsa key pair
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
            //how to get the private key
            RSAParameters privKey = csp.ExportParameters(true);
            string privKeyString;
            //and the public key ...
            RSAParameters pubKey = csp.ExportParameters(false);
            //converting the public key into a string representation
            string pubKeyString;
            {
                //we need some buffer
                var sw = new StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, pubKey);
                //get the string from the stream
                pubKeyString = sw.ToString();
                tb_SerRSAPubkey.Text = pubKeyString;
            }
            {
                //we need some buffer
                var sw = new StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, privKey);
                //get the string from the stream
                privKeyString = sw.ToString();
                tb_SerRSAprikey.Text = privKeyString;
            }
        }
        public string RSAEncrypt(string strText, string publicKey)
        {
            //Import Key from Server
            var textData = Encoding.UTF8.GetBytes(strText);
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    rsa.FromXmlString(publicKey);
                    var encryptedData = rsa.Encrypt(textData, true);
                    var base64Encrypted = Convert.ToBase64String(encryptedData);
                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
        public string RSADecrypt(string strText)
        {

            var privateKey = tb_SerRSAprikey.Text;
            var testData = Encoding.UTF8.GetBytes(strText);
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    var base64Encrypted = strText;
                    rsa.FromXmlString(privateKey);
                    var resultBytes = Convert.FromBase64String(base64Encrypted);
                    var decryptedBytes = rsa.Decrypt(resultBytes, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedData.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
        private string AESEncrypt(string plainText, string strKey)
        {
            byte[] Key = Encoding.ASCII.GetBytes(strKey);
            string IV;

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.GenerateIV();
                IV = Convert.ToBase64String(aesAlg.IV);
                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            string encryptedString = Convert.ToBase64String(encrypted);

            //Combine IV and message
            encryptedString = IV + "|" + encryptedString;
            return encryptedString;
        }
        private string AESDecrypt(string cipherText, string strKey)
        {
            byte[] Key = Encoding.ASCII.GetBytes(strKey);
            string IV = cipherText.Split('|')[1];
            cipherText = cipherText.Split("|")[2];

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = Encoding.ASCII.GetBytes(IV);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
        public Server()
        {
            InitializeComponent();
        }
        private async void AcceptClients()
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
                    stream.Flush();
                    lock (userNames)
                    {
                        userNames[client] = userName;
                    }
                    Log($"[NEW USER] - {userName} connected from: {client.Client.RemoteEndPoint}.");

                    //Đọc khóa RSA công khai
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string pubKey = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    stream.Flush();
                    lock (RSAPubKeys) 
                    { 
                        RSAPubKeys[client] = pubKey;
                    }

                    //Gửi RSA public của server tới tất cả client
                    await SendMessageToAllClients(tb_SerRSAPubkey.Text, 0);

                    //Đọc khóa AES được client gửi tới
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string aesKey = RSADecrypt(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    AESKeys.TryAdd(client, aesKey);

                    await SendMessageToAllClients($"[NEW USER] - {userName} is connected.",2 );

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

                        //Giải mã thông điệp
                        receivedData = AESDecrypt(receivedData, AESKeys[client]);

                        //In ra màn hình
                        Log($"{receivedData}");
                        //Gửi tới các client khác nữa
                        SendMessageToAllClients($"{receivedData}",2);
                    }
                }
                //Xoá Client khỏi ds
                lock (connectedClients)
                {
                    connectedClients.Remove(client);
                }
                lock (RSAPubKeys)
                {
                    RSAPubKeys.Remove(client,out var key);
                }
                lock (AESKeys)
                {
                    AESKeys.Remove(client, out var key);
                }
                if (client != null && client.Client != null)
                {
                    Log($"Client disconnected: {client.Client.RemoteEndPoint}\n");
                    stream.Close();
                }
            }

        }
        private async Task SendMessageToAllClients(string message, int type)
        {
            //type == 0: no encrypt; 1: RSA; 2: AES
            if(type == 0)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                foreach (TcpClient client in RSAPubKeys.Keys)
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(buffer, 0,buffer.Length);
                }
            }
            if(type == 2)
            {
                foreach (TcpClient client in AESKeys.Keys)
                {
                    message = AESEncrypt(message, AESKeys[client]);
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
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

            //Tạo khóa
            CreateNewRSAKeys();
        }

        private void bt_StopListen_Click(object sender, EventArgs e)
        {
            bt_Listen.Enabled = true;
            bt_StopListen.Enabled = false;

            isServerRunning = false;
            //Dừng máy chủ
            server.Stop();


            //test
            // Ngắt kết nối với tất cả các client
            if (connectedClients.Count > 0)
            {
                lock (connectedClients)
                {
                    // Duyệt qua danh sách các client và ngắt kết nối với mỗi client
                    foreach (TcpClient client in connectedClients)
                    {
                        client.Close();
                    }
                }
            }
            //Xoá danh sách các Clients đang kết nối
            connectedClients.Clear();

            userNames.Clear();
            RSAPubKeys.Clear();
            AESKeys.Clear();
            tb_SerRSAprikey.Clear();
            tb_SerRSAPubkey.Clear();
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
