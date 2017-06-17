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
    /// Interaction logic for CreateRoleWindow.xaml
    /// </summary>
    public partial class CreateRoleWindow : Window
    {
        private AdminControlWindow adminControlWindow;

        public CreateRoleWindow(AdminControlWindow adminControlWindow)
        {
            InitializeComponent();

            this.adminControlWindow = adminControlWindow;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (tbRoleName.Text == "")
            {
                MessageBox.Show("Please fill out the field");
                return;
            }

            string name = tbRoleName.Text;

            WebserviceObject wsObj = new WebserviceObject();
            wsObj = WebserviceCalls.CreateRole(name);

            if (wsObj.Success)
            {
                adminControlWindow.GetRoles();
                Close();
            }
            else
                MessageBox.Show(wsObj.Response.ToString());
        }
    }
}
