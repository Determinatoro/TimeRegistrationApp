using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRegistrationApp.Webservice
{
    public class Customer
    {
        private int customerId;
        private string name;

        public Customer(int customerId, string name)
        {
            this.customerId = customerId;
            this.name = name;
        }

        public Customer()
        {

        }

        public int CustomerId
        {
            get
            {
                return customerId;
            }

            set
            {
                customerId = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
    }
}
