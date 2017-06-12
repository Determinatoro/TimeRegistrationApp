using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRegistrationApp.Webservice
{
    public class Order : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private int orderId;
        private string orderName;
        private string description;
        private string customerName;
        private string roleName;

        public int OrderId
        {
            get
            {
                return orderId;
            }

            set
            {
                orderId = value;
                NotifyPropertyChanged("OrderId");
            }
        }

        public string OrderName
        {
            get
            {
                return orderName;
            }

            set
            {
                orderName = value;
                NotifyPropertyChanged("OrderName");
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public string CustomerName
        {
            get
            {
                return customerName;
            }

            set
            {
                customerName = value;
                NotifyPropertyChanged("CustomerName");
            }
        }

        public string RoleName
        {
            get
            {
                return roleName;
            }

            set
            {
                roleName = value;
                NotifyPropertyChanged("RoleName");
            }
        }

        public Order(int orderId, string orderName, string description, string customerName, string roleName)
        {
            this.OrderId = orderId;
            this.OrderName = orderName;
            this.Description = description;
            this.CustomerName = customerName;
            this.RoleName = roleName;
        }

        public Order()
        {

        }
    }
}