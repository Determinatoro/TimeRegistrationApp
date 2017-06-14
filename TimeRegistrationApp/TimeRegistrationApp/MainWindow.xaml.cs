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

            SetStartAndEndTime(DateTime.Now, DateTime.Now);

            tbStartTimeHour.MaxLength = 2;
            tbStartTimeMinutes.MaxLength = 2;

            GetTimeRegistrations();
        }

        public void GetTimeRegistrations()
        {
            WebserviceObject wsObj = WebserviceCalls.GetTimeRegistrations(user.UserId);

            ObservableCollection<TimeRegistration> list = new ObservableCollection<TimeRegistration>();

            foreach (TimeRegistration obj in (List<object>)wsObj.Response)
                list.Add(obj);

            list = new ObservableCollection<TimeRegistration>(from o in list orderby DateTime.Parse(o.StartTime) descending select o);

            dgTimeRegistrations.ItemsSource = list;
        }

        public void SetOrderId(Order order)
        {
            this.order = order;
            tbOrderId.Text = order.OrderId.ToString();
            lbDescription.Content = order.Description;
            lbRole.Content = order.RoleName;
        }

        private void SetStartAndEndTime(DateTime startTime, DateTime? endTime)
        {
            dpStartTimeDate.SelectedDate = startTime;

            if (endTime != null)
            {
                dpEndTimeDate.SelectedDate = endTime;

                if (endTime != startTime)
                {
                    tbEndTimeHour.Text = endTime.Value.ToString("HH");
                    tbEndTimeMinutes.Text = endTime.Value.ToString("mm");
                }
            }

            tbStartTimeHour.Text = startTime.ToString("HH");
            tbStartTimeMinutes.Text = startTime.ToString("mm");
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
                        tbEndTimeMinutes.Focus();
                        tbEndTimeMinutes.SelectAll();
                        break;
                    case "tbEndTimeMinutes":
                        tbEndTimeHour.Focus();
                        tbEndTimeHour.SelectAll();
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

        #region TimeRegistration

        private void dgTimeRegistrations_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // execute some code
        }

        private void dgTimeRegistrations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try {
                ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;

                var tr = list[dgTimeRegistrations.SelectedIndex];

                var dtStart = DateTime.Parse(tr.StartTime);

                if (tr.EndTime != "")
                {
                    var dtEnd = DateTime.Parse(tr.EndTime);
                    SetStartAndEndTime(dtStart, dtEnd);
                }
                else
                {
                    SetStartAndEndTime(dtStart, null);
                }
            } catch { }
        }

        #endregion

        private void btnAdminControls_click(object sender, RoutedEventArgs e)
        {
            AdminUserWindow adminWindow = new AdminUserWindow();
            adminWindow.ShowDialog();
        }

        private void btnStartContinue_Click(object sender, RoutedEventArgs e)
        {
            var dt = DateTime.Now;

            var wsObj = WebserviceCalls.StartTimeRegistration(dt.ToString("yyyy-MM-dd'T'HH:mm:ss"), user.UserId, order.OrderId);

            if (wsObj.Success)
            {
                GetTimeRegistrations();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            /*var dt = dpEndTimeDate.SelectedDate.Value;
            TimeSpan ts = new TimeSpan(int.Parse(tbEndTimeHour.Text), int.Parse(tbEndTimeMinutes.Text), 0);
            dt = dt + ts;*/

            var dt = DateTime.Now;

            ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;

            var tr = list[0];

            var wsObj = WebserviceCalls.EndTimeRegistration(tr.TimeRegId, dt.ToString("yyyy-MM-dd'T'HH:mm:ss"));

            if (wsObj.Success)
            {
                GetTimeRegistrations();
            }
        }
    }
}
