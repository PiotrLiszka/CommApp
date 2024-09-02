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

        private const string templateString = "Type message...";


        public MainWindow()
        {
            InitializeComponent();

            messageReceiver = new(false);
            messageSender = new(false);

            messageReceiver.GetMessages();

            UpdateMessagesInWindow();

            Console.WriteLine(" Press [enter] to exit.");

        }

        private Task<MessageInfo?> JsonDeserializeMessage(string? jsonMessage)
        {
            MessageInfo? messageInfo = System.Text.Json.JsonSerializer.Deserialize<MessageInfo>(jsonMessage);
            return Task.FromResult(messageInfo);
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
                        if (messageQueue.TryDequeue(out string? message))
                        {
                            MessageInfo? messageInfo = await JsonDeserializeMessage(message);
                            //RecMessBox.AppendText("\n" + "Otrzymano: " + messageQueue.Dequeue());
                            RecMessBox.AppendText($"\n{messageInfo.TimeSend.ToShortTimeString()}  ID: {messageInfo.SenderID} -> {messageInfo.MessageBody} ");
                        }
                    }
                    else
                    {
                        if (messageQueue.TryDequeue(out string? message))
                        {
                            MessageInfo? messageInfo = await JsonDeserializeMessage(message);
                            //RecMessBox.Dispatcher.Invoke(() => { RecMessBox.AppendText("\n" + "Otrzymano: " + messageQueue.Dequeue()); });
                            RecMessBox.Dispatcher.Invoke(() =>
                            {
                                RecMessBox.AppendText($"\n{messageInfo.TimeSend.ToShortTimeString()}  ID: {messageInfo.SenderID} -> {messageInfo.MessageBody} ");
                            });
                        }
                    }
                }

                await Task.Delay(200);
            }
        }

        private void ParseMessageToSender()
        {
            string textToSend = SendMessBox.Text.Trim();

            if (textToSend.Length > 0)
            {
                RecMessBox.AppendText("\n" + "Wysłano: " + textToSend);
                messageSender.SendMessage(textToSend);
                SendMessBox.Clear();
            }
            else
            {
                SendMessBox.Clear();
            }
        }

        private void SendMessButton_Click(object sender, RoutedEventArgs e)
        {
            if (SendMessBox.Text.Length > 0 && SendMessBox.Text is not templateString)
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
            if (SendMessBox.Text.Trim().Length > 0 && SendMessBox.Text is templateString)
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
                SendMessBox.Text = templateString;
            }
        }
    }
}