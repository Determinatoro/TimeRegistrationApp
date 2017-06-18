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
    /// Interaction logic for AdminstrateRolesWindow.xaml
    /// </summary>
    public partial class AdministrateRolesWindow : Window
    {
        private OrdersWindow ordersWindow;
        private Order order;
        private User user;

        public AdministrateRolesWindow(OrdersWindow ordersWindow, Order order, User user)
        {
            InitializeComponent();

            this.ordersWindow = ordersWindow;
            this.order = order;
            this.user = user;

            GetOrderRoles();
        }

        public void GetOrderRoles()
        {
            WebserviceObject wsObj = WebserviceCalls.GetOrderRoles(order.OrderId);

            List<OrderRole> orderRoleList = new List<OrderRole>();

            if (wsObj.Success)
            {
                foreach (OrderRole obj in (List<object>)wsObj.Response)
                    orderRoleList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            orderRoleList = orderRoleList.OrderBy(x => x.Name).ToList();

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(orderRoleList);            

            dgOrderRoles.ItemsSource = oList;

            ordersWindow.GetOrders();
        }

        private void dgOrderRoles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            ObservableCollection<object> list = (ObservableCollection<object>)dgOrderRoles.ItemsSource;

            var orderRole = (OrderRole)list[row.GetIndex()];

            CreateOrderRelationWindow window = new CreateOrderRelationWindow(this, order, orderRole);
            window.ShowDialog();
        }

        private void btnCreateOrderRole_Click(object sender, RoutedEventArgs e)
        {
            CreateOrderRelationWindow window = new CreateOrderRelationWindow(this, order, null);
            window.ShowDialog();
        }

        private void btnDeleteOrderRole_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrderRoles.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an order relation");
                return;
            }

            ObservableCollection<object> list = (ObservableCollection<object>)dgOrderRoles.ItemsSource;

            var orderRole = (OrderRole)list[dgOrderRoles.SelectedIndex];

            WebserviceObject wsObj = WebserviceCalls.DeleteOrderRole(orderRole.OrderRoleId, orderRole.OrderId);

            if (wsObj.Success)
            {
                GetOrderRoles();
                ordersWindow.GetOrders();
            }
            else
                MessageBox.Show(wsObj.Response.ToString());
    
        }
    }
}
