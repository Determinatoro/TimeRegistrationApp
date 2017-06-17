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
    /// Interaction logic for CreateOrderRelationWindow.xaml
    /// </summary>
    public partial class CreateOrderRelationWindow : Window
    {
        private AdministrateRolesWindow administrateRolesWindow;
        private OrderRole orderRole;
        private Order order;

        public CreateOrderRelationWindow(AdministrateRolesWindow administrateRolesWindow, Order order, OrderRole orderRole)
        {
            InitializeComponent();

            this.administrateRolesWindow = administrateRolesWindow;
            this.orderRole = orderRole;
            this.order = order;

            WebserviceObject wsObj = WebserviceCalls.GetUsers();

            List<User> usersList = new List<User>();

            if (wsObj.Success)
            {
                foreach (User obj in (List<object>)wsObj.Response)
                    usersList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            ObservableCollection<object> oList = new ObservableCollection<object>(usersList);

            dgUsers.ItemsSource = oList;

            wsObj = WebserviceCalls.GetRoles();

            List<Role> rolesList = new List<Role>();

            if (wsObj.Success)
            {
                foreach (Role obj in (List<object>)wsObj.Response)
                    rolesList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());
            
            oList = new ObservableCollection<object>(rolesList);

            dgRoles.ItemsSource = oList;

            if (orderRole != null)
            {
                Title = "TimeRegistrationApp - Update order relation";
                btnCreate.Content = "Update order relation";

                var user = usersList.Where(x => x.UserId == orderRole.UserId).FirstOrDefault();

                dgUsers.SelectedItem = user;

                var role = rolesList.Where(x => x.RoleId == orderRole.RoleId).FirstOrDefault();

                dgRoles.SelectedItem = role;

                dgUsers.IsEnabled = false;
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<object> list = (ObservableCollection<object>)dgUsers.ItemsSource;

            var user = (User)list[dgUsers.SelectedIndex];

            list = (ObservableCollection<object>)dgRoles.ItemsSource;

            var role = (Role)list[dgRoles.SelectedIndex];            

            if (orderRole != null)
            {                
                var wsObj = WebserviceCalls.UpdateOrderRole(orderRole.OrderRoleId, order.OrderId, user.UserId, role.RoleId);

                if (wsObj.Success)
                {
                    administrateRolesWindow.GetOrderRoles();
                    Close();
                }
                else
                    MessageBox.Show(wsObj.Response.ToString());
            }
            else
            {
                var wsObj = WebserviceCalls.CreateOrderRole(order.OrderId, user.UserId, role.RoleId);

                if (wsObj.Success)
                {
                    administrateRolesWindow.GetOrderRoles();
                    Close();
                }
                else
                    MessageBox.Show(wsObj.Response.ToString());
            }
        }
    }
}
