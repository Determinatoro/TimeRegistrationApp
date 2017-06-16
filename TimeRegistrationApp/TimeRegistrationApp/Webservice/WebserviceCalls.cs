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
        private static Object GetObject(this Dictionary<string, object> dict, Type type, Type secondary)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key);
                if (prop == null) continue;

                object value = kv.Value;
                if (value is Dictionary<string, object>)
                    value = GetObject((Dictionary<string, object>)value, prop.PropertyType);

                if (value is Object[])
                    value = GetObject((Dictionary<string, object>)value, prop.PropertyType);

                prop.SetValue(obj, value, null);
            }
            return obj;
        }

        /***********************************************************/
        // Parse dictionary into object
        /***********************************************************/
        public static Object GetObject(Dictionary<string, object> dict, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key);
                if (prop == null) continue;

                object value = kv.Value;
                if (value is Dictionary<string, object>)
                    value = GetObject((Dictionary<string, object>)value, prop.PropertyType);

                if (value is Object[])
                {
                    List<object> objectList = new List<object>();

                    foreach (var item in (Object[])value)
                    {
                        objectList.Add((Dictionary<string, object>)item);
                    }

                    value = objectList;
                }

                prop.SetValue(obj, value, null);
            }
            return obj;
        }

        /***********************************************************/
        // Get data from webservice call either a list or an object
        /***********************************************************/
        public static WebserviceObject GetWebserviceObject(string json, Type type)
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

        /***********************************************************/
        // CREATE CUSTOMER - name
        /***********************************************************/
        public static WebserviceObject CreateCustomer(string name)
        {
            return GetWebserviceObjectAfterCall(string.Format("CreateCustomer?name={0}", name), typeof(Customer));
        }

        /***********************************************************/
        // CREATE ORDER - userId, name, description, customerId
        /***********************************************************/
        public static WebserviceObject CreateOrder(int userId, string name, string description, int customerId)
        {
            return GetWebserviceObjectAfterCall(string.Format("CreateOrder?userId={0}&name={1}&description={2}&customerId={3}", userId, name, description, customerId), typeof(Order));
        }

        /***********************************************************/
        // CREATE ORDER ROLE - orderId, userId, name, roleId
        /***********************************************************/
        public static WebserviceObject CreateOrderRole(int orderId, int userId, int roleId)
        {
            return GetWebserviceObjectAfterCall(string.Format("CreateOrderRole?orderId={0}&userId={1}&roleId={2}", orderId, userId, roleId), typeof(OrderRole));
        }

        /***********************************************************/
        // CREATE ROLE - name
        /***********************************************************/
        public static WebserviceObject CreateRole(string name)
        {
            return GetWebserviceObjectAfterCall(string.Format("CreateRole?name={0}", name), typeof(Role));
        }

        /***********************************************************/
        // CREATE USER - firstName, lastName, admin, password
        /***********************************************************/
        public static WebserviceObject CreateUser(string firstName, string lastName, bool admin, string password)
        {
            return GetWebserviceObjectAfterCall(string.Format("CreateUser?firstName={0}&lastName={1}&admin={2}&password={3}", firstName, lastName, admin, password), typeof(User));
        }

        /***********************************************************/
        // DELETE CUSTOMER - customerId
        /***********************************************************/
        public static WebserviceObject DeleteCustomer(int customerId)
        {
            return GetWebserviceObjectAfterCall(string.Format("DeleteCustomer?customerId={0}", customerId), typeof(Customer));
        }

        /***********************************************************/
        // DELETE ORDER - orderId
        /***********************************************************/
        public static WebserviceObject DeleteOrder(int orderId)
        {
            return GetWebserviceObjectAfterCall(string.Format("DeleteOrder?orderId={0}", orderId), typeof(Order));
        }

        /***********************************************************/
        // DELETE ORDER ROLE - orderroleId
        /***********************************************************/
        public static WebserviceObject DeleteOrderRole(int orderRoleId, int orderId)
        {
            return GetWebserviceObjectAfterCall(string.Format("DeleteOrderRole?orderRoleId={0}&orderId={1}", orderRoleId, orderId), typeof(OrderRole));
        }

        /***********************************************************/
        // UPDATE ORDER ROLE - orderroleId
        /***********************************************************/
        public static WebserviceObject UpdateOrderRole(int orderRoleId, int orderId, int userId, int roleId)
        {
            return GetWebserviceObjectAfterCall(string.Format("UpdateOrderRole?orderRoleId={0}&orderId={1}&userId={2}&roleId={3}", orderRoleId, orderId, userId, roleId), typeof(OrderRole));
        }

        /***********************************************************/
        // DELETE ROLE - roleId
        /***********************************************************/
        public static WebserviceObject DeleteRole(int roleId)
        {
            return GetWebserviceObjectAfterCall(string.Format("DeleteRole?roleId={0}", roleId), typeof(Role));
        }

        /***********************************************************/
        // DELETE USER - userId
        /***********************************************************/
        public static WebserviceObject DeleteUser(int userId)
        {
            return GetWebserviceObjectAfterCall(string.Format("DeleteUser?userId={0}", userId), typeof(User));
        }

        /***********************************************************/
        // GET CUSTOMERS - orderId
        /***********************************************************/
        public static WebserviceObject GetOrderRoles(int orderId)
        {
            return GetWebserviceObjectAfterCall(string.Format("GetOrderRoles?orderId={0}", orderId), typeof(OrderRole));
        }

        /***********************************************************/
        // GET ROLES - No Parameters
        /***********************************************************/
        public static WebserviceObject GetRoles()
        {
            return GetWebserviceObjectAfterCall(string.Format("GetRoles"), typeof(Role));
        }

        /***********************************************************/
        // GET USER - userId
        /***********************************************************/
        public static WebserviceObject GetUser(int userId)
        {
            return GetWebserviceObjectAfterCall(string.Format("GetUser", userId), typeof(User));
        }

        /***********************************************************/
        // RESET PASSWORD - userId, password
        /***********************************************************/
        public static WebserviceObject ResetPassword(int userId, string password)
        {
            return GetWebserviceObjectAfterCall(string.Format("ResetPassword?userId={0}&password={1}", userId, password), typeof(User));
        }

        /***********************************************************/
        // UPDATE ORDER ROLE - orderId, userId, roleId
        /***********************************************************/
        public static WebserviceObject OrderRole(int orderId, int userId, int roleId)
        {
            return GetWebserviceObjectAfterCall(string.Format("UpdateOrderRole?orderId={0}?&userId={1}&roleId={2}", orderId, userId, roleId), typeof(OrderRole));
        }

        /***********************************************************/
        // UPDATE USER - userId, firstName, lastName, admin
        /***********************************************************/
        public static WebserviceObject UpdateUser(int userId, string firstName, string lastName, bool admin)
        {
            return GetWebserviceObjectAfterCall(string.Format("UpdateUser?userId={0}&firstName={1}&lastName={2}&admin={3}", userId, firstName, lastName, admin), typeof(User));
        }

        #endregion
    }
}
