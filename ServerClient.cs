using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IPZ_Lab2
{
    public static class ServerClient
    {
        // Налаштування мають співпадати з сервером!
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 8888;

        public static string SendRequest(string message)
        {
            try
            {
                // 1. Підключаємося до сервера
                TcpClient client = new TcpClient();
                client.Connect(SERVER_IP, SERVER_PORT);
                NetworkStream stream = client.GetStream();

                // 2. Відправляємо повідомлення (перетворюємо в байти)
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // 3. Отримуємо відповідь
                byte[] responseBuffer = new byte[256];
                int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);

                string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

                // 4. Закриваємо з'єднання
                client.Close();

                return response;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося з'єднатися з сервером!\n" + ex.Message, "Помилка мережі");
                return "ERROR";
            }
        }
    }
}