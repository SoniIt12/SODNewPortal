﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;


namespace SOD.Services.Repository
{
    public class VendorApprovalRepository : IVendorApprovalRepository
    {
        private readonly SodEntities _context;
        public VendorApprovalRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
        public List<VendorModels> GetVendorApprovalList(string hodmailID)
        {

            List<VendorModels> lst = _context.NONSODVendorMaster.Where(n => n.ApprovedbyEmpEmailID== hodmailID).OrderByDescending(x => x.IsApproved == 0).ThenByDescending(o => o.ID).ToList();
            return lst;
        }
        public List<VendorModels> GetApprovedList(string ReqEmpCode)
        {
            List<VendorModels> lst = _context.NONSODVendorMaster.Where(n => n.IsActive != null && n.IsActive.Value && n.IsApproved == 1 && n.ReqEmpCode == ReqEmpCode).OrderByDescending(o => o.ID).ToList();
            return lst;
        }
        public List<VendorModels> getlistasperBatchID(int BatchID)
        {
            // string UserEmailID;
            List<VendorModels> list = _context.NONSODVendorMaster.Where(n => n.BatchId == BatchID).ToList();
            return list;
        }
        public string AppvendorList(int BatchID, string approveremailID, string type)
        {
            var status = string.Empty;
            try
            {
                List<VendorModels> datalist = _context.NONSODVendorMaster.Where(a => (a.BatchId == BatchID || a.ID == BatchID) && a.IsApproved!=0).ToList();
                if (datalist.Count > 0)
                {
                    foreach (var i in datalist)
                    {
                        if (i.IsApproved == 1)
                        {
                            return "Vendor Detail has been approved already";//Exist
                        }
                        else if (i.IsApproved == 2)
                        {
                            return "Vendor Detail has been rejected already";//Exist
                        }
                    }
                }
                else
                {
                    datalist = _context.NONSODVendorMaster.Where(a => (a.BatchId == BatchID || a.ID == BatchID) && a.IsApproved != 1).ToList();
                    List<VendorApprovallog> logTable = new List<VendorApprovallog>();
                    if (datalist != null)
                    {
                        foreach (var data in datalist)
                        {
                            VendorApprovallog modal = new VendorApprovallog();
                            data.IsApproved = 1;
                            data.Approvaldate = DateTime.Now;
                            data.ApprovedbyEmpEmailID = approveremailID;
                            modal.VendorCode = data.VendorCode;
                            modal.ApprovalStatus = data.IsApproved;
                            modal.ApprovedbyEmpEmailID = data.ApprovedbyEmpEmailID;
                            modal.Approvaldate = DateTime.Now;
                            modal.ApprovalModifiedDate = DateTime.Now;
                            modal.BatchID = data.BatchId;
                            modal.Remarks = "Approved";
                            logTable.Add(modal);
                            var insertdata = _context.NONSODVendorApprovalLog.AddRange(logTable);
                            var s = _context.SaveChanges();
                        }
                        status = "Vendor Detail has been Approved";
                    }
                    else
                    {
                        return "error";//Error
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return (status);
        }
        public string RejectVendorList(int BatchID, string ApprovermailId, string type)

        {
            var status = string.Empty;
            List<VendorModels> datalist = _context.NONSODVendorMaster.Where(a => (a.BatchId == BatchID ||a.ID==BatchID) && a.IsApproved!=0).ToList();
            if (datalist.Count > 0)
            {
                foreach (var i in datalist)
                {
                    if (i.IsApproved == 1)
                    {
                        status = "Vendor Detail has been approved already";//Exist
                    }
                    else if (i.IsApproved == 2)
                    {
                        status = "Vendor Detail has been rejected already";//Exist
                    }
                }
            }
            else
            {
                var data = _context.NONSODVendorMaster.Where(a => (a.BatchId == BatchID || a.ID == BatchID)).FirstOrDefault();
                List<VendorApprovallog> logTable = new List<VendorApprovallog>();
                //var adddata = _context.NONSODVendorApprovalLog
                if (data != null)
                {
                    VendorApprovallog modal = new VendorApprovallog();
                    data.IsApproved = 2;
                    data.Approvaldate = DateTime.Now;
                    data.ApprovedbyEmpEmailID = ApprovermailId;
                    modal.VendorCode = data.VendorCode;
                    modal.ApprovalStatus = data.IsApproved;
                    modal.ApprovedbyEmpEmailID = data.ApprovedbyEmpEmailID;
                    modal.Approvaldate = DateTime.Now;
                    modal.ApprovalModifiedDate = DateTime.Now;
                    modal.BatchID = data.BatchId;
                    modal.Remarks = "Rejected";
                    logTable.Add(modal);
                    var insertdata = _context.NONSODVendorApprovalLog.AddRange(logTable);
                    // _context.SaveChanges();
                    _context.SaveChanges();
                    status = "Vendor Detail has been Rejected";
                }
                else
                    status = "error";
            }
            return status;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
