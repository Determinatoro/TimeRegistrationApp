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
    /// Interaction logic for AdminCustomerWindow.xaml
    /// </summary>
    public partial class AdminCustomerWindow : Window
    {
        public AdminCustomerWindow()
        {
            InitializeComponent();

            WebserviceObject wsObj = WebserviceCalls.GetCustomers();

            List<Customer> customersList = new List<Customer>();

            if (wsObj.Success)
            {
                foreach (Customer obj in (List<object>)wsObj.Response)
                    customersList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(customersList);

            dgUsers.ItemsSource = oList;
        }
    }
}
