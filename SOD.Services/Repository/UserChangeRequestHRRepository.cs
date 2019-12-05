using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using System.IO;

namespace SOD.Services.Repository
{
    public class UserChangeRequestHRRepository : IUserChangeRequestHRRepository
    {
        private readonly SodEntities _context;
        public UserChangeRequestHRRepository(SodEntities sodEntities)
        {
            _context = sodEntities;
        }

        /// <summary>
        /// Mail Remarks
        /// </summary>
        /// <param name="HRremarks"></param>
        /// <param name="ReqID"></param>
        public void updateconfirmationMailRemarks(string HRremarks, Int64 ReqID)
        {
            var data = _context.SODEmployeeChangeRequestDetails.Where(x => x.ReqId == ReqID).FirstOrDefault();
            if (data != null && data.ToString() != "")
            {
                data.HRApprovalRemarks = HRremarks;
                _context.SaveChanges();
            }
        }


        /// <summary>
        /// Resend Mail to HR
        /// </summary>
        /// <param name="ReqId"></param>
        /// <returns></returns>
        public SodChangeRequestView ResendMailtoHRData(Int64 ReqId)
        {
            var data = (from a in _context.SODEmployeeChangeRequestMaster
                        join b in _context.SODEmployeeChangeRequestDetails
                        on a.ReqId equals b.ReqId
                        where a.ReqId == ReqId
                        select new
                        {
                            a.ReqId,
                            b.HRApprovalStatus,
                            a.RequestedEmpName,
                            b.UserRemarks,
                            b.RequestTypeId,
                            b.CRUpdatet1,
                            b.CRUpdatet2,
                            b.CRUpdatet3,
                            a.PhoneNo,
                            a.EmailID,
                            a.RequestedEmpId,
                            a.DesignationName
                        }).FirstOrDefault();

            SodChangeRequestView _obj = new SodChangeRequestView();
            if (data != null)
            {
                _obj.RequestedEmpName = data.RequestedEmpName;
                _obj.ReqId = data.ReqId;
                _obj.Phone = data.PhoneNo;
                _obj.EmailID = data.EmailID;
                _obj.CR1Update = data.CRUpdatet1;
                _obj.CR2Update = data.CRUpdatet2;
                _obj.CR3Update = data.CRUpdatet3;
                _obj.UserRemarks = data.UserRemarks;
                _obj.RequestTypeId = data.RequestTypeId;
                _obj.EmployeCode = data.RequestedEmpId;
                _obj.Designation = data.DesignationName;


            }
            return _obj;
        }

        /// <summary>
        /// CR Mail Data
        /// </summary>
        /// <param name="idd"></param>
        /// <param name="status"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public SodChangeRequestView CRMailData(Int64 idd, string status, string remarks)
        {
            var sodchangedetails = _context.SODEmployeeChangeRequestDetails.Where(x => x.ReqId == idd).FirstOrDefault();
            if (sodchangedetails != null)
            {
                if (status != null && status != "" && status != "Cancel")
                {
                    if (status == "Approve")
                    {
                        sodchangedetails.HRApprovalStatus = true;
                        sodchangedetails.HRApprovalRemarks = "Approved by HR";
                    }
                    else if (status == "Reject")
                    {
                        //sodchangedetails.HRApprovalStatus = true;
                        sodchangedetails.HRApprovalStatus = true;
                        sodchangedetails.HRApprovalRemarks = "Rejected by HR";
                    }
                    else
                    {
                        sodchangedetails.FinanceStatus = true;
                        sodchangedetails.FinanceApprovalRemarks = remarks; //!= null && remarks != "" ? remarks : sodchangedetails.FinanceApprovalRemarks;
                    }
                }
                else
                {
                    sodchangedetails.FinanceStatus = false;
                    sodchangedetails.FinanceApprovalRemarks = remarks; //!= null && remarks != "" ? remarks : sodchangedetails.FinanceApprovalRemarks;
                }
                _context.SaveChanges();
            }

            var data = (from a in _context.SODEmployeeChangeRequestMaster
                        join b in _context.SODEmployeeChangeRequestDetails
                        on a.ReqId equals b.ReqId
                        where a.ReqId == idd
                        select new { a.ReqId, b.HRApprovalStatus, a.RequestedEmpName, b.UserRemarks, b.RequestTypeId, b.CRUpdatet1, b.CRUpdatet2, b.CRUpdatet3, a.PhoneNo, a.EmailID, a.RequestedEmpId, a.DesignationName, b.HRApprovalRemarks, b.FinanceApprovalRemarks }).FirstOrDefault();

            SodChangeRequestView _obj = new SodChangeRequestView();
            if (data != null)
            {
                _obj.RequestedEmpName = data.RequestedEmpName;
                _obj.ReqId = data.ReqId;
                _obj.Phone = data.PhoneNo;
                _obj.EmailID = data.EmailID;
                _obj.CR1Update = data.CRUpdatet1;
                _obj.CR2Update = data.CRUpdatet2;
                _obj.CR3Update = data.CRUpdatet3;
                _obj.UserRemarks = data.UserRemarks;
                _obj.RequestTypeId = data.RequestTypeId;
                _obj.RemarksFin = data.FinanceApprovalRemarks;

            }
            return _obj;
        }

        /// <summary>
        /// Change Request data According to Employee Code
        /// </summary>
        /// <param name="Empcode"></param>
        /// <returns></returns>
        public List<SodChangeRequestView> getchangeRequestData(string Empcode)
        {
            var data = from a in _context.SODEmployeeChangeRequestMaster
                       join b in _context.SODEmployeeChangeRequestDetails
                       on a.ReqId equals b.ReqId
                       join c in _context.CRHRApprovalStatus on a.ReqId equals c.ReqId into d
                       from e in d.DefaultIfEmpty()
                       where a.RequestedEmpId == Empcode
                       orderby a.RequestedDate descending                      
                       select new
                       {
                           a.ReqId,
                           b.HRApprovalStatus,
                           a.RequestedEmpName,
                           b.UserRemarks,
                           b.RequestTypeName,
                           b.CRUpdatet1,
                           b.CRUpdatet2,
                           b.CRUpdatet3,
                           a.PhoneNo,
                           a.EmailID,
                           a.RequestedEmpId,
                           a.RequestedDate,
                           b.RequestTypeId,
                           b.FinanceStatus,
                           IsReject = (bool?)e.IsRejected,
                           ISAccept=(bool?)e.IsApproved,
                           b.FinanceApprovalRemarks
                       };

            List<SodChangeRequestView> changerequestHistory = new List<SodChangeRequestView>();
            if (data != null && data.Count() > 0)
            {
                foreach (var dt in data)
                {
                    SodChangeRequestView _obj = new SodChangeRequestView();
                    _obj.ReqId = dt.ReqId;
                    _obj.RequestDate = dt.RequestedDate.ToShortDateString();
                    _obj.EmailID = dt.EmailID;
                    _obj.Phone = dt.PhoneNo;
                    _obj.RequestType = dt.RequestTypeName;
                    _obj.CR1Update = dt.CRUpdatet1;
                    _obj.CR2Update = dt.CRUpdatet2;
                    _obj.CR3Update = dt.CRUpdatet3;
                    _obj.RequestedEmpName = dt.RequestedEmpName;
                    _obj.UserRemarks = dt.UserRemarks;
                    _obj.HRApprovalStatus = dt.HRApprovalStatus;
                    _obj.RequestTypeId = dt.RequestTypeId;
                    _obj.FinanceStatus = dt.FinanceStatus;
                    _obj.IsRejectHR = dt.IsReject;
                    _obj.ISAccept = dt.ISAccept;
                    _obj.FinanceApprovalRemarks = dt.FinanceApprovalRemarks;
                    changerequestHistory.Add(_obj);
                }
            }
            return changerequestHistory;
        }

        /// <summary>
        /// Save Change Request Data
        /// </summary>
        /// <param name="sodChangeMaster"></param>
        /// <param name="sodChangeDetails"></param>
        /// <returns></returns>
        public Int64 SaveChangeRequest(SODEmployeeChangeRequestMaster sodChangeMaster, SODEmployeeChangeRequestDetails sodChangeDetails)
        {
            Int64 id = 0;
            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SODEmployeeChangeRequestMaster.Add(sodChangeMaster);
                    _context.SaveChanges();
                    //Get Max ID 
                    id = _context.SODEmployeeChangeRequestMaster.DefaultIfEmpty().Max(x => x == null ? 1 : x.ReqId);
                    //Add Id in Details
                    sodChangeDetails.ReqId = id;
                    _context.SODEmployeeChangeRequestDetails.Add(sodChangeDetails);
                    //_context.SaveChanges();
                    _context.SaveChanges();
                    dbTran.Commit();
                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs
                    dbTran.Rollback();
                    id = -1;
                    throw;
                }
            }
            return id;

        }

        /// <summary>
        ///  Get HR Email
        /// </summary>
        /// <param name="DeptId"></param>
        /// <returns></returns>
        public string GetHREmail(int DeptId)
        {
            var HREmail = _context.HRDepartmentsRight.Where(x => x.DepartmentId == DeptId && x.IsActive == 1).FirstOrDefault();
            return HREmail == null ? "" : HREmail.EmailId;
        }


        /// <summary>
        /// HR Change Request Details
        /// </summary>
        /// <returns></returns>
        public List<SodChangeRequestView> ChangeRequestHRRights(string empCode)
        {
            var data = from a in _context.SODEmployeeChangeRequestMaster
                       join b in _context.SODEmployeeChangeRequestDetails on
                        a.ReqId equals b.ReqId
                       where (from c in _context.HRDepartmentsRight where c.EmployeeCode == empCode select c.DepartmentId).Contains(a.DeptId)
                        && b.HRApprovalStatus == false orderby a.RequestedDate descending
                       select new
                       {
                           a.DeptId,
                           a.ReqId,
                           b.HRApprovalStatus,
                           a.RequestedEmpName,
                           b.UserRemarks,
                           b.RequestTypeName,
                           b.CRUpdatet1,
                           b.CRUpdatet2,
                           b.CRUpdatet3,
                           a.PhoneNo,
                           a.EmailID,
                           a.RequestedEmpId,
                           a.RequestedDate,
                           b.RequestTypeId,
                           a.DepartmentName

                       };
            List<SodChangeRequestView> HrRightsData = new List<SodChangeRequestView>();
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data)
                {
                    SodChangeRequestView _obj = new SodChangeRequestView();
                    _obj.RequestedEmpId = item.RequestedEmpId;
                    _obj.ReqId = item.ReqId;
                    _obj.DeptId = item.DeptId;
                    _obj.EmailID = item.EmailID;
                    _obj.Phone = item.PhoneNo;
                    _obj.UserRemarks = item.UserRemarks;
                    _obj.CR1Update = item.CRUpdatet1;
                    _obj.CR2Update = item.CRUpdatet2;
                    _obj.CR3Update = item.CRUpdatet3;
                    _obj.HRApprovalStatus = item.HRApprovalStatus;
                    _obj.RequestType = item.RequestTypeName;
                    _obj.RequestDate = item.RequestedDate.ToString("dd/MM/yyyy h:mm tt");
                    _obj.RequestedEmpName = item.RequestedEmpName;
                    _obj.RequestTypeId = item.RequestTypeId;
                    _obj.DepartmentName = item.DepartmentName;
                    HrRightsData.Add(_obj);
                }

            }
            return HrRightsData;
        }



        /// <summary>
        /// HR Change Request Details History
        /// </summary>
        /// <returns></returns>
        public List<SodChangeRequestView> ChangeRequestHRRightsHistory(string empCode)
        {
            var data = from a in _context.SODEmployeeChangeRequestMaster
                       join b in _context.SODEmployeeChangeRequestDetails on
                        a.ReqId equals b.ReqId join e in _context.CRHRApprovalStatus on a.ReqId equals e.ReqId 
                       where (from c in _context.HRDepartmentsRight where c.EmployeeCode == empCode select c.DepartmentId).Contains(a.DeptId)
                        && b.HRApprovalStatus == true
                       orderby a.RequestedDate descending
                       select new
                       {

                           a.DeptId,
                           a.ReqId,
                           b.HRApprovalStatus,
                           a.RequestedEmpName,
                           b.UserRemarks,
                           b.RequestTypeName,
                           b.CRUpdatet1,
                           b.CRUpdatet2,
                           b.CRUpdatet3,
                           a.PhoneNo,
                           a.EmailID,
                           a.RequestedEmpId,
                           a.RequestedDate,
                           b.RequestTypeId,
                           a.DepartmentName,
                           e.IsApproved,
                           e.IsRejected,
                           b.HRApprovalRemarks

                       };
            List<SodChangeRequestView> HrRightsData = new List<SodChangeRequestView>();
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data)
                {
                    SodChangeRequestView _obj = new SodChangeRequestView();
                    _obj.RequestedEmpId = item.RequestedEmpId;
                    _obj.ReqId = item.ReqId;
                    _obj.DeptId = item.DeptId;
                    _obj.EmailID = item.EmailID;
                    _obj.Phone = item.PhoneNo;
                    _obj.UserRemarks = item.UserRemarks;
                    _obj.CR1Update = item.CRUpdatet1;
                    _obj.CR2Update = item.CRUpdatet2;
                    _obj.CR3Update = item.CRUpdatet3;
                    _obj.HRApprovalStatus = item.HRApprovalStatus;
                    _obj.RequestType = item.RequestTypeName;
                    _obj.RequestDate = item.RequestedDate.ToString("dd/MM/yyyy h:mm tt");
                    _obj.RequestedEmpName = item.RequestedEmpName;
                    _obj.RequestTypeId = item.RequestTypeId;
                    _obj.DepartmentName = item.DepartmentName;
                    _obj.ISAccept = item.IsApproved;
                    _obj.IsRejectHR = item.IsRejected;
                    _obj.HRApprovalRemarks = item.HRApprovalRemarks;
                    HrRightsData.Add(_obj);
                }

            }
            return HrRightsData;
        }


        /// <summary>
        /// Set Sod User Dp
        /// </summary>
        /// <param name="model"></param>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public Int64 UserSetDp(SodUserProfileDp model, string EmpCode)
        {
            Int64 ID = 0;
            var dp = model.ImageFile;
            byte[] dpimage = null;
            if (dp != null)
            {
                BinaryReader reader = new BinaryReader(dp.InputStream);
                dpimage = reader.ReadBytes(dp.ContentLength);
                var dataexists = _context.SodUserProfileDp.Where(x => x.EmployeeCode == EmpCode).FirstOrDefault();
                if (dataexists != null)
                {
                    dataexists.ImageName = DateTime.Now.Ticks + model.ImageFile.FileName;
                    dataexists.DpImage = dpimage;
                    ID = dataexists.dpid;
                    _context.SaveChanges();
                }
                else
                {
                    model.ImageName = DateTime.Now.Ticks + model.ImageFile.FileName;
                    model.DpImage = dpimage;
                    model.EmployeeCode = EmpCode;
                    _context.SodUserProfileDp.Add(model);
                    _context.SaveChanges();
                    ID = model.dpid;
                }
                //_context.SaveChanges();
            }

            return ID;
        }

        /// <summary>
        /// Display Dp
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public SodUserProfileDp DisplayDp(string EmpCode)
        {
            var data = _context.SodUserProfileDp.Where(x => x.EmployeeCode == EmpCode).FirstOrDefault();

            return data;
        }

        /// <summary>
        /// Finance CR Data
        /// </summary>
        /// <returns></returns>
        public List<SodChangeRequestView> ChangeRequestFinanceRights()
        {
            //var financedata = from a in _context.SODEmployeeChangeRequestMaster
            //                  join b in _context.SODEmployeeChangeRequestDetails
            //                  on a.ReqId equals b.ReqId
            //                  where b.HRApprovalStatus == true && b.FinanceStatus == false
            //                  select new
            //                  {
            //                      a.DeptId,
            //                      a.ReqId,
            //                      b.HRApprovalStatus,
            //                      a.RequestedEmpName,
            //                      b.UserRemarks,
            //                      b.RequestTypeName,
            //                      b.CRUpdatet1,
            //                      b.CRUpdatet2,
            //                      b.CRUpdatet3,
            //                      a.PhoneNo,
            //                      a.EmailID,
            //                      a.RequestedEmpId,
            //                      a.RequestedDate,
            //                      b.RequestTypeId,
            //                      b.FinanceStatus,
            //                      a.DepartmentName
            //                  };

            var financedata = from a in _context.SODEmployeeChangeRequestMaster
                              join b in _context.SODEmployeeChangeRequestDetails
                              on a.ReqId equals b.ReqId
                              where b.HRApprovalStatus==true && b.FinanceStatus == false  && b.FinanceApprovalRemarks == null && b.HRApprovalRemarks != "Rejected by HR"
                              orderby a.ReqId descending
                              select new
                              {
                                  a.DeptId,
                                  a.ReqId,
                                  b.HRApprovalStatus,
                                  a.RequestedEmpName,
                                  b.UserRemarks,
                                  b.RequestTypeName,
                                  b.CRUpdatet1,
                                  b.CRUpdatet2,
                                  b.CRUpdatet3,
                                  a.PhoneNo,
                                  a.EmailID,
                                  a.RequestedEmpId,
                                  a.RequestedDate,
                                  b.RequestTypeId,
                                  b.FinanceStatus,
                                  a.DepartmentName,
                                
                              };

            List<SodChangeRequestView> FinanceRightsData = new List<SodChangeRequestView>();
            if (financedata != null && financedata.Count() > 0)
            {
                foreach (var item in financedata)
                {
                    SodChangeRequestView _obj = new SodChangeRequestView();
                    _obj.RequestedEmpId = item.RequestedEmpId;
                    _obj.ReqId = item.ReqId;
                    _obj.DeptId = item.DeptId;
                    _obj.EmailID = item.EmailID;
                    _obj.Phone = item.PhoneNo;
                    _obj.UserRemarks = item.UserRemarks;
                    _obj.CR1Update = item.CRUpdatet1;
                    _obj.CR2Update = item.CRUpdatet2;
                    _obj.CR3Update = item.CRUpdatet3;
                    _obj.HRApprovalStatus = item.HRApprovalStatus;
                    _obj.RequestType = item.RequestTypeName;
                    _obj.RequestDate = item.RequestedDate.ToShortDateString();
                    _obj.RequestedEmpName = item.RequestedEmpName;
                    _obj.RequestTypeId = item.RequestTypeId;
                    _obj.FinanceStatus = item.FinanceStatus;
                    _obj.DepartmentName = item.DepartmentName;
                    FinanceRightsData.Add(_obj);
                }

            }

            return FinanceRightsData;
        }



        /// <summary>
        /// Finance CR Data History
        /// </summary>
        /// <returns></returns>
        public List<SodChangeRequestView> ChangeRequestFinanceRightsHistory()
        {
            var financedata = from a in _context.SODEmployeeChangeRequestMaster
                              join b in _context.SODEmployeeChangeRequestDetails
                              on a.ReqId equals b.ReqId
                              where b.FinanceApprovalRemarks !=null
                              orderby a.ReqId descending
                              select new
                              {
                                  a.DeptId,
                                  a.ReqId,
                                  b.HRApprovalStatus,
                                  a.RequestedEmpName,
                                  b.UserRemarks,
                                  b.RequestTypeName,
                                  b.CRUpdatet1,
                                  b.CRUpdatet2,
                                  b.CRUpdatet3,
                                  a.PhoneNo,
                                  a.EmailID,
                                  a.RequestedEmpId,
                                  a.RequestedDate,
                                  b.RequestTypeId,
                                  b.FinanceStatus,
                                  a.DepartmentName,
                                  b.FinanceApprovalRemarks
                              };

            List<SodChangeRequestView> FinanceRightsData = new List<SodChangeRequestView>();
            if (financedata != null && financedata.Count() > 0)
            {
                foreach (var item in financedata)
                {
                    SodChangeRequestView _obj = new SodChangeRequestView();
                    _obj.RequestedEmpId = item.RequestedEmpId;
                    _obj.ReqId = item.ReqId;
                    _obj.DeptId = item.DeptId;
                    _obj.EmailID = item.EmailID;
                    _obj.Phone = item.PhoneNo;
                    _obj.UserRemarks = item.UserRemarks;
                    _obj.CR1Update = item.CRUpdatet1;
                    _obj.CR2Update = item.CRUpdatet2;
                    _obj.CR3Update = item.CRUpdatet3;
                    _obj.HRApprovalStatus = item.HRApprovalStatus;
                    _obj.RequestType = item.RequestTypeName;
                    _obj.RequestDate = item.RequestedDate.ToShortDateString();
                    _obj.RequestedEmpName = item.RequestedEmpName;
                    _obj.RequestTypeId = item.RequestTypeId;
                    _obj.FinanceStatus = item.FinanceStatus;
                    _obj.DepartmentName = item.DepartmentName;
                    _obj.FinanceApprovalRemarks = item.FinanceApprovalRemarks;
                    FinanceRightsData.Add(_obj);
                }
            }
            return FinanceRightsData;
        }

        /// <summary>
        /// HR Approval
        /// </summary>
        /// <param name="ReqId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Int64 SaveHRApproval(Int64 ReqId, string status)
        {
            //Int64 ApprovalID = 0;
            int flag=0;
            var deptId = _context.SODEmployeeChangeRequestMaster.Where(x => x.ReqId == ReqId).Select(x => x.DeptId).FirstOrDefault();
            var HRRightsData = _context.HRDepartmentsRight.Where(x => x.DepartmentId == deptId && x.IsActive == (short)1).FirstOrDefault();

            if (HRRightsData != null && HRRightsData.ToString() != "")
            {
                var datacheck = _context.CRHRApprovalStatus.Where(x => x.ReqId == ReqId).FirstOrDefault();
                if (datacheck != null && datacheck.ToString() != "")
                {
                    return flag;
                    //datacheck.StatusTime = DateTime.Now;
                    //datacheck.IsApproved = status == "Approve" ? true : false;
                    //datacheck.IsRejected = status == "Reject" ? true : false;
                   // ApprovalID = datacheck.appid;
                    //_context.SaveChanges();
                }
                else
                {
                    CRHRApprovalStatus _obj = new CRHRApprovalStatus()
                    {
                        ReqId = ReqId,
                        HREmpCode = HRRightsData.EmployeeCode,
                        StatusTime = DateTime.Now,
                        IsApproved = status == "Approve" ? true : false,
                        IsRejected = status == "Reject" ? true : false
                    };
                    _context.CRHRApprovalStatus.Add(_obj);
                    _context.SaveChanges();
                    //ApprovalID = _obj.appid;
                    flag = 1;
                }
            }
            return flag;
        }

        /// <summary>
        /// Remove Dp
        /// </summary>
        /// <param name="EmpCode"></param>
        public void RemoveDP(string EmpCode)
        {
            var data = _context.SodUserProfileDp.Where(x => x.EmployeeCode == EmpCode).FirstOrDefault();
            if (data != null)
            {
                _context.SodUserProfileDp.Remove(data);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Exists Dp
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public bool checkdp(string EmpCode)
        {
            var data = _context.SodUserProfileDp.Where(x => x.EmployeeCode == EmpCode).FirstOrDefault();
            return data != null ? true : false;
        }

        /// <summary>
        /// Dynamic Bind All Menus
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        public List<SodUserMenu> AllMenus(string Role)
        {
            var names =  Role.Split(',');
            var mids = (from d in _context.SodMenuRight
                        where (names.Contains(d.RoleId.ToString()))
                        select d.MenuId.ToString()).ToList();
            var allmenu = (from f in _context.SodUserMenu
                           where mids.Contains(f.MenuId.ToString())
                           orderby f.OrderPriority
                           select f).ToList();
            return allmenu;
        }


        /// <summary>
        /// Get Finance Email ID
        /// </summary>
        /// <param name="ReqId"></param>
        /// <returns></returns>
        public string GetFinanceEmailId(int deptId)
        {
            var FinEmail = _context.CRFinanceUpdateRight.Where(x => x.DepartmentId == deptId && x.isActive == true).ToList();
            return FinEmail != null ? FinEmail[0].Email.ToString() : "";
        }

        //check Already Sent Mail to User  
        public bool SendMAiltoUserCheck(Int64 Reqid)
        {
            var data = _context.SODEmployeeChangeRequestDetails.Where(x => x.ReqId == Reqid && x.FinanceStatus == true && x.FinanceApprovalRemarks != null).FirstOrDefault();
            return data != null ? true : false;
        }


        /// <summary>
        /// to check the role of User
        /// </summary>
        /// <param name="EmpCOde"></param>
        /// <returns></returns>
        public string CheckRoleOfUser(string EmpCOde)
        {
            //string empCode = "00" + EmpCOde.ToString();
            //string role = string.Empty;
            var role = _context.CRFinanceUpdateRight.Where(x => x.EmpCode == EmpCOde && x.isActive == true).FirstOrDefault();
            if(role == null)
            {
                var roles = _context.HRDepartmentsRight.Where(x => x.EmployeeCode == EmpCOde && x.IsActive == 1).FirstOrDefault();
                if (roles == null)
                {
                    return "user";
                }
                else
                {
                    return "hr";
                }
            }
            else {
                return "Finance";
           }           
        }
    }
}
