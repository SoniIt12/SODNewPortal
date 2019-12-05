using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Model;
using OfficeOpenXml;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System.Data.Entity.Infrastructure;
using System.Configuration;
using System.Text;
using SOD.CommonWebMethod;
using SOD.EmailNotification;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SOD.Controllers
{

    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class cityController : Controller, IActionFilter, IExceptionFilter
    {
        // GET: City
        private  IcityMasterRepository _cityMasterRepository;
        public cityController()
        {
            _cityMasterRepository = new cityMasterRepository(new SodEntities());            
        }

        public ActionResult citymaster()
        {
            return View();
        }


        public ActionResult viewcitymaster()
        {
            return View();
        }  

        /// <summary>
        /// get city list data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCityListData()
        {
            var s = _cityMasterRepository.GetCityListData();
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// get hotel info by id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCityInfoById()
        {
            var id = Request.QueryString["Id"].ToString();
            var s = _cityMasterRepository.GetCityInfoById(id);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// update city master data
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateCityList(List<SodCityCodeMasterModels> elist)
        {
            var s = _cityMasterRepository.UpdateCityList(elist);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Add city list
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddNewCity(List<SodCityCodeMasterModels> elist)
        {
            var s = _cityMasterRepository.AddNewCity(elist);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

    }
}