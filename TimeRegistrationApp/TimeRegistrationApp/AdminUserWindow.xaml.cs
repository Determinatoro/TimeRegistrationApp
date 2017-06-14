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
    /// Interaction logic for AdminUserWindow.xaml
    /// </summary>
    public partial class AdminUserWindow : Window
    {
        public AdminUserWindow()
        {
            InitializeComponent();

            WebserviceObject wsObj = WebserviceCalls.GetUsers();

            List<User> usersList = new List<User>();

            if (wsObj.Success)
            {
                foreach (User obj in (List<object>)wsObj.Response)
                    usersList.Add(obj);
            }
            else
                MessageBox.Show(wsObj.Response.ToString());

            ObservableCollection<object> oList;
            oList = new ObservableCollection<object>(usersList);

            dgUsers.ItemsSource = oList;
        }
    }
}
