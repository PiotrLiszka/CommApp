using CommunicationsLib.MsgParser;
using CommunicationsLib.MsgServices;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace commappcs
{
    public class MainWindowHandler
    {
        public const string templateString = "Type message...";

        /// references to the xaml UI elements
        ///
        private readonly TextBox sendMessBox;
        private readonly Label addedImageName;
        private readonly TabControl messageTabs;

        private readonly string currentUser;

        private readonly Sender messageSender;
        private readonly Receiver messageReceiver;

        internal readonly Dictionary<string, MessageTabContent> openFriendTabs;

        public MainWindowHandler(ref TextBox sendMessBox, ref Label addedImageName, ref TabControl messageTabs, string currentUser)
        {
            this.sendMessBox = sendMessBox;
            this.addedImageName = addedImageName;
            this.messageTabs = messageTabs;
            this.currentUser = currentUser;


            // starting messaging services
            //
            messageSender = new Sender(true);
            messageReceiver = new(currentUser, true);
            InitializeMessageReceiver();


            /// helper dictionary to get into the RichTextBox objects of opened UI message tabs
            ///
            openFriendTabs = new Dictionary<string, MessageTabContent>();
        }


        /// <summary>
        /// Creates UI element containing message, wrapped in a bubble
        /// </summary>
        /// <param name="received">Is message sent or received</param>
        /// <param name="message">Message body</param>
        /// <param name="bubbleParagraph">Returns FlowDocument's paragraph with message bubble</param>
        private void CreateMessageBubble(bool received, string message, out Paragraph bubbleParagraph)
        {
            Border bdrBubble = new Border();
            {
                bdrBubble.BorderThickness = new Thickness(2);
                bdrBubble.BorderBrush = Brushes.Black;
                bdrBubble.CornerRadius = new CornerRadius(20);
                bdrBubble.Padding = new Thickness(10);
            }

            TextBlock txtBubble = new TextBlock();
            {
                txtBubble.TextWrapping = TextWrapping.Wrap;
                txtBubble.Background = Brushes.Transparent;
                txtBubble.Foreground = Brushes.White;
                txtBubble.ClipToBounds = true;
                txtBubble.TextAlignment = TextAlignment.Left;
                txtBubble.MaxWidth = messageTabs.ActualWidth / 3 * 2;
                txtBubble.MinWidth = 30;
            }

            bdrBubble.Child = txtBubble;
            bubbleParagraph = new Paragraph();
            bubbleParagraph.Inlines.Add(bdrBubble);

            if (!received)
            {
                bubbleParagraph.TextAlignment = TextAlignment.Right;
                bdrBubble.Background = new LinearGradientBrush(Color.FromRgb(48, 138, 199), Color.FromRgb(62, 177, 255), new Point(0, 1), new Point(0, 0));
                txtBubble.Text = message;
            }
            else
            {
                bubbleParagraph.TextAlignment = TextAlignment.Left;
                bdrBubble.Background = new LinearGradientBrush(Color.FromRgb(34, 177, 76), Color.FromRgb(25, 133, 57), new Point(0, 1), new Point(0, 0));
                txtBubble.Text = message;
            }
        }

        /// <summary>
        /// Displays received message in a specific message tab
        /// </summary>
        /// <param name="senderId">Message sender's name (ID)</param>
        /// <param name="message">Message to display</param>
        private void DisplayReceivedMessage(MessageInfo messageInfo)
        {
            if (messageInfo.MessageBody == null)
                return; 

            CreateMessageBubble(received: true,
                message: string.Concat(messageInfo.TimeSentDT.ToString("HH:mm"), " ID: ", messageInfo.SenderID, " -> ", messageInfo.MessageBody),
                out Paragraph bubbleParagraph);


            TabItem incomingMessageTab;
            int incomingMessTabId;

            if (openFriendTabs.TryGetValue(messageInfo.SenderID, out MessageTabContent? messageContent))
            {
                incomingMessTabId = messageTabs.Items.IndexOf(messageContent.Item);
                incomingMessageTab = (TabItem)messageTabs.Items.GetItemAt(incomingMessTabId);
            }
            else
            {
                // open new tab if message sender's tab is not open
                incomingMessageTab = (TabItem)messageTabs.Items.GetItemAt(OpenNewMessageTab(messageInfo.SenderID));
            }

            RichTextBox incomingMessageTextBox = (RichTextBox)incomingMessageTab.Content;
            incomingMessageTextBox.Document.Blocks.Add(bubbleParagraph);
        }

        /// <summary>
        /// Displays message in a specific message tab (currently opened tab)
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <returns>Name of a current tab item (friend's name)</returns>
        private string DisplaySentMessage(string message)
        {
            CreateMessageBubble(received: false,
                message: string.Concat(message, " <- ", TimeOnly.FromDateTime(DateTime.Now).ToShortTimeString()),
                out Paragraph bubbleParagraph);

            TabItem currentTab = (TabItem)messageTabs.Items.GetItemAt(messageTabs.SelectedIndex);
            RichTextBox sendingTextBox = (RichTextBox)currentTab.Content;
            sendingTextBox.Document.Blocks.Add(bubbleParagraph);

            return currentTab.Name;
        }

        private void InitializeMessageReceiver()
        {
            messageReceiver.StartMessageConsumer((model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageType = ea.BasicProperties.ContentType;
                var message = Encoding.UTF8.GetString(body);

                var routingKey = ea.RoutingKey;

                MessageInfo? messageInfo = MessageJsonSerializer.JsonDeserializeMessage(message);

                if (messageInfo == null)
                    return;

                Debug.WriteLine($"{message}");

                messageTabs.Dispatcher.Invoke(new Action(() =>
                {
                    if (messageInfo == null)
                        return;
                    DisplayReceivedMessage(messageInfo);
                }));

            });
        }

        /// <summary>
        /// Checking what user is sending, and parsing text message, image file or both
        /// </summary>
        /// <param name="imagePath">Loaded image path</param>
        internal void CheckAndParseMessages(ref string? imagePath)
        {
            string txtToSend;
            string messageType;

            if (sendMessBox.Text.Length > 0 && sendMessBox.Text is not templateString)
            {
                txtToSend = sendMessBox.Text.Trim();
                sendMessBox.Clear();
                messageType = "text";

                if (txtToSend.Length == 0)
                    return;

                messageSender.SendMessage(txtToSend, messageType, sendTo: DisplaySentMessage(txtToSend), messFrom: currentUser);
            }

            if (imagePath is not null)
            {
                txtToSend = ImageConverter.ImageToStringConverter(ref imagePath);
                messageType = System.IO.Path.GetExtension(imagePath);
                imagePath = null;
                addedImageName.Content = null;

                if (txtToSend.Length == 0)
                    return;

                // image extension is send as a messageType
                messageSender.SendMessage(txtToSend, messageType, sendTo: DisplaySentMessage(txtToSend), messFrom: currentUser);
            }
        }

        /// <summary>
        /// Opens new message tab in the UI
        /// </summary>
        /// <param name="friend">Friend's tab name</param>
        /// <returns>Index of the newly opened tab</returns>
        internal int OpenNewMessageTab(string friend)
        {
            MessageTabContent mtc = new(friend: friend);

            messageTabs.Items.Add(mtc.Item);
            openFriendTabs.Add(friend, mtc);

            // Click event for this tabs button (closing tab)
            //
            mtc.TabCloseButton.Click += CloseMessageTab;
            mtc.MessTextBox.TextChanged += MessTextBox_ScrollWithMessages;

            return messageTabs.Items.IndexOf(mtc.Item);
        }

        //  when message is reccived, and user is not scrolling messages with mouse over the textbox, then it automatically
        //  scrolls down to the lastest messages
        //
        private void MessTextBox_ScrollWithMessages(object sender, TextChangedEventArgs e)
        {
            RichTextBox messBox = (RichTextBox) sender;
            if (!messBox.IsMouseOver)
                messBox.ScrollToEnd();
        }

        //  click event to close the message tab via button
        //
        private void CloseMessageTab(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            messageTabs.Items.Remove(openFriendTabs[button.Name].Item);
            openFriendTabs.Remove(button.Name);
        }
    }
}
