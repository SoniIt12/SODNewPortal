using System;
using System.Collections.Generic;
using System.IO;
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
    public class InclusionMasterController : Controller, IActionFilter, IExceptionFilter
    {
        private readonly IInclusionRepository _inclusionRepository;
        private readonly ITransportRepository _transportRepository;
        public InclusionMasterController()
        {
            _inclusionRepository = new InclusionRepository(new SodEntities());
            _transportRepository = new TransportRepository(new SodEntities());
        }

        /// <summary>
        /// Inclusion Master Main Form
        /// </summary>
        /// <returns></returns>
        public ActionResult InclusionMaster()
        {
            return View();
        }

        /// <summary>
        /// HOTEC Master main form
        /// </summary>
        /// <returns></returns>
        public ActionResult HotelMaster()
        {
            return View();
        }


        /// <summary>
        /// Only View Hotel Right
        /// </summary>
        /// <returns></returns>
        public ActionResult viewhotelmaster()
        {
            return View();
        }


        /// <summary>
        /// Inclusion Master Main Form
        /// </summary>
        /// <returns></returns>
        public ActionResult viewinclusionmaster()
        {
            return View();
        }

        /// <summary>
        /// get hotel inclusion list master
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHotelInclusionList()
        {
            var s = _inclusionRepository.GetHotelInclusionList();
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// get hotel list data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHotelListData()
        {
            var s = _inclusionRepository.GetHotelListData();
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// get inclusion info by id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetInclusionInfoById()
        {
            var id = Request.QueryString["Id"].ToString();
            var s = _inclusionRepository.GetInclusionInfoById(id);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// get hotel info by id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHotelInfoById()
        {
            var id = Request.QueryString["Id"].ToString();
            var s = _inclusionRepository.GetHotelInfoById(id);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// update inclusion master data
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateHotelInclusion(List<HotelInclusionMasterModels> elist)
        {
            var s = _inclusionRepository.UpdateHotelInclusion(elist);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// update hotel master data
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateHotelList(List<SodHotelListDataModels> elist, List<SodHotelPriceListMasterModels> plist)
        {
            var s = _inclusionRepository.UpdateHotelList(elist, plist);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Add inclusion list
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddHotelInclusion(List<HotelInclusionMasterModels> elist)
        {
            var s = _inclusionRepository.AddHotelInclusion(elist);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Add hotel list
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddNewHotel(List<SodHotelListDataModels> elist, List<SodHotelPriceListMasterModels> plist)
        {
            var s = _inclusionRepository.AddNewHotel(elist, plist);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// fill hotel names from database
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult HotelListData()
        {
            List<SelectListItem> hotelItems = new List<SelectListItem>();
            SodEntities entities = new SodEntities();
            var count = entities.SodHotelListDataModels.Count();
            for (int i = 0; i < count; i++)
            {
                hotelItems.Add(new SelectListItem
                {
                    Value = entities.SodHotelListDataModels.ToList()[i].HotelCode,
                    Text = entities.SodHotelListDataModels.ToList()[i].HotelName
                });
            }

            return Json(hotelItems);
        }

        /// <summary>
        /// fill city code names from database
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DropDownCityCodeData()
        {
            List<SelectListItem> hotelItems = new List<SelectListItem>();
            SodEntities entities = new SodEntities();
            var count = entities.SodHotelListDataModels.Count();
            for (int i = 0; i < count; i++)
            {
                hotelItems.Add(new SelectListItem
                {
                    Value = entities.SodHotelListDataModels.ToList()[i].City,
                    Text = entities.SodHotelListDataModels.ToList()[i].StationCode
                });
            }

            return Json(hotelItems);
        }

        /// <summary>
        /// find hotel inclusions
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult findHotelInclusions(string hotelcity, string hotelname)
        {
            var inclusionList = new List<HotelInclusionMasterModels>();
            inclusionList = _transportRepository.findHotelInclusions(hotelcity, hotelname);
            return Json(inclusionList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get hotel list by name and code
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindHotelListByNameAndCode(string hotelcity, string hotelname)
        {
            var list = new List<SodHotelListDataModels>();
            list = _inclusionRepository.FindHotelListByNameAndCode(hotelcity, hotelname);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Export data in an Excel Format
        /// </summary>
        /// <param name="strprm"></param>
        public void ExportListFromTable()
        {
            var data = _inclusionRepository.GetHotelInclusionDetails_ExportToExcel();

            var arr = data.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=HotacInclusionList.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(arr, Response.Output);
            Response.End();
        }

        /// <summary>
        /// Export hotel data in an Excel Format
        /// </summary>
        /// <param name="strprm"></param>
        public void ExportHotelData()
        {
            var data = _inclusionRepository.GetHotelDataDetails_ExportToExcel();

            var arr = data.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=HotacMasterList.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(arr, Response.Output);
            Response.End();
        }


        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                         prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }


        /// <summary>
        /// upload contract & save as pdf
        /// </summary>
        /// <param name="HotelCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFiles(string HotelCode)
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {                        
                        HttpPostedFileBase file = files[i];
                        var fileExtn = Path.GetExtension(file.FileName);
                        int fileSize = file.ContentLength;
                        if (fileExtn.ToLower() == ".pdf")
                        {
                            string fname;

                            // Checking for Internet Explorer  
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else
                            {
                                fname = file.FileName;
                            }

                            var fileNames = Path.GetFileNameWithoutExtension(file.FileName);
                            var fileExt = Path.GetExtension(file.FileName);
                            string savedbfile = fileNames.ToString() + "_" + HotelCode.ToString() + fileExt.ToString();

                            // Get the complete folder path and store the file inside it.  
                            fname = Path.Combine(Server.MapPath("../HotelContracts/"), savedbfile);
                            file.SaveAs(fname);

                            var s = _inclusionRepository.SaveUploadData(HotelCode, "../HotelContracts/" + savedbfile);
                        }
                        else
                        {
                            return Json("File should be in Pdf format. Kindly upload pdf file against the hotel added.");
                        }

                    }
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
                return Json("Data Uploaded Successfully!");
            }
            else
            {
                return Json("No files selected to upload contract. Kindly upload pdf file against the hotel added.");
            }
        }

        [HttpGet]
        public JsonResult GetCurrencyList()
        {
            var s = _inclusionRepository.GetCurrencyList();
            return Json(s, JsonRequestBehavior.AllowGet);
        }
    }
}