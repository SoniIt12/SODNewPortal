using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Interface
{
    public interface IVendorApprovalRepository : IDisposable
    {
        List<VendorModels> GetVendorApprovalList(string hodmailID);
        List<VendorModels> GetApprovedList(string ReqEmpCode);
        string AppvendorList(int BatchID, string ApprovermailId, string type);
        string RejectVendorList(int BatchID, string ApprovermailId, string type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        List<VendorModels> getlistasperBatchID(int BatchID);
    }
}
