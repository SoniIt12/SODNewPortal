using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Script.Serialization;

namespace SOD.Scheduler
{
    public class SyncData
    {

        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly IAdminRepository _adminRepository;

        public SyncData()
        {
            this._adminRepository = new AdminRepository(new SodEntities());
        }

        /// <summary>
        /// Sink data with MDM API
        /// </summary>
        /// <returns></returns>
        public async void syncDatas()
        {
            string token = string.Empty;
            var eList = new List<EmdmAPIModels>();
            try
            {
                var auth_url = ConfigurationManager.AppSettings["emdm_Authenticate"].ToString().Trim();
                var api_url = ConfigurationManager.AppSettings["emdm_GetEmpdetails"].ToString().Trim();

                var api_username = ConfigurationManager.AppSettings["emdm_username"].ToString().Trim();
                var api_pwd = ConfigurationManager.AppSettings["emdm_pwd"].ToString().Trim();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(auth_url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // HTTP POST                
                    var body = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Username", api_username), 
                        new KeyValuePair<string, string>("Password", api_pwd)  
                    };
                    var content = new FormUrlEncodedContent(body);
                    HttpResponseMessage response = await client.PostAsync("Token", content);
                    var header = response.Headers;
                    IEnumerable<string> value;// = header.GetValues("Token");
                    if (header.TryGetValues("Token", out value))
                    {
                        token = value.First();
                    }
                }

                Dictionary<string, object> dList = new Dictionary<string, object>();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Token", token);
                    var values = new List<KeyValuePair<string, string>>();
                    values.Add(new KeyValuePair<string, string>("Token", token));
                    var content = new FormUrlEncodedContent(values);
                    var response =await  client.PostAsync(api_url, content);
                    var responseString =await  response.Content.ReadAsStringAsync();

                    var js = responseString.Replace("\\", "");
                    js = js.Substring(1);
                    js = js.Substring(0, js.Length - 1);
                    var jss = new JavaScriptSerializer() { MaxJsonLength = 86753090 };
                    dList = jss.Deserialize<Dictionary<string, object>>(js);
                    ArrayList aList = (ArrayList)dList["Data"];
                    foreach (var l in aList)
                    {
                        Dictionary<string, object> dlst = l as Dictionary<string, object>;
                        string strList = Newtonsoft.Json.JsonConvert.SerializeObject(dlst);
                        EmdmAPIModels emp = Newtonsoft.Json.JsonConvert.DeserializeObject<EmdmAPIModels>(strList);
                        eList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            var s = _adminRepository.SaveEMDMAPI_EmployeeInfo(eList);
        }
    }
}