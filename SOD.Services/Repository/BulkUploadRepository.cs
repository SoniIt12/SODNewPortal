using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;
using SOD.Services.Interface;
using SOD.Services.EntityFramework;
using System.Globalization;
using System.Collections;

namespace SOD.Services.Repository
{
    public class BulkUploadRepository : IBulkUploadRepository
    {

        #region "Constructor Initialization"
        /// <summary>
        /// Constructor Initilization
        /// </summary>
        private readonly SodEntities _context;
        public BulkUploadRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
        #endregion



        /// <summary>
        /// Save Bulk Booking in DB (Temp Table)
        /// </summary>
        /// <param name="bulkUploadModels"></param>
        /// <returns></returns>
        public int SaveBulkUploadTemp(BulkUploadMasterModels bulkUploadMasterModels, List<BulkUploadModels> bulkUploadDetailsModels)
        {
            var s = 0;
            //for bulk master

            _context.BulkUploadMasterModel.Add(bulkUploadMasterModels);
            s = _context.SaveChanges();



            //Get Max Request Id
            var trMaxId = _context.BulkUploadMasterModel.DefaultIfEmpty().Max(x => x == null ? 1 : x.TRId);

            //for bulk Upload details
            foreach (var list in bulkUploadDetailsModels)
            {
                list.TrnId = trMaxId;
            }

            _context.BulkUploadModel.AddRange(bulkUploadDetailsModels);
            s = _context.SaveChanges();
            return s > 0 ? Int32.Parse(trMaxId.ToString()) : 0;
        }



        /// <summary>
        /// Update Bulk Booking PNR Status
        /// </summary>
        /// <param name="bulkUploadDetailsModels"></param>
        /// <returns></returns>
        public int UpdatePNRStatusList(int s, List<BulkEmployeeList> bulkEmployeeList)
        {
            //for bulk Upload details
            var blist = new List<BulkUploadModels>();
            blist = _context.BulkUploadModel.Where(x => x.TrnId == s).ToList();
            //bulkEmployeeList
            foreach (var elist in bulkEmployeeList)
            {
                foreach (var list in blist)
                {
                    if (elist.EmpCode == list.EmpCode && elist.BTrId == list.BReqId && elist.PNRStatus.Split('|')[0] != null)
                    {
                        list.PNR = elist.PNRStatus.Split('|')[0];
                        list.PNR_Status = elist.PNRStatus.Split('|')[0].Equals("ERR001") ? Convert.ToInt16("2") : Convert.ToInt16("1");
                    }
                }
            }
            var i = _context.SaveChanges();
            //Manage Status
            var trlistCount = _context.BulkUploadModel.Where(x => x.TrnId == s).ToList().Count();
            var trstatusCount = _context.BulkUploadModel.Where(x => x.TrnId == s && x.PNR_Status != 0).ToList().Count();
            if (trlistCount == trstatusCount)
            {
                var objBulkUploadMasterModels = new BulkUploadMasterModels();
                objBulkUploadMasterModels = _context.BulkUploadMasterModel.Where(x => x.TRId == s).SingleOrDefault();
                objBulkUploadMasterModels.FileStatus = "Close";
                objBulkUploadMasterModels.CreatedDate = System.DateTime.Now;
                _context.SaveChanges();
            }
            else if (trstatusCount > 0)
            {
                var objBulkUploadMasterModels = new BulkUploadMasterModels();
                objBulkUploadMasterModels = _context.BulkUploadMasterModel.Where(x => x.TRId == s).SingleOrDefault();
                objBulkUploadMasterModels.FileStatus = "Pending";
                objBulkUploadMasterModels.CreatedDate = System.DateTime.Now;
                _context.SaveChanges();
            }
            return i;
        }


        /// <summary>
        /// Reject Bulk Booking
        /// </summary>
        /// <param name="breqIdList"></param>
        /// <param name="trID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int UpdateStatusOnRejection(List<long> breqIdList, string trID, string type)
        {
            var i = 0;
            foreach (var itm in breqIdList)
            {
                var blist = new List<BulkUploadModels>();
                if (type == "B")
                {
                    blist = _context.BulkUploadModel.Where(x => x.BReqId == itm).ToList();
                }
                else
                {
                    var trid = Convert.ToInt64(trID);
                    blist = _context.BulkUploadModel.Where(x => x.TrnId == trid && x.EmpCode == itm.ToString()).ToList();
                }
                blist[0].PNR = "";
                blist[0].PNR_Status = 3;
            }
            i = _context.SaveChanges();
            return i;
        }


        /// <summary>
        /// Get Current Uploaded Excel File Data : After Generating PNR
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        public List<BulkUpload_ExcelExport> GetCurrentBulkBookingData_ExportToExcel(int TrId)
        {
            var blist = new List<BulkUpload_ExcelExport>();
            var result = _context.BulkUploadModel.Where(x => x.TrnId == TrId).ToList();
            var counter = 1;
            foreach (var item in result)
            {
                var lst = new BulkUpload_ExcelExport();
                lst.SrNo = counter;
                lst.EmpCode = item.EmpCode;
                lst.Title = item.Title;
                lst.FirstName = item.FirstName;
                lst.LastName = item.LastName;
                lst.Designation = item.Designation;
                lst.Department = item.Department;
                lst.MobileNo = item.MobileNo;
                lst.TravelDate = item.TravelDate;
                lst.FlightNo = item.FlightNo;
                lst.Sector = item.Sector;
                lst.Purpose = item.Purpose;
                lst.Meal = item.Meal;
                lst.Beverage = item.Beverage;
                lst.PNR = item.PNR;
                lst.BookingType = item.BookingType;
                blist.Add(lst);
                counter++;
            }
            return blist;
        }



        /// <summary>
        /// Validatee Employee Code with DB Record
        /// </summary>
        /// <param name="empList"></param>
        /// <returns></returns>
        public List<string> ValidateEmployeeCode(List<string> empList)
        {
            var s = ADO.SodCommonServices.ValidateEmployeeCode(empList, 1);
            return s;
        }


        /// <summary>
        /// Validatee Employee Verticals
        /// </summary>
        /// <param name="empList"></param>
        /// <returns></returns>
        public List<string> ValidateEmployeeVerticals(List<string> empVerticalList, string verticalList)
        {
            var s = ADO.SodCommonServices.ValidateEmployeeVerticals(empVerticalList, verticalList, 1);
            return s;
        }


        /// <summary>
        /// Get Bulk Booking Master Data
        /// </summary>
        /// <param name="fromdate"></param
        /// <param name="todate"></param>
        /// <returns></returns>
        public List<BulkUploadMasterModels> GetBulkBookingMasterData(DateTime fromdate, DateTime todate, Int32 DepartmentId, string VerticalCode, string BookingType)
        {
            var blist = new List<BulkUploadMasterModels>();
            var counter = 0;
            var tdate = todate.AddDays(1);
            if (DepartmentId == 0 && VerticalCode == "")
            {
                var result = _context.BulkUploadMasterModel.Where(x => x.CreatedDate >= fromdate.Date && x.CreatedDate <= tdate && x.BookingType == BookingType).ToList().OrderByDescending(x => x.TRId);
                foreach (var item in result)
                {
                    counter = counter + 1;
                    var lst = new BulkUploadMasterModels();
                    lst.SNo = counter;
                    lst.TransactionDate = item.TransactionDate;
                    lst.TRId = item.TRId;
                    lst.FileName = item.FileName;
                    lst.CreatedById = item.CreatedById;
                    lst.CreatedByName = item.CreatedByName;
                    lst.CreatedDate = item.CreatedDate;
                    lst.BookingType = item.BookingType;
                    lst.FileStatus = item.FileStatus;
                    blist.Add(lst);
                }
            }
            else
            {
                var result = _context.BulkUploadMasterModel.Where(x => x.CreatedDate >= fromdate.Date && x.CreatedDate <= tdate && x.BookingType == BookingType && x.VerticalCode == VerticalCode && x.DepartmentId == DepartmentId).ToList().OrderByDescending(x => x.TRId);

                foreach (var item in result)
                {
                    counter = counter + 1;
                    var lst = new BulkUploadMasterModels();
                    lst.SNo = counter;
                    lst.TransactionDate = item.TransactionDate;
                    lst.TRId = item.TRId;
                    lst.FileName = item.FileName;
                    lst.CreatedById = item.CreatedById;
                    lst.CreatedByName = item.CreatedByName;
                    lst.CreatedDate = item.CreatedDate;
                    lst.BookingType = item.BookingType;
                    lst.FileStatus = item.FileStatus;
                    blist.Add(lst);
                }
            }

            return blist;
        }



        /// <summary>
        /// Get Bulk Booking Master For HOD Approval
        /// </summary>
        /// <returns></returns>
        public List<BulkUploadMasterModels> GetBulkBookingHODApprovalList_MasterData(String empcode, int criteria)
        {
            var blist = ADO.SodCommonServices.GetBulkBookingHODApprovalList_MasterData(empcode, criteria);
            return blist;
        }

        /// <summary>
        /// Get hotel bulk booking data for traveldesk
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<BulkUploadMasterModels> GetBulkBookingHotelListData(string bookingType)
        {
            var blist = ADO.SodCommonServices.GetBulkBookingHotelListData(bookingType);
            return blist;
        }

        /// <summary>
        /// Bulk Upload Details Data
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        public List<BulkUploadModels> GetBulkBookinDetailsData(string TrId)
        {
            var blist = new List<BulkUploadModels>();
            long trid = Convert.ToInt64(TrId);

            var result = _context.BulkUploadModel.Where(x => x.TrnId == trid).ToList();
            foreach (var item in result)
            {
                var lst = new BulkUploadModels();
                lst.BReqId = item.BReqId;
                lst.Title = item.Title;
                lst.EmpCode = item.EmpCode;
                lst.FirstName = item.FirstName;
                lst.LastName = item.LastName;
                lst.Sector = item.Sector.ToUpper();
                lst.FlightNo = item.FlightNo;
                lst.PNR = item.PNR;
                lst.PNR_Status = item.PNR_Status;
                lst.TravelDate = item.TravelDate;
                lst.BookingType = item.BookingType;
                lst.IsHotelRequired = item.IsHotelRequired;
                lst.Meal = item.Meal.ToUpper();
                lst.Beverage = item.Beverage.ToUpper();
                lst.AgencyCode = item.AgencyCode;
                lst.CheckInDate = item.CheckInDate;
                lst.CheckOutDate = item.CheckOutDate;
                lst.CheckinTime = item.CheckinTime;
                lst.CheckoutTime = item.CheckoutTime;
                lst.AgencyCode = item.AgencyCode;
                lst.MobileNo = item.MobileNo;
                lst.AirportTransport = item.AirportTransport;
                lst.IsBookingcancelled = item.IsBookingcancelled;
                blist.Add(lst);
            }
            return blist;
        }

        /// <summary>
        /// Is emp rest for approval
        /// </summary>
        /// <param name="travelRequest"></param>
        /// <param name="AddNo"></param>
        /// <returns></returns>
        public List<BulkUploadModels> IsEmpRestForApproval(int travelRequest, int AddNo)
        {
            var blist = new List<BulkUploadModels>();
            long trid = Convert.ToInt64(travelRequest);
            var result = _context.BulkUploadModel.Where(x => x.TrnId == trid && x.AddNo == AddNo).ToList();
            foreach (var item in result)
            {
                if ((item.PNR_Status == 0) && (item.IsBookingcancelled != true))
                {
                    var lst = new BulkUploadModels();
                    lst.BReqId = item.BReqId;
                    lst.AddNo = item.AddNo;
                    lst.IsHotelRequired = item.IsHotelRequired;//added by soni 16 sep 2019
                    blist.Add(lst);
                }
            }
            return blist;
        }


        /// <summary>
        /// Is emp rest for approval
        /// </summary>
        /// <param name="travelRequest"></param>
        /// <param name="AddNo"></param>
        /// <returns></returns>
        public List<BulkUploadModels> IsEmpRestForApproval(int travelRequest)
        {
            var blist = new List<BulkUploadModels>();
            long trid = Convert.ToInt64(travelRequest);
            var result = _context.BulkUploadModel.Where(x => x.TrnId == trid).ToList();
            foreach (var item in result)
            {
                if ((item.PNR_Status == 0) && (item.IsBookingcancelled != true))
                {
                    var lst = new BulkUploadModels();
                    lst.BReqId = item.BReqId;
                    lst.AddNo = item.AddNo;
                    blist.Add(lst);
                }
            }
            return blist;
        }

        /// <summary>
        /// Get hotel details for a travelRequestId
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        public List<BulkUploadModels> GetHotelListPopup(string TrId)
        {
            var blist = ADO.SodCommonServices.GetHotelListPopup(TrId);
            return blist;
        }


        /// <summary>
        /// Fetch Employee details with department wise
        /// </summary>
        /// <param name="newEcode"></param>
        /// <param name="dept"></param>
        /// <returns></returns>
        public List<EmdmAPIModels> fetchdetails(string newEcode, int dept)
        {
            string eCode = newEcode.Length == 8 ? newEcode : newEcode.Insert(0, "00");
            if (dept == 0)
            {
                var list = _context.EmdmAPIModel.Where(o => o.EmployeeCode == eCode).ToList();
                return list;
            }
            else
            {
                var list = _context.EmdmAPIModel.Where(o => o.EmployeeCode == eCode && o.DepartmentID == dept).ToList();
                return list;
            }
        }


        /// <summary>
        /// Get employee details
        /// </summary>
        /// <param name="Empcode"></param>
        /// <returns></returns>
        public List<EmdmAPIModels> fetchEmpdetails(string Empcode)
        {
            string eCode = Empcode.Length == 8 ? Empcode : Empcode.Insert(0, "00");
            var list = _context.EmdmAPIModel.Where(o => o.EmployeeCode == eCode).ToList();
            return list;
        }

        /// <summary>
        /// Check Duplicacy of employee details
        /// </summary>
        /// <param name="newEcode"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        public string check_duplicacy(string newEcode, Int64 trid)
        {
            var check_duplicate = _context.BulkUploadModel.Where(x => x.TrnId == trid && x.EmpCode == newEcode).ToList();
            if (check_duplicate.Count > 0)
            {
                return "Exist";
            }
            return "not Exist";
        }

        /// <summary>
        /// This method will be used for to replace employee details against the same PNR
        /// </summary>
        /// <param name="newECode"></param>
        /// <param name="upDatedrow"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        public Int64 UpdateBulkDetail(string oldEcode, List<BulkUploadModels> upDatedrow, Int64 trid)
        {
            //var blist = new List<BulkUploadModels>();
            int s = 0;
            string eCode = upDatedrow[0].EmpCode.Length == 8 ? upDatedrow[0].EmpCode : upDatedrow[0].EmpCode.Insert(0, "00");
            var list = _context.EmdmAPIModel.Where(o => o.EmployeeCode == eCode).ToList();
            if (list.Count > 0)
            {
                var oldEmpcode = oldEcode;
                var sector = upDatedrow[0].Sector.Trim();
                var updated1 = _context.BulkUploadModel.Where(x => x.TrnId == trid && x.EmpCode == oldEcode && x.Sector == sector).ToList();
                foreach (var p in updated1)
                {
                    p.EmpCode = upDatedrow[0].EmpCode;
                    p.FirstName = list[0].FirstName;
                    p.LastName = list[0].LastName;
                    if (list[0].Gender == "M")
                    {
                        p.Title = "Mr";
                    }
                    else
                    {
                        p.Title = "Ms";
                    }
                    p.Department = list[0].DepartmentName;
                    p.Designation = list[0].DesignationName;
                    p.MobileNo = list[0].Phone;
                    p.EmailId = list[0].Email;
                    if (updated1[0].IsHotelRequired)
                    {
                        p.CheckInDate = upDatedrow[0].CheckInDate;
                        p.CheckOutDate = upDatedrow[0].CheckOutDate;
                    }
                    p.IsChange = true;
                }

                s = _context.SaveChanges();
                return s;
            }
            else
            {
                return s = -1;
            }
        }

        /// <summary>
        /// This method will be used to update hotel details against the same PNR
        /// </summary>
        /// <param name="newECode"></param>
        /// <param name="upDatedrow"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        public Int64 UpdateHotelDetails(string oldEcode, List<BulkUploadModels> upDatedrow, Int64 trid)
        {
            int s = 0;
            var blist = _context.BulkTravelRequestHotelDetailModel.Where(x => x.TravelRequestId == trid && x.EmployeeCode == oldEcode).ToList();
            if (blist.Count > 0)
            {
                blist[0].EmployeeCode = upDatedrow[0].EmpCode;
                blist[0].CheckInDate = upDatedrow[0].CheckInDate;
                blist[0].CheckOutDate = upDatedrow[0].CheckOutDate;
            }
            s = _context.SaveChanges();
            return s;
        }

        /// <summary>
        /// Cancel Booking Details
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        public int CancelBookingRequest(string empcode, Int64 trid, String ReasonForCancellation)
        {
            int s = 0;
            var RemovedDataList = _context.BulkUploadModel.Where(x => x.TrnId == trid && x.EmpCode == empcode).ToList();
            if (RemovedDataList.Count > 0)
            {
                RemovedDataList[0].IsBookingcancelled = true;
                RemovedDataList[0].ReasonForCancellation = ReasonForCancellation;
            }

            s = _context.SaveChanges();
            return s;
        }

        /// <summary>
        /// Save New Row for Bulk Booking
        /// </summary>
        /// <param name="DetalsList"></param>
        /// <returns></returns>
        public int saveBulk_newRow(List<BulkUploadModels> DetalsList)
        {
            var s = 0;
            _context.BulkUploadModel.AddRange(DetalsList);
            var trnId = DetalsList[0].TrnId;
            var blist = _context.BulkUploadMasterModel.Where(x => x.TRId == trnId).ToList();

            if (blist[0].FileStatus == "Close")
            {
                blist[0].FileStatus = "Pending";
            }

            s = _context.SaveChanges();
            return s;
        }


        /// <summary>
        /// Find new Added Employee Details
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        public int FindAddNo(Int64 trid)
        {
            var addNo = _context.BulkUploadModel.Where(p => p.TrnId == trid).Max(w => w == null ? 1 : w.AddNo);
            return addNo;
        }


        /// <summary>
        /// View traveldesk hotel status
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        public int GetViewstatushotel(string TrId)
        {
            var blist = new List<BulkTravelRequestHotelDetailModels>();
            long trid = Convert.ToInt64(TrId);
            var flag = 0;
            blist = _context.BulkTravelRequestHotelDetailModel.Where(x => x.TravelRequestId == trid).ToList();
            foreach (var i in blist)
            {
                if (i.HotelConfirmationNo == null || i.HotelConfirmationNo == "")
                {
                    flag = 1;
                }
            }
            return flag;
        }


        /// <summary>
        /// approve hotel request by traveldesk
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        public int ApprovedHotelRequest(string empcode, string TravelRequestId)
        {
            var s = 0;
            var reqId = Convert.ToInt64(TravelRequestId);

            var updateItem = _context.BulkTravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.EmployeeCode == empcode);
            foreach (var p in updateItem)
            {
                p.IsAllocated = 1;
            }
            s = _context.SaveChanges();
            return s;
        }


        /// <summary>
        /// get hotel details for mail
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <returns></returns>
        public List<BulkTravelRequestHotelDetailModels> GetHotelDetailForMail(string empcode, string TravelRequestId)
        {
            //var _context = new SodEntities();
            var blist = new List<BulkTravelRequestHotelDetailModels>();
            var reqId = Convert.ToInt64(TravelRequestId);
            blist = _context.BulkTravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.EmployeeCode == empcode).ToList();
            return blist;
        }

        /// <summary>
        /// changes status from approval to Cancellation Approved
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        public int ChangeHotelStatus(string empcode, string TravelRequestId)
        {
            var _context = new SodEntities();
            var s = 0;
            var blist = new List<BulkTravelRequestHotelDetailModels>();
            var reqId = Convert.ToInt64(TravelRequestId);
            blist = _context.BulkTravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.EmployeeCode == empcode).ToList();
            foreach (var item in blist)
            {
                item.HotelStatus = "Cancellation Confirm";
            }
            s = _context.SaveChanges();
            return s;
        }


        /// <summary>
        /// get user details for mail
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <returns></returns>
        public List<BulkUploadModels> GetUserDetailForMail(string empcode, string TravelRequestId)
        {
            var _context = new SodEntities();
            var blist = new List<BulkUploadModels>();
            var reqId = Convert.ToInt64(TravelRequestId);
            blist = _context.BulkUploadModel.Where(o => o.TrnId == reqId && o.EmpCode == empcode).ToList();
            return blist;
        }


        /// <summary>
        /// Get Shared User Details
        /// </summary>
        /// <param name="sharingid"></param>
        /// <param name="clubid"></param>
        /// <param name="empcode"></param>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        public List<BulkUploadModels> GetSharedUserDetails(Int16 sharingid, Int32 clubid, string empcode, string TravelRequestId)
        {
            var _context = new SodEntities();
            var blist = new List<BulkUploadModels>();
            var shareList = new List<BulkTravelRequestHotelDetailModels>();
            var reqId = Convert.ToInt64(TravelRequestId);

            shareList = _context.BulkTravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.clubId == clubid && o.sharingId == sharingid && o.EmployeeCode != empcode).ToList();
            if (shareList.Count > 0)
            {
                var sharedemployeecode = shareList[0].EmployeeCode;

                var list = _context.BulkUploadModel.Where(o => o.EmpCode == sharedemployeecode).ToList()[0];
                blist.Add(new BulkUploadModels()
                {
                    TrnId = list.TrnId,
                    FirstName = list.FirstName,
                    LastName = list.LastName,
                    Title = list.Title,
                    EmailId = list.EmailId,
                    EmpCode = list.EmpCode,
                    MobileNo = list.MobileNo,
                    CheckInDate = list.CheckInDate,
                    CheckOutDate = list.CheckOutDate,
                    TravelDate = list.TravelDate,
                    sharingId = shareList[0].sharingId
                }
                );
            }
            return blist;
        }

        /// <summary>
        /// Get User details for sending email to Spoc & cc to travel desk
        /// </summary>
        /// <param name="?"></param>
        /// <param name="travelrequestid"></param>
        /// <returns></returns>
        public List<BulkUploadModels> GetSharedUserDetailsNew(int clubId, Int64 travelrequestid)
        {
            var blist1 = new List<BulkUploadModels>();
            var blist = (from a in _context.BulkUploadModel
                         join b in _context.BulkTravelRequestHotelDetailModel
                         on a.BReqId equals b.BReqId
                         where (a.TrnId == b.TravelRequestId && b.clubId == clubId)
                         select new
                         {
                             a.TrnId,
                             a.FirstName,
                             a.LastName,
                             a.Title,
                             a.EmailId,
                             a.EmpCode,
                             a.MobileNo,
                             a.CheckInDate,
                             a.CheckOutDate,
                             a.TravelDate,
                             b.sharingId
                         }).ToList();
            if (blist.Count > 0)
            {
                foreach (var s in blist)
                {
                    var bmodel = new BulkUploadModels();
                    bmodel.TrnId = s.TrnId;
                    bmodel.FirstName = s.FirstName;
                    bmodel.LastName = s.LastName;
                    bmodel.Title = s.Title;
                    bmodel.EmailId = s.EmailId;
                    bmodel.EmpCode = s.EmpCode;
                    bmodel.MobileNo = s.MobileNo;
                    bmodel.CheckInDate = s.CheckInDate;
                    bmodel.CheckOutDate = s.CheckOutDate;
                    bmodel.TravelDate = s.TravelDate;
                    bmodel.sharingId = s.sharingId;
                    blist1.Add(bmodel);
                }
            }
            return blist1;
        }

        /// <summary>
        /// Get Bulk Booking PNR Wise Data
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <returns></returns>
        public List<BulkUploadModels> GetBulkBookingPNRWiseDetails(DateTime fromdate, DateTime todate, String empcode, int criteria)
        {
            var blist = ADO.SodCommonServices.GetBulkBookingPNRWiseDetails(fromdate, todate, empcode, criteria);
            return blist;
        }


        /// <summary>
        /// Emport PNR wise details
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<BulkUpload_ExcelExportPNRWise> GetBulkBookingPNRWiseDetails_ExportToExcel(DateTime fromdate, DateTime todate, string empcode, int criteria)
        {
            var blist = ADO.SodCommonServices.GetBulkBookingPNRWiseDetails_ExportToExcel(fromdate, todate, empcode, criteria);
            return blist;
        }


        /// <summary>
        /// Get Employee Code wise Details 
        /// </summary>
        /// <param name="empList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<EmployeeCodewiseDetailModel> GetEmployeeCodewiseDetails(List<string> empList, int criteria)
        {
            var s = ADO.SodCommonServices.GetEmployeeCodewiseDetails(empList, criteria);
            return s;
        }


        /// <summary>
        /// Get Employee Booking Agency Rights
        /// </summary>
        /// <param name="empCode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<string> GetEmployeeBookingAgencyRight(string empCode, int criteria)
        {
            var s = ADO.SodCommonServices.GetEmployeeBookingAgencyRight(empCode, criteria);
            return s;
        }

        ///// <summary>
        ///// Get Department Right Employee wise
        ///// </summary>
        ///// <param name="empCode"></param>
        ///// <param name="criteria"></param>
        ///// <returns></returns>
        public List<EmployeeDepartmentRights> GetEmployeeDepartmentRight(string empCode, int criteria)
        {
            var s = _context.EmployeeDepartmentRight.Where(a => a.EmpCode == empCode).ToList();
            return s;
        }


        /// <summary>
        /// To save Approved/Reject Bulk Booking Request
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        public int ApproveBulkSodBookingRequest(BulkBookingRequestApprovalModels bulkBookingRequestApprovalModel)
        {
            var TrnId = bulkBookingRequestApprovalModel.TrRequestId;
            var c = _context.BulkBookingRequestApprovalModel.Where(o => o.TrRequestId == bulkBookingRequestApprovalModel.TrRequestId && o.BReqId == bulkBookingRequestApprovalModel.BReqId).ToList();
            if (c.Count() > 0)
                return 0;

            _context.BulkBookingRequestApprovalModel.Add(bulkBookingRequestApprovalModel);
            List<BulkUploadMasterModels> bkMaster = _context.BulkUploadMasterModel.Where(y => y.TRId == TrnId).ToList();
            if (bkMaster.Count > 0)
            {
                bkMaster[0].CreatedDate = DateTime.Now;
            }
            var s = _context.SaveChanges();

            return s;
        }
        public int ApproveBulkSodBookingRequestSelective(List<BulkBookingRequestApprovalModels> bulkBookingRequestApprovalModel)
        {
            var s = 0;
            var TrnId = bulkBookingRequestApprovalModel[0].TrRequestId;
            _context.BulkBookingRequestApprovalModel.AddRange(bulkBookingRequestApprovalModel);

            List<BulkUploadMasterModels> bkMaster = _context.BulkUploadMasterModel.Where(y => y.TRId == TrnId).ToList();
            if (bkMaster.Count > 0)
            {
                bkMaster[0].CreatedDate = DateTime.Now;
            }

            s = _context.SaveChanges();
            return s;
        }



        /// <summary>
        /// Rollback approval status by HOD
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        public int RollBackApprovalByHOD(BulkBookingRequestApprovalModels bulkBookingRequestApprovalModel)
        {
            //Bulk Booking Request approval rollback status
            var TrnId = bulkBookingRequestApprovalModel.TrRequestId;
            var rollbackItem = _context.BulkBookingRequestApprovalModel.Where(o => o.TrRequestId == bulkBookingRequestApprovalModel.TrRequestId).ToList();
            if (rollbackItem.Count() > 0)
            {
                foreach (var p in rollbackItem)
                {
                    p.ApprovalStatus = bulkBookingRequestApprovalModel.ApprovalStatus;
                    p.ApprovedByEmpId = 0;
                    p.Comment = bulkBookingRequestApprovalModel.Comment;
                }
            }
            List<BulkUploadMasterModels> bkMaster = _context.BulkUploadMasterModel.Where(y => y.TRId == TrnId).ToList();
            if (bkMaster.Count > 0)
            {
                bkMaster[0].CreatedDate = DateTime.Now;
            }
            return _context.SaveChanges();
        }



        /// <summary>
        /// Get Bulk Booking Info For PNR Generation
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetBulkBookingInfoForPNRGeneration(Int64 travelReqId)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var bulkMaster = new List<BulkUploadMasterModels>();
            var bulkDetails = new List<BulkUploadModels>();

            bulkMaster = _context.BulkUploadMasterModel.Where(m => m.TRId == travelReqId).ToList();
            bulkDetails = _context.BulkUploadModel.Where(d => d.TrnId == travelReqId).ToList();

            dicInfo.Add("bulkMasterInfo", bulkMaster);
            dicInfo.Add("bulkDetailInfo", bulkDetails);

            return dicInfo;
        }

        public Dictionary<string, object> GetBulkBookingInfoForPNRGeneration_edit(Int64 travelReqId, Int32 AddNo)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            //var bulkMaster = new List<BulkUploadMasterModels>();
            var bulkDetails = new List<BulkUploadModels>();
            //bulkMaster = _context.BulkUploadMasterModel.Where(m => m.TRId == travelReqId).ToList();
            bulkDetails = _context.BulkUploadModel.Where(d => d.TrnId == travelReqId && d.AddNo == AddNo).ToList();
            dicInfo.Add("bulkDetailInfoEdit", bulkDetails);
            return dicInfo;
        }



        /// <summary>
        /// Get BulkBooking Data for selective Pnr generation
        /// </summary>
        /// <param name="vlist"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetBulkBookingInfoForPNRGenerationSelective(List<long> vlist)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var bulkDetails = new List<BulkUploadModels>();
            bulkDetails = _context.BulkUploadModel.Where(t => vlist.Contains(t.BReqId)).ToList();
            dicInfo.Add("bulkDetailInfoSelective", bulkDetails);
            return dicInfo;
        }



        /// <summary>
        /// To save Reject Booking Request
        /// </summary>
        /// <param name="BulkBookingRequestApprovalModels"></param>
        /// <returns></returns>
        public int RejectSodBulkBookingRequest(BulkBookingRequestApprovalModels bulkBookingRequestApprovalModel)
        {
            var s = 0;
            var TrnId = bulkBookingRequestApprovalModel.TrRequestId;
            //For Bulk Booking Travel Request Approval
            var updateItem = _context.BulkBookingRequestApprovalModel.Where(o => o.TrRequestId == bulkBookingRequestApprovalModel.TrRequestId && o.AddNo == bulkBookingRequestApprovalModel.AddNo).ToList();
            if (updateItem.Count() > 0)
            {
                foreach (var p in updateItem)
                {
                    p.ApprovalStatus = bulkBookingRequestApprovalModel.ApprovalStatus;
                    p.ApprovedByEmpId = bulkBookingRequestApprovalModel.ApprovedByEmpId;
                    p.ApprovalDate = System.DateTime.Now;
                    p.Comment = bulkBookingRequestApprovalModel.Comment;
                }
            }
            else
            {
                _context.BulkBookingRequestApprovalModel.Add(bulkBookingRequestApprovalModel);
            }
            List<BulkUploadMasterModels> bkMaster = _context.BulkUploadMasterModel.Where(y => y.TRId == TrnId).ToList();
            if (bkMaster.Count > 0)
            {
                bkMaster[0].CreatedDate = DateTime.Now;
            }
            return _context.SaveChanges();
        }

        /// <summary>
        /// Reject Sod Bulk Booking Request Edit
        /// </summary>
        /// <param name="bulkBookingRequestApprovalModel"></param>
        /// <param name="AddNo"></param>
        /// <returns></returns>
        public int RejectSodBulkBookingRequest_edit(BulkBookingRequestApprovalModels bulkBookingRequestApprovalModel, Int32 AddNo)
        {
            var s = 0;
            var TrnId = bulkBookingRequestApprovalModel.TrRequestId;
            //For Bulk Booking Travel Request Approval
            var updateItem = _context.BulkBookingRequestApprovalModel.Where(o => o.TrRequestId == bulkBookingRequestApprovalModel.TrRequestId && o.AddNo == AddNo).ToList();
            if (updateItem.Count() > 0)
            {
                foreach (var p in updateItem)
                {
                    p.ApprovalStatus = bulkBookingRequestApprovalModel.ApprovalStatus;
                    p.ApprovedByEmpId = bulkBookingRequestApprovalModel.ApprovedByEmpId;
                    p.ApprovalDate = System.DateTime.Now;
                    p.Comment = bulkBookingRequestApprovalModel.Comment;
                }
            }
            else
            {
                _context.BulkBookingRequestApprovalModel.Add(bulkBookingRequestApprovalModel);
            }
            List<BulkUploadMasterModels> bkMaster = _context.BulkUploadMasterModel.Where(y => y.TRId == TrnId).ToList();
            if (bkMaster.Count > 0)
            {
                bkMaster[0].CreatedDate = DateTime.Now;
            }
            return _context.SaveChanges();
        }


        /// <summary>
        /// Rejection & Close  Bulk Booking
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="Criteria"></param>
        /// <returns></returns>
        public int RejectionCloseBulkBookingRequest(Int64 TravelRequestId, int Criteria)
        {
            if (Criteria == 1)
            {
                var result = _context.BulkUploadMasterModel.Where(x => x.TRId == TravelRequestId).ToList();
                result[0].FileStatus = "Rejected";
            }
            return _context.SaveChanges();
        }



        /// <summary>
        /// Bulk Booking List
        /// </summary>
        /// <param name="blist"></param>
        /// <returns></returns>
        public List<string> ValidateDuplicateBulkBooking(List<BulkUploadModels> blist)
        {
            var bl = new List<string>();
            var vlist = new List<string>();
            //group by date
            foreach (var item in blist.GroupBy(g => new { g.TravelDate }).Select(g => g.FirstOrDefault()))
            {
                vlist.Add(item.TravelDate);
            }
            //Filter by date
            var s = _context.BulkUploadModel.Where(x => vlist.Contains(x.TravelDate)).ToList();
            //Check duplicacy
            foreach (var slst in blist)
            {
                foreach (var mlst in s)
                {
                    if (mlst.EmpCode == slst.EmpCode
                       && mlst.FlightNo == slst.FlightNo
                       && mlst.Sector == slst.Sector
                       && mlst.TravelDate == slst.TravelDate
                       && mlst.BookingType.ToLower() == slst.BookingType.ToLower()
                       && mlst.PNR != "ERR001"
                       && mlst.PNR != "")
                    {
                        bl.Add(mlst.EmpCode + "|" + mlst.FlightNo + "|" + mlst.TravelDate + "|" + mlst.Sector + "|" + mlst.BookingType);
                    }
                }
            }

            //Remove duplicacy
            return bl.Distinct().ToList();
        }

        /// <summary>
        /// Save bulk hotel data
        /// </summary>
        /// <param name="hlist"></param>
        /// <returns></returns>
        public int SaveBulkHotelUpload(List<BulkTravelRequestHotelDetailModels> hlist)
        {
            _context.BulkTravelRequestHotelDetailModel.AddRange(hlist);

            var trid = hlist[0].TravelRequestId;
            var updateItem = _context.BulkUploadMasterModel.Where(o => o.TRId == trid).ToList();
            foreach (var item in updateItem)
            {
                item.HotelStatus = 1;
            }

            var s = _context.SaveChanges();
            return s;
        }

        /// <summary>
        /// save approval request for non-contractual by HOD from traveldesk
        /// </summary>
        /// <param name="elist"></param>
        /// <param name="hodemail1"></param>
        /// <param name="hodemail2"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public int SaveHODFinancialApprovalRequest(List<BulkTravelRequestHotelDetailModels> hlist, string hodemail1, string hodemail2, List<BulkHotelInclusionNonContractualMasterModels> inclist, string sodOat)
        {
            var s = 0;
            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.BulkTravelRequestHotelDetailModel.AddRange(hlist);
                    for (var i = 0; i < hlist.Count; i++)
                    {
                        var reqId = Convert.ToInt64(hlist[i].TravelRequestId);
                        var BreqId = Convert.ToInt64(hlist[i].BReqId);
                        if (sodOat == "SOD")
                        {
                            var updateItem = _context.BulkUploadMasterModel.Where(o => o.TRId == reqId).ToList();
                            foreach (var item in updateItem)
                            {
                                item.HotelStatus = 1;
                            }

                            var hodList1 = _context.NonContractualHotelApprovalMasterModel.Where(n => n.EmailId == hodemail1).ToList();
                            if (hodemail2.Length > 0)
                            {
                                var hodList2 = _context.NonContractualHotelApprovalMasterModel.Where(n => n.EmailId == hodemail2).ToList();
                                var applist = new BulkBookingHODFinancialApprovalModels()
                                {
                                    TravelRequestId = reqId,
                                    ApproverEmpCodeLevel1 = hodList1[0].EmpCode,
                                    ApproverEmpCodeLevel2 = hodList2[0].EmpCode,
                                    ApproverNameLevel1 = hodList1[0].EmpName,
                                    ApproverNameLevel2 = hodList2[0].EmpName,
                                    ApprovalDateLevel1 = new DateTime(1900, 1, 1),
                                    ApprovalDateLevel2 = new DateTime(1900, 1, 1),
                                    ApprovalStatusLevel1 = 0,
                                    ApprovalStatusLevel2 = 0,
                                    ApprovalStatus = 0,
                                    HotelRequestId = inclist[0].HotelRequestId,
                                    BreqId = hlist[i].BReqId
                                };
                                _context.BulkBookingHODFinancialApprovalModel.Add(applist);
                            }
                            else
                            {
                                var applist = new BulkBookingHODFinancialApprovalModels()
                                {
                                    TravelRequestId = reqId,
                                    ApproverEmpCodeLevel1 = hodList1[0].EmpCode,
                                    ApproverNameLevel1 = hodList1[0].EmpName,
                                    ApprovalDateLevel1 = new DateTime(1900, 1, 1),
                                    ApprovalDateLevel2 = new DateTime(1900, 1, 1),
                                    ApprovalStatusLevel1 = 0,
                                    ApprovalStatusLevel2 = 1,
                                    ApprovalStatus = 0,
                                    HotelRequestId = inclist[0].HotelRequestId,
                                    BreqId = hlist[i].BReqId
                                };
                                _context.BulkBookingHODFinancialApprovalModel.Add(applist);
                            }
                            var newIncList = new BulkHotelInclusionNonContractualMasterModels()
                            {
                                TravelRequestId = reqId,
                                HotelName = inclist[i].HotelName,
                                Accomodation = inclist[i].Accomodation,
                                Food = inclist[i].Food,
                                AirportTransfers = inclist[i].AirportTransfers,
                                RoomService = inclist[i].RoomService,
                                BuffetTime = inclist[i].BuffetTime,
                                Laundry = inclist[i].Laundry,
                                BreqId = hlist[i].BReqId,
                                HotelRequestId = inclist[i].HotelRequestId
                            };
                            _context.BulkHotelInclusionNonContractualMasterModel.Add(newIncList);
                        }
                        s = _context.SaveChanges();
                    }
                    dbTran.Commit();
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
        /// get bulk user info for hotel
        /// </summary>
        /// <param name="requestList"></param>
        /// <param name="hotelname"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetBulkHotelandUserInfo(Int64 travelRequestID, Int32 clubid, string hotelname, int criteria)
        {
            return ADO.SodCommonServices.GetBulkHotelandUserInfo(travelRequestID, clubid, criteria);
        }

        /// <summary>
        /// find clubid list from bulk booking details
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        public List<BulkTravelRequestHotelDetailModels> FindClubid(string empcode, string TravelRequestId)
        {
            var trid = Convert.ToInt64(TravelRequestId);
            var list = new List<BulkTravelRequestHotelDetailModels>();
            list = _context.BulkTravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.EmployeeCode == empcode).ToList();
            var clubid = list[0].clubId;

            var returnList = new List<BulkTravelRequestHotelDetailModels>();
            returnList = _context.BulkTravelRequestHotelDetailModel.Where(o => o.clubId == clubid && o.TravelRequestId == trid).ToList();
            return returnList;
        }

        /// <summary>
        /// Find bulk List By ClubId
        /// </summary>
        /// <param name="clubid"></param>
        /// <returns></returns>
        public List<BulkTravelRequestHotelDetailModels> FindListByClubId(string clubid)
        {
            var cId = Convert.ToInt32(clubid);
            var returnList = new List<BulkTravelRequestHotelDetailModels>();
            returnList = _context.BulkTravelRequestHotelDetailModel.Where(o => o.clubId == cId).ToList();
            return returnList;
        }


        /// <summary>
        /// Approve Non-Contractual Hotel Request by hod
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="approverEmpcCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int BulkApproveNonContractualHotelRequest(string travelReqID, string hid, int clubId, string approverEmpcCode, string type)
        {
            var s = 0;
            var IdList = travelReqID.Split(',');
            var hidList = hid.Split(',');
            for (var i = 0; i < IdList.Length; i++)
            {
                var trid = Convert.ToInt64(IdList[i]);
                var hId = Convert.ToInt32(hidList[i]);
                var findIfRejected = _context.BulkTravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.clubId == clubId).ToList();
                foreach (var f in findIfRejected)
                {
                    if (f.HodApprovalStatus == "2")
                    {
                        return s;
                    }
                }
                var updateItem2 = _context.BulkBookingHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel1 == approverEmpcCode && o.HotelRequestId == hId).ToList();
                foreach (var p in updateItem2)
                {
                    if (p.ApproverEmpCodeLevel1 == approverEmpcCode && p.ApprovalStatusLevel1 == 1)
                    {
                        return s = -1;
                    }
                    p.ApprovalDateLevel1 = DateTime.Now;
                    p.ApprovalStatusLevel1 = 1;
                }

                var updateItem3 = _context.BulkBookingHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel2 == approverEmpcCode && o.HotelRequestId == hId).ToList();
                foreach (var p in updateItem3)
                {

                    if (p.ApproverEmpCodeLevel2 == approverEmpcCode && p.ApprovalStatusLevel2 == 1)
                    {
                        return s = -1;
                    }
                    else
                    {
                        p.ApprovalDateLevel2 = DateTime.Now;
                        p.ApprovalStatusLevel2 = 1;
                    }
                }

                var updateItem4 = _context.BulkBookingHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hId).ToList();
                foreach (var p in updateItem4)
                {
                    if (p.ApprovalStatusLevel1 == 1 && p.ApprovalStatusLevel2 == 1)
                    {
                        p.ApprovalStatus = 1;
                        foreach (var f in findIfRejected)
                        {
                            f.HodApprovalStatus = "1";
                        }
                    }
                    else
                    {
                        foreach (var f in findIfRejected)
                        {
                            f.HodApprovalStatus = "0";
                        }
                    }
                }
            }
            s = _context.SaveChanges();
            return s;
        }

        /// <summary>
        /// get hotel details and shared user details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetBulkHotelDetailbyTrID(long travelReqId)
        {
            //var _context = new SodEntities();
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();

            var hotelData = new List<BulkTravelRequestHotelDetailModels>();
            var dataForFinanacialApproval = new List<BulkBookingHODFinancialApprovalModels>();
            var inclist = new List<BulkHotelInclusionNonContractualMasterModels>();


            hotelData = _context.BulkTravelRequestHotelDetailModel.Where(a => a.TravelRequestId == travelReqId).ToList();
            dataForFinanacialApproval = _context.BulkBookingHODFinancialApprovalModel.Where(a => a.TravelRequestId == travelReqId).ToList();
            inclist = _context.BulkHotelInclusionNonContractualMasterModel.Where(a => a.TravelRequestId == travelReqId).ToList();
            var listofHotelType = _context.BulkBookingHODFinancialApprovalModel.Where(x => x.TravelRequestId == travelReqId).Select(a => a.HotelRequestId).Distinct().ToList();
            var clubIdList = _context.BulkTravelRequestHotelDetailModel.Where(a => a.TravelRequestId == travelReqId && a.HotelType != "Contractual" && a.HotelStatus != "Approved by Hotel" && a.HotelStatus != "Rejected by Hotel").Select(a => a.clubId).Distinct().ToList();

            dicInfo.Add("hotelinfobyTrid", hotelData);
            dicInfo.Add("listForFinancialApproval", dataForFinanacialApproval);
            dicInfo.Add("inclist", inclist);
            dicInfo.Add("ListOfHotelReferenceId", listofHotelType);
            dicInfo.Add("ListOfclubId", clubIdList);
            return dicInfo;
        }

        /// <summary>
        /// Update Hotel Status
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public int UpdateHotelStatus(Int64 TravelRequestId, string clubId)
        {
            var s = 0;
            var hotelData = new List<BulkTravelRequestHotelDetailModels>();
            var ClubId = Convert.ToInt16(clubId);
            hotelData = _context.BulkTravelRequestHotelDetailModel.Where(a => a.TravelRequestId == TravelRequestId && a.clubId == ClubId).ToList();
            if (hotelData.Count > 0)
            {
                foreach (var hd in hotelData)
                {
                    hd.HotelStatus = "Pending from Hotel";
                }
                s = _context.SaveChanges();
            }
            return s;
        }

        /// <summary>
        /// Reject Non-Contractual Hotel Request by hod
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="approverEmpcCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int BulkRejectNonContractualHotelRequest(string travelReqID, string hid, int clubid, string approverEmpcCode, string type)
        {
            var s = 0;
            var IdList = travelReqID.Split(',');
            var hidList = hid.Split(',');
            for (var i = 0; i < IdList.Length - 1; i++)
            {
                var trid = Convert.ToInt64(IdList[i]);
                var hID = Convert.ToInt32(hidList[i]);
                var findIfRejected = _context.BulkTravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.clubId == clubid).ToList();
                foreach (var f in findIfRejected)
                {
                    if (f.HodApprovalStatus == "2")
                    {
                        return s = -1;
                    }
                }

                var findIfApproved = _context.BulkBookingHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hID).ToList();
                foreach (var f in findIfApproved)
                {
                    if (f.ApproverEmpCodeLevel1 == approverEmpcCode && f.ApprovalStatusLevel1 == 1)
                    {
                        return s;
                    }
                    else if (f.ApproverEmpCodeLevel2 == approverEmpcCode && f.ApprovalStatusLevel2 == 1)
                    {
                        return s;
                    }

                }
                var updateItem = _context.BulkTravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.clubId == clubid).ToList();
                foreach (var p in updateItem)
                {
                    p.HodApprovalStatus = "2";
                }

                var updateItem2 = _context.BulkBookingHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel1 == approverEmpcCode && o.HotelRequestId == hID).ToList();
                foreach (var p in updateItem2)
                {
                    p.ApprovalDateLevel1 = DateTime.Now;
                    p.ApprovalStatusLevel1 = 2;
                }

                var updateItem3 = _context.BulkBookingHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel2 == approverEmpcCode && o.HotelRequestId == hID).ToList();
                foreach (var p in updateItem3)
                {
                    p.ApprovalDateLevel2 = DateTime.Now;
                    p.ApprovalStatusLevel2 = 2;
                }

                var updateItem4 = _context.BulkBookingHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hID).ToList();
                foreach (var p in updateItem4)
                {
                    if (p.ApprovalStatusLevel1 == 2 || p.ApprovalStatusLevel2 == 2)
                    {
                        p.ApprovalStatus = 2;
                    }
                }
            }
            s = _context.SaveChanges();
            return s;
        }

        /// <summary>
        /// find hotel inclusions of non contractual hotel
        /// </summary>
        /// <param name="travelreqid"></param>
        /// <returns></returns>
        public List<BulkHotelInclusionNonContractualMasterModels> FindNonContractualHotelInclusions(Int64 travelreqid, int HotelRequestId)
        {
            var list = new List<BulkHotelInclusionNonContractualMasterModels>();
            list = _context.BulkHotelInclusionNonContractualMasterModel.Where(o => o.TravelRequestId == travelreqid && o.HotelRequestId == HotelRequestId).ToList();
            return list;
        }


        /// <summary>
        /// view detail for hod approval status
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        public List<BulkBookingHODFinancialApprovalModels> GetdetailHODApproval(Int64 trid, int breqId, string sodoat)
        {
            var list = new List<BulkBookingHODFinancialApprovalModels>();
            if (sodoat == "SOD")
            {
                list = _context.BulkBookingHODFinancialApprovalModel.Where(a => a.TravelRequestId == trid && a.BreqId == breqId).ToList();
            }
            return list;
        }

        /// <summary>
        /// Find Spoc Details
        /// </summary>
        /// <param name="travelrequestid"></param>
        /// <returns></returns>
        public List<BulkUploadMasterModels> FindSpocDetails(string travelrequestid)
        {
            var spocDetails = new List<BulkUploadMasterModels>();
            var trid = Int64.Parse(travelrequestid);
            spocDetails = _context.BulkUploadMasterModel.Where(a => a.TRId == trid).ToList();
            return spocDetails;
        }

        /// <summary>
        /// Get HOD Details
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public string getHodEmployee(int deptId, string verticalCode)
        {
            var lst = new List<SodApproverModels>();
            lst = _context.SodApprovers.Where(a => a.DepartmentId == deptId && a.IsActive == 1 && a.VerticleId == verticalCode).ToList();

            return lst[0].EmailId; 

            return lst[0].EmailId;

        }


        /// <summary>
        /// Save SMS Log after the HOD approval
        /// </summary>
        /// <param name="smsLogModel"></param>
        /// <returns></returns>
        public int SaveApproverSMSLog(SodApproverSMSLogModels smsLogModel)
        {
            try
            {
                _context.SodApproverSMSLogModel.Add(smsLogModel);
                return _context.SaveChanges();
            }
            catch(Exception ex)
            {

            }
            return 0;
        }

        /// <summary>
        /// Dispose Objet
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
