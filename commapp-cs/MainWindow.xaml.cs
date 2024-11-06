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
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Reflection;


namespace commappcs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Sender messageSender;
        private Receiver messageReceiver;
        private FlowDocument messagesFlowDocument;

        private readonly Dictionary<string, MessageTabContent> openFriendTabs; // --- nowe
        private readonly MainWindowHandler MainWindowMethods;

        private string? imagePath = null;

        private const string templateString = "Type message...";

        public Queue<string> messageQueue;


        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            string currentUser = string.Empty;

            /// mainwindow visual and other options
            /// 

            messagesFlowDocument = new FlowDocument();
            RecMessBox.Document = messagesFlowDocument;
            SendMessBox.Foreground = Brushes.Gray;

            /// show loginwindow with main window in background and not active
            ///
            this.Show();

            LoginWindow login = new LoginWindow()
            {
                Owner = this
            };

            /// TODO: user and pass authentication
            /// for now sets user name for testing
            ///
            switch (login.ShowDialog())
            {
                case true:
                    currentUser = login.GetLoginName();
                    this.Title = string.Concat(this.Title, " - ", currentUser);
                    break;
                case false:
                    this.Close();
                    break;
            }

            MainWindowMethods = new MainWindowHandler(ref SendMessBox, ref AddedImageName, ref MessageTabs, currentUser);
            SendMessBox.Text = MainWindowMethods.GetTemplateString;

            FriendsList.SelectionMode = SelectionMode.Single;

            messageQueue = Receiver.messageQueue;

            /// helper dictionary to get into the RichTextBox objects of opened UI message tabs
            ///
            openFriendTabs = new Dictionary<string, MessageTabContent>();

            /// starting messaging services
            ///
            messageReceiver = new(currentUser, true);
            //messageSender = new(true);
            messageReceiver.StartMessageConsumer((model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageType = ea.BasicProperties.ContentType;
                var message = Encoding.UTF8.GetString(body);

                var routingKey = ea.RoutingKey;

                Debug.WriteLine($"{message}");

                //messageQueue.Enqueue(message);
                //messageQueue.Enqueue(messageType);    // delete it

                void WriteMessagePlz()
                {
                    MessageInfo? messageInfo = MessageJson.JsonDeserializeMessage(message);

                    if (messageInfo == null)
                        return;

                    //Debug.WriteLine($"W invoke: {routingKey}");

                    Border bdrBubble = new Border();
                    {
                        bdrBubble.BorderThickness = new Thickness(2);
                        bdrBubble.BorderBrush = Brushes.Black;
                        bdrBubble.CornerRadius = new CornerRadius(20);
                        bdrBubble.Padding = new Thickness(10);
                        bdrBubble.Background = new LinearGradientBrush(Color.FromRgb(48, 138, 199), Color.FromRgb(62, 177, 255), new Point(0, 1), new Point(0, 0));
                    }

                    TextBlock txtBubble = new TextBlock();
                    {
                        txtBubble.TextWrapping = TextWrapping.Wrap;
                        txtBubble.Background = Brushes.Transparent;
                        txtBubble.Foreground = Brushes.White;
                        txtBubble.ClipToBounds = true;
                        txtBubble.TextAlignment = TextAlignment.Left;
                        txtBubble.MaxWidth = RecMessBox.ViewportWidth / 3 * 2;
                    }

                    bdrBubble.Child = txtBubble;
                    txtBubble.Text = string.Concat(messageInfo.TimeSentDT.ToString("HH:mm"), " ID: ", messageInfo.SenderID, " -> ", messageInfo.MessageBody);

                    Paragraph receivedParagraph = new Paragraph();
                    receivedParagraph.TextAlignment = TextAlignment.Left;
                    receivedParagraph.Inlines.Add(bdrBubble);

                    TabItem currentTab = (TabItem)MessageTabs.Items.GetItemAt(MessageTabs.SelectedIndex);

                    //int incomingMessTabId = MessageTabs.Items.IndexOf(openFriendTabs[routingKey].Item);
                    //if (incomingMessTabId == -1)
                    //{
                    //    // jeśli przyjdzie wiadomość od nieznajomego then .......

                    //    throw new NotImplementedException();
                    //}
                    //else
                    //{
                        
                    //}

                    //Debug.WriteLine($"ID OF TAB: {MessageTabs.Items.IndexOf(openFriendTabs[routingKey].Item)}");

                    RichTextBox currentTabTextBox = (RichTextBox)currentTab.Content;
                    currentTabTextBox.Document.Blocks.Add(receivedParagraph);
                }
                //
                //
                //     DO KONTYNUACJI!!
                //
                //

                MessageTabs.Dispatcher.InvokeAsync(WriteMessagePlz);
            });

            //UpdateMessagesInWindow();
        }

        private async void UpdateMessagesInWindow() /// !!! TODO: fix this method !!!
        {
            while (true)
            {

                if (messageQueue.Count > 0)
                {
                    Paragraph paragraphReceived = new Paragraph();

                    paragraphReceived.TextAlignment = TextAlignment.Left;

                    if (messageQueue.TryDequeue(out string? message))
                    {
                        if (messageQueue.TryDequeue(out string? messageType))
                        {
                            if (message == null)
                                continue;

                            MessageInfo? messageInfo = MessageJson.JsonDeserializeMessage(message);

                            // if message did not pass the json schema validation, it will not be shown at all for now
                            // TODO: write some error message about it
                            if (messageInfo == null)
                                continue;

                            bool uiAccess = RecMessBox.Dispatcher.CheckAccess();

                            if (uiAccess)
                            {
                                if (messageType == "text/plain")
                                {
                                    Border bdrBubble = new Border();
                                    {
                                        bdrBubble.BorderThickness = new Thickness(2);
                                        bdrBubble.BorderBrush = Brushes.Black;
                                        bdrBubble.CornerRadius = new CornerRadius(20);
                                        bdrBubble.Padding = new Thickness(10);
                                        bdrBubble.Background = new LinearGradientBrush(Color.FromRgb(48, 138, 199), Color.FromRgb(62, 177, 255), new Point(0, 1), new Point(0, 0));
                                    }

                                    TextBox txtBubble = new TextBox();
                                    {
                                        txtBubble.TextWrapping = TextWrapping.Wrap;
                                        txtBubble.Background = Brushes.Transparent;
                                        txtBubble.Foreground = Brushes.White;
                                        txtBubble.BorderThickness = new Thickness(0);
                                        txtBubble.ClipToBounds = true;
                                        txtBubble.TextAlignment = TextAlignment.Left;
                                        txtBubble.MaxWidth = RecMessBox.ViewportWidth / 3 * 2;
                                    }

                                    bdrBubble.Child = txtBubble;
                                    txtBubble.Text = string.Concat(messageInfo.TimeSentDT.ToString("HH:mm"), " ID: ", messageInfo.SenderID, " -> ", messageInfo.MessageBody);

                                    paragraphReceived.Inlines.Add(bdrBubble);
                                    messagesFlowDocument.Blocks.Add(paragraphReceived);

                                    //paragraphReceived.Inlines.Add(string.Concat(messageInfo.TimeSentDT.ToString("HH:mm"), " ID: ", messageInfo.SenderID, " -> ", messageInfo.MessageBody));
                                    //messagesFlowDocument.Blocks.Add(paragraphReceived);
                                }
                                else
                                {
                                    if (messageInfo.MessageBody == null || messageType == null)
                                        continue;

                                    ImageConverter.StringToImageConverter(messageInfo.MessageBody, messageType);
                                    paragraphReceived.Inlines.Add(string.Concat(messageInfo.TimeSentDT.ToString("HH:mm"), " ID: ", messageInfo.SenderID, " ->  Image: ", messageType));
                                    messagesFlowDocument.Blocks.Add(paragraphReceived);
                                }
                            }
                            else
                            {
                                //RecMessBox.Dispatcher.Invoke(() =>
                                //{
                                //    RecMessBox.AppendText($"{Environment.NewLine}{messageInfo.TimeSentDT.ToString("HH:mm")}  ID: {messageInfo.SenderID} -> {messageInfo.MessageBody} ");
                                //});
                            }
                        }
                    }
                }
                await Task.Delay(200);
            }
        }

        //public void ParseMessageToSenderClass(string textToSend, string messageType)
        //{
        //    if (textToSend.Length > 0)
        //    {
        //        Paragraph paragraphSent = new Paragraph();
        //            paragraphSent.TextAlignment = TextAlignment.Right;

        //        if (messageType == "text")
        //            paragraphSent.Inlines.Add(string.Concat("Wysłano: ", textToSend));
        //        else
        //            // test to see if images are read correctly -> messageType = file extension
        //            paragraphSent.Inlines.Add(string.Concat("Wysłano: ", messageType));

        //        messagesFlowDocument.Blocks.Add(paragraphSent);
        //        //RecMessBox.AppendText(string.Concat(Environment.NewLine,"Wysłano: ", textToSend));

        //        messageSender.SendMessage(textToSend, messageType);
        //    }
        //}

        //private void CheckAndParseMessages()
        //{
        //    string txtToSend;
        //    string messageType;

        //    if (SendMessBox.Text.Length > 0 && SendMessBox.Text is not templateString)
        //    {
        //        txtToSend = SendMessBox.Text.Trim();
        //        messageType = "text";
        //        ParseMessageToSenderClass(ref txtToSend, ref messageType);
        //        SendMessBox.Clear();
        //    }

        //    if (imagePath is not null)
        //    {
        //        txtToSend = ImageConverter.ImageToStringConverter(ref imagePath);
        //        messageType = System.IO.Path.GetExtension(imagePath);
        //        ParseMessageToSenderClass(ref txtToSend, ref messageType);
        //        imagePath = null;
        //        AddedImageName.Content = null;
        //    }
        //}

        private void SendMessButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindowMethods.CheckAndParseMessages(ref imagePath);

            if (SendMessButton.IsKeyboardFocused)
            {
                while (!SendMessBox.Focus()) ;
            }
        }


        private void SendMessBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                /// --- new
                /// finding open tab and adding paragraph with sent message to it
                /// TODO : move it to another method, probably ParseMessageToSenderClass
                ///

                //TabItem currentTab = (TabItem)MessageTabs.Items.GetItemAt(MessageTabs.SelectedIndex);

                //RichTextBox currentTabTextBox = (RichTextBox)currentTab.Content;

                //Paragraph paragraph = new Paragraph();
                //paragraph.TextAlignment = TextAlignment.Right;
                //paragraph.Inlines.Add(SendMessBox.Text);
                //currentTabTextBox.Document.Blocks.Add(paragraph);

                MainWindowMethods.CheckAndParseMessages(ref imagePath);
            }
        }

        // visual setting of TextBox dependent on keyboard focus
        private void SendMessBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (SendMessBox.Text.Trim().Length > 0 && SendMessBox.Text is templateString)
            {
                SendMessBox.Clear();
                SendMessBox.Foreground = Brushes.Black;
            }
;
        }
        // visual setting of TextBox dependent on keyboard focus
        private void SendMessBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (SendMessBox.Text.Trim().Length == 0)
            {
                SendMessBox.Foreground = Brushes.Gray;
                //SendMessBox.Text = templateString;
                SendMessBox.Text = MainWindowMethods.GetTemplateString;
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

        // Saves chosen image path to use later in a conversion and sending method
        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                // Filter files by extension
                Filter = "Image Files |*.jpg;*.jpeg;*.bmp;*.png",

                // Setting dialog options
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Choose an image to send"
            };

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

        public void AddFriendToList(string friend)
        {
            if (FriendsList.Items.Contains(friend))
                return;

            FriendsList.Items.Add(friend);
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            var addFriendWindow = new AddFriendWindow();

            addFriendWindow.Owner = this;
            switch (addFriendWindow.ShowDialog())
            {
                case true:
                    AddFriendToList(addFriendWindow.GetFriendUsername());
                    break;
                case false:
                    break;
                default:
                    break;
            }
        }
        //  opens new message tab depending on what friend was selected when double click occured
        //
        private void FriendsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (FriendsList.HasItems)
            {
                if (FriendsList.SelectedItem != null)
                {
                    string friend = (string)FriendsList.SelectedItem;

                    //  if tab is already open, then switches to that tab
                    //
                    if (openFriendTabs.TryGetValue(friend, out MessageTabContent? value))
                    {
                        MessageTabs.SelectedIndex = MessageTabs.Items.IndexOf(value.Item);
                    }
                    //  if tab is not open, then creates new instance of tab content and adds it to TabControl item list (MessageTab)
                    //
                    else
                    {
                        MessageTabContent mtc = new(friend);

                        MessageTabs.Items.Add(mtc.Item);
                        MessageTabs.SelectedIndex = MessageTabs.Items.IndexOf(mtc.Item);

                        openFriendTabs.Add(friend, mtc);

                        // Click event for this tabs button (closing tab)
                        //
                        mtc.TabCloseButton.Click += CloseMessageTab;
                    }
                }
                FriendsList.UnselectAll();
            }
        }

        //  click event to close the message tab via button
        //
        private void CloseMessageTab(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            MessageTabs.Items.Remove(openFriendTabs[button.Name].Item);
            openFriendTabs.Remove(button.Name);
        }
    }
}