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

            SetStartAndEndTime(DateTime.Now, DateTime.Today);

            tbStartTimeHour.MaxLength = 2;
            tbStartTimeMinutes.MaxLength = 2;

            GetTimeRegistrations();
        }

        #region Functions

        public void GetTimeRegistrations()
        {
            WebserviceObject wsObj = WebserviceCalls.GetTimeRegistrations(user.UserId);

            ObservableCollection<TimeRegistration> list = new ObservableCollection<TimeRegistration>();

            foreach (TimeRegistration obj in (List<object>)wsObj.Response)
                list.Add(obj);

            list = new ObservableCollection<TimeRegistration>(from o in list orderby DateTime.Parse(o.StartTime) descending select o);

            var tr = (from o in list where o.EndTime == "" select o).FirstOrDefault();

            if (tr != null)
            {
                list.Remove(tr);
                list.Insert(0, tr);
            }

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

                tbEndTimeHour.Text = endTime.Value.ToString("HH");
                tbEndTimeMinutes.Text = endTime.Value.ToString("mm");
            }
            else
            {
                tbEndTimeHour.Text = "";
                tbEndTimeMinutes.Text = "";
            }

            tbStartTimeHour.Text = startTime.ToString("HH");
            tbStartTimeMinutes.Text = startTime.ToString("mm");
        }

        #endregion

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
                if (tbOrderId.Text == "")
                {
                    MessageBox.Show("Please give an order ID");
                    return;
                }

                int orderId = int.Parse(tbOrderId.Text);

                WebserviceObject wsObj = WebserviceCalls.GetOrder(user.UserId, orderId);

                if (wsObj.Success)
                    SetOrderId((Order)wsObj.Response);
                else
                    MessageBox.Show(wsObj.Response.ToString());
            }
        }

        #region TimeRegistration

        #region TextBox events

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

        #endregion

        /***********************************************************/
        // Double click on datagrid row
        /***********************************************************/
        private void dgTimeRegistrations_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dgr = sender as DataGridRow;

            ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;

            var tr = list[dgr.GetIndex()];

            NoteWindow noteWindow = new NoteWindow(this, tr);
            noteWindow.ShowDialog();
        }

        /***********************************************************/
        // Selected datagridrow
        /***********************************************************/
        private void dgTimeRegistrations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;

                var tr = list[dgTimeRegistrations.SelectedIndex];

                var dtStart = DateTime.Parse(tr.StartTime);

                if (tr.EndTime != "")
                {
                    var dtEnd = DateTime.Parse(tr.EndTime);
                    SetStartAndEndTime(dtStart, dtEnd);
                }
                else
                    SetStartAndEndTime(dtStart, null);
            }
            catch { }
        }

        private void btnStartContinue_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;

            var tr = list[0];

            if (tr.EndTime == "")
            {
                MessageBox.Show("Please end your current time registration first");
                return;
            }

            if (order == null)
            {
                MessageBox.Show("Please select an order");
                return;
            }

            var dt = DateTime.Parse(dpStartTimeDate.SelectedDate.Value.ToString("dd-MM-yyyy"));
            TimeSpan ts = new TimeSpan(int.Parse(tbStartTimeHour.Text), int.Parse(tbStartTimeMinutes.Text), 0);
            dt = dt + ts;

            if (dt.ToString("dd-MM-yyyy HH:mm") != DateTime.Now.ToString("dd-MM-yyyy HH:mm"))
            {
                if (MessageBox.Show("Start time is set before or after current time. Want to set it to current?", "Start time", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    dt = DateTime.Now;
                    SetStartAndEndTime(DateTime.Now, null);
                }
            }

            var wsObj = WebserviceCalls.StartTimeRegistration(dt.ToString("yyyy-MM-dd'T'HH:mm:ss"), user.UserId, order.OrderId);

            if (wsObj.Success)
                GetTimeRegistrations();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            var dt = DateTime.Now;

            ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;
            
            var tr = list[0];

            if (tr.EndTime != "")
            {
                MessageBox.Show("You have not started a time registration");
                return;
            }

            var wsObj = WebserviceCalls.EndTimeRegistration(tr.TimeRegId, dt.ToString("yyyy-MM-dd'T'HH:mm:ss"));

            if (wsObj.Success)
                GetTimeRegistrations();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (tbStartTimeHour.Text == "" || tbStartTimeMinutes.Text == "" || tbEndTimeHour.Text == "" || tbEndTimeMinutes.Text == "")
            {
                MessageBox.Show("Please fill out hours and minutes");
                return;
            }

            var dtStart = DateTime.Parse(dpStartTimeDate.SelectedDate.Value.ToString("dd-MM-yyyy"));
            TimeSpan ts = new TimeSpan(int.Parse(tbStartTimeHour.Text), int.Parse(tbStartTimeMinutes.Text), 0);
            dtStart += ts;

            var dtEnd = DateTime.Parse(dpEndTimeDate.SelectedDate.Value.ToString("dd-MM-yyyy"));
            ts = new TimeSpan(int.Parse(tbEndTimeHour.Text), int.Parse(tbEndTimeMinutes.Text), 0);
            dtEnd += ts;

            if (dtEnd < dtStart)
            {
                MessageBox.Show("End time is before start time");
                return;
            }

            ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;

            TimeRegistration tr = null;

            if (dgTimeRegistrations.SelectedIndex == -1)
                tr = list[0];
            else
                tr = list[dgTimeRegistrations.SelectedIndex];

            var wsObj = WebserviceCalls.UpdateTimeRegistration(tr.TimeRegId, dtStart.ToString("yyyy-MM-dd'T'HH:mm:ss"), dtEnd.ToString("yyyy-MM-dd'T'HH:mm:ss"));

            if (wsObj.Success)
                GetTimeRegistrations();
            else
                MessageBox.Show(wsObj.Response.ToString());
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            if (order == null)
            {
                MessageBox.Show("Please select an order");
                return;
            }

            if (tbStartTimeHour.Text == "" || tbStartTimeMinutes.Text == "" || tbEndTimeHour.Text == "" || tbEndTimeMinutes.Text == "")
            {
                MessageBox.Show("Please fill out hours and minutes");
                return;
            }

            var dtStart = DateTime.Parse(dpStartTimeDate.SelectedDate.Value.ToString("dd-MM-yyyy"));
            TimeSpan ts = new TimeSpan(int.Parse(tbStartTimeHour.Text), int.Parse(tbStartTimeMinutes.Text), 0);
            dtStart += ts;

            var dtEnd = DateTime.Parse(dpEndTimeDate.SelectedDate.Value.ToString("dd-MM-yyyy"));
            ts = new TimeSpan(int.Parse(tbEndTimeHour.Text), int.Parse(tbEndTimeMinutes.Text), 0);
            dtEnd += ts;

            if (dtEnd < dtStart)
            {
                MessageBox.Show("End time is before start time");
                return;
            }

            var wsObj = WebserviceCalls.CreateTimeRegistration(dtStart.ToString("yyyy-MM-dd'T'HH:mm:ss"), dtEnd.ToString("yyyy-MM-dd'T'HH:mm:ss"), user.UserId, order.OrderId);

            if (wsObj.Success)
                GetTimeRegistrations();
            else
                MessageBox.Show(wsObj.Response.ToString());
        }

        private void btnDeleteTimeRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (dgTimeRegistrations.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a time registration");
                return;
            }

            ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;

            var tr = list[dgTimeRegistrations.SelectedIndex];

            var wsObj = WebserviceCalls.DeleteTimeRegistration(tr.TimeRegId);

            if (wsObj.Success)
                GetTimeRegistrations();
        }

        #endregion

        private void btnAdminControls_click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.ShowDialog();
        }
    }
}
