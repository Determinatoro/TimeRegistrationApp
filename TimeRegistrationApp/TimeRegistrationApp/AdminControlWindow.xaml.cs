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
    /// Interaction logic for AdminControlWindow.xaml
    /// </summary>
    public partial class AdminControlWindow : Window
    {
        private List<User> usersList = new List<User>();
        private List<Customer> customersList = new List<Customer>();
        private List<Role> rolesList = new List<Role>();
        private List<Order> ordersTimeList = new List<Order>();
        private List<User> usersTimeList = new List<User>();

        public AdminControlWindow()
        {
            InitializeComponent();

            btnManageUsers_Click(null, null);
        }

        #region Manage control

        private void btnManageUsers_Click(object sender, RoutedEventArgs e)
        {
            Title = "TimeRegistrationApp - Manage users";

            GetUsers();

            gridUsers.Visibility = Visibility.Visible;
            gridCustomers.Visibility = Visibility.Hidden;
            gridRoles.Visibility = Visibility.Hidden;
            gridTimeRegistrationsOnOrder.Visibility = Visibility.Hidden;
        }

        private void btnManageCustomers_Click(object sender, RoutedEventArgs e)
        {
            Title = "TimeRegistrationApp - Manage customers";

            GetCustomers();

            gridUsers.Visibility = Visibility.Hidden;
            gridCustomers.Visibility = Visibility.Visible;
            gridRoles.Visibility = Visibility.Hidden;
            gridTimeRegistrationsOnOrder.Visibility = Visibility.Hidden;
        }

        private void btnManageRoles_Click(object sender, RoutedEventArgs e)
        {
            Title = "TimeRegistrationApp - Manage roles";

            GetRoles();

            gridUsers.Visibility = Visibility.Hidden;
            gridCustomers.Visibility = Visibility.Hidden;
            gridRoles.Visibility = Visibility.Visible;
            gridTimeRegistrationsOnOrder.Visibility = Visibility.Hidden;
        }

        private void btnManageTimeregistrationsOnOrder_Click(object sender, RoutedEventArgs e)
        {
            Title = "TimeRegistrationApp - See time registrated on order";

            GetOrders();

            gridUsers.Visibility = Visibility.Hidden;
            gridCustomers.Visibility = Visibility.Hidden;
            gridRoles.Visibility = Visibility.Hidden;
            gridTimeRegistrationsOnOrder.Visibility = Visibility.Visible;
        }

        #endregion

        #region Timeregistrations on order

        public void GetOrders()
        {
            WebserviceObject wsObj = WebserviceCalls.GetOrdersAdmin();

            List<Order> ordersTimeList = new List<Order>();

            if (wsObj.Success)
            {
                foreach (Order obj in (List<object>)wsObj.Response)
                    ordersTimeList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            this.ordersTimeList = ordersTimeList;

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(ordersTimeList);

            dgOrdersTime.ItemsSource = oList;
        }

        private void dgOrdersTime_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            ObservableCollection<object> list = (ObservableCollection<object>)dgOrdersTime.ItemsSource;

            var order = (Order)list[row.GetIndex()];

            TimeRegistratedWindow window = new TimeRegistratedWindow(order);
            window.ShowDialog();
        }

        private void tbSearchOrders_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            string t = tb.Text.ToLower();

            var temp = ordersTimeList.Where(o => o.OrderName.ToLower().Contains(t) || o.Description.ToLower().Contains(t));

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(temp);

            dgOrdersTime.ItemsSource = oList;
        }

        #endregion

        #region Roles

        public void GetRoles()
        {
            WebserviceObject wsObj = WebserviceCalls.GetRoles();

            List<Role> rolesList = new List<Role>();

            if (wsObj.Success)
            {
                foreach (Role obj in (List<object>)wsObj.Response)
                    rolesList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            this.rolesList = rolesList;

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(rolesList);

            dgRoles.ItemsSource = oList;
        }

        private void tbSearchRoles_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            string t = tb.Text.ToLower();

            var temp = rolesList.Where(o => o.Name.ToLower().Contains(t));

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(temp);

            dgRoles.ItemsSource = oList;
        }

        private void btnCreateRole_Click(object sender, RoutedEventArgs e)
        {
            CreateRoleWindow window = new CreateRoleWindow(this);
            window.ShowDialog();
        }

        #endregion

        #region Customers

        public void GetCustomers()
        {
            WebserviceObject wsObj = WebserviceCalls.GetCustomers();

            List<Customer> customersList = new List<Customer>();

            if (wsObj.Success)
            {
                foreach (Customer obj in (List<object>)wsObj.Response)
                    customersList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            this.customersList = customersList;

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(customersList);

            dgCustomers.ItemsSource = oList;
        }

        private void tbSearchCustomers_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            string t = tb.Text.ToLower();

            var temp = customersList.Where(o => o.Name.ToLower().Contains(t));

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(temp);

            dgCustomers.ItemsSource = oList;
        }

        private void btnCreateCustomer_Click(object sender, RoutedEventArgs e)
        {
            CreateCustomerWindow window = new CreateCustomerWindow(this);
            window.ShowDialog();
        }

        #endregion

        #region Users

        public void GetUsers()
        {
            WebserviceObject wsObj = WebserviceCalls.GetUsers();

            List<User> usersList = new List<User>();

            if (wsObj.Success)
            {
                foreach (User obj in (List<object>)wsObj.Response)
                    usersList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            this.usersList = usersList;

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(usersList);

            dgUsers.ItemsSource = oList;
        }

        private void dgUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            ObservableCollection<object> list = (ObservableCollection<object>)dgUsers.ItemsSource;

            var user = (User)list[row.GetIndex()];

            CreateUserWindow window = new CreateUserWindow(this, user, false);
            window.ShowDialog();
        }

        private void tbSearchUsers_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            string t = tb.Text.ToLower();

            var temp = usersList.Where(o => o.FirstName.ToLower().Contains(t) || o.LastName.ToLower().Contains(t));

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(temp);

            dgUsers.ItemsSource = oList;
        }

        private void btnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            CreateUserWindow window = new CreateUserWindow(this, null, false);
            window.ShowDialog();
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<object> list = (ObservableCollection<object>)dgUsers.ItemsSource;

            var user = (User)list[dgUsers.SelectedIndex];

            CreateUserWindow window = new CreateUserWindow(this, user, true);
            window.ShowDialog();
        }






        #endregion

        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
