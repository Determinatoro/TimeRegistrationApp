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
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text != "" && tbPassword.Password != "")
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
            else
                MessageBox.Show("Please write an username and/or password");
        }
    }
}
