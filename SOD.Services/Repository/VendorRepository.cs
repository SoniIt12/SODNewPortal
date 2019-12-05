using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;

namespace SOD.Services.Repository
{
    public class VendorRepository : IVendorRepository
    {
        private readonly SodEntities _context;
        public VendorRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
        public string AddUpdateVendor(VendorModels vendorModels)
        {
            string msg = "Record successfully added!";           

            try
            {
                if (vendorModels.ID > 0)
                {
                    var data = _context.NONSODVendorMaster.Where(n => n.ID == vendorModels.ID).OrderBy(n => n.ID).FirstOrDefault();
                    if (data != null)
                    {
                        msg = "Record has been updated successfully.";                        
                        data.FirstName = vendorModels.FirstName;
                        data.LastName = vendorModels.LastName;
                        data.MobileNo = vendorModels.MobileNo;
                        data.EmailId = vendorModels.EmailId;
                        data.Gender = vendorModels.Gender;
                        data.CompanyName = vendorModels.CompanyName;
                        data.ModifiedDate = vendorModels.ModifiedDate;
                        data.AddVendorOnBehalfof = vendorModels.AddVendorOnBehalfof;
                    }
                }
                else
                {
                    var num = _context.NONSODVendorMaster.Select(x => x.ID).Max();
                    vendorModels.VendorCode = WebConfigurationManager.AppSettings["VendorPrefixKey"] + (num > 0 ? (num + 1) : 1);
                    _context.NONSODVendorMaster.Add(vendorModels);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            return msg;
        }
        public int DeleteVendor(int Id)
        {
            var data = _context.NONSODVendorMaster.Where(n => n.ID == Id).FirstOrDefault();
            _context.NONSODVendorMaster.Remove(data);
            return _context.SaveChanges();
        }
        public List<VendorModels> GetVendorList()
        {
            var lst = new List<VendorModels>();
            try
            {
                lst = _context.NONSODVendorMaster.Where(n => n.IsActive != null && n.IsActive.Value).OrderByDescending(x => x.IsApproved == 0).ThenByDescending(o => o.ID).ToList();
                foreach (var item in lst)
                {
                    Console.WriteLine(item);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return lst;
        }
        public List<VendorModels> GetVendorDataDetails_ExportToExcel()
        {
            var list = new List<VendorModels>();
            list = _context.NONSODVendorMaster.ToList();
            return list;
        }
        public string IsExistEmailOrMobile(string Email, string Mobile, int Id)
        {
            string name = null;
            VendorModels model = null;
            if (Id > 0)
            {
                var list = _context.NONSODVendorMaster.Where(n => n.EmailId == Email).ToList();
                var list1 = _context.NONSODVendorMaster.Where(n => n.MobileNo == Mobile).ToList();
                if (list.Count > 1)
                    name = "This email is already exist with " + (list[list.Count - 1].FirstName + " " + list[list.Count - 1].LastName);
                else
                {
                    if (list1.Count > 1)
                        name = "This mobile no. is already exist with " + (list1[list.Count - 1].FirstName + " " + list1[list.Count - 1].LastName);
                }
            }
            else
            {
                model = _context.NONSODVendorMaster.Where(n => n.EmailId == Email).FirstOrDefault();
                if (model != null)
                    name = "This email is already exist with " + (model.FirstName + " " + model.LastName);
                else
                {
                    model = _context.NONSODVendorMaster.Where(n => n.MobileNo == Mobile).FirstOrDefault();
                    if (model != null)
                        name = "This mobile no. is already exist with " + (model.FirstName + " " + model.LastName);
                }
            }
            return name;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public List<VendorHODDetails> Hoddetails(string useremailid)
        {
            List<VendorHODDetails> list = _context.NONSODVendorApproverMaster.Where(n => n.UserEmailID == useremailid && n.IsActive == true).ToList();
            return list;
        }


        public Int32 UpdateBatchId(List<VendorModels> mailerlist)
        {
            foreach (var mlist in mailerlist)
            {
                    List<VendorModels> list = _context.NONSODVendorMaster.Where(n => n.ID == mlist.ID).ToList();
                    if (list != null)
                    {
                        list[0].IsMailSent = true;
                    }
            }
            Int32 maxBatchId = Convert.ToInt32(_context.NONSODVendorMaster.Select(a => a.BatchId).Max());
            var getvenderlist = _context.NONSODVendorMaster.ToList();
            foreach (var g in getvenderlist)
            {
                foreach (var j in mailerlist)
                {
                    if (g.VendorCode == j.VendorCode)
                    {
                        g.BatchId = maxBatchId + 1;
                        break;
                    }
                }
            }
            var s = _context.SaveChanges();
            return maxBatchId + 1;
        }
    }
}