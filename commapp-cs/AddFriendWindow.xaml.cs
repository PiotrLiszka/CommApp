using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace commappcs
{
    /// <summary>
    /// Logika interakcji dla klasy AddFriendWindow.xaml
    /// </summary>
    public partial class AddFriendWindow : Window
    {
        public AddFriendWindow()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            FriendTextBox.KeyDown += FriendTextBox_EnterPressed;
            FriendTextBox.Focus();
        }

        private void FriendTextBox_EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton_Click(sender, e);
            }
        }

        public string GetFriendUsername()
        {
            return this.FriendTextBox.Text.Trim();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (FriendTextBox.Text.Trim().Length > 0)
            {
                DialogResult = true;
                this.Close();
            }
        }

    }
}
