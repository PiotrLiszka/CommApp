using CommunicationsLib.MsgParser;
using CommunicationsLib.MsgServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace commappcs
{
    public class MainWindowHandler
    {
        private const string templateString = "Type message...";

        private readonly TextBox sendMessBox;
        private readonly Label addedImageName;
        private readonly TabControl messageTabs;
        private readonly string currentUser;

        private readonly Sender messageSender;

        public string GetTemplateString => templateString;

        public MainWindowHandler(ref TextBox sendMessBox, ref Label addedImageName, ref TabControl messageTabs, string currentUser)
        {
            this.sendMessBox = sendMessBox;
            this.addedImageName = addedImageName;
            this.messageTabs = messageTabs;
            messageSender = new Sender(true);
            this.currentUser = currentUser;
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
                messageType = "text";
                ParseMessageToSenderClass(txtToSend,messageType);
                sendMessBox.Clear();
            }

            if (imagePath is not null)
            {
                txtToSend = ImageConverter.ImageToStringConverter(ref imagePath);
                messageType = System.IO.Path.GetExtension(imagePath);
                ParseMessageToSenderClass(txtToSend, messageType);
                imagePath = null;
                addedImageName.Content = null;
            }
        }

        internal void ParseMessageToSenderClass(string textToSend, string messageType)
        {
            if (textToSend.Length == 0)
                return;
            
            Paragraph paragraphSent = new Paragraph();
            paragraphSent.TextAlignment = TextAlignment.Right;

            if (messageType == "text")
                paragraphSent.Inlines.Add(string.Concat("Wysłano: ", textToSend));
            else
                // test to see if images are read correctly -> messageType = file extension
                paragraphSent.Inlines.Add(string.Concat("Wysłano: ", messageType));

            TabItem currentTab = (TabItem)messageTabs.Items.GetItemAt(messageTabs.SelectedIndex);

            RichTextBox currentTabTextBox = (RichTextBox)currentTab.Content;

            Paragraph paragraph = new()
            {
                TextAlignment = TextAlignment.Right
            };

            paragraph.Inlines.Add(sendMessBox.Text);
            currentTabTextBox.Document.Blocks.Add(paragraph);

            Debug.WriteLine($"Current tab: {currentTab.Name}");

            messageSender.SendMessage(textToSend, messageType, currentTab.Name, currentUser);
        }
    }
}
