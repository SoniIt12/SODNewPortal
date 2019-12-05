using System;
using System.Collections.Generic;
using SOD.Services.EntityFramework;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Script.Serialization;
using System.Collections;
using System.Net.Http;
using SOD.Model;

namespace SOD.Services.API
{
    public class APIServices
    {

        /// <summary>
        /// Get EMDM API Data
        /// </summary>
        /// <returns></returns>
        
        //public static async Task<List<EmdmAPIModels>> GetEMDMAPIData()
        //{
        //    string token = string.Empty;
        //    var eList = new List<EmdmAPIModels>();
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            // TODO - Send HTTP requests   
        //            //client.BaseAddress = new Uri("http://emdmapi-prod.azurewebsites.net/api/Authenticate/");//Production
        //            client.BaseAddress = new Uri("http://emdmapi-uat.azurewebsites.net/api/Authenticate/");//UAT
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            // HTTP POST                
        //            var body = new List<KeyValuePair<string, string>>
        //            {
        //                new KeyValuePair<string, string>("Username", "spicejet"),
        //                new KeyValuePair<string, string>("Password", "spicejet")                    
        //            };
        //            var content = new FormUrlEncodedContent(body);
        //            HttpResponseMessage response = await client.PostAsync("Token", content);
        //            var header = response.Headers;
        //            IEnumerable<string> value;// = header.GetValues("Token");
        //            if (header.TryGetValues("Token", out value))
        //            {
        //                token = value.First();
        //            }
        //        }
        //        Dictionary<string, object> dList = new Dictionary<string, object>();
        //        using (var client = new HttpClient())
        //        {
        //            //var url = "http://emdmapi-prod.azurewebsites.net/api/GetEmployeeDetails/";//Production
        //            var url = "http://emdmapi-uat.azurewebsites.net/api/GetEmployeeDetails/";//UAT
        //            client.DefaultRequestHeaders.Add("Token", token);
        //            var values = new List<KeyValuePair<string, string>>();
        //            values.Add(new KeyValuePair<string, string>("Token", token));
        //            var content = new FormUrlEncodedContent(values);
        //            var response = await client.PostAsync(url, content);
        //            var responseString = await response.Content.ReadAsStringAsync();
        //            //var jsons = Json(responseString, JsonRequestBehavior.AllowGet);
        //            var js = responseString.Replace("\\", "");
        //            js = js.Substring(1);
        //            js = js.Substring(0, js.Length - 1);
        //            var jss = new JavaScriptSerializer() { MaxJsonLength = 86753090 };
        //            dList = jss.Deserialize<Dictionary<string, object>>(js);
        //            //eList=dList["Data"] as List<EmdmAPIModels>;
        //            ArrayList aList = (ArrayList)dList["Data"];
        //            foreach (var l in aList)
        //            {
        //                //var emp = new EmdmAPIModels();
        //                Dictionary<string, object> dlst = l as Dictionary<string, object>;
        //                string strList = Newtonsoft.Json.JsonConvert.SerializeObject(dlst);
        //                EmdmAPIModels emp = Newtonsoft.Json.JsonConvert.DeserializeObject<EmdmAPIModels>(strList);
        //                eList.Add(emp);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return eList;
        //}
    }
}
