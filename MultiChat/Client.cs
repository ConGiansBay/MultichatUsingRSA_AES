using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Cryptography;
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

        private string serPubKey;
        public Client()
        {
            InitializeComponent();
        }
        //RSA
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
                tb_ClientRSApubkey.Text = pubKeyString;
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
                tb_ClientRSAprikey.Text = privKeyString;
            }
        }
        public string RSAEncrypt(string strText)
        {
            var textData = Encoding.UTF8.GetBytes(strText);
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    rsa.FromXmlString(serPubKey.ToString());
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
            var privateKey = tb_ClientRSAprikey.Text;
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
        //AES
        private void createNewAESKey()
        {
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.KeySize = 128;
                aesAlgorithm.GenerateKey();
                string keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);
                tb_AESkey.Text = keyBase64;
            }
        }
        private string AESEncrypt(string plainText) 
        {
            byte[] Key = Encoding.ASCII.GetBytes(tb_AESkey.Text);
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
        private string AESDecrypt(string cipherText)
        {
            byte[] Key = Encoding.ASCII.GetBytes(tb_AESkey.Text);
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
                if (receivedData != null)
                {
                    if (receivedData.Contains("<?xml")) { serPubKey = receivedData; AppendSerPubKey(receivedData); }

                    else
                        if (receivedData.Contains("|"))
                            AppendMessage(AESDecrypt(receivedData));
                    else AppendMessage(RSADecrypt(receivedData));
                }
                else
                {
                    AppendMessage("Server disconnected.");
                    break;
                }
            }
        }

        private async void bt_Connect_Click(object sender, EventArgs e)
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
                        receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                        //Thực thi luồng
                        receiveThread.Start();
                        // Thiết lập trạng thái kết nối
                        isConnectedToServer = true;

                        // lấy tên người dùng ra và gửi tới Server
                        userName = tb_UserName.Text;
                        tb_UserName.Font = new Font(tb_UserName.Font, FontStyle.Bold);
                        // Gửi tên người dùng tới server
                        SendMessage($"{userName}",0);
                        //Tạo khóa RSA
                        CreateNewRSAKeys();
                        createNewAESKey();
                        //Gửi khóa Public
                        SendMessage(tb_ClientRSApubkey.Text,0);
                        //chờ đến khi display server's public key. cho đến lúc đó không được gửi khóa AES

                        SendMessage(tb_AESkey.Text, 2);

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
                    SendMessage($"{tb_UserName.Text} is disconected",1);
                    this.Invoke((Action)(() =>
                    {
                        tb_UserName.Enabled = true;
                        tb_UserName.ReadOnly = false;
                        tb_Message.ReadOnly = true;

                        bt_Send.Enabled = false;
                        bt_Connect.Text = "Connect";

                        tb_SerRSAPubkey.Clear();
                        tb_ClientRSAprikey.Clear();
                        tb_ClientRSApubkey.Clear();
                        tb_AESkey.Clear();

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


            SendMessage($"{userName}: {message}",0);

        }
        private void SendMessage(string message, int method)
        {
            //method=0: no encrypt; method=1: AES
            if (method == 1) message = AESEncrypt(message);
            if (method == 2) message = RSAEncrypt(message);
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
        private void AppendSerPubKey(string message)
        {
            //check coi đúng luồng không, nếu ở luồng khác, sử dụng Invoke để gọi lại AppendMessage trên luồng chính
            if (tb_SerRSAPubkey.InvokeRequired)
            {
                tb_SerRSAPubkey.Invoke(new Action(()=> AppendSerPubKey(message)));
            }
            else
            {

                tb_SerRSAPubkey.AppendText(message + "\n");

            }
        }
        private void AppendMessage(string message)
        {
            //check coi đúng luồng không, nếu ở luồng khác, sử dụng Invoke để gọi lại AppendMessage trên luồng chính
            if (tb_MessageLog.InvokeRequired)
            {
                tb_MessageLog.Invoke(new Action(()=> AppendMessage(message)));
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