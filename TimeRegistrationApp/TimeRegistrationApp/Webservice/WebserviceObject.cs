using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRegistrationApp.Webservice
{
    public class WebserviceObject
    {
        private bool success;
        private object response;

        public WebserviceObject(bool success, object response)
        {
            this.success = success;
            this.response = response;
        }

        public WebserviceObject()
        {

        }

        public bool Success
        {
            get { return success; }
            set { success = value; }
        }

        public object Response
        {
            get { return response; }
            set { response = value; }
        }
    }
}
