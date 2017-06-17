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
    /// Interaction logic for CreateUser.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {
        private AdminControlWindow adminControlWindow;
        private User user;
        private bool resetPassword;

        public CreateUserWindow(AdminControlWindow adminControlWindow, User user, bool resetPassword)
        {
            InitializeComponent();

            this.adminControlWindow = adminControlWindow;
            this.user = user;
            this.resetPassword = resetPassword;

            string[] admin = new string[] { "True", "False" };
            cbbAdmin.ItemsSource = admin;

            if (user != null)
            {
                tbFirstName.Text = user.FirstName;
                tbLastname.Text = user.LastName;
                tbPassword.Visibility = Visibility.Hidden;

                if (user.Admin)
                    cbbAdmin.SelectedIndex = 0;
                else
                    cbbAdmin.SelectedIndex = 1;

                rowPassword.Height = new GridLength(0, GridUnitType.Pixel);
                this.Height = 180;
                btnCreate.Content = "Update user";
            }

            if (resetPassword)
            {
                tbFirstName.IsEnabled = false;
                tbLastname.IsEnabled = false;
                cbbAdmin.IsEnabled = false;

                tbPassword.Visibility = Visibility.Visible;
                tbPassword.Text = "";

                rowPassword.Height = new GridLength(30, GridUnitType.Pixel);
                this.Height = 210;
                btnCreate.Content = "Reset password";
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (tbFirstName.Text == "" || tbLastname.Text == "" || (tbPassword.Text == "" && tbPassword.Visibility == Visibility.Visible) || cbbAdmin.SelectedItem == null)
            {
                MessageBox.Show("Please fill out the fields");
                return;
            }

            string firstname = tbFirstName.Text;
            string lastname = tbLastname.Text;            
            string password = tbPassword.Text;
            bool admin = bool.Parse(cbbAdmin.SelectedItem.ToString());

            WebserviceObject wsObj = new WebserviceObject();

            if (resetPassword)
                wsObj = WebserviceCalls.ResetPassword(user.UserId, password);
            else if (user != null)
                wsObj = WebserviceCalls.UpdateUser(user.UserId, firstname, lastname, admin);
            else
                wsObj = WebserviceCalls.CreateUser(firstname, lastname, admin, password);

            if (wsObj.Success)
            {
                adminControlWindow.GetUsers();
                Close();
            }
            else
                MessageBox.Show(wsObj.Response.ToString());
        }
    }
}
