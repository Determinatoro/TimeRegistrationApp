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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimeRegistrationApp.Webservice;

namespace TimeRegistrationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User user;
        private Order order;

        public MainWindow(User user)
        {
            InitializeComponent();

            this.user = user;

            Title = string.Format("Welcome {0} {1}", user.FirstName, user.LastName);

            if (!user.Admin)
                btnAdminControls.Visibility = Visibility.Hidden;
        }

        public void SetOrderId(Order order)
        {
            this.order = order;
            tbOrderId.Text = order.OrderId.ToString();
            lbDescription.Content = order.Description;
            lbRole.Content = order.RoleName;
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void btnSearchOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow ordersWindow = new OrdersWindow(this, user);
            ordersWindow.ShowDialog();
        }

        private void tbOrderId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int orderId = int.Parse(tbOrderId.Text);

                WebserviceObject wsObj = WebserviceCalls.GetOrder(user.UserId, orderId);

                if (wsObj.Success)
                    SetOrderId((Order)wsObj.Response);
                else
                    MessageBox.Show(wsObj.Response.ToString());
            }
        }
    }
}
