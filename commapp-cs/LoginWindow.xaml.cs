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
        }

        public string GetLoginName()
        {
            return this.LoginNameBox.Text.Trim();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.LoginNameBox.Text.Trim().Length > 0)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

    }
}
