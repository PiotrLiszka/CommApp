using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunicationsLib;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Threading;
using System.Threading.Tasks;

namespace commappcs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Sender messageSender;
        private Receiver messageReceiver;
        public Queue<string?> messageQueue = Receiver.messageQueue;
        
        public MainWindow()
        {
            InitializeComponent();

            messageReceiver = new(true);
            messageSender = new(true);

            messageReceiver.GetMessages();

            UpdateMessagesInWindow();

            Console.WriteLine(" Press [enter] to exit.");

        }

        private async void UpdateMessagesInWindow()
        {
            while (true)
            {
                bool uiAccess = RecMessBox.Dispatcher.CheckAccess();

                if (messageQueue.Count > 0)
                {
                    if (uiAccess)
                    {
                        RecMessBox.AppendText("\n" + "Otrzymano: " + messageQueue.Dequeue());
                    }
                    else
                    {
                        RecMessBox.Dispatcher.Invoke(() => { RecMessBox.AppendText("\n" + "Otrzymano: " + messageQueue.Dequeue()); });
                    }
                }

                await Task.Delay(250);
            }
        }

        private void ParseMessageToSender()
        {
            string textToSend = SendMessBox.Text.Trim();

            if (textToSend.Length > 0)
            {
                messageSender.SendMessage(textToSend);
                RecMessBox.AppendText("\n" + "Wysłano: " + textToSend);
                SendMessBox.Clear();
            }
            else
            {
                SendMessBox.Clear();
            }
        }

        private void SendMessButton_Click(object sender, RoutedEventArgs e)
        {
            if (SendMessBox.Text.Length > 0)
            {
                ParseMessageToSender();
            }

            if (SendMessButton.IsKeyboardFocused)
            {
                while (!SendMessBox.Focus()) ;
            }
        }

        private void SendMessBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (SendMessBox.Text.Trim().Length > 0 && SendMessBox.Text == "Type message...")
            {
                SendMessBox.Clear();
                SendMessBox.Foreground = Brushes.Black;
            }
;       }

        private void SendMessBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ParseMessageToSender();
            }
        }

        private void SendMessBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (SendMessBox.Text.Trim().Length == 0)
            {
                SendMessBox.Foreground = Brushes.Gray;
                SendMessBox.Text = "Type message...";
            }
        }
    }
}