using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            dpStartTimeDate.SelectedDate = DateTime.Now;
            dpEndTimeDate.SelectedDate = DateTime.Now;

            tbStartTimeHour.Text = DateTime.Now.ToString("HH");
            tbStartTimeMinutes.Text = DateTime.Now.ToString("mm");

            tbStartTimeHour.MaxLength = 2;
            tbStartTimeMinutes.MaxLength = 2;

            WebserviceObject wsObj = WebserviceCalls.GetTimeRegistrations(user.UserId);

            ObservableCollection<TimeRegistration> list = new ObservableCollection<TimeRegistration>();

            foreach (TimeRegistration obj in (List<object>)wsObj.Response)
                list.Add(obj);

            dgTimeRegistrations.ItemsSource = list; 
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

        private void tbTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void tbTime_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void tbTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                var tb = sender as TextBox;

                switch (tb.Name)
                {
                    case "tbStartTimeHour":
                        tbStartTimeMinutes.Focus();
                        tbStartTimeMinutes.SelectAll();
                        break;
                    case "tbStartTimeMinutes":
                        tbStartTimeHour.Focus();
                        tbStartTimeHour.SelectAll();
                        break;
                    case "tbEndTimeHour":
                        tbStartTimeMinutes.Focus();
                        tbStartTimeMinutes.SelectAll();
                        break;
                    case "tbEndTimeMinuutes":
                        tbStartTimeMinutes.Focus();
                        tbStartTimeMinutes.SelectAll();
                        break;
                }
                

                e.Handled = true;
            }
        }

        private void tbTime_Lostfocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;

            tb.Text = tb.Text.PadLeft(2, '0');
        }

        private void btnAdminControls_click(object sender, RoutedEventArgs e)
        {
            AdminUserWindow adminWindow = new AdminUserWindow();
            adminWindow.ShowDialog();
        }
    }
}
