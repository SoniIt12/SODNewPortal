using System;
using System.Collections.Generic;
using System.Linq;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using System.Configuration;

namespace SOD.Services.Repository
{
    public class UserRepository : IUserRepository
    {

        #region "Constructor Initialization"
        /// <summary>
        /// Constructor Initilization
        /// </summary>
        private readonly SodEntities _context;
        public UserRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
        #endregion

        #region "Methods"
        /// <summary>
        /// Save Sod Booking Request
        /// </summary>
        /// <param name="travelRequestModels"></param>
        /// <returns></returns>
        public int SaveSodBookingRequest(TravelRequestModels travelRequestModels)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Save Sod and non-Sod Request Info
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="passengerList"></param>
        /// <returns></returns>
        public Int64 SaveHotelRequest(List<TravelRequestHotelDetailModels> hotelDetailList)
        {
            var s = 0;
            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    //Get Max Request Id
                    //var requestMaxId = _context.TravelRequestHotelDetailModel.DefaultIfEmpty().Max(x => x == null ? 1 : x.TravelRequestId);

                    //hotelDetailList[0].TravelRequestId = requestMaxId;

                    hotelDetailList[0].HotelReferenceID = ConfigurationManager.AppSettings["Hotel_Booking_ReferenceId"].Trim() + hotelDetailList[0].TravelRequestId;
                    foreach (var i in hotelDetailList)
                    {
                        _context.TravelRequestHotelDetailModel.Add(i);
                    }

                    var trId = Convert.ToInt32(hotelDetailList[0].TravelRequestId);
                    var updateItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == trId);
                    foreach (var p in updateItem)
                    {
                        p.IsHotelRequired = true;
                    }
                    _context.SaveChanges();
                    //_context.TravelRequestMasterModel.Add(updateItem);

                    dbTran.Commit();
                    s = (int)hotelDetailList[0].TravelRequestId;

                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs
                    dbTran.Rollback();
                    s = -1;
                    throw;
                }
            }
            return s;
        }

        /// <summary>
        /// Save Sod and non-Sod Request Info
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="passengerList"></param>
        /// <returns></returns>
        public int SaveBookingRequest(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> passengerList, List<PassengerMealAllocationModels> passengerMealsList, List<TravelRequestCabDetailModels> CabDetailList, List<TravelRequestHotelDetailModels> hotelDetailList)
        {
            var s = 0;
            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    //for Travel Request Info
                    _context.TravelRequestMasterModel.Add(sodRequestsList[0]);
                    _context.SaveChanges();

                    //Get Max Request Id
                    var requestMaxId = _context.TravelRequestMasterModel.DefaultIfEmpty().Max(x => x == null ? 1 : x.TravelRequestId);

                    //Update Code for Travel Request Info
                    var updateItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == requestMaxId);
                    if (sodRequestsList[0].IsSJSC == true)
                    {
                        foreach (var p in updateItem)
                        {
                            p.TravelRequestCode = "SOD" + " " + "SJSC" + "-" + requestMaxId;
                        }
                    }
                    else if (sodRequestsList[0].SodBookingTypeId == 2)
                    {
                        foreach (var p in updateItem)
                        {
                            p.TravelRequestCode = "NON" + "-" +"SOD" + "-" + requestMaxId;
                        }
                    }
                    else
                    {
                        foreach (var p in updateItem)
                        {
                            p.TravelRequestCode = "SOD" + "-" + requestMaxId;
                        }
                    }
                   
                    //for Flight Info
                    foreach (var list in sodflightList)
                    {
                        list.TravelRequestId = requestMaxId;
                    }
                    //for Passenger Info
                    foreach (var list in passengerList)
                    {
                        list.TravelRequestId = requestMaxId;
                    }

                    //for Passenger Meals Info
                    foreach (var list in passengerMealsList)
                    {
                        list.TravelRequestId = requestMaxId;
                    }

                    //Cab allocation Info                    
                    _context.FlightDetailModel.AddRange(sodflightList);
                    _context.PassengerDetailModel.AddRange(passengerList);
                    _context.PassengerMealAllocationModel.AddRange(passengerMealsList);
                    if (sodRequestsList[0].IsCabRequired)
                    {
                        CabDetailList[0].TravelRequestId = requestMaxId;
                        CabDetailList[0].CabReferenceID = ConfigurationManager.AppSettings["Cab_Booking_ReferenceId"].Trim() + requestMaxId;
                        _context.TravelRequestCabDetailModel.Add(CabDetailList[0]);
                    }

                    //Hotel allocation Info
                    _context.FlightDetailModel.AddRange(sodflightList);
                    _context.PassengerDetailModel.AddRange(passengerList);
                    _context.PassengerMealAllocationModel.AddRange(passengerMealsList);
                    if (sodRequestsList[0].IsHotelRequired)
                    {
                        foreach (var list in hotelDetailList)
                        {
                            list.TravelRequestId = requestMaxId;
                            list.HotelReferenceID = ConfigurationManager.AppSettings["Hotel_Booking_ReferenceId"].Trim() + requestMaxId;
                        }
                        _context.TravelRequestHotelDetailModel.AddRange(hotelDetailList);
                    }

                    _context.SaveChanges();
                    //commit transaction
                    dbTran.Commit();
                    s = (int)requestMaxId;
                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs
                    dbTran.Rollback();
                    s = -1;
                    throw;
                }
            }
            return s;
        }



        /// <summary>
        /// Get Employee common info List
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public IList<EmployeeModel> GetEmployeeList(int empId)
        {
            var lstEmployee = ADO.SodCommonServices.GetEmployeeCommonDetails(empId);
            return lstEmployee.ToList();
        }


        /// <summary>
        /// Get Employee Id
        /// </summary>
        /// <param name="empCode"></param>
        /// <returns></returns>
        public int GetLoginEmployeeID(string empCode)
        {
            return ADO.SodCommonServices.GetLoginEmployeeID(empCode, 1);
        }


        /// <summary>
        /// Dispose object
        /// </summary>
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Check Blanket Approver Role
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public bool IsBlanketApproverRole(int empId)
        {
            var s = from b in _context.SodBlanketApprovals
                    where b.EmployeeId == empId && b.IsActive.Equals(1)
                    select b;

            return (s.Count() > 0 ? true : false);

        }


        /// <summary>
        /// Check Approver or HOD Role
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public bool IsSodApproverHodRole(int employeeId)
        {
            var s = from a in _context.SodApprovers
                    where a.EmployeeId == employeeId
                    select a;


            var s1 = from a in _context.SodApproverOnlyStandbyModel
                     where a.EmployeeId == employeeId
                     select a;


            return ((s.Count() > 0 || s1.Count() > 0) ? true : false);
        }


        /// <summary>
        /// Check CXO Level Approver Role
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public bool IsCXOApproverRole(int employeeId)
        {
            var s = from a in _context.SodCXOLevelModel
                    where a.EmpId == employeeId && a.IsActive == true
                    select a;

            return s.Count() > 0 ? true : false;
        }


        /// <summary>
        /// Get CXO Approver List
        /// </summary>
        /// <returns></returns>
        public List<string> GetCXO_ApproverList()
        {
            return _context.SodCXOLevelModel.Select(o => o.CXOName).ToList();
        }


        /// <summary>
        /// Get CXO Approver List : OverLoad Function
        /// </summary>
        /// <returns></returns>
        public List<string> GetCXO_ApproverList(Int16 deptId, Int16 bookingfor)
        {
            var cxolist = new List<string>();
            Random rand;
            var s1 = _context.SodCXODeptMappingModel.Where(o => o.AllocatedDeptId == deptId).ToList();
            if (s1.Count > 0)
                cxolist.Add(s1[0].CXOName);
            else
            {
                var s2 = _context.SodCXOLevelModel.Select(o => o.CXOName).ToList();
                rand = new Random();
                var index = rand.Next(0, s2.Count);
                cxolist.Add(s2[index]);
            }
            if (bookingfor == 2)
            {
                var s2 = _context.SodCXOLevelModel.Select(o => o.CXOName).ToList();
                if (s2.Count > 0)
                    s2.Remove(cxolist[0].ToString());
                rand = new Random();
                var index = rand.Next(0, s2.Count);
                if (s2.Count > 0)
                    cxolist.Add(s2[index]);
            }
            return cxolist;
        }


        /// <summary>
        /// Get HOD/Approver Email id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public string GetHODEmailId(string verticalId)
        {
            var str = ADO.SodCommonServices.GetHodEmailDetails(verticalId, 1);
            return str;
        }


        /// <summary>
        /// Get only Standby HOD/Approver Email id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public string GetHODEmailId_OnlyStandby(string verticalId)
        {
            return ADO.SodCommonServices.GetHodEmailDetails(verticalId, 3);
        }


        /// <summary>
        /// Update PNR for Booking Request Table
        /// </summary>
        /// <param name="pnr"></param>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public int UpdatePnr(string pnr, int requestid)
        {
            var s = 0;
            var updateItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == requestid);
            foreach (var p in updateItem)
            {
                p.Pnr = pnr.Split('|')[0];
                p.PnrAmount = Convert.ToDecimal(pnr.Split('|')[1]);
                p.BookingStatus = pnr.Split('|')[2];
                p.StatusDate = System.DateTime.Now;
                if (p.IsHotelRequired == true)
                {
                    p.HotelApproval = true;
                }
            }
            s = _context.SaveChanges();
            return s;
        }


        /// <summary>
        /// save hotel approval status
        /// </summary>
        /// <param name="travelrequestid"></param>
        public void UpdateHotelApprovalStatus(long travelrequestid)
        {
            var updateItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == travelrequestid);
            foreach (var p in updateItem)
            {
                p.HotelApproval = true;
                p.StatusDate = DateTime.Now;
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// save only hotel approval status
        /// </summary>
        /// <param name="travelrequestid"></param>
        public void UpdateOnlyHotelApprovalStatus(long travelrequestid)
        {
            var updateItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == travelrequestid);
            foreach (var p in updateItem)
            {
                p.HotelApproval = true;
                p.StatusDate = DateTime.Now;
                p.BookingStatus = "Close";
            }
            _context.SaveChanges();
        }


        /// <summary>
        /// Get PNR Amount and Time
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public string GetPNRAmountAndTime(Int64 requestid)
        {
            var str = "";
            var pnrItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == requestid).ToList();
            foreach (var p in pnrItem)
            {
                str = p.Pnr + "|" + p.PnrAmount + "|" + p.StatusDate + "|" + p.BookingStatus;
            }
            return str;
        }



        /// <summary>
        /// Get Approver Dept & Designation
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public string GetApproverDept_Desig(int empId)
        {
            var str = "";
            var s = from b in _context.SodApprovers
                    where b.EmployeeId == empId && b.IsActive.Equals(1)
                    select b;

            if (s.Count() > 0)
            {
                foreach (var p in s)
                {
                    str = p.DepartmentId + "," + p.DesignationId;
                }
            }
            else
            {
                var s1 = from b in _context.SodApproverOnlyStandbyModel
                         where b.EmployeeId == empId && b.IsActive.Equals(1)
                         select b;

                foreach (var p in s1)
                {
                    str = p.DepartmentId + "," + p.DesignationId;
                }
            }
            return str;
        }


        /// <summary>
        /// Get SOD Hotel Info
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodHotelInfo(long travelReqId)
        {
            var _context = new SodEntities();
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var bookingInfoList = new List<TravelRequestMasterModels>();

            sodflightList = _context.FlightDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            passengerList = _context.PassengerDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            hotelList = _context.TravelRequestHotelDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            bookingInfoList = _context.TravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();

            dicInfo.Add("hotelinfolist", hotelList);
            dicInfo.Add("flightInfolist", sodflightList);
            dicInfo.Add("passInfolist", passengerList);
            dicInfo.Add("bookingInfolist", bookingInfoList);
            return dicInfo;
        }



        /// <summary>
        /// Get CXO Email for CC
        /// </summary>
        /// <returns></returns>
        public string GetCXO_EmailCC(int deptId)
        {
            string str = "";
            var s = _context.SodCXODeptMappingModel.Where(o => o.AllocatedDeptId == deptId).ToList();
            if (s.Count > 0)
            {
                string name = s[0].CXOName;
                var d = _context.SodCXOLevelModel.Where(e => e.CXOName == name).FirstOrDefault();

                if (d != null)
                {
                    str = d.CXOName + "|" + d.EmailId;
                }
            }
            return str;
        }


        /// <summary>
        /// Get HOD Email Id Details
        /// </summary>
        /// <param name="verticalId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public string GetHODEmailIdDetails(string verticalId, int criteria)
        {
            var str = ADO.SodCommonServices.GetHodEmailDetails(verticalId, criteria);
            return str;
        }

        /// <summary>
        /// Save master data, flight data and hotel data from "Only Hotel" link page 
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="hotelDetailList"></param>
        /// <returns></returns>
        public int SaveOnlyHotelBookingRequest(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<TravelRequestHotelDetailModels> hotelDetailList)
        {
            var s = 0;
            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.TravelRequestMasterModel.Add(sodRequestsList[0]);
                    _context.SaveChanges();
                    //Get Max Request Id
                    var requestMaxId = _context.TravelRequestMasterModel.DefaultIfEmpty().Max(x => x == null ? 1 : x.TravelRequestId);
                    //Update Code for Travel Request Info
                    var updateItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == requestMaxId);
                    foreach (var p in updateItem)
                    {
                        if (sodRequestsList[0].TravelRequestTypeId == 5)
                        {
                            p.TravelRequestCode = "SJSC" + "-" + requestMaxId;
                        }
                        else if (sodRequestsList[0].SodBookingTypeId == 1)
                        {
                            p.TravelRequestCode = "SOD" + "-" + requestMaxId;
                        }
                        if (p.Pnr == null || p.Pnr == "-")
                        {
                            p.Pnr = "NA";
                        }
                    }
                    //for Flight Info
                    foreach (var list in sodflightList)
                    {
                        list.TravelRequestId = requestMaxId;
                    }
                    _context.FlightDetailModel.AddRange(sodflightList);
                    //for hotel info
                    foreach (var list in hotelDetailList)
                    {
                        list.TravelRequestId = requestMaxId;
                        list.HotelReferenceID = ConfigurationManager.AppSettings["Hotel_Booking_ReferenceId"].Trim() + requestMaxId;
                    }
                    _context.TravelRequestHotelDetailModel.AddRange(hotelDetailList);
                    _context.SaveChanges();
                    dbTran.Commit();
                    s = (int)requestMaxId;
                    return s;
                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs
                    dbTran.Rollback();
                    s = -1;
                    throw;
                }
            }
        }

        /// <summary>
        /// Check HR  Approver Role
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public bool IsHRApproverRole(string EmployeeCode)
        {
            var s = from a in _context.HRDepartmentsRight
                    where a.EmployeeCode == EmployeeCode && a.IsActive == 1
                    select a;
            return s.Count() > 0 ? true : false;
        }


        /// <summary>
        /// Get Dropdown Data for aappliaction access in drop down button
        /// </summary>
        /// <param name="EmployeeCode"></param>
        /// <returns></returns>
        public List<SODIntegratedApplicationMaster> GetDropdownData(string EmployeeCode)
        {
            var data = _context.SODIntegratedApplicationMaster.Join(_context.SODIntegratedApplicationUserAllocation.Where(x => x.EmployeeCode == EmployeeCode && x.IsActive == true), x => x.ApplicationId, y => y.ApplicationId, (a, b) => a).ToList();
            return data;
        }

        /// <summary>
        /// Get Employee Role
        /// </summary>
        /// <param name="empcode"></param>
        /// <returns></returns>
        public string GetRoleOFEmp(string empcode)
        {
            var Role = _context.SODUserRoleAllocation.Where(x => x.EmployeeCode == empcode && x.IsActive == true).FirstOrDefault();
            return Role != null ? Role.RoleId + ",1" : "1";
        }

        /// <summary>
        /// Save SMS Log after the HOD approval
        /// </summary>
        /// <param name="smsLogModel"></param>
        /// <returns></returns>
        public int SaveApproverSMSLog(SodApproverSMSLogModels smsLogModel)
        {
            _context.SodApproverSMSLogModel.Add(smsLogModel);
            return _context.SaveChanges();
        }
        public List<TravelRequestHotelDetailModels> GetReminderListTosendApproval(int criteria)
        {
            var List = ADO.SodCommonServices.GetListForReminderMailToTrigger(criteria);
            return List;
        }
        //public List<TravelRequestHotelDetailModels> GetListTosendTriggerFinancalApproval()
        //{
        //    var List = ADO.SodCommonServices.getListForFinancialApprovalToTrigger();
        //    return List;
        //}

        /// <summary>
        /// getHodDetailByTravelReqId
        /// </summary>
        /// <param name="TravelReqId"></param>
        /// <returns></returns>
        public String GetHODEmailIdByTravelReqID(String TravelReqId)
        {
            var str = ADO.SodCommonServices.GetHODByTravelRequestId(TravelReqId, 1);
            return str;
        }
    }
}