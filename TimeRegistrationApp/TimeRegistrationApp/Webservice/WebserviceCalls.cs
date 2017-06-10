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
        private static string webservice = "http://jako3498.web.techcollege.dk/TimeRegistration.asmx/";
        private static WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };

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

        private static Object GetObject(this Dictionary<string, object> dict, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key);
                if (prop == null) continue;

                object value = kv.Value;
                if (value is Dictionary<string, object>)
                    value = GetObject((Dictionary<string, object>)value, prop.PropertyType); // <= This line

                prop.SetValue(obj, value, null);
            }
            return obj;
        }

        private static WebserviceObject GetWebserviceObject(string json, bool isListResponse, Type type)
        {
            WebserviceObject wsObj = new JavaScriptSerializer().Deserialize<WebserviceObject>(json);

            if (isListResponse)
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

        /***********************************************************/
        // CHECK LOGIN - Username and password as input
        /***********************************************************/
        public static WebserviceObject CheckLogin(string username, string password)
        {
            WebserviceObject wsObj = CallWebservice(string.Format("CheckLogin?Username={0}&Password={1}", username, password));            

            if (wsObj.Success)
                wsObj = GetWebserviceObject((string)wsObj.Response, false, typeof(User));

            return wsObj;
        }

        /***********************************************************/
        // GetUsers - No parameters
        /***********************************************************/
        public static WebserviceObject GetUsers()
        {
            WebserviceObject wsObj = CallWebservice("GetUsers");

            if (wsObj.Success)
                wsObj = GetWebserviceObject((string)wsObj.Response, true, typeof(User));

            return wsObj;
        }
    }
}
