using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOD.Controllers
{
    public class TestfluploadController : Controller
    {
       
        // GET: Testflupload
        public ActionResult fileUploads()
        {
            return View();
        }

        // GET: Testflupload
        public ActionResult Upload1()
        {
            return View();
        }
       
        
        
        /// <summary>
        /// Action Method to Handle the Upload Functionalty
        /// </summary>
        /// <param name="aFile"></param>
        public ActionResult Upload(FormCollection formCollection)
        {
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    var usersList = new List<Users>();
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;

                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var user = new Users();
                            user.FirstName = workSheet.Cells[rowIterator, 1].Value.ToString();
                            user.LastName = workSheet.Cells[rowIterator, 2].Value.ToString();
                            usersList.Add(user);
                        }

                        ViewBag.userList = usersList;
                    }
                }
            }
            return View("fileUploads");
        }




 
        /// <summary>
        /// Save File on Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveFile()
        {
            try
            {
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var file = System.Web.HttpContext.Current.Request.Files["bulkfile"];
                    HttpPostedFileBase filebase = new HttpPostedFileWrapper(file);
                   
                    //Read Excel File
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        var usersList = new List<Users>();
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;

                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                var user = new Users();
                                user.FirstName = workSheet.Cells[rowIterator, 1].Value.ToString();
                                user.LastName = workSheet.Cells[rowIterator, 2].Value.ToString();
                                usersList.Add(user);
                            }

                            //Save User List
                            ViewBag.userList = usersList;
                        }

                        //Save File
                        var fileNames = Path.GetFileName(filebase.FileName);
                        var path = Path.Combine(Server.MapPath("~/UploadFile/"), fileNames);
                        file.SaveAs(path);
                    }
                    return Json("File Saved Successfully.");
                }
                else { return Json("No File Saved."); }
            }
            catch (Exception ex) 
            {
                return Json("Error While Saving."); 
            }
        }
    }


    //class
    public class Users { public string FirstName { get; set; } public string LastName { get; set; } } 
}