using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRegistrationApp.Webservice
{
    public class Order
    {
        private int orderId;
        private string orderName;
        private string description;
        private string customerName;        
        private List<object> rolesList;

        public Order()
        {

        }

        public int OrderId
        {
            get
            {
                return orderId;
            }

            set
            {
                orderId = value;
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
            }
        }

        public List<object> RolesList
        {
            get
            {
                return rolesList;
            }

            set
            {
                if (rolesList == null || rolesList.OfType<Role>().Count() != rolesList.Count)
                {
                    List<Role> temp = new List<Role>();

                    foreach (var item in value)
                        temp.Add((Role)WebserviceCalls.GetObject((Dictionary<string, object>)item, typeof(Role)));

                    rolesList = new List<object>();

                    foreach (Role item in temp)
                        rolesList.Add(item);                    
                }
                else
                    rolesList = value;
            }
        }

        public string Roles
        {
            get
            {
                if (rolesList.OfType<Role>().Count() == rolesList.Count)
                {
                    string roles = "";

                    foreach (Role item in rolesList)
                    {
                        if (rolesList.IndexOf(item) != rolesList.Count - 1)
                            roles += item.Name + ", ";
                        else
                            roles += item.Name; 
                    }

                    return roles;
                }
                else
                {
                    return "";
                }
            }          
        }
    }
}
