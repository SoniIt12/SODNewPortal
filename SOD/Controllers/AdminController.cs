using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Script.Serialization;
using System.Collections;
using System.Data.Entity.Infrastructure;
using System.Configuration;

namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class AdminController : Controller, IActionFilter, IExceptionFilter
    {
        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly IAdminRepository _adminRepository;
        private readonly IHdRepository _hdRepositorty;
        public static int ctr = 0;

        public AdminController()
        {
            this._adminRepository = new AdminRepository(new SodEntities());
            this._hdRepositorty = new HdRepository(new SodEntities());
        }


        #region "View"

        /// <summary>
        /// Open Index View
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //if (HttpContext.Request.Cookies["loginCookie"] == null)
            //    return RedirectToAction("Login", "Login");
            return View();
        }

        /// <summary>
        /// Allocate/Entry View for Sod Approver 
        /// </summary>
        /// <returns></returns>
        public ActionResult AllocateApprover()
        {
            ViewData["DepartmentList"] = _adminRepository.GetDepartmentList();
            return View();
        }


        // GET: Get Employee Data
        public ActionResult employeeDetails()
        {
            return View();
        }


        /// <summary>
        /// View Sod Approver List
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        [OutputCache(Duration = 0)]
        public ActionResult SodApproverList(int? deptId)
        {
            return View();
        }


        /// <summary>
        /// Sink API Data View
        /// </summary>
        /// <returns></returns>
        public ActionResult sinkDataemdmapi()
        {
            return View();
        }


        /// <summary>
        /// Sink API Data View
        /// </summary>
        /// <returns></returns>
        public ActionResult syncDynamicSecapi()
        {
            return View();
        }


        /// <summary>
        /// Get Department Json Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult BindDepartment()
        {
            return Json(_adminRepository.GetDepartmentList(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Department Json Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult BindDepartmentVerticals()
        {
            return Json(_adminRepository.GetDepartmentVerticalList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Department Json Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult BindDeptVerticals()
        {
            return Json(_adminRepository.GetDeptVerticals(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Sod Approver List
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SodApproverLists(int dept)
        {
            dept = 0;
            if (Request.QueryString[0] != "")
                dept = Convert.ToInt32(Request.QueryString["dept"].ToString());
            return Json(_adminRepository.GetSodApproverList(dept, 2), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Allocate Blanket Approver Entry View
        /// </summary>
        /// <returns></returns>
        public ActionResult AllocateBlanketApprover()
        {
            return View();
        }


        /// <summary>
        /// View Sod Blanket Approver List
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public ActionResult SodBlanketApproverList(int? deptId)
        {
            ViewData["SodBlanketList"] = _adminRepository.GetSodBlanketApproverList(deptId, 1);
            ViewData["DepartmentList"] = _adminRepository.GetDepartmentList();
            return View();
        }


        /// <summary>
        /// Return View for CXO Dept Mapping.
        /// </summary>
        /// <returns></returns>
        public ActionResult Cxodeptmapping()
        {
            return View();
        }

        #endregion


        /// <summary>
        /// Get Employee Designation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDesignationList()
        {
            var id = Convert.ToInt32(Request.QueryString["ddl"].ToString());
            return Json(_adminRepository.GetDesignationsList(id), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Employee Common Info
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeList(int id)
        {
            return Json(_adminRepository.GetEmployeeList(id), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Employee Common Info
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeInfo()
        {
            string empcode = Request.QueryString[0].ToString().Trim();
            return Json(_adminRepository.GetEmployeeInfo(empcode, 2), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Approver in database
        /// </summary>
        /// <param name="sodApproverList"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveApprover(List<SodApproverModels> sodApproverList)
        {
            var counter = 0;
            var response = new StringBuilder();
            var a = _adminRepository.SaveSodApprover(sodApproverList);
            if (a == 0)
                response.Append("Selected Department  already exists. " + "<br/>");
            else if (a > 0)
                response.Append("Allocated Department(s)  saved successfully." + "<br/>");
            else
                response.Append("Error : facing error in request processing.");

            return Json(response.ToString(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Save Employee Id for Allocate Blanket Booking Approvals
        /// </summary>
        /// <param name="EmpId"></param>
        [HttpPost]
        public ActionResult AllocateBlanketApp(string EmpId)
        {
            var blanketApproval = new SodBlanketApprovalModels
            {
                EmployeeId = Convert.ToInt32(EmpId),
                IsActive = 1
            };
            var s = _adminRepository.SaveSodBlanketApprover(blanketApproval);
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Allocate Department to Select CXO Level
        /// </summary>
        /// <param name="EmpId"></param>
        [HttpPost]
        public ActionResult AllocateCxoDeptMappingList(string strval)
        {
            var cxodeptMapping = new SodCXODeptMappingModels
            {
                CXOName = strval.Split(',')[0].Trim(),
                AllocatedDeptId = Convert.ToInt32(strval.Split(',')[1].Trim()),
                IsActive = 1
            };
            var s = _adminRepository.AllocateCxoDeptMappingList(cxodeptMapping);
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Update Status for Sod Approval List
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateStatus(string EmpId)
        {
            var s = 1;// _adminRepository.SaveSodBlanketApprover(blanketApproval);
            return s >= 1 ? "Save successfully" : string.Empty;
        }


        /// <summary>
        /// Remove Blanket Approver Rights
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult RemoveApproverRight(string did)
        {
            var s = 0;
            if (Request.QueryString["did"] != null)
            {
                var approverId = Request.QueryString["did"].ToString().Trim();
                s = _adminRepository.RemoveApproverRightByID(Convert.ToInt32(approverId));
            }
            return Json(s >= 1 ? "Save successfully" : string.Empty, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Remove Blanket Approver Rights
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult RemoveCxoDeptMappingList(string dept)
        {
            var s = 0;
            if (Request.QueryString["dept"] != null)
            {
                var deptId = Request.QueryString["dept"];
                s = _adminRepository.RemoveCxoDeptMappingList(Convert.ToInt32(deptId));
            }
            return Json(s >= 1 ? "Removed successfully" : string.Empty, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Remove Blanket Approver Rights
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult RemoveBlanketApproverRights(string EmpId)
        {
            var s = _adminRepository.RemoveBlanketApproverRights(Convert.ToInt32(Request.QueryString["EmpId"].ToString()));
            return Json(s >= 1 ? "Removed successfully" : string.Empty, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get CXO Department Mapping List
        /// </summary>
        /// <param name="cxo"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetcxoDeptMappingList(string cxo)
        {
            return Json(_adminRepository.GetcxoDeptMappingList(cxo, 1), JsonRequestBehavior.AllowGet);           
        }

        /// <summary>
        /// Assign User Role
        /// </summary>
        /// <returns></returns>
        public ActionResult AssignRole()
        {
            return View();
        }

        /// <summary>
        /// Sink data with MDM API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> sinkData()
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
                        new KeyValuePair<string, string>("username", api_username),
                        new KeyValuePair<string, string>("password", api_pwd)
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
                    var response = await client.PostAsync(api_url, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    //******Satyam****************************************
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
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Sync Dynamic Sector API Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult syncDynamicSec()
        {
            var s = CommonWebMethod.CommonWebMethods.GetOriginStationSync();
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get All Employee Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeViewDetails(string EmpId)
        {
            var s = Json(_hdRepositorty.GetSodEmployeeViewDetails(EmpId, 3), JsonRequestBehavior.AllowGet);
            s.MaxJsonLength = int.MaxValue;
            return s;
        }

        public JsonResult ViewUserRoles(string Empcode)
        {
            var data = _adminRepository.GetEmpRoles(Empcode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Save update Emp Role in SODUserRoleAllocation
        public JsonResult UpdateEmpRoleInfo(string empcode, string newroles)
        {
            return Json(_adminRepository.SaveEmpRole(empcode, newroles) != 0 ? 1 : 0, JsonRequestBehavior.AllowGet);
        }

        //Delete Emp Role in SODUserRoleAllocation
        public JsonResult DeleteEmpRoleinfo()
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        //Menu Rights View 
        public ActionResult MenuRights()
        {
            return View();
        }

        /// <summary>
        /// Bind SOD Role Master
        /// </summary>
        /// <returns></returns>
        public JsonResult BindSODRoleMaster()
        {
            return Json(_adminRepository.GetAllRoles(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Load All Menus
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadAllMenus()
        {
            return Json(_adminRepository.GetSODMenus(Convert.ToInt32(Request.QueryString["Role"])), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save SOD Role Menus ID
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="Menuids"></param>
        /// <returns></returns>
        public JsonResult SaveSODRoleMenusID(int RoleID, string Menuids)
        {
            return Json(_adminRepository.SaveSODMenuRights(RoleID, Menuids) != 0 ? 1 : 0, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Action Filter Method
        /// </summary>
        /// <param name="filterContext"></param>
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            //If Required : Need to Implement
        }


        /// <summary>
        /// On OnActionExecuting
        /// </summary>
        /// <param name="filterContext"></param>
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["EmpId"] == null)
            {
                Response.Clear();
                Response.Redirect("../Login/UserAccount");
            }
        }


        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/AdminController.cs");
        }



        /// <summary>
        /// Get Admin URL
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAdminUrl()
        {
            try
            {
                if (Session["formtype"] != null)
                {
                    var s = CommonWebMethod.CommonWebMethods.GetXMLNodeRights(Session["formtype"].ToString().Trim());
                    return Json(CommonWebMethod.CommonWebMethods.GetXMLNodeRights(Session["formtype"].ToString().Trim()), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (NullReferenceException exc)
            {
                return Json("Nul", JsonRequestBehavior.AllowGet);
            }
        }
    }
}