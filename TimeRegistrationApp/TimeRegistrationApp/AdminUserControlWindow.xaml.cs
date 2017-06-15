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
    /// Interaction logic for AdminUserControlWindow.xaml
    /// </summary>
    public partial class AdminUserControlWindow : Window
    {
        public AdminUserControlWindow(User user)
        {
            InitializeComponent();

            WebserviceObject wsObj = WebserviceCalls.GetOrders(user.UserId);

            List<Order> orderList = new List<Order>();

            if (wsObj.Success)
            {
                foreach (Order obj in (List<object>)wsObj.Response)
                    orderList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(orderList);

            dgOrders.ItemsSource = oList;
        }

        private void dgOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            ObservableCollection<object> list = (ObservableCollection<object>)dgOrders.ItemsSource;

            Order order = (Order)list[row.GetIndex()];
        }
    }
}
