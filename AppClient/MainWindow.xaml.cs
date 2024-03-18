using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;

namespace AppClient
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private StreamWriter writer;
        private string clientName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new TcpClient();
                client.Connect("192.168.1.69", 27001);
                writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

                clientName = clientNameTxt.Text;
                writer.WriteLine(clientName);

                clientNameTxt.Text = "";

                ReceiveMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}");
            }
        }

        private async void ReceiveMessages()
        {
            try
            {
                using (StreamReader reader = new StreamReader(client.GetStream()))
                {
                    while (true)
                    {
                        string message = await reader.ReadLineAsync();

                        int index = message.IndexOf(':');
                        if (index != -1 && index < message.Length - 1)
                        {
                            string senderName = message.Substring(0, index);
                            string actualMessage = message.Substring(index + 1);
                            Application.Current.Dispatcher.Invoke(() => messageListBox.Items.Add($"{senderName} - [{actualMessage.TrimStart()}]")); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => MessageBox.Show($"Error receiving message: {ex.Message}")); 
            }
        }
    }
}
