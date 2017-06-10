using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TimeRegistrationApp.Webservice
{
    public static class WebserviceCalls
    {
        #region VARIABLES

        // Webservice URL
        private static string webservice = "http://jako3498.web.techcollege.dk/TimeRegistration.asmx/";
        // Web client used to call webservice
        private static WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };

        #endregion

        #region WEBSERVICE CALL AND PARSE METHODS

        /***********************************************************/
        // Call webservice method and get JSON back
        /***********************************************************/
        private static WebserviceObject CallWebservice(string method)
        {
            string json = "";

            try
            {
                using (wc)
                    json = wc.DownloadString(webservice + method);
            }
            catch (Exception e)
            {
                json = e.Message;
                return new WebserviceObject(false, json);
            }

            return new WebserviceObject(true, json);
        }

        /***********************************************************/
        // Parse dictionary into object
        /***********************************************************/
        private static Object GetObject(this Dictionary<string, object> dict, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key);
                if (prop == null) continue;

                object value = kv.Value;
                if (value is Dictionary<string, object>)
                    value = GetObject((Dictionary<string, object>)value, prop.PropertyType); 

                prop.SetValue(obj, value, null);
            }
            return obj;
        }

        /***********************************************************/
        // Get data from webservice call either a list or an object
        /***********************************************************/
        private static WebserviceObject GetWebserviceObject(string json, Type type)
        {
            WebserviceObject wsObj = new JavaScriptSerializer().Deserialize<WebserviceObject>(json);

            if (wsObj.Response as object[] != null)
            {
                List<object> tempList = new List<object>();

                foreach (var item in (object[])wsObj.Response)
                    tempList.Add(GetObject((Dictionary<string, object>)item, type));

                if (wsObj.Success)
                    wsObj.Response = tempList;
            }
            else
            {
                if (wsObj.Success)
                    wsObj.Response = GetObject((Dictionary<string, object>)wsObj.Response, type);
            }

            return wsObj;
        }

        #endregion

        #region WEBSERVICE CALLS

        /***********************************************************/
        // CHECK LOGIN - Username, Password
        /***********************************************************/
        public static WebserviceObject CheckLogin(string username, string password)
        {
            WebserviceObject wsObj = CallWebservice(string.Format("CheckLogin?Username={0}&Password={1}", username, password));            

            if (wsObj.Success)
                wsObj = GetWebserviceObject((string)wsObj.Response, typeof(User));

            return wsObj;
        }

        /***********************************************************/
        // GetUsers - No parameters
        /***********************************************************/
        public static WebserviceObject GetUsers()
        {
            WebserviceObject wsObj = CallWebservice("GetUsers");

            if (wsObj.Success)
                wsObj = GetWebserviceObject((string)wsObj.Response, typeof(User));

            return wsObj;
        }



        #endregion
    }
}
