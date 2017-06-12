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
        public MainWindow(User user)
        {
            InitializeComponent();

            Title = string.Format("Welcome {0} {1}", user.FirstName, user.LastName);

            if (!user.Admin)
                btnAdminControls.Visibility = Visibility.Hidden;


            WebserviceObject wsObj = WebserviceCalls.GetOrders(user.UserId);

            List<Order> orderList = new List<Order>();

            if (wsObj.Success)
            {

                foreach (Order obj in (List<object>)wsObj.Response)
                    orderList.Add(obj);
            }
            else MessageBox.Show(wsObj.Response.ToString());

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(orderList);

            lbOrders.DataContext = oList;

            Binding binding = new Binding();
            lbOrders.SetBinding(ListBox.ItemsSourceProperty, binding);
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
