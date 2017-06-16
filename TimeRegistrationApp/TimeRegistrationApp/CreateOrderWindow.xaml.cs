using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CreateOrderWindow.xaml
    /// </summary>
    public partial class CreateOrderWindow : Window
    {
        private User user;
        private OrdersWindow ordersWindow;
        private List<Customer> customersList = new List<Customer>();

        public CreateOrderWindow(OrdersWindow ordersWindow, User user)
        {
            InitializeComponent();

            this.user = user;
            this.ordersWindow = ordersWindow;

            WebserviceObject wsObj = WebserviceCalls.GetCustomers();

            List<Customer> customersList = new List<Customer>();

            if (wsObj.Success)
            {
                foreach (Customer obj in (List<object>)wsObj.Response)
                    customersList.Add(obj);

                this.customersList = customersList;
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            List<string> names = customersList.Select(x => x.Name).ToList();            

            cbbCustomers.ItemsSource = names;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (tbOrderName.Text == "" || tbOrderDescription.Text == "" || cbbCustomers.SelectedItem == null)
            {
                MessageBox.Show("Please fill out the fields");
                return;
            }

            string name = tbOrderName.Text;
            string description = tbOrderDescription.Text;
            int customerId = customersList[cbbCustomers.SelectedIndex].CustomerId;

            WebserviceObject wsObj = WebserviceCalls.CreateOrder(user.UserId, name, description, customerId);

            if (wsObj.Success)
            {
                ordersWindow.GetOrders();
                Close();
            }
            else
                MessageBox.Show(wsObj.Response.ToString());
        }
    }
}
