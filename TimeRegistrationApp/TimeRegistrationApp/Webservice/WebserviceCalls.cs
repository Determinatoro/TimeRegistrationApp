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

            if (wsObj.Success)
            {
                if (wsObj.Response as object[] != null)
                {
                    List<object> tempList = new List<object>();

                    foreach (var item in (object[])wsObj.Response)
                        tempList.Add(GetObject((Dictionary<string, object>)item, type));

                    wsObj.Response = tempList;
                }
                else if (wsObj.Response as string != null)
                    wsObj.Response = wsObj.Response as string;
                else
                    wsObj.Response = GetObject((Dictionary<string, object>)wsObj.Response, type);
            }

            return wsObj;
        }
    
        private static WebserviceObject GetWebserviceObjectAfterCall(string query, Type type)
        {
            WebserviceObject wsObj = CallWebservice(query);

            if (wsObj.Success)
                wsObj = GetWebserviceObject((string)wsObj.Response, type);

            return wsObj;
        }

        #endregion

        #region WEBSERVICE CALLS

        /***********************************************************/
        // CHECK LOGIN - Username, Password
        /***********************************************************/
        public static WebserviceObject CheckLogin(string username, string password)
        {
            return GetWebserviceObjectAfterCall(string.Format("CheckLogin?Username={0}&Password={1}", username, password), typeof(User));
        }

        /***********************************************************/
        // GetUsers - No parameters
        /***********************************************************/
        public static WebserviceObject GetUsers()
        {
            return GetWebserviceObjectAfterCall("GetUsers", typeof(User));
        }

        /***********************************************************/
        // GetUsers - No parameters
        /***********************************************************/
        public static WebserviceObject GetCustomers()
        {
            return GetWebserviceObjectAfterCall("GetCustomers", typeof(Customer));
        }

        /***********************************************************/
        // GetOrders - userId
        /***********************************************************/
        public static WebserviceObject GetOrders(int userId)
        {
            return GetWebserviceObjectAfterCall(string.Format("GetOrders?userId={0}", userId), typeof(Order));            
        }

        /***********************************************************/
        // GetOrders - userId, orderId
        /***********************************************************/
        public static WebserviceObject GetOrder(int userId, int orderId)
        {
            return GetWebserviceObjectAfterCall(string.Format("GetOrder?userId={0}&orderId={1}", userId, orderId), typeof(Order));
        }

        /***********************************************************/
        // GetTimeRegistrations - userId
        /***********************************************************/
        public static WebserviceObject GetTimeRegistrations(int userId)
        {
            return GetWebserviceObjectAfterCall(string.Format("GetTimeRegistrations?userId={0}", userId), typeof(TimeRegistration));
        }

        /***********************************************************/
        // StartTimeRegistration - startTime, userId, orderId 
        /***********************************************************/
        public static WebserviceObject StartTimeRegistration(string startTime, int userId, int orderId)
        {
            return GetWebserviceObjectAfterCall(string.Format("StartTimeRegistration?startTime={0}&userId={1}&orderId={2}", startTime, userId, orderId), typeof(TimeRegistration));
        }

        /***********************************************************/
        // EndTimeRegistration - timeRegId, endTime 
        /***********************************************************/
        public static WebserviceObject EndTimeRegistration(int timeRegId, string endTime)
        {
            return GetWebserviceObjectAfterCall(string.Format("EndTimeRegistration?timeRegId={0}&endTime={1}", timeRegId, endTime), typeof(TimeRegistration));
        }

        /***********************************************************/
        // CreateTimeRegistration - startTime, endTime, userId, orderId 
        /***********************************************************/
        public static WebserviceObject CreateTimeRegistration(string startTime, string endTime, int userId, int orderId)
        {
            return GetWebserviceObjectAfterCall(string.Format("CreateTimeRegistration?startTime={0}&endTime={1}&userId={2}&orderId={3}", startTime, endTime, userId, orderId), typeof(TimeRegistration));
        }

        /***********************************************************/
        // DeleteTimeRegistration - timeRegId
        /***********************************************************/
        public static WebserviceObject DeleteTimeRegistration(int timeRegId)
        {
            return GetWebserviceObjectAfterCall(string.Format("DeleteTimeRegistration?timeRegId={0}", timeRegId), typeof(TimeRegistration));
        }

        /***********************************************************/
        // UpdateTimeRegistration - timeRegId, startTime, endTime
        /***********************************************************/
        public static WebserviceObject UpdateTimeRegistration(int timeRegId, string startTime, string endTime)
        {
            return GetWebserviceObjectAfterCall(string.Format("UpdateTimeRegistration?timeRegId={0}&startTime={1}&endTime={2}", timeRegId, startTime, endTime), typeof(TimeRegistration));
        }

        /***********************************************************/
        // SetNoteForTimeRegistration - timeRegId, note
        /***********************************************************/
        public static WebserviceObject SetNoteForTimeRegistration(int timeRegId, string note)
        {
            return GetWebserviceObjectAfterCall(string.Format("SetNoteForTimeRegistration?timeRegId={0}&note={1}", timeRegId, note), typeof(TimeRegistration));
        }

        #endregion
    }
}
