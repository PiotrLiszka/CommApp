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
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            LoginNameBox.Focus();

            //  test login name
            //
            LoginNameBox.Text = "Kenny";

            LoginNameBox.KeyDown += LoginNameBox_EnterPressed;
            PasswordBox.KeyDown += PasswordBox_EnterPressed;
        }

        private void PasswordBox_EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton_Click(sender, e);
            }
        }

        private void LoginNameBox_EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton_Click(sender, e);
            }
        }

        public string GetLoginName()
        {
            return this.LoginNameBox.Text.Trim();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if(GetLoginName().Length > 0)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

    }
}
