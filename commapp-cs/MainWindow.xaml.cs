using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace commappcs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowHandler mainWindowHandler;

        private string? imagePath = null;
        private const string templateString = MainWindowHandler.templateString;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            string currentUser = string.Empty;
            
            /// mainwindow visual and other options
            /// 
            SendMessBox.Foreground = Brushes.Gray;

            SendMessBox.IsEnabled = false;
            AddImageButton.IsEnabled = false;
            SendMessButton.IsEnabled = false;

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

            mainWindowHandler = new MainWindowHandler(ref SendMessBox, ref AddedImageName, ref MessageTabs, currentUser);
            SendMessBox.Text = templateString;

            FriendsList.SelectionMode = SelectionMode.Single;
        }

        private void SendMessButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindowHandler.CheckAndParseMessages(ref imagePath);

            if (SendMessButton.IsKeyboardFocused)
            {
                while (!SendMessBox.Focus()) ;
            }
        }

        private void SendMessBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mainWindowHandler.CheckAndParseMessages(ref imagePath);
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
                SendMessBox.Text = templateString;
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
            AddFriendWindow addFriendWindow = new();
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
            if (!FriendsList.HasItems)
                return;

            if (FriendsList.SelectedItem == null)
                return;

            string friend = (string)FriendsList.SelectedItem;

            //  if tab is already open, then switches to that tab
            //
            if (mainWindowHandler.openFriendTabs.TryGetValue(friend, out MessageTabContent? value))
            {
                MessageTabs.SelectedIndex = MessageTabs.Items.IndexOf(value.Item);
            }
            //  if tab is not open, then creates new instance of tab content and adds it to TabControl item list (MessageTab)
            //
            else
            {
                MessageTabs.SelectedIndex = mainWindowHandler.OpenNewMessageTab(friend);
            }

            FriendsList.UnselectAll();
        }

        //  checks after opening/closing message tabs if TabControl should be visible and if message controls should be enabled
        //
        private void MessageTabs_LayoutUpdated(object sender, EventArgs e)
        {
            if (MessageTabs.Items.Count > 0)
            {
                MessageTabs.Visibility = Visibility.Visible;
                SendMessBox.IsEnabled = true;
                AddImageButton.IsEnabled = true;
                SendMessButton.IsEnabled = true;
            }
            else
            {
                MessageTabs.Visibility = Visibility.Collapsed;
                SendMessBox.IsEnabled = false;
                AddImageButton.IsEnabled = false;
                SendMessButton.IsEnabled = false;
            }
        }
    }
}