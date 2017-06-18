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
    /// Interaction logic for TimeRegistratedWindow.xaml
    /// </summary>
    public partial class TimeRegistratedWindow : Window
    {
        private Order order;

        public TimeRegistratedWindow(Order order)
        {
            InitializeComponent();

            this.order = order;

            GetTimeRegistratedOnOrder();
        }

        public void GetTimeRegistratedOnOrder()
        {
            WebserviceObject wsObj = WebserviceCalls.GetTimeRegistratedPerUserOnOrder(order.OrderId);

            List<TimeRegistrated> timeRegistratedList = new List<TimeRegistrated>();

            if (wsObj.Success)
            {
                foreach (TimeRegistrated obj in (List<object>)wsObj.Response)
                    timeRegistratedList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(timeRegistratedList);

            dgTimeRegistratedOnOrder.ItemsSource = oList;
        }
    }
}
