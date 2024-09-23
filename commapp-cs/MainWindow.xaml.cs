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

using System.Diagnostics;
using CommunicationsLib.MsgServices;
using CommunicationsLib.MsgParser;

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

        private string? imagePath = null;

        //private readonly string imgPth = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), System.IO.Path.DirectorySeparatorChar, "img.jpg");
        //private readonly string imgPthRec = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), System.IO.Path.DirectorySeparatorChar, "imgRec.jpg");

        private const string templateString = "Type message...";

        public Queue<string?> messageQueue = Receiver.messageQueue;

        public MainWindow()
        {
            InitializeComponent();

            messagesFlowDocument = new FlowDocument();

            RecMessBox.Document = messagesFlowDocument;

            messageReceiver = new(true);
            messageSender = new(true);


            messageReceiver.StartMessageConsumer();

            //messageSender.SendMessage(messageSender.ImageToStringConv(imgPth));
            //ConvertStringToImage(MessageConverter.ImageToStringConverter(imgPth));

            //messageSender.SendMessage(ImageConverter.ImageToStringConverter(imgPth));
            //ImageConverter.StringToImageConverter(ImageConverter.ImageToStringConverter(imgPth));

            UpdateMessagesInWindow();

        }

        //private void StringToImageConverter(string imgString)
        //{
        //    byte[] bytes = Convert.FromBase64String(imgString);
        //    using (var imgFile = new System.IO.FileStream(imgPthRec, System.IO.FileMode.OpenOrCreate))
        //    {
        //        imgFile.WriteAsync(bytes, 0, bytes.Length);
        //        imgFile.Flush();
        //    }
        //}

        private async void UpdateMessagesInWindow()
        {
            while (true)
            {

                if (messageQueue.Count > 0)
                {
                    Paragraph paragraphReceived = new Paragraph();

                    paragraphReceived.TextAlignment = TextAlignment.Left;

                    if (messageQueue.TryDequeue(out string? message))
                    {
                        MessageInfo? messageInfo = MessageJson.JsonDeserializeMessage(message);

                        // if message did not pass the json schema validation, it will not be shown at all for now
                        // TODO: write some error message about it
                        if (messageInfo == null)
                            continue;

                        bool uiAccess = RecMessBox.Dispatcher.CheckAccess();

                        if (uiAccess)
                        {
                            paragraphReceived.Inlines.Add(string.Concat(messageInfo.TimeSentDT.ToString("HH:mm"), " ID: ", messageInfo.SenderID, " -> ", messageInfo.MessageBody));
                            messagesFlowDocument.Blocks.Add(paragraphReceived);
                        }
                        else
                        {
                            RecMessBox.Dispatcher.Invoke(() =>
                            {
                                RecMessBox.AppendText($"{Environment.NewLine}{messageInfo.TimeSentDT.ToString("HH:mm")}  ID: {messageInfo.SenderID} -> {messageInfo.MessageBody} ");
                            });
                        }
                    }
                }
                await Task.Delay(200);
            }
        }

        private void ParseMessageToSenderClass(string textToSend)
        {
            if (textToSend.Length > 0)
            {
                Paragraph paragraphSent = new Paragraph();

                paragraphSent.TextAlignment = TextAlignment.Right;
                //paragraphSent.BorderBrush = Brushes.Orange;
                //paragraphSent.BorderThickness = new Thickness(5);
                paragraphSent.Inlines.Add(string.Concat("Wysłano: ", textToSend));

                messagesFlowDocument.Blocks.Add(paragraphSent);
                //RecMessBox.AppendText(string.Concat(Environment.NewLine,"Wysłano: ", textToSend));

                messageSender.SendMessage(textToSend);
            }
        }
        private void CheckAndParseMessages()
        {
            if (SendMessBox.Text.Length > 0 && SendMessBox.Text is not templateString)
            {
                ParseMessageToSenderClass(SendMessBox.Text.Trim());
                SendMessBox.Clear();
            }

            if (imagePath is not null)
            {
                ParseMessageToSenderClass(ImageConverter.ImageToStringConverter(imagePath));
                imagePath = null;
                AddedImageName.Content = null;
            }
        }

        private void SendMessButton_Click(object sender, RoutedEventArgs e)
        {
            CheckAndParseMessages();

            if (SendMessButton.IsKeyboardFocused)
            {
                while (!SendMessBox.Focus()) ;
            }
        }


        private void SendMessBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //ParseMessageToSenderClass(SendMessBox.Text.Trim());
                CheckAndParseMessages();
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

        //  if message is send/received, scroll to the end of textbox
        private void RecMessBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // blocking scrolling if user is manually scrolling to read older messages
            if (!RecMessBox.IsMouseOver)
            {
                RecMessBox.ScrollToEnd();
            }
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();

            // Filter files by extension
            dialog.Filter = "Image Files |*.jpg;*.jpeg;*.bmp;*.png";

            // Setting dialog options
            dialog.Multiselect = false;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Title = "Choose an image to send";

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Get image path
                imagePath = dialog.FileName;

                AddedImageName.Content = string.Concat("Image loaded: ", imagePath);
            }
        }

        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            imagePath = null;
            AddedImageName.Content = null;
        }
    }
}