using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
        /***********************************************************************/
        // VARIABLES
        /***********************************************************************/
        #region Variables

        private User user;
        private Order order;

        #endregion

        /***********************************************************************/
        // CONSTRUCTOR
        /***********************************************************************/
        #region Constructor

        public MainWindow(User user)
        {
            InitializeComponent();

            this.user = user;

            Title = string.Format("TimeRegistrationApp - Welcome {0} {1}", user.FirstName, user.LastName);

            // Hides admin controls if user is not admin
            if (!user.Admin)
                btnAdminControls.Visibility = Visibility.Hidden;

            // Sets default values for start- and endtime
            SetStartAndEndTime(DateTime.Now, DateTime.Today);
           
            tbStartTimeHour.MaxLength = 2;
            tbStartTimeMinutes.MaxLength = 2;

            // Get timeregistrations for the current user
            GetTimeRegistrations();
        }

        #endregion

        /***********************************************************************/
        // FUNCTIONS
        /***********************************************************************/
        #region Functions

        /***********************************************************/
        // Gets the timeregistrations for user logged in
        /***********************************************************/
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

        /***********************************************************/
        // Gets information about an order and shows the data on screen
        /***********************************************************/
        public void SetOrderId(Order order)
        {
            this.order = order;
            tbOrderId.Text = order.OrderId.ToString();
            lbDescription.Content = order.Description;
            lbRole.Content = order.Roles;
        }

        /***********************************************************/
        // Sets start- and endtime to the specified datetimes
        /***********************************************************/
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

        /***********************************************************************/
        // GENERAL
        /***********************************************************************/
        #region General

        /***********************************************************/
        // "Log out" button click
        /***********************************************************/
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        #endregion

        /***********************************************************************/
        // ORDERS
        /***********************************************************************/
        #region Orders

        /***********************************************************/
        // More icon besides Order ID textbox click
        /***********************************************************/
        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow ordersWindow = new OrdersWindow(this, user);
            ordersWindow.ShowDialog();
        }

        /***********************************************************/
        // Order ID textbox keydown
        /***********************************************************/
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

        #endregion

        /***********************************************************************/
        // TIMEREGISTRATIONS
        /***********************************************************************/
        #region TimeRegistrations

        /*****************************************************************/
        // TEXTBOX EVENTS
        /*****************************************************************/
        #region TextBox events

        /***********************************************************/
        // Used to only allow numbers to be written in the textbox
        /***********************************************************/
        private void tbTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /***********************************************************/
        // Used to disable copy, paste and cut in the textbox
        /***********************************************************/
        private void tbTime_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        /***********************************************************/
        // Tab between hour and minutes
        /***********************************************************/
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

        /***********************************************************/
        // Padleft number with '0'
        /***********************************************************/
        private void tbTime_Lostfocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;

            tb.Text = tb.Text.PadLeft(2, '0');
        }

        #endregion

        /*****************************************************************/
        // DATAGRID EVENTS
        /*****************************************************************/
        #region DataGrid events

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

        #endregion

        /*****************************************************************/
        // BUTTON EVENTS
        /*****************************************************************/
        #region Button events

        /***********************************************************/
        // "Start / Continue" click
        /***********************************************************/
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

        /***********************************************************/
        // "Stop" click
        /***********************************************************/
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

        /***********************************************************/
        // "Update" click
        /***********************************************************/
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Check if start- and endtime hours and minutes has been filled out
            if (tbStartTimeHour.Text == "" || tbStartTimeMinutes.Text == "" || tbEndTimeHour.Text == "" || tbEndTimeMinutes.Text == "")
            {
                MessageBox.Show("Please fill out hours and minutes");
                return;
            }

            // Get start time
            var dtStart = DateTime.Parse(dpStartTimeDate.SelectedDate.Value.ToString("dd-MM-yyyy"));
            TimeSpan ts = new TimeSpan(int.Parse(tbStartTimeHour.Text), int.Parse(tbStartTimeMinutes.Text), 0);
            dtStart += ts;

            // Get end time
            var dtEnd = DateTime.Parse(dpEndTimeDate.SelectedDate.Value.ToString("dd-MM-yyyy"));
            ts = new TimeSpan(int.Parse(tbEndTimeHour.Text), int.Parse(tbEndTimeMinutes.Text), 0);
            dtEnd += ts;

            // Check if end time is before start time
            if (dtEnd < dtStart)
            {
                MessageBox.Show("End time is before start time");
                return;
            }

            ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;

            TimeRegistration tr = null;

            // If no row is selected select first object
            if (dgTimeRegistrations.SelectedIndex == -1)
                tr = list[0];
            else
                tr = list[dgTimeRegistrations.SelectedIndex];

            // Update time registration
            var wsObj = WebserviceCalls.UpdateTimeRegistration(tr.TimeRegId, dtStart.ToString("yyyy-MM-dd'T'HH:mm:ss"), dtEnd.ToString("yyyy-MM-dd'T'HH:mm:ss"));
            
            if (wsObj.Success)
                GetTimeRegistrations();
            else
                MessageBox.Show(wsObj.Response.ToString());
        }

        /***********************************************************/
        // "Set" click
        /***********************************************************/
        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            // Check if an order has been selected
            if (order == null)
            {
                MessageBox.Show("Please select an order");
                return;
            }

            // Check if start- and endtime hours and minutes has been filled out
            if (tbStartTimeHour.Text == "" || tbStartTimeMinutes.Text == "" || tbEndTimeHour.Text == "" || tbEndTimeMinutes.Text == "")
            {
                MessageBox.Show("Please fill out hours and minutes");
                return;
            }

            // Get start time
            var dtStart = DateTime.Parse(dpStartTimeDate.SelectedDate.Value.ToString("dd-MM-yyyy"));
            TimeSpan ts = new TimeSpan(int.Parse(tbStartTimeHour.Text), int.Parse(tbStartTimeMinutes.Text), 0);
            dtStart += ts;

            // Get end time
            var dtEnd = DateTime.Parse(dpEndTimeDate.SelectedDate.Value.ToString("dd-MM-yyyy"));
            ts = new TimeSpan(int.Parse(tbEndTimeHour.Text), int.Parse(tbEndTimeMinutes.Text), 0);
            dtEnd += ts;

            // Check if endtime is before starttime
            if (dtEnd < dtStart)
            {
                MessageBox.Show("End time is before start time");
                return;
            }

            // Create time registration
            var wsObj = WebserviceCalls.CreateTimeRegistration(dtStart.ToString("yyyy-MM-dd'T'HH:mm:ss"), dtEnd.ToString("yyyy-MM-dd'T'HH:mm:ss"), user.UserId, order.OrderId);
            
            if (wsObj.Success)                
                GetTimeRegistrations();
            else
                MessageBox.Show(wsObj.Response.ToString());
        }

        /***********************************************************/
        // "Delete timeregistration" click
        /***********************************************************/
        private void btnDeleteTimeRegistration_Click(object sender, RoutedEventArgs e)
        {
            // Has there been selected a row
            if (dgTimeRegistrations.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a time registration");
                return;
            }

            // Get the selected row object
            ObservableCollection<TimeRegistration> list = (ObservableCollection<TimeRegistration>)dgTimeRegistrations.ItemsSource;

            var tr = list[dgTimeRegistrations.SelectedIndex];

            // Delete the object
            var wsObj = WebserviceCalls.DeleteTimeRegistration(tr.TimeRegId);
            
            if (wsObj.Success)                
                GetTimeRegistrations();
            else
                MessageBox.Show(wsObj.Response.ToString());
        }

        #endregion

        #endregion

        /***********************************************************************/
        // ADMIN
        /***********************************************************************/
        #region Admin

        /***********************************************************/
        // "Admin controls click
        /***********************************************************/
        private void btnAdminControls_click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.ShowDialog();
        }

        #endregion
    }
}
