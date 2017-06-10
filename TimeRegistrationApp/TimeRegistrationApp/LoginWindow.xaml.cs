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
using TimeRegistrationApp.Webservice;

namespace TimeRegistrationApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            tbUsername.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            WebserviceObject wsObj = WebserviceCalls.CheckLogin(tbUsername.Text, tbPassword.Password);

            if (wsObj.Success)
            {
                MainWindow mainWindow = new MainWindow((User)wsObj.Response);
                mainWindow.Show();
                this.Close();
                // Go to next window
            }
            else
                MessageBox.Show(wsObj.Response.ToString());
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnLogin_Click(null, null);
        }
    }
}
