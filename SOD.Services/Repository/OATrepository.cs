using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;
using SOD.Services.Interface;
using SOD.Services.EntityFramework;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Drawing;


namespace SOD.Services.Repository
{
    public class OATrepository : IOATrepository
    {
        #region "Constructor Initialization"
        /// <summary>
        /// Constructor Initilization
        /// </summary>
        private readonly SodEntities _context;
        public OATrepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
        #endregion

        public OATTravelRequestMasterModal GetUserDetail(Int64 oatRequestId)
        {
            var detail = _context.OATTravelRequestMasterModals.Where(y => y.OATRequestID == oatRequestId).FirstOrDefault();
            return detail;
        }

        public Int64 submitDataForsendingToIth(List<OATTravelRequestMasterModal> personalInfo, List<OATTravelRequestPassengerDetailModal> passangerInfo, List<OATTravelRequestFlightDetailModal> flightInfo)
        {
            Int64 OATRequestMaxId = 0;
            var id = new OATTravelRequestMasterModal();
            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    //Master List
                    _context.OATTravelRequestMasterModals.Add(personalInfo[0]);
                    _context.SaveChanges();

                    OATRequestMaxId = _context.OATTravelRequestMasterModals.DefaultIfEmpty().Max(x => x == null ? 1 : x.OATRequestID);
                    //Update Code for OAT Travel Request Info
                    var mlists = _context.OATTravelRequestMasterModals.Where(o => o.OATRequestID == OATRequestMaxId);
                    foreach (var list in mlists)
                    {
                        list.OATRequestCode = "OAT-" + OATRequestMaxId;
                    }
                    //For OAT passanger Details
                    foreach (var list in passangerInfo)
                    {
                        if (list.EmployeeCode.Length == 6)
                        {
                            list.EmployeeCode = "00" + list.EmployeeCode;
                        }
                        list.OATTravelRequestId = OATRequestMaxId;
                    }
                    _context.OATTravelRequestPassengerDetailModal.AddRange(passangerInfo);
                    _context.SaveChanges();
                    if (flightInfo != null)
                    {
                        var plist = _context.OATTravelRequestPassengerDetailModal.Where(o => o.OATTravelRequestId == OATRequestMaxId).ToList();
                        var k = 0;
                        for (var i = 0; i < flightInfo.Count; i++)
                        {
                            if (flightInfo[i].MulticityLength > 0)
                            {
                                for (var j = 0; j <= flightInfo[i].MulticityLength; j++)
                                {
                                    flightInfo[i + j].PassengerID = plist[k].PassengerID;
                                    flightInfo[i + j].OATTravelRequestId = OATRequestMaxId;
                                }
                                i = i + flightInfo[i].MulticityLength;
                            }
                            else if (flightInfo[0].FlightType == "Round Trip")
                            {
                                for (var j = 0; j <= 1; j++)
                                {
                                    flightInfo[i + j].PassengerID = plist[k].PassengerID;
                                    flightInfo[i + j].OATTravelRequestId = OATRequestMaxId;
                                    // flightInfo[i + j].DepartureDate = ;
                                }
                                i = i + 1;
                            }
                            else
                            {
                                flightInfo[i].PassengerID = plist[k].PassengerID;
                                flightInfo[i].OATTravelRequestId = OATRequestMaxId;
                            }
                            k++;
                        }
                        _context.OATTravelRequestFlightDetailModal.AddRange(flightInfo);
                        _context.SaveChanges();
                    }
                    dbTran.Commit();
                }
                catch (Exception ex)
                {
                    var Exception = ex.InnerException.InnerException.Message.ToString();
                    //Rollback transaction if exception occurs
                    dbTran.Rollback();
                    OATRequestMaxId = -1;
                    throw ex;
                }
            }
            return OATRequestMaxId;
        }

        public Int64 submitDataForOnlyFlight(List<OATTravelRequestFlightDetailModal> flightInfo)
        {
            var s = 0;
            try
            {
                _context.OATTravelRequestFlightDetailModal.AddRange(flightInfo);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                var Exception = ex.InnerException.InnerException.Message.ToString();
                s = -1;
            }
            return s;
        }
        public string CheckOnlyflightBooking(List<OATTravelRequestFlightDetailModal> flightInfo)
        {
            string check = "";
            foreach (var i in flightInfo)
            {
                var list = _context.OATTravelRequestFlightDetailModal.Where(a => a.OriginPlace == i.OriginPlace && a.DestinationPlace == i.DestinationPlace && a.DepartureDate == i.DepartureDate).ToList();
                if (list.Count > 0)
                {
                    Int64 PassengerId;
                    foreach (var j in list)
                    {
                        if (i.Empcode.Length == 6)
                        {
                            i.Empcode = "00" + i.Empcode;
                        }
                        PassengerId = j.PassengerID;
                        var lst = _context.OATTravelRequestPassengerDetailModal.Where(a => a.PassengerID == PassengerId && a.EmployeeCode == i.Empcode).ToList();
                        if (lst.Count > 0)
                        {
                            check = i.Empcode;
                            break;
                        }
                    }
                }
                else
                {
                    check = "";
                }
            }
            return check;
        }

        public List<ITACodeMasterModel> fetchIOTAList(string LikeITACode, string unlikeIotaCode,string IsDomestic)
        {
            var list = new List<ITACodeMasterModel>();
            if (IsDomestic == "true")
            {
                list = _context.ITACodeMasterModels.Where(p => (p.SectorCode.Contains(LikeITACode) || p.SectorName.Contains(LikeITACode))&& p.CountryCode == "IN").ToList();
                if (unlikeIotaCode != "")
                    list = list.Where(p => !p.SectorCode.Contains(unlikeIotaCode)  && p.CountryCode == "IN").ToList();
            }
            else
            {
               list = _context.ITACodeMasterModels.Where(p => (p.SectorCode.Contains(LikeITACode) || p.SectorName.Contains(LikeITACode)) && p.CountryCode != "IN").ToList();
                if (unlikeIotaCode != "")
                    list = list.Where(p => !p.SectorCode.Contains(unlikeIotaCode) && p.CountryCode != "IN").ToList();
            }
             
            
            return list;
        }

        public string getBookingType(Int64 oatReqId)
        {
            string bookingType = _context.OATTravelRequestMasterModals.Where(p => p.OATRequestID == oatReqId).Select(y => y.BookingFor).FirstOrDefault();
            return bookingType;
        }

        public string getClevelApproverMail(Int64 oatReqId)
        {
            var deptId = _context.OATTravelRequestMasterModals.Where(p => p.OATRequestID == oatReqId).Select(y => y.RequestedEmpDept).FirstOrDefault();
            int deptID = _context.EmdmAPIModel.Where(p => p.DepartmentName == deptId).Select(y => y.DepartmentID).FirstOrDefault();
            //var cxoDetail = _context.SodCXODeptMappingModel.Join(_context.SodCXOLevelModel, r => r.AllocatedDeptId, p => p.DepartmentId, (r, p) => new { p, r }).Where(y => y.r.AllocatedDeptId == deptID && y.r.CXOName == y.p.CXOName).Select(y => new { y.r.CXOName, y.p.EmailId }).FirstOrDefault();
            var CXODetails = _context.SodCXOLevelModel.Where(y => y.DepartmentId == deptID).FirstOrDefault();
            //var CXODetails = cxoDetail.FirstOrDefault();
            return CXODetails.EmailId + "|" + CXODetails.CXOName;
            //return "";
        }

        public bool getAmountForFinancialApproval(Int64 oatReqId)
        {
            var amountlst = _context.ITHTransactionDetailModal.Where(p => p.OATRequestID == oatReqId && p.OATDeskApproval ==1).ToList();
            bool IsAmountGreater = (amountlst.Where(x => x.Amount >= 8000).Count() == 0) ? false : true;
           return IsAmountGreater;
        }

        public int saveFinacialDetailForApproval(List<OATFinancialApprovalDetail_RoisteringModal> ListOfFinancialApproval)
        {
            var s = 0;
            var FinancialApproval = new List<OATFinancialApprovalDetail_RoisteringModal>();
            try
            {
                FinancialApproval = _context.OATFinancialApprovalDetail_RoisteringModal.AddRange(ListOfFinancialApproval).ToList();
                s = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                s = -1;
            }
            return s;
        }

        public OATFinancialApprovalDetail_RoisteringModal GetFinacialDetailForApproval(Int64 oatReqID)
        {

            var FinancialApproval = _context.OATFinancialApprovalDetail_RoisteringModal.Where(x => x.OATId == oatReqID).FirstOrDefault();
            //s = _context.SaveChanges();

            return FinancialApproval;
        }

        public List<OATFinancialApprovalMaster_RoisteringModal> getFinacialMasterDetail(Int64 oatReqId)
        {
            var FinancialApproval = new List<OATFinancialApprovalMaster_RoisteringModal>();
            var RoisteringType = _context.OATTravelRequestMasterModals.Where(p => p.OATRequestID == oatReqId).Select(x => x.BookingForOnBehalfof).FirstOrDefault();
            FinancialApproval = _context.OATFinancialApprovalMaster_RoisteringModal.Where(p => p.ApprovalFor == RoisteringType).ToList();
            return FinancialApproval;
        }

        public List<OATPAXTyepeMasterModal> GetPaxType()
        {
            var list = _context.OATPAXTyepeMasterModal.ToList();
            return list;
        }
        public List<OATBookingRightModal> getOATBookingRight(String EmpCode)
        {
            var list = _context.OATBookingRightModal.Where(y=>y.EmpCode == EmpCode).ToList();
            return list;
        }

       
        /// <summary>
        /// getOAt booking list as per User
        /// </summary>
        /// <param name="empCode"></param>
        /// <returns></returns>
        public List<OATTravelRequestMasterModal> getOatMasterlist(String empCode)
        {
            var oatMasterList = new List<OATTravelRequestMasterModal>();
            oatMasterList = _context.OATTravelRequestMasterModals.OrderByDescending(y => y.OATRequestID).Where(a => a.RequestedEmpCode.Trim() == empCode.Trim()).ToList();
            return oatMasterList;
        }

        /// <summary>
        /// get deatils of booking as per oat requestID 
        /// </summary>
        /// <param name="oatReqId"></param>
        /// <param name="trreqId"></param>
        /// <param name="bookingfor"></param>
        /// <returns></returns>
        public List<viewOatDetailsModal> getselecteddetails(Int64 oatReqId, Int64 trreqId, String bookingfor)
        {
            var getSelectedList = new List<viewOatDetailsModal>();

            var s = ADO.SodCommonServices.GetOATBookingList(oatReqId, 1);
            return s;
        }

        public List<viewOatHotelDetailsModal> getViewOatHotelDetail(String trId)
        {
            var hotelDetail = new List<viewOatHotelDetailsModal>();
            long reqId = Convert.ToInt64(trId);
            var query = from bd in _context.BulkUploadModel
                        join bt in _context.BulkTravelRequestHotelDetailModel on bd.TrnId equals bt.TravelRequestId into details
                        from p in details.DefaultIfEmpty()
                        where bd.BReqId == reqId
                        select new viewOatHotelDetailsModal
                        {
                            TravelRequestId = bd.TrnId,
                            EmpCode = bd.EmpCode,
                            City = bd.HotelCity,
                            Sector = bd.Sector,
                            CheckInDate = bd.CheckInDate,
                            CheckOutDate = bd.CheckOutDate,
                            ReasonForCancellation = bd.ReasonForCancellation,
                            IsBookingcancelled = bd.IsBookingcancelled,
                            HotelStatus = p.HotelStatus,
                            HotelConfirmationNo = p.HotelConfirmationNo,
                            HotelName = p.HotelName,
                            HotelAddress = p.HotelAddress,
                            HotelPhoneNo = p.HotelPhoneNo,
                            Remarks_Status = p.Remarks_Status
                        };
            hotelDetail = query.ToList();
            return hotelDetail;
        }

        public List<OATTravelRequestFlightDetailModal> getViewOatFlightDetail(Int64 oatReqID, Int64 ID)
        {
            var list = _context.OATTravelRequestFlightDetailModal.Where(x => x.OATTravelRequestId == oatReqID && x.ID == ID).ToList();
            list[0].IthDetail = _context.ITHTransactionMasterModal.Where(x => x.OATRequestID == oatReqID).FirstOrDefault();
            foreach (var x in list)
            {
                x.cancellationSrc = x.CancellationAttachment != null ? Convert.ToBase64String(x.CancellationAttachment) : "";
                if (x.CancellationAttachment != null) x.CancellationAttachment = null;
                var PassengerDetail = _context.OATTravelRequestPassengerDetailModal.Where(y => y.PassengerID == x.PassengerID).ToList();
                x.Empcode = PassengerDetail[0].EmployeeCode;
                x.EmpName = PassengerDetail[0].Gender + " " + PassengerDetail[0].FirstName + " " + PassengerDetail[0].LastName;
                var itenaryModal = _context.OATUploadItenaryModal.Where(y => y.OATRequestId == x.OATTravelRequestId && y.PassengerID == x.PassengerID && y.OriginPlace == x.OriginPlace && y.DestinationPlace == x.DestinationPlace).FirstOrDefault();
                if (itenaryModal != null)
                {
                    var dfh = System.Web.HttpUtility.UrlEncode(itenaryModal.FileName);
                    var sdh = System.Web.HttpUtility.UrlDecode(dfh);
                    string sd = "../OatUploadAttachments/BookedItenary/" + sdh;
                    x.filePath = sd;
                    x.PNR = itenaryModal.PNR;
                };
                String bookingType = getBookingType(x.OATTravelRequestId);
                if (bookingType == "SPOC" || bookingType == "Roistering(Flight ops / In Flight)")
                {
                    var ithDetail = _context.ITHTransactionDetailModal.Where(m => m.OATRequestID == x.OATTravelRequestId && m.PassengerID == x.PassengerID && m.OriginPlace == x.OriginPlace && m.DestinationPlace == x.DestinationPlace && m.UserApproval == 1).FirstOrDefault();
                    if (ithDetail != null) ithDetail.ITHUploadRefFile = null;
                    x.Amount = ithDetail == null ? 0 : ithDetail.Amount;
                    x.AirCraftName = ithDetail == null ? "" : ithDetail.AirCraftName;
                    x.FlightNumber = ithDetail == null ? "" : ithDetail.FlightNumber;
                    x.DepartureTime = ithDetail == null ? "" : ithDetail.DepartureTime;
                }
                else
                {
                    var ithDetail = _context.ITHTransactionDetailModal.Where(m => m.OATRequestID == x.OATTravelRequestId && m.OriginPlace == x.OriginPlace && m.DestinationPlace == x.DestinationPlace && m.HodApproval == 1).FirstOrDefault();

                    if (ithDetail != null) ithDetail.ITHUploadRefFile = null;
                    x.Amount = ithDetail == null ? 0 : ithDetail.Amount;

                    x.AirCraftName = ithDetail == null ? "" : ithDetail.AirCraftName;
                    x.FlightNumber = ithDetail == null ? "" : ithDetail.FlightNumber;
                    x.DepartureTime = ithDetail == null ? "" : ithDetail.DepartureTime;
                }
            }
            return list;
        }

        /// <summary>
        /// get all list for oat traveldesk
        /// </summary>
        /// <returns></returns>
        public List<OATTravelRequestMasterModal> getOatMasterlist()
        {
            var oatMasterLists = new List<OATTravelRequestMasterModal>();
            try
            {
                var oatMasterList = _context.OATTravelRequestMasterModals.Where(a => _context.OATTravelRequestFlightDetailModal.Any(b => a.OATRequestID == b.OATTravelRequestId)).OrderByDescending(x => x.RequestDate).ToList();
                var FlightDetail = _context.OATTravelRequestFlightDetailModal.GroupBy(x => x.OATTravelRequestId).Select(group => group.FirstOrDefault()).ToList();
                var query = from r in oatMasterList
                            join Fd in FlightDetail on r.OATRequestID equals Fd.OATTravelRequestId into FlightDetails
                            from F in FlightDetails.DefaultIfEmpty()
                            join bt in _context.ITHTransactionMasterModal on r.OATRequestID equals bt.OATRequestID into details
                            from p in details.DefaultIfEmpty()
                            join Fin in _context.OATFinancialApprovalDetail_RoisteringModal on r.OATRequestID equals Fin.OATId into FinDetails
                            from FinApp in FinDetails.DefaultIfEmpty()
                            select new OATTravelRequestMasterModal
                            {
                                OATRequestID = r.OATRequestID,
                                TravelRequestId = r.TravelRequestId,
                                OATRequestCode = r.OATRequestCode,
                                RequestDate = r.RequestDate,
                                ReasonForTravel = r.ReasonForTravel,
                                Gender = r.Gender,
                                RequestedEmpId = r.RequestedEmpId,
                                RequestedEmpCode = r.RequestedEmpCode,
                                RequestedEmpName = r.RequestedEmpName,
                                RequestedEmpDesignation = r.RequestedEmpDesignation,
                                RequestedEmpDept = r.RequestedEmpDept,
                                EmailId = r.EmailId,
                                PhoneNo = r.PhoneNo,
                                PaxNo = r.PaxNo,
                                BookingFor = r.BookingFor,
                                BookingForOnBehalfof = r.BookingForOnBehalfof,
                                BookingType = r.BookingType,
                                Base = r.Base,
                                BookingStatus = r.BookingStatus,
                                ApproverEmailID = r.ApproverEmailID,
                                ApproverDate = r.ApproverDate,
                                IsSendRequestToITh = r.IsSendRequestToITh,
                                OATDeskApproval = p != null ? p.OATDeskApproval : 0,
                                IsITHSentResponse = p != null ? p.IsITHSentResponse : 0,
                                HodApproval = p != null ? p.HodApproval : 0,
                                UserApproval = p != null ? p.UserApproval : 0,
                                Sector = F.OriginPlace + "-" + F.DestinationPlace,
                                TravelDate = F.DepartureDate,
                                CancellationStatus = F.IsFlightCancel,
                                cancelBy = F.FlightCancelBy,
                                ApprovalStatus = FinApp != null ? FinApp.ApprovalStatus : -1,
                                Partailcancellation = _context.OATTravelRequestFlightDetailModal.Where(z => z.OATTravelRequestId == r.OATRequestID && z.IsFlightCancel == true).Count() == _context.OATTravelRequestFlightDetailModal.Where(z => z.OATTravelRequestId == r.OATRequestID ).Count() ? "Cancelled" :  _context.OATTravelRequestFlightDetailModal.Where(z => z.OATTravelRequestId == r.OATRequestID && z.IsFlightCancel == false).Count() == _context.OATTravelRequestFlightDetailModal.Where(z => z.OATTravelRequestId == r.OATRequestID).Count() ? "":"Partial Cancelled"
            };
                oatMasterLists = query.ToList();
                //var jfkj = _context.OATTravelRequestFlightDetailModal.Where(z => z.OATTravelRequestId == 4 && z.IsFlightCancel == true).Count();// == null ? "cancelled" : //; _context.OATTravelRequestFlightDetailModal.Where(z => z.OATTravelRequestId == r.OATRequestID && z.IsFlightCancel == false).ToList() == null ? "":"partialCancelled"
            }
            catch (Exception ex)
            {
                return oatMasterLists;
            }
            return oatMasterLists;
        }

        public int UpdateTrnIdOnOatMaster(long trnID, long oatReqNo)
        {
            var s = 0;
            var list = _context.OATTravelRequestMasterModals.Where(x => x.OATRequestID == oatReqNo).ToList();
            list[0].TravelRequestId = trnID;
            s = _context.SaveChanges();
            return s;
        }

        public int updateFlightDetail(OATTravelRequestFlightDetailModal Detail)
        {
            var s = 0;
            var list = _context.OATTravelRequestFlightDetailModal.Where(x => x.ID == Detail.ID).FirstOrDefault();
            list.DepartureDate = Detail.DepartureDate;
            s = _context.SaveChanges();
            return s;
        }

        public int UpdatePassengerModal(long OatReqID, long PassID, int criteria)
        {
            var s = 0;
            var list = _context.OATTravelRequestPassengerDetailModal.Where(x => x.OATTravelRequestId == OatReqID && x.PassengerID == PassID).ToList();
            if (criteria == 1)
            {
                list[0].IsFlightRequired = true;
            }
            else
            {
                list[0].IsHotelRequired = true;
            }
            s = _context.SaveChanges();
            return s;
        }

        public List<OATTravelRequestPassengerDetailModal> getPassengerDetail(long oatReqID, long PassengerID)
        {
            var list = _context.OATTravelRequestPassengerDetailModal.Where(x => x.OATTravelRequestId == oatReqID && x.PassengerID == PassengerID).ToList();
            return list;
        }


        public List<String> getIthListName()
        {
            var IthNameList = new List<String>();
            IthNameList = _context.ITHVenderModal.Select(a => a.ITHName).ToList();
            return IthNameList;
        }

        public List<ITHVenderModal> getIthDetailPerName(string ithName)
        {
            var ithDetailsList = _context.ITHVenderModal.Where(a => a.ITHName == ithName).ToList();
            return ithDetailsList;
        }

        public Int64 saveIthReqDetail(ITHTransactionMasterModal modal, List<ITHTransactionDetailModal> transactionDetailList)
        {
            Int32 s = 0;
            Int64 ithReqId = 0;
            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var ISlistPresent = _context.ITHTransactionMasterModal.Where(x => x.OATRequestID == modal.OATRequestID).FirstOrDefault();
                    //Master List
                    if (ISlistPresent == null)
                    {
                        _context.ITHTransactionMasterModal.Add(modal);
                        _context.SaveChanges();
                        ithReqId = _context.ITHTransactionMasterModal.DefaultIfEmpty().Max(x => x == null ? 1 : x.TrnID);
                        //Update Code for ith transaction detail     
                        foreach (var lst in transactionDetailList)
                        {
                            lst.TrnId = ithReqId;
                        }
                        _context.ITHTransactionDetailModal.AddRange(transactionDetailList);
                        var oatMasterList = _context.OATTravelRequestMasterModals.Where(x => x.OATRequestID == modal.OATRequestID).FirstOrDefault();
                        oatMasterList.IsSendRequestToITh = true;
                    }
                    else
                    {
                        ithReqId = ISlistPresent.TrnID;
                        ISlistPresent.ITHVendorCode = modal.ITHVendorCode;
                        ISlistPresent.RequestedBy = modal.RequestedBy;
                        ISlistPresent.RequestedDate = modal.RequestedDate;
                        ISlistPresent.IsITHSentResponse = 0;
                    }
                    s = _context.SaveChanges();
                    dbTran.Commit();
                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs

                    dbTran.Rollback();
                    s = -1;
                    var Exception = ex.InnerException.InnerException.Message.ToString();
                    throw ex;
                }
            }
            return ithReqId;

        }
        public Dictionary<string, object> getOatDetailsPerReqId(long oatReqID)
        {
            var detailList = new Dictionary<string, object>();
            var oatMasterList = _context.OATTravelRequestMasterModals.Where(a => a.OATRequestID == oatReqID).ToList();
            var oatFlightList = _context.OATTravelRequestFlightDetailModal.Where(a => a.OATTravelRequestId == oatReqID).ToList();
            var oatPassangerList = _context.OATTravelRequestPassengerDetailModal.Where(a => a.OATTravelRequestId == oatReqID).ToList();
            detailList.Add("oatMasterList", oatMasterList);
            detailList.Add("oatFlightList", oatFlightList);
            detailList.Add("oatPassangerList", oatPassangerList);
            return detailList;
        }
        public Dictionary<string, object> getIthDetailAsPerReqID(long OatReqID)
        {
            var lst = new Dictionary<String, object>();
            var FlightDetail = new List<OATTravelRequestFlightDetailModal>();
            var IthDetails = new List<ITHTransactionDetailModal>();
            //var IthDetailLog = new List<ITHTransactionDetailLogModal>();
            var IthVenderDetail = new List<ITHVenderModal>();
            var RequestedEmployee = new List<OATTravelRequestMasterModal>();
            Int64 TransactionId = 0;
            var count = 0;
            RequestedEmployee = _context.OATTravelRequestMasterModals.Where(x => x.OATRequestID == OatReqID).ToList();
            FlightDetail = _context.OATTravelRequestFlightDetailModal.Where(x => x.OATTravelRequestId == OatReqID).ToList();
            var PassengerDetails = _context.OATTravelRequestPassengerDetailModal.Where(y => y.OATTravelRequestId == OatReqID).ToList();
            foreach (var x in FlightDetail)
            {
                if (x.IsFlightCancel) count++;
                //x.cancellationSrc = x.CancellationAttachment != null ? Convert.ToBase64String(x.CancellationAttachment) : "";
                x.CancellationAttachment = null;
                var PassengerDetail = _context.OATTravelRequestPassengerDetailModal.Where(y => y.PassengerID == x.PassengerID).ToList();
                x.Empcode = PassengerDetail[0].EmployeeCode;
                x.EmpName = PassengerDetail[0].Gender + " " + PassengerDetail[0].FirstName + " " + PassengerDetail[0].LastName;
                var itenaryModal = _context.OATUploadItenaryModal.Where(y => y.OATRequestId == x.OATTravelRequestId && y.PassengerID == x.PassengerID && y.OriginPlace == x.OriginPlace && y.DestinationPlace == x.DestinationPlace).FirstOrDefault();
                if (itenaryModal != null)
                {
                    var dfh = System.Web.HttpUtility.UrlEncode(itenaryModal.FileName);
                    var sdh = System.Web.HttpUtility.UrlDecode(dfh);
                    string sd = "../OatUploadAttachments/BookedItenary/" + sdh;
                    x.filePath = sd;
                };

            }
            List<ITHTransactionMasterModal> ithMasterList = _context.ITHTransactionMasterModal.Where(x => x.OATRequestID == OatReqID).ToList();
            TransactionId = ithMasterList[0].TrnID;
            var venderCode = ithMasterList[0].ITHVendorCode;
            ithMasterList[0].IsFlightOnHold = FlightDetail[0].IsFlightOnHold;
            ithMasterList[0].BookingFor = RequestedEmployee[0].BookingFor;//getBookingType(OatReqID);
            ithMasterList[0].BookingStatus = RequestedEmployee[0].BookingStatus;//_context.OATTravelRequestMasterModals.Where(x => x.OATRequestID == OatReqID).Select(y => y.BookingStatus).FirstOrDefault();
            var cancellationFlightAccount = _context.OATTravelRequestFlightDetailModal.Where(y => y.IsFlightCancel == true && y.OATTravelRequestId == OatReqID).Count();
            ithMasterList[0].CancellationStatus = FlightDetail.Count() == _context.OATTravelRequestFlightDetailModal.Where(y=> y.IsFlightCancel == true && y.OATTravelRequestId == OatReqID).Count()? "FC":"PC";
            if (FlightDetail.Count == count)
            {
                if (RequestedEmployee[0].BookingStatus != 1)
                    ithMasterList[0].CancellationType = FlightDetail.Where(x => x.CancelType == "c").Select(x => x.CancelType).FirstOrDefault();
                else
                    ithMasterList[0].CancellationType = FlightDetail.Where(x => x.CancelType == "frc").Select(x => x.CancelType).FirstOrDefault();
            }
            //= Convert.ToInt16();
            IthVenderDetail = _context.ITHVenderModal.Where(x => x.ITHCode == venderCode).ToList();
            if (IthVenderDetail.Count != 0)
            {
                IthVenderDetail[0].submittedBy = ithMasterList[0].RequestedBy;
                if (ithMasterList[0].IsITHSentResponse == 2) IthVenderDetail[0].ithResponsestatus = "Rejected";
                else if (ithMasterList[0].IsITHSentResponse == 1) IthVenderDetail[0].ithResponsestatus = "Approved";
                else IthVenderDetail[0].ithResponsestatus = "Pending";
            }
            var IthTransactionLog = _context.ITHTransactionRejectionModal.Where(x => x.OATRequestID == OatReqID && x.TrnID == TransactionId).ToList();
            if (IthTransactionLog.Count != 0)
            {
                foreach (var logList in IthTransactionLog)
                {
                    var IthVenderDetaillogList = _context.ITHVenderModal.Where(x => x.ITHCode == logList.ITHVendorCode).FirstOrDefault();
                    IthVenderDetaillogList.submittedBy = logList.RequestedBy;
                    IthVenderDetaillogList.ithResponsestatus = logList.Remarks;
                    IthVenderDetail.Add(IthVenderDetaillogList);
                }
            }



            if (ithMasterList[0].IsReqSentToIth == true)
            {
                IthDetails = _context.ITHTransactionDetailModal.Where(x => x.TrnId == TransactionId && x.OATRequestID == OatReqID).ToList();
                foreach (var i in IthDetails)
                {

                    i.ITHUploadRefFile = null;
                    i.AttachedFileToRevert = null;
                }
            }
            //IthDetailLog = _context.ITHTransactionDetailLogModal.Where(x => x.TrnId == TransactionId).ToList();

            var IthDetailLog = _context.ITHTransactionDetailLogModal.Where(x => x.TrnId == TransactionId).Select(n => new
            {
                n.AirCraftName,
                n.OriginPlace,
                n.DestinationPlace,
                n.Amount,
                n.FlightNumber,
                n.TrnId,
                n.OATRequestID,
                n.ID,
                n.DepartureDate,
                n.PassengerID,
                n.ArrivalDate,
                n.ArrivalTime,
                n.DepartureTime,
                n.FlightType,
                n.OATDeskRemarks
            }).ToList();



            var transactionDetail = new List<ITHResponseDataViewModal>();
            var uniqueSectorFlight = FlightDetail.GroupBy(d => new { d.OriginPlace, d.DestinationPlace, d.DepartureDate }).Select(group => group.First()).ToList();
            foreach (var i in uniqueSectorFlight)
            {
                var modal = new ITHResponseDataViewModal();
                modal.OriginPlace = i.OriginPlace;
                modal.DestinationPlace = i.DestinationPlace;
                modal.DepartureDate = i.DepartureDate;
                modal.IsFlightOnHold = i.IsFlightOnHold;
                modal.FlightOnHoldBy = i.FlightOnHoldBy;
                modal.filePath = i.filePath;
                if (FlightDetail.Count == count) modal.IsFlightCancel = FlightDetail[0].IsFlightCancel;
                var isApproved = _context.ITHTransactionDetailModal.Where(x => x.OriginPlace == i.OriginPlace && x.DestinationPlace == i.DestinationPlace && x.OATRequestID == i.OATTravelRequestId && (x.OATDeskApproval == 2 || x.OATDeskApproval == 1)).Select(m => m.OATDeskApproval).FirstOrDefault();
                if (isApproved == 2 || isApproved == 1) i.oatDeskApproval = isApproved;
                modal.oatDeskApproval = isApproved;
                modal.newIthDetail = _context.ITHTransactionDetailModal.Where(x => x.OriginPlace == i.OriginPlace && x.DestinationPlace == i.DestinationPlace && x.OATRequestID == i.OATTravelRequestId).ToList();
                foreach(var list in modal.newIthDetail)
                {
                    list.isAlreadyNoShow = _context.OATFlightNoShowReportModal.Where(y => y.OATReqId == list.OATRequestID && y.Sector == list.OriginPlace + "-" + list.DestinationPlace).Count() == 0 ? false: true;
                    
                }                   
              
                transactionDetail.Add(modal);
            }
            //var uniqueSectorFlight = FlightDetail.Select(m => new { m.OriginPlace, m.DestinationPlace, m.DepartureDate }).Distinct().ToList();
            lst.Add("FlightDetail", FlightDetail);
            lst.Add("IthDetails", IthDetails);
            lst.Add("IthDetailLog", IthDetailLog);
            lst.Add("IthVenderDetail", IthVenderDetail);
            lst.Add("PassengerDetails", PassengerDetails);
            lst.Add("RequestedEmployee", RequestedEmployee);
            lst.Add("ithMasterList", ithMasterList);
            lst.Add("uniqueSectorFlight", uniqueSectorFlight);
            lst.Add("transactionDetail", transactionDetail);
            return lst;
        }

        public Dictionary<string, object> GetDetailForIthResponse(Int64 oatReqId)
        {
            var lst = new Dictionary<String, object>();
            var passengerDetail = new List<OATTravelRequestPassengerDetailModal>();
            //var ithDetail = new List<ITHTransactionDetailModal>();
            var flightDetails = new List<OATTravelRequestFlightDetailModal>();
            String flightStatus = String.Empty;

            try
            {             
                flightDetails = _context.OATTravelRequestFlightDetailModal.Where(y => y.OATTravelRequestId == oatReqId && y.IsFlightCancel == false && y.IsFlightOnHold == false).ToList();
                if (flightDetails.Count != 0)
                {
                    foreach (var x in flightDetails)
                    {
                        var PassengerDetail = _context.OATTravelRequestPassengerDetailModal.Where(y => y.PassengerID == x.PassengerID).FirstOrDefault();
                        x.Empcode = PassengerDetail.EmployeeCode;
                        x.EmpName = PassengerDetail.Gender + " " + PassengerDetail.FirstName + " " + PassengerDetail.LastName;
                        x.PhoneNo = PassengerDetail.PhoneNo;
                        x.Emaild = PassengerDetail.EmailId;
                        String bookingType = getBookingType(oatReqId);
                        if (bookingType == "SPOC" || bookingType == "Roistering(Flight ops / In Flight)")
                        {
                            var ithDetail = _context.ITHTransactionDetailModal.Where(m => m.OATRequestID == oatReqId && m.PassengerID == x.PassengerID && m.OriginPlace == x.OriginPlace && m.DestinationPlace == x.DestinationPlace && m.UserApproval == 1).FirstOrDefault();
                            x.Amount = ithDetail.Amount;
                            x.AirCraftName = ithDetail.AirCraftName;
                            x.FlightNumber = ithDetail.FlightNumber;
                            x.DepartureTime = ithDetail.DepartureTime;
                        }
                        else
                        {
                            var ithDetail = _context.ITHTransactionDetailModal.Where(m => m.OATRequestID == oatReqId && m.OriginPlace == x.OriginPlace && m.DestinationPlace == x.DestinationPlace && m.HodApproval == 1).FirstOrDefault();
                            x.Amount = ithDetail.Amount;
                            x.AirCraftName = ithDetail.AirCraftName;
                            x.FlightNumber = ithDetail.FlightNumber;
                            x.DepartureTime = ithDetail.DepartureTime;
                        }
                        var itenaryModal = _context.OATUploadItenaryModal.Where(y => y.OATRequestId == x.OATTravelRequestId && y.PassengerID == x.PassengerID && y.OriginPlace == x.OriginPlace && y.DestinationPlace == x.DestinationPlace).FirstOrDefault();
                        if (itenaryModal != null)
                        {
                            var dfh = System.Web.HttpUtility.UrlEncode(itenaryModal.FileName);
                            var sdh = System.Web.HttpUtility.UrlDecode(dfh);
                            string sd = "../OatUploadAttachments/BookedItenary/" + sdh;
                            x.filePath = sd;
                            x.PNR = itenaryModal.PNR;
                        };
                    }
                }
                else
                {
                    var flightDetail = _context.OATTravelRequestFlightDetailModal.Where(y => y.OATTravelRequestId == oatReqId && y.IsFlightOnHold == false).ToList();
                    if (flightDetail.Count == 0)
                    {
                        flightStatus = "This flight booking request is on HOLD.";
                    }
                    else
                    {
                        flightStatus = "This flight booking request is Cancelled.";
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            lst.Add("SectorDetail", flightDetails);
            lst.Add("flightStatus", flightStatus);
            return lst;
        }

        public Dictionary<string, object> GetDetailForIthResponse(Int64 oatReqId, Int64 ithTransactionID, string type)
        {
            bool allcancel = false;
            var lst = new Dictionary<String, object>();
            var PassengerDetail = new List<OATTravelRequestPassengerDetailModal>();
            //var IthDetails = new List<ITHTransactionDetailModal>();
            var IthDetailLog = new List<ITHTransactionDetailLogModal>();
            PassengerDetail = _context.OATTravelRequestPassengerDetailModal.Where(x => x.OATTravelRequestId == oatReqId).ToList();
            var ithMasterList = _context.ITHTransactionMasterModal.Where(x => x.TrnID == ithTransactionID).FirstOrDefault();

            if (type == "ORa")
            {
                var IthDetails = _context.ITHTransactionDetailModal.Where(x => x.TrnId == ithTransactionID && x.OATRequestID == oatReqId && x.OATDeskApproval == 2).Select(n => new
                {
                    n.AirCraftName,
                    n.OriginPlace,
                    n.DestinationPlace,
                    n.Amount,
                    n.TrnId,
                    n.OATRequestID,
                    n.ID,
                    n.DepartureDate,
                    n.PassengerID,
                    n.ArrivalDate,
                    n.ArrivalTime,
                    n.DepartureTime,
                    n.FlightBookingStatus,
                    n.IsFlightCancel,
                    n.FlightCancelBy,
                    n.FlightNumber,
                    n.FlightOnHoldBy,
                    n.FlightType,
                    n.HodApproval,
                    n.HodApprovalDate,
                    n.OATDeskApproval,
                    n.OATDeskApprovalDate,
                    n.UserApproval,
                    n.IsFlightOnHold,
                    n.UserApprovalDate
                }).ToList();
                var sectorDetail = IthDetails.GroupBy(d => new { d.OriginPlace, d.DestinationPlace, d.DepartureDate }).Select(group => group.Last()).ToList();
                //IthDetails.Select(m => new { m.OriginPlace, m.DestinationPlace, m.DepartureDate }).Distinct().ToList();
                var CancelCount = sectorDetail.Where(y => y.IsFlightCancel != true).ToList();
                if (CancelCount.Count == 0)
                {
                    allcancel = true;
                };
                //.Select(m => new { m.Key.CategoryId, m.Key.CategoryName }) IthDetails.GroupBy(x => x.OriginPlace && x.DestinationPlace).Select(x => x.FirstOrDefault());
                //         .Select(m => new { m.OriginPlace, m.DestinationPlace, m.DepartureDate, }).Distinct().ToList();
                lst.Add("IthDetails", IthDetails);
                lst.Add("SectorDetail", sectorDetail);
                lst.Add("allcancel", allcancel);
            }
            else
            {
                var IthDetails = _context.ITHTransactionDetailModal.Where(x => x.TrnId == ithTransactionID && x.OATRequestID == oatReqId).Select(n => new
                {
                    n.AirCraftName,
                    n.OriginPlace,
                    n.DestinationPlace,
                    n.Amount,
                    n.TrnId,
                    n.OATRequestID,
                    n.ID,
                    n.DepartureDate,
                    n.PassengerID,
                    n.ArrivalDate,
                    n.ArrivalTime,
                    n.DepartureTime,
                    n.FlightBookingStatus,
                    n.IsFlightCancel,
                    n.IsInternational,
                    n.FlightCancelBy,
                    n.FlightNumber,
                    n.FlightOnHoldBy,
                    n.FlightType,
                    n.HodApproval,
                    n.HodApprovalDate,
                    n.OATDeskApproval,
                    n.OATDeskApprovalDate,
                    n.UserApproval,
                    n.IsFlightOnHold,
                    n.UserApprovalDate
                }).ToList();

                var sectorDetail = IthDetails.GroupBy(d => new { d.OriginPlace, d.DestinationPlace, d.DepartureDate }).Select(group => group.First()).ToList();
                var CancelCount = sectorDetail.Where(y => y.IsFlightCancel != true).ToList();
                if (CancelCount.Count == 0)
                {  allcancel = true;
                };
                //.Select(m => new { m.Key.CategoryId, m.Key.CategoryName }) IthDetails.GroupBy(x => x.OriginPlace && x.DestinationPlace).Select(x => x.FirstOrDefault());
                //         .Select(m => new { m.OriginPlace, m.DestinationPlace, m.DepartureDate, }).Distinct().ToList();
                lst.Add("IthDetails", IthDetails);
                lst.Add("SectorDetail", sectorDetail);
                lst.Add("allcancel", allcancel);
            }
            //IthDetailLog = _context.ITHTransactionDetailLogModal.Where(x => x.TrnId == ithTransactionID).ToList();
            
            lst.Add("PassengerDetail", PassengerDetail);
            lst.Add("IthDetailLog", IthDetailLog);
            return lst;
        }

        public int SubmitIthDetailResponse(List<ITHTransactionDetailModal> newIthDetail)
        {
            var dataexists = new List<ITHTransactionDetailModal>();
            var ithTransactionID = newIthDetail[0].TrnId;
            var s = 0;
            Int64 max = 0;
            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var ModalList = new List<ITHTransactionDetailLogModal>();
                    var uniqueSectorlst = newIthDetail.Select(m => new { m.OriginPlace, m.DestinationPlace }).Distinct().ToList(); // m.DepartureDate
                    foreach (var sec in uniqueSectorlst)
                    {
                        var oldDataInITH = new List<ITHTransactionDetailModal>();
                        oldDataInITH = _context.ITHTransactionDetailModal.Where(x => x.TrnId == ithTransactionID && x.OriginPlace == sec.OriginPlace && x.DestinationPlace == sec.DestinationPlace).ToList();
                        if (oldDataInITH.Count > uniqueSectorlst.Count)
                        {
                            max = _context.ITHTransactionDetailLogModal.Where(x => x.TrnId == ithTransactionID).DefaultIfEmpty().Max(x => x == null ? 0 : x.LogBatchID);
                            foreach (var lst in oldDataInITH)
                            {
                                var modal = new ITHTransactionDetailLogModal();
                                modal.TrnId = lst.TrnId;
                                modal.OATRequestID = Convert.ToInt64(newIthDetail[0].OATRequestID);
                                modal.PassengerID = lst.PassengerID;
                                modal.FlightType = lst.FlightType;
                                modal.OriginPlace = lst.OriginPlace;
                                modal.DestinationPlace = lst.DestinationPlace;
                                modal.DepartureDate = lst.DepartureDate;
                                modal.DepartureTime = lst.DepartureTime;
                                modal.ArrivalDate = lst.ArrivalDate;
                                modal.ArrivalTime = lst.ArrivalTime;
                                modal.AirCraftName = lst.AirCraftName;
                                modal.FlightNumber = lst.FlightNumber;
                                modal.Amount = lst.Amount;
                                modal.ITHUploadRefFileType = lst.ITHUploadRefType;
                                modal.IsInternational = lst.IsInternational;
                                modal.ITHUploadRefFile = lst.ITHUploadRefFile;
                                modal.OATDeskApproval = lst.OATDeskApproval;
                                modal.OATDeskApprovalDate = lst.OATDeskApprovalDate;
                                modal.OATDeskRemarks = lst.ReasonForCancellation;
                                modal.EntryDate = DateTime.Now;
                                modal.OATDeskUploadRefFile = lst.AttachedFileToRevert;
                                modal.OAtDeskUploadRefFileType = lst.AttachedFileToRevertType;// == null ? null : lst.AttachedFileToRevert;
                                modal.LogBatchID = max + 1;
                                ModalList.Add(modal);
                            }
                            //add detail in ITH log table
                            _context.ITHTransactionDetailLogModal.AddRange(ModalList);
                        }
                        dataexists.AddRange(oldDataInITH);

                    }
                    //remove previous detail from ith detail table   
                    _context.ITHTransactionDetailModal.RemoveRange(dataexists);
                    // add ith response in ith detail table
                    _context.ITHTransactionDetailModal.AddRange(newIthDetail);

                    //update ithresponse in ithtransactiondetailMater
                    var ithResponse = _context.ITHTransactionMasterModal.Where(y => y.TrnID == ithTransactionID).FirstOrDefault();
                    if (ithResponse != null)
                    {
                        ithResponse.IsITHSentResponse = 1;
                        ithResponse.OATDeskApproval = 0;
                        ithResponse.IthSentResponseDate = DateTime.Now;
                    }
                    s = _context.SaveChanges();
                    dbTran.Commit();
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    s = -1;
                    var Exception = ex.InnerException.InnerException.Message.ToString();
                    throw ex;
                }
            }
            return s;
        }

        public int SubmitItenaryListOfBooking(List<OATUploadItenaryModal> UploadedItenary)
        {
            var s = 0;
            _context.OATUploadItenaryModal.AddRange(UploadedItenary);
            var oatReqId = UploadedItenary[0].OATRequestId;
            //update OatmasterList 
            var list = _context.OATTravelRequestMasterModals.Where(x => x.OATRequestID == oatReqId).ToList();
            list[0].BookingStatus = 1;
            s = _context.SaveChanges();

            

            //s = _context.SaveChanges();
            return s;
        }

        public string viewIthAttachedSrc(long Id, int criteria)
        {
            string base64String = "";
            var type = "";
            byte[] imagedp = null;
            if (criteria == 1)
            {
                var DetailList = _context.ITHTransactionDetailModal.Where(y => y.ID == Id).FirstOrDefault();
                imagedp = DetailList.ITHUploadRefFile;
                type = DetailList.ITHUploadRefType;

            }
            else if (criteria == 2)
            {
                var DetailList = _context.ITHTransactionDetailLogModal.Where(y => y.ID == Id).FirstOrDefault();
                imagedp = DetailList.ITHUploadRefFile;
                type = DetailList.ITHUploadRefFileType;
            }
            else if (criteria == 3)
            {
                var DetailList = _context.OATTravelRequestFlightDetailModal.Where(y => y.ID == Id).FirstOrDefault();
                imagedp = DetailList.CancellationAttachment;
                type = DetailList.CancellationAttachmentType;
            }
            else
            {
                var DetailList = _context.ITHTransactionDetailModal.Where(y => y.ID == Id).FirstOrDefault();
                imagedp = DetailList.AttachedFileToRevert;
                type = DetailList.AttachedFileToRevertType;

            }
            base64String = imagedp != null ? Convert.ToBase64String(imagedp, 0, imagedp.Length) : "";
            type = type != null ? type : "";
            return base64String + "|" + type;
        }

        public int ConfirmIThResponse(List<ITHTransactionDetailModal> SelectedResponse)
        {
            var s = 0;
            try
            {
                var TranactionId = SelectedResponse[0].TrnId;
                var ithdetail = _context.ITHTransactionDetailModal.Where(y => y.TrnId == TranactionId).ToList();
                foreach (var i in ithdetail)
                {
                    if (SelectedResponse.Any(j => j.ID == i.ID))
                    {
                        i.OATDeskApproval = 1;
                        i.OATDeskApprovalDate = DateTime.Now;
                    }

                }
                // update master table 
                var reqID = SelectedResponse[0].OATRequestID;
                var ithMasterList = _context.ITHTransactionMasterModal.Where(x => x.OATRequestID == reqID).FirstOrDefault();
                ithMasterList.OATDeskApproval = 1;
                ithMasterList.OATDeskApprovalDate = DateTime.Now;
                s = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                s = 1;
            }

            return s;
        }
        public int SubmitHodResponse(List<ITHTransactionDetailModal> SelectedResponse)
        {
            var s = 0;
            try
            {
                //var ithdetail = _context.ITHTransactionDetailModal.Where(y => y.ID == SelectedResponse.ID && y.TrnId == SelectedResponse.TrnId).ToList();
                var oatReqId = SelectedResponse[0].OATRequestID;
                var UserResponse = new List<ITHTransactionDetailModal>();
                foreach (var lst in SelectedResponse)
                {
                    var ithdetail = _context.ITHTransactionDetailModal.Where(y => y.ID == lst.ID && y.TrnId == lst.TrnId).FirstOrDefault();
                    UserResponse.Add(ithdetail);
                }
                foreach (var i in UserResponse)
                {
                    i.HodApproval = 1;
                    i.HodApprovalDate = DateTime.Now;
                }
                var ithMasterList = _context.ITHTransactionMasterModal.Where(x => x.OATRequestID == oatReqId).FirstOrDefault();
                ithMasterList.HodApproval = 1;
                ithMasterList.HodApprovalDate = DateTime.Now;
                s = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                s = 1;
            }
            return s;
        }
        public int SubmitUserResponse(List<ITHTransactionDetailModal> SelectedResponse)
        {
            var s = 0;
            try
            {
                var oatReqId = SelectedResponse[0].OATRequestID;
                var UserResponse = new List<ITHTransactionDetailModal>();
                foreach (var lst in SelectedResponse)
                {
                    var ithdetail = _context.ITHTransactionDetailModal.Where(y => y.ID == lst.ID && y.TrnId == lst.TrnId).FirstOrDefault();
                    UserResponse.Add(ithdetail);
                }
                foreach (ITHTransactionDetailModal i in UserResponse)
                {
                    i.UserApproval = 1;
                    i.UserApprovalDate = DateTime.Now;
                }
                var ithMasterList = _context.ITHTransactionMasterModal.Where(x => x.OATRequestID == oatReqId).FirstOrDefault();
                ithMasterList.UserApproval = 1;
                ithMasterList.UserApprovalDate = DateTime.Now;
                s = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                s = 1;
            }
            return s;
        }
        public int RevertToITh(ITHTransactionDetailLogModal revertToIthSrc)
        {
            var s = 0;
            try
            {
                //var logbatchID = _context.ITHTransactionDetailLogModal.Where(y=>y.TrnId == revertToIthSrc.TrnId).DefaultIfEmpty().Max(x => x == null ? 0 : x.LogBatchID);
                //var selectedSector = _context.ITHTransactionDetailModal.Where(y => y.ID == revertToIthSrc.ID).FirstOrDefault();
                var ithdDetail = _context.ITHTransactionDetailModal.Where(y => y.TrnId == revertToIthSrc.TrnId).ToList();
                foreach (var i in ithdDetail)
                {
                    if ((revertToIthSrc.OriginPlace == i.OriginPlace) && (revertToIthSrc.DestinationPlace == i.DestinationPlace))
                    {
                        i.OATDeskApproval = 2;
                        i.OATDeskApprovalDate = DateTime.Now;
                        i.ReasonForCancellation = revertToIthSrc.OATDeskRemarks;
                        i.AttachedFileToRevert = revertToIthSrc.ITHUploadRefFile;
                        i.AttachedFileToRevertType = revertToIthSrc.OAtDeskUploadRefFileType;
                    }
                }
                // update master table 
                var ithMasterList = _context.ITHTransactionMasterModal.Where(x => x.OATRequestID == revertToIthSrc.OATRequestID).FirstOrDefault();
                ithMasterList.OATDeskApproval = 2;
                ithMasterList.IsITHSentResponse = -1;
                ithMasterList.OATDeskApprovalDate = DateTime.Now;
                s = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                s = -1;
            }

            return s;
        }
        /// <summary>
        /// criteria 1 for cancelled by flight ID 
        /// </summary>
        /// <param name="cancellationsrc"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Int64 cancelFlightReq(OATTravelRequestFlightDetailModal cancellationsrc, int criteria)
        {
            var lstdetail = new List<OATTravelRequestFlightDetailModal>();
            var Ithdetail = new List<ITHTransactionDetailModal>();
            var flightList = _context.OATTravelRequestFlightDetailModal.Where(y => y.OriginPlace == cancellationsrc.OriginPlace && y.DestinationPlace == cancellationsrc.DestinationPlace && y.OATTravelRequestId == cancellationsrc.OATTravelRequestId && y.IsFlightCancel != true).ToList();
            if (criteria == 1)
            {
                lstdetail = _context.OATTravelRequestFlightDetailModal.Where(y => y.PassengerID == cancellationsrc.PassengerID && y.OriginPlace == cancellationsrc.OriginPlace && y.DestinationPlace == cancellationsrc.DestinationPlace).ToList();
            }
            else
                lstdetail = _context.OATTravelRequestFlightDetailModal.Where(y => y.OATTravelRequestId == cancellationsrc.OATTravelRequestId && y.IsFlightCancel != true).ToList();
            foreach (var i in lstdetail)
            {
                i.IsFlightCancel = cancellationsrc.IsFlightCancel;
                i.FlightCancelBy = cancellationsrc.FlightCancelBy;
                i.CancellationReason = cancellationsrc.CancellationReason;
                i.CancellationAttachment = cancellationsrc.CancellationAttachment;
                i.CancelledTime = cancellationsrc.CancelledTime;
                i.CancelType = cancellationsrc.CancelType;
                i.CancellationAttachmentType = cancellationsrc.CancellationAttachmentType;
            }
            if (criteria == 1)
            {
                Ithdetail = _context.ITHTransactionDetailModal.Where(y => y.OriginPlace == cancellationsrc.OriginPlace && y.DestinationPlace == cancellationsrc.DestinationPlace && y.OATRequestID == cancellationsrc.OATTravelRequestId).ToList();
            }
            else
                Ithdetail = _context.ITHTransactionDetailModal.Where(y => y.OATRequestID == cancellationsrc.OATTravelRequestId).ToList();
            
            if (flightList.Count == lstdetail.Count)
            {
                foreach (var i in Ithdetail)
                {
                    i.IsFlightCancel = cancellationsrc.IsFlightCancel;
                    i.FlightCancelBy = cancellationsrc.FlightCancelBy;
                }
            }

            var s = _context.SaveChanges();
            return lstdetail[0].OATTravelRequestId;
        }
        public int sendHoldRequestToIth(Int64 oatReqId ,string holdBy)
        {
            int result = 0;
            try
            {
                var lstdetail = _context.OATTravelRequestFlightDetailModal.Where(y => y.OATTravelRequestId == oatReqId).ToList();
                foreach (var i in lstdetail)
                {
                    i.IsFlightOnHold = true;
                    i.FlightOnHoldBy = holdBy;
                }
                var Ithdetail = _context.ITHTransactionDetailModal.Where(y => y.OATRequestID == oatReqId).ToList();
                if (Ithdetail.Count > 0)
                {
                    foreach (var i in Ithdetail)
                    {
                        i.IsFlightOnHold = true;
                        i.FlightOnHoldBy = holdBy;

                    }
                }
                var s = _context.SaveChanges();
                result = Ithdetail.Count;
            }
            catch (Exception ex)
            {
                result = -1;
                throw ex;
            }
            return result;
        }

        public int undoHoldRequestToIth(Int64 oatReqId, string holdBy)
        {
            int result = 0;
            try
            {
                var lstdetail = _context.OATTravelRequestFlightDetailModal.Where(y => y.OATTravelRequestId == oatReqId).ToList();
                foreach (var i in lstdetail)
                {
                    i.IsFlightOnHold = false;
                    i.FlightOnHoldBy = holdBy;
                }
                var Ithdetail = _context.ITHTransactionDetailModal.Where(y => y.OATRequestID == oatReqId).ToList();
                if (Ithdetail.Count > 0)
                {
                    foreach (var i in Ithdetail)
                    {
                        i.IsFlightOnHold = false;
                        i.FlightOnHoldBy = holdBy;

                    }
                }
                var s = _context.SaveChanges();
                result = Ithdetail.Count;
            }
            catch (Exception ex)
            {
                result = -1;
                throw ex;
            }
            return result;
        }

        public int cancelFlightReqAsPerReqId(Int64 oatReqId)
        {
            int result = 0;
            try
            {
                var lstdetail = _context.OATTravelRequestFlightDetailModal.Where(y => y.OATTravelRequestId == oatReqId).ToList();
                foreach (var i in lstdetail)
                {
                    i.IsFlightCancel = true;
                    i.FlightCancelBy = "By OAT Desk";
                }
                var Ithdetail = _context.ITHTransactionDetailModal.Where(y => y.OATRequestID == oatReqId).ToList();
                if (Ithdetail.Count > 0)
                {
                    foreach (var i in Ithdetail)
                    {
                        i.IsFlightCancel = true;
                        i.FlightCancelBy = "By OAT Desk";
                    }
                }
                var s = _context.SaveChanges();
                result = Ithdetail.Count;
            }
            catch (Exception ex)
            {
                result = -1;
                throw ex;
            }
            return result;
        }

        public bool ISReqSentToIth(Int64 oatReqId)
        {
            bool response = false;
            response = _context.ITHTransactionMasterModal.Where(y => y.OATRequestID == oatReqId).Select(x => x.IsReqSentToIth).FirstOrDefault();
            return response;
        }

        /// <summary>
        /// Check status of is Ith sent response or not
        /// </summary>
        /// <param name="types"></param>
        /// <param name="OatReqId"></param>
        /// <returns></returns>
        public String checkstatusOfOthBookingResponse(string types, Int64 OatReqId)
        {
            string retMsg = " ";
            Int16? response = 0;
            try
            {
                if (types == "a" || types == "r" || types == "ORa" || types == "ORr")
                {
                    response = _context.ITHTransactionMasterModal.Where(y => y.OATRequestID == OatReqId).Select(x => x.IsITHSentResponse).FirstOrDefault();
                    if (response == 1)
                    {
                        retMsg = "Your response has been already submitted. Please wait for OAT TravelDesk Response.";
                    }
                    else if (response == 2)
                    {
                        retMsg = "Your response has been already Rejected.";
                    }

                }
                if (types == "ua" || types == "UR" || types == "UA")
                {
                    response = _context.ITHTransactionMasterModal.Where(y => y.OATRequestID == OatReqId).Select(x => x.UserApproval).FirstOrDefault();
                    if (response == 1)
                    {
                        retMsg = "Your response has been already submitted. Please wait till further Notification.";
                    }
                    else if (response == 2)
                    {
                        retMsg = "Your response has been already Rejected.";
                    }
                }
                if (types == "HA" || types == "HR"|| types == "ha")
                {
                    response = _context.ITHTransactionMasterModal.Where(y => y.OATRequestID == OatReqId).Select(x => x.HodApproval).FirstOrDefault();
                    if (response == 1)
                    {
                        retMsg = "Your response has been already submitted. Please wait till further Notification.";
                    }
                    else if (response == 2)
                    {
                        retMsg = "Your response has been already Rejected.";
                    }
                }
                if (types == "FA" || types == "FR")
                {
                    response = _context.OATFinancialApprovalDetail_RoisteringModal.Where(y => y.OATId == OatReqId).Select(x => x.ApprovalStatus).FirstOrDefault();
                    if (response == 1)
                    {
                        retMsg = "Your response has been already submitted.";
                    }
                    else if (response == 2)
                    {
                        retMsg = "Your response has been already Rejected.";
                    }
                }
                if (types == "FBA" || types == "FBR")
                {
                    response = _context.OATTravelRequestMasterModals.Where(y => y.OATRequestID == OatReqId).Select(x => x.BookingStatus).FirstOrDefault();
                    if (response == 1)
                    {
                        retMsg = "Your response has been already submitted.";
                    }
                    else if (response == 2)
                    {
                        retMsg = "Your response has been already Rejected.";
                    }
                }

                return retMsg;
            }
            catch (Exception ex)
            {
                retMsg = "Oops! Something Went Wrong.";
                return retMsg;
                throw ex;
            }
        }
        public String RejectIthResponse(string types, Int64 oatreqId)
        {
            string retMsg = " ";
            try
            {
                var IthTransactionMaster = _context.ITHTransactionMasterModal.Where(y => y.OATRequestID == oatreqId).FirstOrDefault();
                if (types == "r"/*|| types == "FBR"*/)
                {
                    //add in log table 
                    var IthrejectionlogModal = new ITHTransactionRejectionModal();
                    IthrejectionlogModal.TrnID = IthTransactionMaster.TrnID;
                    IthrejectionlogModal.OATRequestID = IthTransactionMaster.OATRequestID;
                    IthrejectionlogModal.RejectedDate = DateTime.Now;
                    IthrejectionlogModal.RequestedBy = IthTransactionMaster.RequestedBy;
                    IthrejectionlogModal.ITHVendorCode = IthTransactionMaster.ITHVendorCode;
                    IthrejectionlogModal.IsITHSentResponse = 2;
                    IthrejectionlogModal.Remarks = "Rejected";
                    _context.ITHTransactionRejectionModal.Add(IthrejectionlogModal);

                    //updatemaster table
                    IthTransactionMaster.IsITHSentResponse = 2;
                    IthTransactionMaster.IthSentResponseDate = DateTime.Now;
                }
                if (types == "UR")
                {
                    IthTransactionMaster.UserApproval = 2;
                    IthTransactionMaster.UserApprovalDate = DateTime.Now;

                    //update ithtransactionDetail table
                    var IthTransactionDetail = _context.ITHTransactionDetailModal.Where(y => y.OATRequestID == oatreqId).FirstOrDefault();
                    IthTransactionDetail.UserApproval = 2;
                    IthTransactionDetail.UserApprovalDate = DateTime.Now;
                }
                if (types == "HR")
                {
                    IthTransactionMaster.HodApproval = 2;
                    IthTransactionMaster.HodApprovalDate = DateTime.Now;

                    //update ithtransactionDetail table
                    var IthTransactionDetail = _context.ITHTransactionDetailModal.Where(y => y.OATRequestID == oatreqId).FirstOrDefault();
                    IthTransactionDetail.HodApproval = 2;
                    IthTransactionDetail.HodApprovalDate = DateTime.Now;
                }
                var s = _context.SaveChanges();
                retMsg = "Your response has been successfully Rejected.";
            }
            catch (Exception ex)
            {
                retMsg = "Oops! Something Went Wrong.";
                return retMsg;
                throw ex;
            }
            return retMsg;
        }
        public string SaveManageNoShow()
        {
            var aa = "call successfully";
            return aa;
        }
        public List<ITHTransactionDetailModal> acceptReponseByHodOrUserAsPre(string types, Int64 oatReqId)
        {
            string retMsg = " ";
            var getList = new List<ITHTransactionDetailModal>();
            try
            {
                if (types == "UA")
                {
                    getList = _context.ITHTransactionDetailModal.Where(y => y.OATRequestID == oatReqId && y.OATDeskApproval == 1).ToList();
                    //getList.UserApproval = 1;
                    // getList.UserApprovalDate = DateTime.Now;
                    //retMsg = "Your response has been successfully submitted as same as Oat Desk.";
                }
                if (types == "HA")
                {
                    var uniquesector = _context.ITHTransactionDetailModal.Where(y => y.OATRequestID == oatReqId).Select(m => new { m.OriginPlace, m.DestinationPlace }).Distinct().ToList();
                    var list = _context.ITHTransactionDetailModal.Where(y => y.OATRequestID == oatReqId && y.OATDeskApproval == 1 && y.UserApproval == 1).ToList();
                    if (list != null && (list.Count == uniquesector.Count))
                    {
                        getList = list;
                    }
                    //if (getList == null)
                    //{
                    //    retMsg = "Choice of Oat Desk and User has Conflict, to Submit your choice Please click on the modify Link .";
                    //}
                    //else
                    //{
                    //    //getList.HodApproval = 1;
                    //    //getList.HodApprovalDate = DateTime.Now;
                    //    retMsg = "Your response has been successfully submitted as same as Oat Desk and User.";
                    //}
                }

                //var s = _context.SaveChanges();

            }
            catch (Exception ex)
            {
                retMsg = "Oops! Something Went Wrong.";
                return getList;
                throw ex;
            }
            return getList;
        }


        public String ApproveOrRejectFinancialApproval(string types, Int64 oatreqId)
        {
            string retMsg = " ";
            try
            {
                var FinancialDetail = _context.OATFinancialApprovalDetail_RoisteringModal.Where(y => y.OATId == oatreqId).ToList();
                foreach (var lst in FinancialDetail)
                {
                    lst.ApprovalStatus = (types == "FR") ? (Int16)2 : (Int16)1;
                    lst.ApprovalDate = DateTime.Now;
                }
                var s = _context.SaveChanges();
                retMsg = (types == "FR") ? "Your response has been successfully Rejected." : "Your response has been successfully submitted.";
            }
            catch (Exception ex)
            {
                retMsg = "Oops! Something Went Wrong.";
                return retMsg;
                throw ex;
            }
            return retMsg;
        }


        public String SubmitHoldAcknwledgement(string types, Int64 oatreqId)
        {
            string retMsg = " ";
            try
            {
                var FlightDetail = _context.OATTravelRequestFlightDetailModal.Where(y => y.OATTravelRequestId == oatreqId).ToList();
                if (FlightDetail[0].HoldAcknowlwdgement)
                {
                    retMsg = "Your acknowledgement is already submitted successfully.";
                }
                else
                {
                    foreach (var lst in FlightDetail)
                    {
                        lst.HoldAcknowlwdgement = true;
                        lst.HoldAcknowledgementTime = DateTime.Now;
                    }
                    var s = _context.SaveChanges();
                    retMsg = "Thank you for giving your response, your acknowledgement is submitted successfully.";
                }

            }
            catch (Exception ex)
            {
                retMsg = "Oops! Something Went Wrong.";
                return retMsg;
                throw ex;
            }
            return retMsg;
        }
        public String SubmitCLevelAcknowledgemnt(string types, Int64 oatreqId)
        {
            string retMsg = " ";
            try
            {
                var response = _context.ITHTransactionMasterModal.Where(y => y.OATRequestID == oatreqId).Select(x => x.CXO_ApprovalStatus).FirstOrDefault();
                if (response == 1)
                {
                    retMsg = "Your acknowledgement is already submitted successfully.";
                }
                if (response == 2)
                {
                    retMsg = "Your acknowledgement is already rejected successfully.";
                }
                else
                {
                    //var s = 0;
                    //var cxoDetail = getClevelApproverMail(oatReqId);
                    var IThDetail = _context.ITHTransactionMasterModal.Where(y => y.OATRequestID == oatreqId).FirstOrDefault();
                    //IThDetail.CXOEmailId = cxoDetail.Split('|')[0];
                    IThDetail.CXO_ApprovalDate = DateTime.Now;
                    IThDetail.CXO_ApprovalStatus = (types == "CLA") ? (Int16)1 : (Int16)2;
                    _context.SaveChanges();
                    //return s;
                    retMsg = "Thank you for giving your response, your response is submitted successfully.";
                }

            }
            catch (Exception ex)
            {
                retMsg = "Oops! Something Went Wrong.";
                return retMsg;
                throw ex;
            }
            return retMsg;
        }

        public String SubmitCancelAcknwledgement(string types, Int64 oatreqId, int criteria)
        {
            string retMsg = " ";
            try
            {
                var FlightDetail = new List<OATTravelRequestFlightDetailModal>();
                if (criteria == 1)
                {
                    var passID = Convert.ToInt64(types.Split('|')[0]);
                    var Origin = types.Split('|')[1];
                    var Destination = types.Split('|')[2];
                    FlightDetail = _context.OATTravelRequestFlightDetailModal.Where(y => y.OATTravelRequestId == oatreqId && y.PassengerID == passID && y.OriginPlace == Origin && y.DestinationPlace == Destination).ToList();
                }
                else
                    FlightDetail = _context.OATTravelRequestFlightDetailModal.Where(y => y.ID == oatreqId).ToList();
                if (!FlightDetail[0].CancelledAcknowledgement)
                {
                    foreach (var lst in FlightDetail)
                    {
                        lst.CancelledAcknowledgement = true;
                        lst.CancelledAcknowledgementTime = DateTime.Now;
                    }
                    var s = _context.SaveChanges();
                    retMsg = "Thank you for giving your response, your acknowledgement is submitted successefully.";
                }
                else
                {
                    retMsg = "You have already submitted your Acknowledgement.";
                }

            }
            catch (Exception ex)
            {
                retMsg = "Oops! Something Went Wrong.";
                return retMsg;
                throw ex;
            }
            return retMsg;
        }

        public int UpdateClevelNotification(Int64 oatReqId)
        {
            var s = 0;
            var cxoDetail = getClevelApproverMail(oatReqId);
            var IThDetail = _context.ITHTransactionMasterModal.Where(y => y.OATRequestID == oatReqId).FirstOrDefault();
            IThDetail.CXOEmailId = cxoDetail.Split('|')[0];
            IThDetail.CXO_ApprovalDate = DateTime.Now;
            IThDetail.CXO_ApprovalStatus = 0;
            s = _context.SaveChanges();
            return s;
        }

        ////getempIDfor HodDetail
        //public int getEmpIdAsPerReqId(Int64 oatReqID)
        //{
        //    var EmpID = _context.OATTravelRequestMasterModals.Where(x => x.OATRequestID == oatReqID).Select(y=> y.RequestedEmpId).FirstOrDefault();
        //    return EmpID;
        //}

        /// <summary>
        /// Dispose Object
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public String GetManageNoShowDetails(ITHTransactionDetailModal mangeShowList,DateTime date)
        {
            var response = "";
            
            try
            {
                var flightDetail = _context.OATTravelRequestFlightDetailModal.Where(x => x.OriginPlace == mangeShowList.OriginPlace && x.DestinationPlace == mangeShowList.DestinationPlace && x.OATTravelRequestId == mangeShowList.OATRequestID).ToList();
                var modal = new OATFlightNoShowReportModal();
                //modal.ID = 1;
                modal.OATReqId = mangeShowList.OATRequestID;
                modal.Sector = mangeShowList.OriginPlace + "-" + mangeShowList.DestinationPlace;
                modal.AirlineName = mangeShowList.AirCraftName;
                modal.Price = Convert.ToDecimal(mangeShowList.Amount);
                modal.FlightNo = mangeShowList.FlightNumber;
                modal.EntryDate = DateTime.Now;
                modal.EntryBy = "OAT TravelDesk";
                modal.IsInternational = _context.OATTravelRequestMasterModals.Where(x => x.OATRequestID == mangeShowList.OATRequestID).Select(x => x.BookingType).ToString() == "Domestic" ? false : true;
                modal.NoOfPassengers = (Int16)flightDetail.Count();
                modal.DepartureTime = mangeShowList.DepartureTime;
                modal.TravelDate = date;
                _context.OATFlightNoShowReportModal.Add(modal);               
                    var s = _context.SaveChanges();
                if(s>0)
                    response = "your data is submitted successefully.";              
                
            }
            catch(Exception ex)
            {
                response = " ";
                return response;
                throw ex;
            }

            return response;
        }
        public List<OATFlightNoShowReportModal> getFlightNoShowList()
        {
            var OatFlightNoShowList = new List<OATFlightNoShowReportModal>();
            try
            {
                var oatfltNoShowRecord = _context.OATFlightNoShowReportModal.Select(x => x).ToList();

                OatFlightNoShowList = oatfltNoShowRecord.ToList();
                //var jfkj = _context.OATTravelRequestFlightDetailModal.Where(z => z.OATTravelRequestId == 4 && z.IsFlightCancel == true).Count();// == null ? "cancelled" : //; _context.OATTravelRequestFlightDetailModal.Where(z => z.OATTravelRequestId == r.OATRequestID && z.IsFlightCancel == false).ToList() == null ? "":"partialCancelled"
            }
            catch (Exception ex)
            {
                return OatFlightNoShowList;
            }
            return OatFlightNoShowList;
        }

        public List<OATUploadItenaryModal> GetListOfAllBookedOATFlightDetail()
        {
            var ListOfBookedFlight = new List<OATUploadItenaryModal>();
            try
            {
                DateTime TodayDate = DateTime.Now.Date;
                TimeSpan TodayTime = DateTime.Now.TimeOfDay;
                 ListOfBookedFlight = _context.OATUploadItenaryModal.Where(x => DbFunctions.TruncateTime(x.DepartureDate) >= TodayDate).ToList();  
                foreach(var lst in ListOfBookedFlight)
                {

                    TimeSpan ts = Convert.ToDateTime(lst.DepartureTime).TimeOfDay;
                    //var hkj = (TodayTime - ts).TotalHours < 3; //- TimeSpan.FromHours(3);
                    //var g = TimeSpan.Compare(ts, TodayTime);
                    //TimeSpan gjgj = '03:23:19.5491526';
                    if ((TodayTime - ts).TotalHours <= 3 )
                    {
                        var passengerDetail = _context.OATTravelRequestPassengerDetailModal.Where(x => x.PassengerID == lst.PassengerID).FirstOrDefault();
                        lst.EmpName = passengerDetail.Gender + " " + passengerDetail.FirstName + " " + passengerDetail.LastName;
                        lst.EmpPhoneNo = passengerDetail.PhoneNo;
                        lst.EmpEmailId = passengerDetail.EmailId;
                    }                       
                }             
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListOfBookedFlight;
        }

        public List<OATFinancialApprovalDetail_RoisteringModal> GetListToSendFinancialApprover()
        {
            DateTime TodayDate = DateTime.Now.Date;
            //ListOfBookedFlight = _context.OATTravelRequestFlightDetailModal.Where(x => DbFunctions.TruncateTime(x.DepartureDate) >= TodayDate).ToList();
            //OATTravelRequestMasterModal
            //     select* from OATTravelRequestMaster M join OATTravelRequestFlightDetail F on m.OATRequestID = f.OATTravelRequestId
            //        join OATFinancialApprovalDetail_Roistering R on f.OATTravelRequestId = R.OATId
            //      where m.BookingFor = 'Roistering(Flight ops / In Flight)'  and r.ApprovalStatus = 0

            List<OATFinancialApprovalDetail_RoisteringModal> getAllList  = new List<OATFinancialApprovalDetail_RoisteringModal>();
            var getListOfFiApproval = _context.OATFinancialApprovalDetail_RoisteringModal.Where(x => x.ApprovalStatus == 0 ).ToList();
            foreach(var lst in getListOfFiApproval)
            {
                var origin = lst.Sector.Split('-')[0];
                var Destination = lst.Sector.Split('-')[1];
                var PassengerDetail = _context.OATTravelRequestPassengerDetailModal.Where(x => x.OATTravelRequestId == lst.OATId && x.PassengerID == lst.PassengerId).FirstOrDefault();
                var FlightDetail = _context.OATTravelRequestFlightDetailModal.Where(x => DbFunctions.TruncateTime(x.DepartureDate) < TodayDate && x.PassengerID == lst.PassengerId && x.OriginPlace== origin && x.DestinationPlace == Destination).FirstOrDefault();
                if (FlightDetail !=null)
                {
                    lst.departureDate = FlightDetail.DepartureDate;
                    lst.PassengerName = PassengerDetail.Gender + PassengerDetail.FirstName + PassengerDetail.LastName;
                    lst.ApproverPhoneNo = _context.OATFinancialApprovalMaster_RoisteringModal.Where(y => y.EmpCode == lst.ApproverEmpCode).Select(z => z.MobileNo).FirstOrDefault();
                    getAllList.Add(lst);
                }
               
            }
            return getAllList;
        }
        public List<OATTravelRequestFlightDetailModal> GetListOfRoisteringBooking(List<OATTravelRequestFlightDetailModal> flightInfo)
        {
            var listofBookings = new List<OATTravelRequestFlightDetailModal>();
            try
            {

                foreach (var lst in flightInfo)
                {

                    listofBookings = _context.OATTravelRequestFlightDetailModal.Where(y => y.DepartureDate == lst.DepartureDate && (y.OriginPlace == lst.OriginPlace && y.DestinationPlace == lst.DestinationPlace)).ToList();
                    foreach (var det in listofBookings)
                    {
                        var passaengerDetail = _context.OATTravelRequestPassengerDetailModal.Where(x => x.PassengerID == det.PassengerID).FirstOrDefault();
                        det.PhoneNo = passaengerDetail.PhoneNo;
                        det.Emaild = passaengerDetail.EmailId;
                        det.EmpName = passaengerDetail.Gender + " " + passaengerDetail.FirstName + " " + passaengerDetail.LastName;
                    }


                }

            }
            catch (Exception es)
            {

            }
            return listofBookings;
        }
    }
}



