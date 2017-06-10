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

        private static Object GetObject(this Dictionary<string, object> dict, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key);
                if (prop == null) continue;

                object value = kv.Value;
                if (value is Dictionary<string, object>)
                {
                    value = GetObject((Dictionary<string, object>)value, prop.PropertyType); // <= This line
                }

                prop.SetValue(obj, value, null);
            }
            return obj;
        }

        //CheckLogin
        public static WebserviceObject CheckLogin(string username, string password)
        {
            string json = "";

            try
            {
                using (WebClient wc = new WebClient())
                    json = wc.DownloadString(string.Format(webservice + "CheckLogin?Username={0}&Password={1}", username, password));
            }
            catch (Exception e) { json = e.Message; }

            WebserviceObject wsObj = new JavaScriptSerializer().Deserialize<WebserviceObject>(json);

            if (wsObj.Success)
            {
                User user = (User)GetObject((Dictionary<string, object>)wsObj.Response, typeof(User));
                wsObj.Response = user;
            }

            return wsObj;
        }
    }
}
