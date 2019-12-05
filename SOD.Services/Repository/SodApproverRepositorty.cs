using System.Collections.Generic;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using System.Collections;
using System;
using System.Linq;

namespace SOD.Services.Repository
{
    public class SodApproverRepositorty : ISodApproverRepositorty
    {

        /// <summary>
        /// Initialized Constructor
        /// </summary>
        private readonly SodEntities _context;
        public SodApproverRepositorty(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }

        /// <summary>
        /// Get Sod Approval Status
        /// </summary>
        /// <param name="sodApproverModels"></param>
        /// <returns></returns>
        public bool GetSodApprovalStatus(SodApproverModels sodApproverModels)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get Sod Approval List
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SodApproverModels> GetSodApprovelList()
        {
            //MyDBDataContext sqlObj = new MyDBDataContext();
            //var employees = from emps in sqlObj.tblEmployees
            //                where emps.Salary > 5000
            //                orderby emps.EmployeeName
            //                select new
            //                {
            //                    emps.EmployeeID,
            //                    emps.EmployeeName,
            //                    emps.Salary
            //                };
            //gvemployees.DataSource = employees;
            //gvemployees.DataBind();


            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Save Sod Approval List
        /// </summary>
        /// <param name="sodApproverModels"></param>
        /// <returns></returns>
        public int Save(SodApproverModels sodApproverModels)
        {
            _context.SodApprovers.Add(sodApproverModels);
            _context.SaveChanges();

            return 1;
        }

        /// <summary>
        /// Object Dispose
        /// </summary>
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Get Sod Booking List for Approval
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="designationId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<TravelRequestModels> GetSodBookingListForApproval(int departmentId, int designationId, int EmpId, int criteria, string HodEmailId)
        {
            return ADO.SodCommonServices.GetSodBookingListForApproval(departmentId, designationId, EmpId, criteria, HodEmailId);
        }


        /// <summary>
        /// Get Sod Emloyee Booking History
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<TravelRequestModels> GetSodEmployeeBookingHistoryList(int? departmentId, int? EmpId, int criteria)
        {
            return ADO.SodCommonServices.GetSodEmployeeBookingHistoryList(departmentId, EmpId, criteria);
        }

        public IList<TravelRequestModels> GetSodEmployeeBookingHistoryListbydate(DateTime fromdate, DateTime todate, Int32 EmpId, int criteria)
        {
            return ADO.SodCommonServices.GetSodEmployeeBookingHistoryListbydate(fromdate, todate, EmpId, criteria);
        }


        /// <summary>
        /// Get Sod Employee Booking History By Employee Code wise
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<TravelRequestModels> GetSodEmployeeBookingHistoryList_ByEmployeeCode(string empcode, int criteria, bool IsVendorBooking, string fdate, string tdate, string Dept)
        {
            return ADO.SodCommonServices.GetSodEmployeeBookingHistoryList_ByEmployeeCode(empcode, criteria, IsVendorBooking, fdate, tdate, Dept);
        }

        public IList<TravelRequestMasterModels> GetSodApproverBookingHistoryList_ByEmployeeCode(string empcode, int criteria, bool IsVendorBooking, string fdate, string tdate, string Dept)
        {
            return ADO.SodCommonServices.GetSodApproverBookingHistoryList_ByEmployeeCode(empcode, criteria, IsVendorBooking, fdate, tdate, Dept);
        }

        /// <summary>
        /// Get Sod Emloyee Booking Status
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<string> GetEmployeeBookingStatus(int reqId, int criteria)
        {
            return ADO.SodCommonServices.GetEmployeeBookingStatus(reqId, criteria);
        }


        /// <summary>
        /// Get SodBookingInfo For PNR
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodBookingInfoForPNR(Int64 travelReqId)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var passengerMealsList = new List<PassengerMealAllocationModels>();
            var approvalList = new List<TravelRequestApprovalModels>();
            var cabList = new List<TravelRequestCabDetailModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();

            sodRequestsList = _context.TravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            sodflightList = _context.FlightDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            passengerList = _context.PassengerDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            passengerMealsList = _context.PassengerMealAllocationModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            approvalList = _context.TravelRequestApprovalModels.Where(b => b.TravelRequestId == travelReqId && b.RevenueApprovedStatus == 2).ToList();
            cabList = _context.TravelRequestCabDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            hotelList = _context.TravelRequestHotelDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();


            dicInfo.Add("bookingInfo", sodRequestsList);
            dicInfo.Add("flightInfo", sodflightList);
            dicInfo.Add("passInfo", passengerList);
            dicInfo.Add("approvalInfo", approvalList);
            dicInfo.Add("mealsInfo", passengerMealsList);
            dicInfo.Add("cabInfo", cabList);
            dicInfo.Add("hotelInfo", hotelList);
            return dicInfo;
        }

        /// <summary>
        /// To save Approved/Reject Booking Request
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        public int ApproveSodBookingRequest(TravelRequestApprovalModels travelRequestApprovalModels)
        {
            var c = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == travelRequestApprovalModels.TravelRequestId).ToList();
            if (c.Count() > 0)
                return 1;

            _context.TravelRequestApprovalModels.Add(travelRequestApprovalModels);
            var s = _context.SaveChanges();
            var data = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == travelRequestApprovalModels.TravelRequestId && o.IsHotelRequired == true).ToList();
            if (data.Count > 0)
                UpdateHotelApprovalStatus(travelRequestApprovalModels.TravelRequestId);

            return s;
        }

        /// <summary>
        /// To save Approved/Reject Booking Request
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        public int ApproveOnlyHotelSodBookingRequest(TravelRequestApprovalModels travelRequestApprovalModels)
        {
            var c = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == travelRequestApprovalModels.TravelRequestId).ToList();
            if (c.Count() > 0)
                return 0;
            _context.TravelRequestApprovalModels.Add(travelRequestApprovalModels);
            var s = _context.SaveChanges();
            return s;
        }


        /// <summary>
        /// To save Reject Booking Request
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        public int RejectSodBookingRequest(TravelRequestApprovalModels trApprovalModels)
        {
            var s = 0;
            //For Travel Request Approval
            var updateItem = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId).ToList();
            if (updateItem.Count() > 0)
            {
                foreach (var p in updateItem)
                {
                    p.ApprovalStatus = trApprovalModels.ApprovalStatus;
                    p.ApprovedByEmpId = trApprovalModels.ApprovedByEmpId;
                    p.ApprovalDate = System.DateTime.Now;
                    p.Comment = trApprovalModels.Comment;
                }
            }
            else
            {
                _context.TravelRequestApprovalModels.Add(trApprovalModels);
            }
            return _context.SaveChanges();
        }

        /// <summary>
        /// To check ApprovalStatus of Hod
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        public int checkApprovalStatusOfHod(int travelReqId)
        {
            var Status = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == travelReqId).ToList();

            return Status.Count > 0 ? Int32.Parse(Status[0].ApprovalStatus.ToString()) : 0;
        }


        /// <summary>
        /// To Save :Reject Booking Request from Revenue
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        public int RejectSodBookingRequest_Revenue(TravelRequestApprovalModels trApprovalModels)
        {
            var s = 0;
            var updateItem = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId).ToList();
            if (updateItem.Count() > 0)
            {
                foreach (var p in updateItem)
                {
                    p.RevenueApprovedStatus = trApprovalModels.RevenueApprovedStatus;
                    p.ApprovedByEmpId = trApprovalModels.ApprovedByEmpId;
                    p.RevenueApprovedDate = System.DateTime.Now;
                    p.Comment = trApprovalModels.Comment;
                }

                s = _context.SaveChanges();

                //Change Travel Request Master Status
                var updateItems = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId);
                foreach (var m in updateItems)
                {
                    m.BookingStatus = "Rejected";
                    m.StatusDate = System.DateTime.Now;
                }
                s = _context.SaveChanges();
            }
            return s;
        }

        /// <summary>
        /// Close Booking Request After Rejection of the Request from HOD 
        /// Stand By Case
        /// </summary>
        /// <param name="trRequest"></param>
        /// <returns></returns>
        public int CloseSodBookingRequest_HOD(TravelRequestMasterModels trRequest)
        {
            //Booking Request Master for Close Request
            var closeItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == trRequest.TravelRequestId).ToList();
            if (closeItem.Count() > 0)
            {
                foreach (var p in closeItem)
                {
                    p.BookingStatus = trRequest.BookingStatus;
                    p.StatusDate = trRequest.StatusDate;
                }
            }
            return _context.SaveChanges();
        }

        /// <summary>
        /// Rollback approval status by HOD
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        public int RollBackApprovalByHOD(TravelRequestApprovalModels trApprovalModels)
        {
            //Booking Request approval rollback status
            var rollbackItem = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId).ToList();
            if (rollbackItem.Count() > 0)
            {
                foreach (var p in rollbackItem)
                {
                    p.ApprovalStatus = trApprovalModels.ApprovalStatus;
                    p.ApprovedByEmpId = 0;
                    p.Comment = trApprovalModels.Comment;
                }
            }
            return _context.SaveChanges();
        }



        /// <summary>
        /// Rollback approval status by CXO 1
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        public int RollBackApprovalByCXO_Level1(TravelRequestApprovalModels trApprovalModels)
        {
            //Booking Request approval rollback status
            var rollbackItem = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId).ToList();
            if (rollbackItem.Count() > 0)
            {
                foreach (var p in rollbackItem)
                {
                    p.ApprovedByEmpIdCLevel1 = 0;
                    p.ApprovalStatusCLevel1 = 0;
                    p.CLevelComment1 = trApprovalModels.CLevelComment1;
                    p.CLevelAppDate1 = trApprovalModels.CLevelAppDate1;
                }
            }
            return _context.SaveChanges();
        }



        /// <summary>
        /// Rollback approval status by CXO 2
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        public int RollBackApprovalByCXO_Level2(TravelRequestApprovalModels trApprovalModels)
        {
            //Booking Request approval rollback status
            var rollbackItem = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId).ToList();
            if (rollbackItem.Count() > 0)
            {
                foreach (var p in rollbackItem)
                {
                    p.ApprovedByEmpIdCLevel2 = 0;
                    p.ApprovalStatusCLevel2 = 0;
                    p.CLevelComment2 = trApprovalModels.CLevelComment1;
                    p.CLevelAppDate2 = trApprovalModels.CLevelAppDate1;
                    p.CLevelAppDate2 = System.DateTime.Now;
                }
            }
            return _context.SaveChanges();
        }


        /// <summary>
        /// Update Booking Request :Revenue Approval 
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        public int UpdateSodBookingRequest_Revenue(TravelRequestApprovalModels trApprovalModels)
        {
            var s = 0;
            var updateItem = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId).ToList();
            foreach (var p in updateItem)
            {
                p.RevenueApprovedDate = System.DateTime.Now;
                p.RevenueApprovedStatus = trApprovalModels.RevenueApprovedStatus;
                p.Comment = trApprovalModels.Comment;
            }
            s = _context.SaveChanges();

            return s;
        }


        /// <summary>
        /// Update Booking Request :CXO Approval Level -1
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        public int UpdateSodBookingRequestCXO_Level1(TravelRequestApprovalModels trApprovalModels)
        {
            var s = 0;
            var updateItem = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId);
            foreach (var p in updateItem)
            {
                p.ApprovalStatusCLevel1 = trApprovalModels.ApprovalStatusCLevel1;
                p.ApprovedByEmpIdCLevel1 = trApprovalModels.ApprovedByEmpIdCLevel1;
                p.CLevelAppDate1 = System.DateTime.Now;
                p.CLevelComment1 = trApprovalModels.CLevelComment1;
            }
            s = _context.SaveChanges();

            return s;
        }


        /// <summary>
        /// Update Booking Request :CXO Approval Level -2
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        public int UpdateSodBookingRequestCXO_Level2(TravelRequestApprovalModels trApprovalModels)
        {
            var s = 0;
            var updateItem = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId);
            foreach (var p in updateItem)
            {
                p.ApprovalStatusCLevel2 = trApprovalModels.ApprovalStatusCLevel2;
                p.ApprovedByEmpIdCLevel2 = trApprovalModels.ApprovedByEmpIdCLevel2;
                p.CLevelAppDate2 = System.DateTime.Now;
                p.CLevelComment2 = trApprovalModels.CLevelComment2;
            }
            s = _context.SaveChanges();

            return s;
        }



        /// <summary>
        /// Check CXO-Level 1 Approval Status
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        public bool CheckSodApprovalStatusCXO_Level1(Int64 requestid)
        {
            var s = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == requestid && o.ApprovedByEmpIdCLevel1 == 0 && o.ApprovalStatusCLevel1 == 0).ToList();
            return s.Count > 0 ? true : false;
        }



        /// <summary>
        /// Check CXO-Level 2 Approval Status
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        public bool CheckSodApprovalStatusCXO_Level2(Int64 requestid)
        {
            var s = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == requestid && o.ApprovedByEmpIdCLevel2 == 0 && o.ApprovalStatusCLevel2 == 0).ToList();
            return s.Count > 0 ? true : false;
        }

        /// <summary>
        /// To Get Priority L1 and Priority L2 Status
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public bool CheckSodApprovalStatusCXO_LevelL1L2(Int64 requestid)
        {
            var s = _context.TravelRequestApprovalModels.Where(o => o.TravelRequestId == requestid && o.ApprovalStatusCLevel1 >= 1 && o.ApprovedByEmpIdCLevel2 >= 1).ToList();
            return s.Count > 0 ? true : false;
        }


        /// <summary>
        /// Check CXO Priority Level  When Logged In
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public Int16 CheckCXOApprover_Priority(int designationId, int departmentId, Int64 trRequestId)
        {
            var s = from a in _context.SodCXOLevelModel
                    where a.DesignationId == designationId && a.DepartmentId == departmentId && a.IsActive.Equals(true)
                    select a.CXOName.ToString();

            string cval = s.First().ToString();

            var l1 = from b in _context.TravelRequestMasterModel
                     where b.CLevelApprover1.Equals(cval) && b.TravelRequestId == trRequestId && b.BookingStatus != "Close"
                     select b.CLevelApprover1;

            var l2 = from c in _context.TravelRequestMasterModel
                     where c.CLevelApprover2.Equals(cval) && c.TravelRequestId == trRequestId && c.BookingStatus != "Close"
                     select c.CLevelApprover2;

            if (l1.Count() > 0)
                return 1;
            else if (l2.Count() > 0)
                return 2;
            else
                return 0;
        }



        /// <summary>
        /// Check CXO Priority Level  when approbed vio email
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public Int16 CheckCXOApprover_PriorityByEmpId(int EmpId, Int64 trRequestId)
        {
            var s = from a in _context.SodCXOLevelModel
                    where a.EmpId == EmpId && a.IsActive.Equals(true)
                    select a.CXOName.ToString();

            string cval = s.First().ToString();

            var l1 = from b in _context.TravelRequestMasterModel
                     where b.CLevelApprover1.Equals(cval) && b.TravelRequestId == trRequestId && b.BookingStatus != "Close"
                     select b.CLevelApprover1;

            var l2 = from c in _context.TravelRequestMasterModel
                     where c.CLevelApprover2.Equals(cval) && c.TravelRequestId == trRequestId && c.BookingStatus != "Close"
                     select c.CLevelApprover2;

            if (l1.Count() > 0)
                return 1;
            else if (l2.Count() > 0)
                return 2;
            else
                return 0;
        }

        /// <summary>
        /// Update PNR for Booking Request Table
        /// </summary>
        /// <param name="pnr"></param>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public int UpdatePnr(string pnr, Int64 requestid)
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
        /// Update Hotel Approval Status
        /// </summary>
        /// <param name="travelrequestid"></param>
        public void UpdateHotelApprovalStatus(long travelrequestid)
        {
            var updateItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == travelrequestid).ToList();
            foreach (var p in updateItem)
            {
                p.HotelApproval = true;
                p.StatusDate = DateTime.Now;
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Update Only Hotel Approval Status
        /// </summary>
        /// <param name="travelrequestid"></param>
        public void UpdateOnlyHotelApprovalStatus(long travelrequestid)
        {
            var updateItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == travelrequestid).ToList();
            foreach (var p in updateItem)
            {
                p.HotelApproval = true;
                p.StatusDate = DateTime.Now;
                p.BookingStatus = "Close";
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Update Only Hotel Approval Status
        /// </summary>
        /// <param name="travelrequestid"></param>
        public bool UpdateOnlyHotelRejectionStatus(long travelrequestid)
        {
            var updateItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == travelrequestid).ToList();
            foreach (var p in updateItem)
            {
                p.HotelApproval = false;
                p.StatusDate = DateTime.Now;
                p.BookingStatus = "Rejected";
            }
            var s = _context.SaveChanges();
            return s > 0 ? true : false;
        }
        /// <summary>
        /// find hotel approval status
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        public bool FindHotelApprovalStatus(long trid)
        {
            var data = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == trid).ToList();
            if (data[0].HotelApproval == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// find hotel Rejection  status
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        public bool FindHotelRejectionStatus(long trid)
        {
            var data = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == trid).ToList();
            if (data[0].BookingStatus == "Rejected")
            {
                return true;
            }
            else
            {
                return false;
            }
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
        /// Get CXO Email ID
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public string GetCXOMailId(string cl1, string cl2)
        {
            var str = string.Empty;
            var counter = 0;
            var emails = _context.SodCXOLevelModel.Where(c => c.CXOName == cl1 || c.CXOName == cl2)
                .Select(c => new { c.EmailId, c.EmpId }).ToList();

            if (emails != null)
                foreach (var s in emails)
                {
                    if (s != null)
                    {
                        if (counter > 0)
                            str = str + "|" + s.EmpId + "|" + s.EmailId;
                        else
                            str = s.EmpId + "|" + s.EmailId;
                        counter++;
                    }
                }
            return str;
        }


        /// <summary>
        /// Get Comment from Revenue Dept.
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public string GetRevenueComment(Int64 reqId)
        {
            var s = _context.TravelRequestApprovalModels.Where(c => c.TravelRequestId == reqId)
                    .Select(c => new { c.Comment }).Single();
            return s.Comment.ToString();
        }


        /// <summary>
        /// Check Revenue Approval Status : YES or NO
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public string CheckRevenueApprovalStatus(Int64 reqId)
        {
            var s = _context.TravelRequestApprovalModels.Where(c => c.TravelRequestId == reqId)
                  .Select(c => new { c.RevenueApprovedStatus }).Single();
            return s.RevenueApprovedStatus.ToString();
        }


        /// <summary>
        /// Checking for User CXO Role
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public bool IsCXORole(Int32 EmpId)
        {
            var s = _context.SodCXOLevelModel.Where(c => c.EmpId == EmpId && c.IsActive == true).ToList();
            if (s.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checking for User HOD/Vertical Head Role
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public bool IsHODRole(Int32 EmpId, string verticalId, int deptId)
        {
            var s = _context.SodApprovers.Where(c => c.EmployeeId == EmpId && c.VerticleId == verticalId && c.DepartmentId == deptId && c.IsActive == 1).ToList();
            if (s.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Save Save Reject with StandBy PNR from Revenue
        /// If Say "No & StandBy"
        /// </summary>
        /// <param name="objRevenueRejectModel"></param>
        /// <param name="strPrm"></param>
        /// <returns></returns>
        public bool SaveRejectwithStandByPNR_Revenue(TravelRequestMasterModels_RejectwithStandByPNR objRevenueRejectModel)
        {
            _context.TravelRequestMasterModels_RejectwithStandByPNR.Add(objRevenueRejectModel);
            var s = _context.SaveChanges();
            return s > 0 ? true : false;
        }

    }
}