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

using System.Diagnostics;

namespace commappcs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Sender messageSender;
        private Receiver messageReceiver;
        private FlowDocument messagesFlowDocument;

        private readonly string imgPth = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), System.IO.Path.DirectorySeparatorChar, "img.jpg");
        private readonly string imgPthRec = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), System.IO.Path.DirectorySeparatorChar, "imgRec.jpg");

        private const string templateString = "Type message...";

        public Queue<string?> messageQueue = Receiver.messageQueue;

        public MainWindow()
        {
            InitializeComponent();

            messagesFlowDocument = new FlowDocument();

            RecMessBox.Document = messagesFlowDocument;

            //paragraphSent = new Paragraph();
            //paragraphReceived = new Paragraph();


            messageReceiver = new(false);
            messageSender = new(false);

            messageReceiver.GetMessages();

            //messageSender.SendMessage(messageSender.ImageToStringConv(imgPth));
            //ConvertStringToImage(MessageConverter.ImageToStringConverter(imgPth));

            UpdateMessagesInWindow();

        }

        private MessageInfo? JsonDeserializeMessage(string? jsonMessage)
        {
            return System.Text.Json.JsonSerializer.Deserialize<MessageInfo>(jsonMessage);
        }

        private void ConvertStringToImage(string imgString)
        {
            byte[] bytes = Convert.FromBase64String(imgString);
            using (var imgFile = new System.IO.FileStream(imgPthRec, System.IO.FileMode.OpenOrCreate))
            {
                imgFile.WriteAsync(bytes, 0, bytes.Length);
                imgFile.Flush();
            }
        }

        private async void UpdateMessagesInWindow()
        {
            while (true)
            {
                bool uiAccess = RecMessBox.Dispatcher.CheckAccess();

                if (messageQueue.Count > 0)
                {
                    Paragraph paragraphReceived = new Paragraph();

                    //paragraphReceived.Background = Brushes.LightBlue;
                    paragraphReceived.TextAlignment = TextAlignment.Left;

                    if (uiAccess)
                    {
                        if (messageQueue.TryDequeue(out string? message))
                        {
                            MessageInfo? messageInfo = JsonDeserializeMessage(message);
                            //RecMessBox.AppendText($"{Environment.NewLine}{messageInfo.TimeSend.ToShortTimeString()}  ID: {messageInfo.SenderID} -> {messageInfo.MessageBody} ");

                            paragraphReceived.Inlines.Add(string.Concat(messageInfo.TimeSend.ToShortTimeString(), " ID: ", messageInfo.SenderID, " -> " , messageInfo.MessageBody));
                            messagesFlowDocument.Blocks.Add(paragraphReceived);
                        }
                    }
                    else
                    {
                        if (messageQueue.TryDequeue(out string? message))
                        {
                            MessageInfo? messageInfo = JsonDeserializeMessage(message);
                            RecMessBox.Dispatcher.Invoke(() =>
                            {
                                RecMessBox.AppendText($"{Environment.NewLine}{messageInfo.TimeSend.ToShortTimeString()}  ID: {messageInfo.SenderID} -> {messageInfo.MessageBody} ");
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

            Paragraph paragraphSent = new Paragraph();

            if (textToSend.Length > 0)
            {
                //paragraphSent.Background = Brushes.OrangeRed;

                paragraphSent.TextAlignment = TextAlignment.Right;
                //paragraphSent.BorderBrush = Brushes.Orange;
                //paragraphSent.BorderThickness = new Thickness(5);
                paragraphSent.Inlines.Add(string.Concat("Wysłano: ", textToSend));

                messagesFlowDocument.Blocks.Add(paragraphSent);
                //RecMessBox.AppendText(string.Concat(Environment.NewLine,"Wysłano: ", textToSend));

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

        private void SendMessBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ParseMessageToSender();
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

        private void SendMessBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (SendMessBox.Text.Trim().Length == 0)
            {
                SendMessBox.Foreground = Brushes.Gray;
                SendMessBox.Text = templateString;
            }
        }

        private void RecMessBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!RecMessBox.IsMouseOver)
            {
                RecMessBox.ScrollToEnd();
            }
        }
    }
}