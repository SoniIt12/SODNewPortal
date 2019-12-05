using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Interface
{
    public interface IVendorRepository : IDisposable
    {
        string AddUpdateVendor(VendorModels vendorModels);
        List<VendorModels> GetVendorList();
        int DeleteVendor(int Id);
        List<VendorModels> GetVendorDataDetails_ExportToExcel();
        string IsExistEmailOrMobile(string Email, string Mobile, int Id);
        List<VendorHODDetails> Hoddetails(string UserEmailID);
        Int32 UpdateBatchId(List<VendorModels> mailerList);
    }
}
