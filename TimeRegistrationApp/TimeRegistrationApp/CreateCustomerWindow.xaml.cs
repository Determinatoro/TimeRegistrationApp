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
    /// Interaction logic for CreateCustomerWindow.xaml
    /// </summary>
    public partial class CreateCustomerWindow : Window
    {
        private AdminControlWindow adminControlWindow;

        public CreateCustomerWindow(AdminControlWindow adminControlWindow)
        {
            InitializeComponent();

            this.adminControlWindow = adminControlWindow;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (tbCustomerName.Text == "")
            {
                MessageBox.Show("Please fill out the field");
                return;
            }

            string name = tbCustomerName.Text;            

            WebserviceObject wsObj = new WebserviceObject();
            wsObj = WebserviceCalls.CreateCustomer(name);
            
            if (wsObj.Success)
            {
                adminControlWindow.GetCustomers();
                Close();
            }
            else
                MessageBox.Show(wsObj.Response.ToString());
        }
    }
}
