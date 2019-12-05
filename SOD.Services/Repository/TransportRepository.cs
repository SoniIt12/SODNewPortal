using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Services.Interface;
using SOD.Model;
using SOD.Services.EntityFramework;

namespace SOD.Services.Repository
{
    public class TransportRepository : ITransportRepository
    {

        /// <summary>
        /// Initialized Constructor
        /// </summary>
        private readonly SodEntities _context;
        public TransportRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }


        /// <summary>
        /// Get Booking and Cab Info
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodBookingandCabInfo(long travelReqId)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var cabList = new List<TravelRequestCabDetailModels>();
            var bookingInfoList = new List<TravelRequestMasterModels>();

            sodflightList = _context.FlightDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            passengerList = _context.PassengerDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            cabList = _context.TravelRequestCabDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            bookingInfoList = _context.TravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();

            dicInfo.Add("cabinfo", cabList);
            dicInfo.Add("flightInfo", sodflightList);
            dicInfo.Add("passInfo", passengerList);
            dicInfo.Add("bookingInfo", bookingInfoList);
            return dicInfo;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodBookingandHotelInfo(long travelReqId)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var bookingInfoList = new List<TravelRequestMasterModels>();

            var sodflightListOat = new List<OALModels>();
            var passengerListOat = new List<OALPassengerModel>();
            var hotelListOat = new List<OALHotelModel>();
            var bookingInfoListOat = new List<OALTravelRequestMasterModel>();

            sodflightList = _context.FlightDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            passengerList = _context.PassengerDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            hotelList = _context.TravelRequestHotelDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            bookingInfoList = _context.TravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();

            //sodflightListOat = _context.OALModels.Where(b => b.TravelRequestId == travelReqId).ToList();
            //passengerListOat = _context.OALPassengerModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            //hotelListOat = _context.OALHotelModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            //bookingInfoListOat = _context.OALTravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();

            dicInfo.Add("hotelinfo", hotelList);
            dicInfo.Add("flightInfo", sodflightList);
            dicInfo.Add("passInfo", passengerList);
            dicInfo.Add("bookingInfo", bookingInfoList);
            dicInfo.Add("hotelinfoOat", hotelListOat);
            dicInfo.Add("flightInfoOat", sodflightListOat);
            dicInfo.Add("passInfoOat", passengerListOat);
            dicInfo.Add("bookingInfoOat", bookingInfoListOat);

            return dicInfo;
        }


        /// <summary>
        /// get lists by travel request id
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodHotelInfo(long travelReqId, int hId)
        {
            //var _context = new SodEntities();
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var bookingInfoList = new List<TravelRequestMasterModels>();

            sodflightList = _context.FlightDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            passengerList = _context.PassengerDetailModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            hotelList = _context.TravelRequestHotelDetailModel.Where(b => b.TravelRequestId == travelReqId && b.HotelRequestId == hId).ToList();
            bookingInfoList = _context.TravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();

            dicInfo.Add("hotelinfolist", hotelList);
            dicInfo.Add("flightInfolist", sodflightList);
            dicInfo.Add("passInfolist", passengerList);
            dicInfo.Add("bookingInfolist", bookingInfoList);
            return dicInfo;
        }

        /// <summary>
        /// Get Sod Hotel Info Oat
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodHotelInfoOat(long travelReqId)
        {
            //var _context = new SodEntities();
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var sodflightList = new List<OALModels>();
            var passengerList = new List<OALPassengerModel>();
            var hotelList = new List<OALHotelModel>();
            var bookingInfoList = new List<OALTravelRequestMasterModel>();

            sodflightList = _context.OALModels.Where(b => b.TravelRequestId == travelReqId).ToList();
            passengerList = _context.OALPassengerModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            //hotelList = _context.OALHotelModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            //bookingInfoList = _context.OALTravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();

            dicInfo.Add("hotelinfolistOat", hotelList);
            dicInfo.Add("flightInfolistOat", sodflightList);
            dicInfo.Add("passInfolistOat", passengerList);
            dicInfo.Add("bookingInfolistOat", bookingInfoList);
            return dicInfo;
        }


        /// <summary>
        /// fill drop down list for contractual hotel
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Dictionary<string, object> hotelfilldropdown(string name)
        {
            //var _context = new SodEntities();
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var hoteldetails = new List<SodHotelListDataModels>();
            var hname = name.Split('-')[0].ToString();
            var citycode = name.Split('-')[1].ToString();

            hoteldetails = _context.SodHotelListDataModels.Where(a => a.HotelName == hname && a.StationCode == citycode).ToList();
            var code = hoteldetails[0].HotelCode;
            var pricedetails = new List<SodHotelPriceListMasterModels>();
            pricedetails = _context.SodHotelPriceListMasterModel.Where(a => a.HotelCode == code).ToList();

            dicInfo.Add("hoteldetails", hoteldetails);
            dicInfo.Add("pricedetails", pricedetails);

            return dicInfo;
        }


        /// <summary>
        /// get hotel details and shared user details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetHotelDetailbyTrID(long travelReqId, int hId)
        {
            //var _context = new SodEntities();
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var hoteldetails = new List<HotelRequestApprovalModel>();
            var hotelData = new List<TravelRequestHotelDetailModels>();
            var sharedtravelReqID = new List<HotelRequestApprovalModel>();
            var sharedUserdetails = new List<TravelRequestMasterModels>();
            var rejectedData = new List<HotelRequestRejectionModel>();
            var cancelledData = new List<HotelCancellationByTraveldeskModel>();

            hoteldetails = _context.HotelRequestApprovalModel.Where(a => a.TravelRequestId == travelReqId && a.HotelRequestId == hId).ToList();
            hotelData = _context.TravelRequestHotelDetailModel.Where(a => a.TravelRequestId == travelReqId && a.HotelRequestId == hId).ToList();
            var clubid = hotelData[0].clubId;
            if (hoteldetails.Count > 0)
            {
                hoteldetails[0].SubmittedBy = hotelData[0].SubmittedBy;
            }
            sharedtravelReqID = _context.HotelRequestApprovalModel.Where(a => a.clubId == clubid && a.TravelRequestId != travelReqId).ToList();
            if (sharedtravelReqID.Count > 0)
            {
                var sharedtravID = sharedtravelReqID[0].TravelRequestId;
                var sharedhId = sharedtravelReqID[0].HotelRequestId;
                sharedUserdetails = _context.TravelRequestMasterModel.Where(a => a.TravelRequestId == sharedtravID).ToList();
                dicInfo.Add("sharedUserdetails", sharedUserdetails);
                dicInfo.Add("sharedtravID", sharedtravID);
                dicInfo.Add("sharedhId", sharedhId);
            }
            else
            {
                dicInfo.Add("sharedUserdetails", "");
                dicInfo.Add("sharedtravID", "");
                dicInfo.Add("sharedhId", "");
            }
            rejectedData = _context.HotelRequestRejectionModel.Where(a => a.TravelRequestId == travelReqId && a.HotelRequestId == hId).ToList();
            cancelledData = _context.HotelCancellationByTraveldeskModel.Where(a => a.TravelRequestId == travelReqId && a.HotelRequestId == hId).ToList();
            dicInfo.Add("hotelinfobyTrid", hoteldetails);
            dicInfo.Add("hotelData", hotelData);
            dicInfo.Add("rejectedData", rejectedData);
            dicInfo.Add("cancelledData", cancelledData);
            return dicInfo;
        }

        /// <summary>
        /// Get Hotel Detail by TrID Oat
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetHotelDetailbyTrIDOat(long travelReqId)
        {
            //var _context = new SodEntities();
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var hoteldetailsoat = new List<HotelRequestApprovalOatModels>();
            var hotelDataOat = new List<OALHotelModel>();
            var sharedtravelReqID = new List<HotelRequestApprovalOatModels>();
            var sharedUserdetails = new List<TravelRequestMasterModels>();

            hoteldetailsoat = _context.HotelRequestApprovalOatModel.Where(a => a.TravelRequestId == travelReqId).ToList();
            hotelDataOat = _context.OALHotelModel.Where(a => a.TravelRequestId == travelReqId).ToList();

            var clubid = hotelDataOat[0].clubId;
            sharedtravelReqID = _context.HotelRequestApprovalOatModel.Where(a => a.clubId == clubid && a.TravelRequestId != travelReqId).ToList();
            if (sharedtravelReqID.Count > 0)
            {
                var sharedtravID = sharedtravelReqID[0].TravelRequestId;
                sharedUserdetails = _context.TravelRequestMasterModel.Where(a => a.TravelRequestId == sharedtravID).ToList();
                dicInfo.Add("sharedUserdetailsOat", sharedUserdetails);
                dicInfo.Add("sharedtravIDOat", sharedtravID);
            }
            else
            {
                dicInfo.Add("sharedUserdetailsOat", "");
                dicInfo.Add("sharedtravIDOat", "");
            }

            dicInfo.Add("hotelinfobyTridOat", hoteldetailsoat);
            dicInfo.Add("hotelDataOat", hotelDataOat);
            return dicInfo;
        }

        /// <summary>
        /// Get Sod Booking Request with Cab Request :TravelDesk
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="empId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodEmployeeBookingHistoryList_TravelDesk(int? departmentId, int? empId, int criteria)
        {
            return ADO.SodCommonServices.GetSodEmployeeBookingHistoryList_TravelDesk(departmentId, empId, criteria);
        }

        /// <summary>
        /// Get Sod Booking Request with Hotel Request :TravelDesk
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="empId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodEmployeeHotelList_TravelDesk(int? departmentId, int? empId, int criteria)
        {
            return ADO.SodCommonServices.GetSodEmployeeHotelList_TravelDesk(departmentId, empId, criteria);
        }

        /// <summary>
        /// Get Sod Cab Booking Status
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<string> GetCabStatus(int reqId, int criteria)
        {
            return ADO.SodCommonServices.GetCabStatus(reqId, criteria);
        }

        /// <summary>
        /// Get Sod Hotel Booking Status
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<string> GetHotelStatus(int reqId, int hId, int criteria)
        {
            return ADO.SodCommonServices.GetHotelStatus(reqId, hId, criteria);
        }

        /// <summary>
        /// Get Vendor List
        /// </summary>
        /// <param name="VendorCode"></param>
        /// <returns></returns>
        public List<TransportVendorMasterModels> GetVendorList(string VendorCode)
        {
            var s = _context.TransportVendorMasterModel.ToList();
            return s;
        }


        /// <summary>
        /// Approve Cab Request with Remarks
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        public int ApprovedCabRequest(Int64 reqId, string strRemarks)
        {
            var s = 0;
            //using (_context)
            //{
            var updateItem = _context.TravelRequestCabDetailModel.Where(o => o.TravelRequestId == reqId).ToList();
            foreach (var p in updateItem)
            {
                p.Remarks_Status = strRemarks;
                p.IsAllocated = 1;
            }
            s = _context.SaveChanges();
            //}
            return s;
        }


        /// <summary>
        /// approve hotel request by traveldesk
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        public int ApprovedHotelRequest(List<TravelRequestHotelDetailModels> elist)
        {
            var s = 0;
            var reqId = Convert.ToInt64(elist[0].TravelRequestId);
            var hId = Convert.ToInt32(elist[0].HotelRequestId);
            var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hId).ToList();
            foreach (var p in updateItem)
            {
                p.IsAllocated = 1;
            }
            s = _context.SaveChanges();
            return s;
        }


        public int ApprovedHotelRequestOat(List<OALHotelModel> elist)
        {
            var s = 0;
            var reqId = Convert.ToInt64(elist[0].TravelRequestId);
            var hId = Convert.ToInt32(elist[0].HotelRequestId);
            //using (_context)
            //{
            var updateItem = _context.OALHotelModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hId).ToList();
            foreach (var p in updateItem)
            {
                p.IsAllocated = 1;
            }
            s = _context.SaveChanges();
            return s;
        }

        /// <summary>
        /// Reject Cab Request with Remarks
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        public int RejectCabRequest(Int64 reqId, string strRemarks)
        {
            var s = 0;
            //using (_context)
            //{
            var updateItem = _context.TravelRequestCabDetailModel.Where(o => o.TravelRequestId == reqId).ToList();
            foreach (var p in updateItem)
            {
                p.Remarks_Status = strRemarks;
                p.IsAllocated = 2;
            }
            s = _context.SaveChanges();
            //}
            return s;
        }


        public int RejectHotelRequest(Int64 reqId, string strRemarks)
        {
            var s = 0;
            //using (_context)
            //{
            var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId).ToList();
            foreach (var p in updateItem)
            {
                p.Remarks_Status = strRemarks;
                p.IsAllocated = 2;
            }
            s = _context.SaveChanges();
            //}
            return s;
        }


        public int RejectHotelRequestOat(Int64 reqId, string strRemarks)
        {
            var s = 0;
            // using (_context)
            // {
            var updateItem = _context.OALHotelModel.Where(o => o.TravelRequestId == reqId).ToList();
            foreach (var p in updateItem)
            {
                p.Remarks_Status = strRemarks;
                p.IsAllocated = 2;
            }
            s = _context.SaveChanges();
            //}
            return s;
        }


        /// <summary>
        /// get hotel and user details for mail
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetHotelandUserInfo(List<String> requestList, List<String> hidList, string hotelname, string sodOat)
        {
            var criteria = 1;
            if (sodOat == "SOD")
            {
                criteria = 1;
            }
            else
            {
                criteria = 2;
            }
            var rqid1 = "";
            var rqid2 = "";
            var hid1 = "";
            var hid2 = "";
            foreach (var i in hidList)
            {
                if (i.ToString() == "0") { }
            }
            if (hidList.Count == 1)
            {
                rqid1 = requestList[0].ToString().Trim();
                hid1 = hidList[0].ToString().Trim();
            }
            else
            {
                rqid1 = requestList[0].ToString().Trim();
                hid1 = hidList[0].ToString().Trim();
                rqid2 = requestList[1].ToString().Trim();
                hid2 = hidList[1].ToString().Trim();
            }
            return ADO.SodCommonServices.GetHotelandUserInfo(rqid1, rqid2, hid1, hid2, criteria, sodOat);
        }


        /// <summary>
        /// get hotel and user details for responsive mail
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetHotelandUserInfoForResponsiveMail(List<String> requestList, List<String> hidList, string hotelname, string sodOat)
        {
            var criteria = 1;
            if (sodOat == "SOD")
            {
                criteria = 1;
            }
            else
            {
                criteria = 2;
            }
            var rqid1 = "";
            var rqid2 = "";
            var hid1 = "";
            var hid2 = "";
            if (hidList.Count == 1)
            {
                rqid1 = requestList[0].ToString().Trim();
                hid1 = hidList[0].ToString().Trim();
            }
            else
            {
                rqid1 = requestList[0].ToString().Trim();
                hid1 = hidList[0].ToString().Trim();
                rqid2 = requestList[1].ToString().Trim();
                hid2 = hidList[1].ToString().Trim();
            }
            return ADO.SodCommonServices.GetHotelandUserInfoForResponsiveMail(rqid1, rqid2, hid1, hid2, criteria, sodOat);
        }


        /// <summary>
        /// save hotel request
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="hotelcode"></param>
        /// <param name="hotelname"></param>
        /// <param name="hoteladd"></param>
        /// <param name="hotelphone"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        public int SaveHotelRequest(List<HotelRequestApprovalModel> elist, string hotelprice, string occupancy, string sodOat)
        {
            var s = 0;
            for (var i = 0; i < elist.Count; i++)
            {
                var reqId = Convert.ToInt64(elist[i].TravelRequestId);
                var hid = Convert.ToInt32(elist[i].HotelRequestId);
                if (sodOat == "SOD")
                {
                    var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                    foreach (var p in updateItem)
                    {
                        p.HotelStatus = "Pending from Hotel";
                        p.HotelType = elist[i].HotelType;
                        p.Remarks_Status = elist[i].Remarks;
                        p.clubId = elist[i].clubId;
                        p.HotelConfirmationNo = elist[i].HotelConfirmationNo;
                        p.HotelPrice = Convert.ToDecimal(hotelprice);
                        p.Occupancy = occupancy;
                        p.IsAllocated = 0;
                        p.HotelCurrencyCode = elist[i].HotelCurrencyCode;
                        p.SubmittedBy = elist[i].SubmittedBy;
                    }
                    var hoteldetail = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                    var destination = hoteldetail[0].City.ToString();

                    var flightDetail = _context.FlightDetailModel.Where(o => o.TravelRequestId == reqId && o.DestinationPlace == destination).ToList();
                    var flight = flightDetail[0].FlightNo;
                    var flightNo = flight.Substring(flight.LastIndexOf(',') + 1).Trim();
                    var eta = flightDetail[0].ArrivalTime.ToString();

                    var cId = elist[0].clubId;
                    var existingAppList = _context.HotelRequestApprovalModel.Where(o => o.clubId == cId && o.TravelRequestId == reqId).ToList();
                    if (existingAppList == null || existingAppList.Count < 1)
                    {
                        var newList = new HotelRequestApprovalModel()
                        {
                            TravelRequestId = reqId,
                            HotelRequestId = hid,
                            HotelCode = elist[i].HotelCode,
                            RequestDate = DateTime.Now,
                            HotelName = elist[i].HotelName,
                            HotelAddress = elist[i].HotelAddress,
                            HotelPhone = elist[i].HotelPhone,
                            HotelType = elist[i].HotelType,
                            PrimaryEmail = elist[i].PrimaryEmail,
                            SecondaryEmail = elist[i].SecondaryEmail,
                            Remarks = elist[i].Remarks,
                            Status = "Pending",
                            ApprovalDate = new DateTime(1900, 1, 1),
                            clubId = elist[i].clubId,
                            FlightNo = flightNo,
                            ETA = eta,
                            IsTaxIncluded = elist[i].IsTaxIncluded,
                            HotelConfirmationNo = elist[i].HotelConfirmationNo,
                            HotelPrice = Convert.ToDecimal(hotelprice),
                            HotelCurrencyCode = elist[i].HotelCurrencyCode
                        };
                        _context.HotelRequestApprovalModel.Add(newList);
                    }
                    else
                    {
                        _context.HotelRequestApprovalModel.RemoveRange(existingAppList);
                        var newList = new HotelRequestApprovalModel()
                        {
                            TravelRequestId = reqId,
                            HotelRequestId = hid,
                            HotelCode = elist[i].HotelCode,
                            RequestDate = DateTime.Now,
                            HotelName = elist[i].HotelName,
                            HotelAddress = elist[i].HotelAddress,
                            HotelPhone = elist[i].HotelPhone,
                            HotelType = elist[i].HotelType,
                            PrimaryEmail = elist[i].PrimaryEmail,
                            SecondaryEmail = elist[i].SecondaryEmail,
                            Remarks = elist[i].Remarks,
                            Status = "Pending",
                            ApprovalDate = new DateTime(1900, 1, 1),
                            clubId = elist[i].clubId,
                            FlightNo = flightNo,
                            ETA = eta,
                            IsTaxIncluded = elist[i].IsTaxIncluded,
                            HotelConfirmationNo = elist[i].HotelConfirmationNo,
                            HotelPrice = Convert.ToDecimal(hotelprice),
                            HotelCurrencyCode = elist[i].HotelCurrencyCode
                        };
                        _context.HotelRequestApprovalModel.Add(newList);
                    }
                }
                else
                {
                    var updateItem = _context.OALHotelModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                    foreach (var p in updateItem)
                    {
                        p.HotelStatus = "Pending from Hotel";
                        p.HotelType = elist[i].HotelType;
                        p.Remarks_Status = elist[i].Remarks;
                        p.clubId = elist[i].clubId;
                        p.HotelPrice = Convert.ToDecimal(hotelprice);
                        p.Occupancy = occupancy;
                    }

                    var hoteldetail = _context.OALHotelModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                    var destination = hoteldetail[0].City.ToString();

                    var flightDetail = _context.OALModels.Where(o => o.TravelRequestId == reqId && o.DestinationPlace == destination).ToList();
                    var flight = flightDetail[0].DepFlightNumber;
                    var flightNo = flight.Substring(flight.LastIndexOf(',') + 1).Trim();
                    var eta = hoteldetail[0].CheckinTime;

                    var newList = new HotelRequestApprovalOatModels()
                    {
                        TravelRequestId = reqId,
                        HotelRequestId = hid,
                        HotelCode = elist[i].HotelCode,
                        RequestDate = DateTime.Now,
                        HotelName = elist[i].HotelName,
                        HotelAddress = elist[i].HotelAddress,
                        HotelPhone = elist[i].HotelPhone,
                        HotelType = elist[i].HotelType,
                        PrimaryEmail = elist[i].PrimaryEmail,
                        SecondaryEmail = elist[i].SecondaryEmail,
                        Remarks = elist[i].Remarks,
                        Status = "Pending",
                        ApprovalDate = new DateTime(1900, 1, 1),
                        clubId = elist[i].clubId,
                        FlightNo = flightNo,
                        ETA = eta,
                        IsTaxIncluded = elist[i].IsTaxIncluded,
                        HotelPrice = Convert.ToDecimal(hotelprice)
                    };
                    _context.HotelRequestApprovalOatModel.Add(newList);
                }
                s = _context.SaveChanges();

            }
            return s;
        }

        /// <summary>
        /// Cancel Hotel Request by user
        /// </summary>
        /// <param name="trid"></param>
        /// <param name="reason"></param>
        /// <param name="status"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int CancelHotelRequest(Int64 trid, int hid, string reason, string status, string type)
        {
            var s = 0;
            if (type == "SOD")
            {
                var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hid).ToList();
                foreach (var p in updateItem)
                {
                    p.usercancellation = "Cancelled by User";
                    p.IsAllocated = 1;
                    p.IsCancelled = 1;
                }
                var newList = new HotelRequestCancellationModels()
                {
                    TravelRequestId = trid,
                    HotelRequestId = hid,
                    CancellationDate = DateTime.Now,
                    CancellationReason = reason,
                    hotelStatus = status
                };
                _context.HotelRequestCancellationModel.Add(newList);

                //Satyam
                //var updateMasterItem = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == trid && o.IsHotelRequired ==1).ToList();
                //updateMasterItem[0].BookingStatus = "Close";
                //updateMasterItem[0].StatusDate = System.DateTime.Now();
            }

            s = _context.SaveChanges();
            return s;
        }

        /// <summary>
        /// Send cancellation Request to hotel
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="hotelcode"></param>
        /// <param name="hotelname"></param>
        /// <param name="address"></param>
        /// <param name="phone"></param>
        /// <param name="remarks"></param>
        /// <param name="primaryemail"></param>
        /// <param name="secondaryemail"></param>
        /// <param name="hoteltype"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public int SaveCancellationRequest(string travReqstId, int hid, string sodOat)
        {
            var s = 0;
            var reqId = Convert.ToInt64(travReqstId);
            //using (_context)
            //{
            if (sodOat == "SOD")
            {
                var updateItem = _context.HotelRequestApprovalModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();

                for (var i = 0; i < updateItem.Count; i++)
                {
                    var newList = new HotelCancellationByTraveldeskModel()
                    {
                        TravelRequestId = updateItem[i].TravelRequestId,
                        HotelRequestId = updateItem[i].HotelRequestId,
                        HotelCode = updateItem[i].HotelCode,
                        RequestDate = updateItem[i].RequestDate,
                        HotelName = updateItem[i].HotelName,
                        HotelAddress = updateItem[i].HotelAddress,
                        HotelPhone = updateItem[i].HotelPhone,
                        HotelType = updateItem[i].HotelType,
                        PrimaryEmail = updateItem[i].PrimaryEmail,
                        SecondaryEmail = updateItem[i].SecondaryEmail,
                        Remarks = updateItem[i].Remarks,
                        Status = "Cancelled by Traveldesk",
                        ApprovalDate = DateTime.Now,
                        clubId = updateItem[i].clubId,
                        FlightNo = updateItem[i].FlightNo,
                        ETA = updateItem[i].ETA,
                        IsTaxIncluded = updateItem[i].IsTaxIncluded,
                        HotelConfirmationNo = updateItem[i].HotelConfirmationNo,
                        HotelPrice = updateItem[i].HotelPrice
                    };
                    _context.HotelCancellationByTraveldeskModel.Add(newList);
                }

                _context.HotelRequestApprovalModel.RemoveRange(updateItem);

                var updateItem2 = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                foreach (var p in updateItem2)
                {
                    if (p.Occupancy != "Single")
                    {
                        var ClubbedEmpDetails = _context.TravelRequestHotelDetailModel.Where(o => o.clubId == p.clubId).ToList();
                        foreach (var c in ClubbedEmpDetails)
                        {
                            if (c.TravelRequestId != reqId && c.IsCancelled == 0)
                            {
                                c.Occupancy = "Single";
                            }
                        }
                    }
                    p.HotelStatus = null;
                    p.HotelType = null;
                    p.Remarks_Status = null;
                    p.clubId = 0;
                    p.HotelConfirmationNo = null;
                    p.IsAllocated = 0;
                    p.HotelCode = "NA";
                    p.HotelName = "NA";
                    p.HotelAddress = "NA";
                    p.HotelPhoneNo = "NA";
                    p.PrimaryEmail = "NA";
                    p.SecondaryEmail = "NA";
                    p.HodApprovalStatus = null;

                }
                //var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId==hid).ToList();
                //foreach (var p in updateItem)
                //{
                //    if (p.UserCheckinCheckoutUpdate == false)
                //    {
                //        p.IsCancelled = 1;
                //        p.usercancellation = "Cancelled";
                //        p.IsAllocated = 1;
                //    }
                //}
            }
            else
            {
                var updateItem = _context.OALHotelModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                foreach (var p in updateItem)
                {
                    p.IsCancelled = 1;
                    p.usercancellation = "Cancelled";
                    p.IsAllocated = 1;
                }
            }
            s = _context.SaveChanges();
            //}
            return s;
        }


        public int SaveCancellationRequestUser(string travReqstId, int hid, string sodOat)
        {
            var s = 0;
            var reqId = Convert.ToInt64(travReqstId);
            //using (_context)
            //{
            if (sodOat == "SOD")
            {
                var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                foreach (var p in updateItem)
                {
                    if (p.Occupancy != "Single")
                    {
                        var ClubbedEmpDetails = _context.TravelRequestHotelDetailModel.Where(o => o.clubId == p.clubId).ToList();
                        foreach (var c in ClubbedEmpDetails)
                        {
                            if (c.TravelRequestId != reqId && c.IsCancelled != 1)
                            {
                                c.Occupancy = "Single";
                            }
                        }
                    }
                    if (p.UserCheckinCheckoutUpdate == false)
                    {
                        p.IsCancelled = 1;
                        p.usercancellation = "Cancelled";
                        p.IsAllocated = 1;
                    }

                }
                var newList = new HotelRequestCancellationModels()
                {
                    TravelRequestId = reqId,
                    HotelRequestId = hid,
                    CancellationDate = DateTime.Now,
                    CancellationReason = "Cancelled by Traveldesk",
                    hotelStatus = updateItem[0].HotelStatus == "Approved by Hotel" ? "Allocated" : "Not Allocated"
                };
                _context.HotelRequestCancellationModel.Add(newList);
            }
            else
            {
                var updateItem = _context.OALHotelModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                foreach (var p in updateItem)
                {
                    p.IsCancelled = 1;
                    p.usercancellation = "Cancelled";
                    p.IsAllocated = 1;
                }
            }
            s = _context.SaveChanges();
            //}
            return s;
        }

        /// <summary>
        /// Get details of approval from hotel by travel request id 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public Dictionary<string, object> getApprovalHotelDetails(string travelReqId, int hId, string sodOat)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var trId = Convert.ToInt64(travelReqId);
            if (sodOat == "SOD")
            {
                var hoteldetails = new List<HotelRequestApprovalModel>();
                hoteldetails = (_context.HotelRequestApprovalModel.Where(o => o.TravelRequestId == trId && o.HotelRequestId == hId)).ToList();
                dicInfo.Add("approvalHotelDetails", hoteldetails);
            }
            else
            {
                var hoteldetails = new List<HotelRequestApprovalOatModels>();
                hoteldetails = (_context.HotelRequestApprovalOatModel.Where(o => o.TravelRequestId == trId && o.HotelRequestId == hId)).ToList();
                dicInfo.Add("approvalHotelDetails", hoteldetails);
            }
            return dicInfo;
        }

        /// <summary>
        /// get requests not approved by hod- exceptional cases
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetHotelInfoExceptional(string trid, int criteria)
        {
            return ADO.SodCommonServices.GetHotelInfoExceptional(trid, criteria);
        }


        /// <summary>
        /// get hotel inclusion data
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        public List<HotelInclusionMasterModels> findHotelInclusions(string hotelcity, string hotelname)
        {
            var list = new List<HotelInclusionMasterModels>();
            list = _context.HotelInclusionMasterModel.Where(o => o.HotelName == hotelname && o.Location == hotelcity).ToList();
            return list;
        }

        /// <summary>
        /// get approver email id's for non contractual hotel approval
        /// </summary>
        /// <returns></returns>
        public List<NonContractualHotelApprovalMasterModels> GetApproverIds()
        {
            var list = new List<NonContractualHotelApprovalMasterModels>();
            list = _context.NonContractualHotelApprovalMasterModel.ToList();
            return list;
        }

        /// <summary>
        /// save approval request for non-contractual by HOD from traveldesk
        /// </summary>
        /// <param name="elist"></param>
        /// <param name="hodemail1"></param>
        /// <param name="hodemail2"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public int SaveHODFinancialApprovalRequest(List<HotelRequestApprovalModel> elist, string hodemail1, string hodemail2, List<HotelInclusionNonContractualMasterModels> inclist, string sodOat)
        {
            var s = 0;
            var occ = "";
            if (elist.Count == 1) occ = "Single"; else occ = "Double";

            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    for (var i = 0; i < elist.Count; i++)
                    {
                        var reqId = Convert.ToInt64(elist[i].TravelRequestId);
                        var hid = elist[i].HotelRequestId;
                        if (sodOat == "SOD")
                        {
                            var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                            foreach (var p in updateItem)
                            {
                                p.HotelType = elist[i].HotelType;
                                p.Remarks_Status = elist[i].Remarks;
                                p.clubId = elist[i].clubId;
                                p.HodApprovalStatus = "0";
                                p.HotelPrice = elist[i].HotelPrice;
                                p.HotelStatus = null;
                                p.Occupancy = occ;
                                p.IsAllocated = 0;
                                p.HotelCurrencyCode = elist[i].HotelCurrencyCode;
                                p.SubmittedBy = elist[i].SubmittedBy;
                            }
                            var hoteldetail = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                            var destination = hoteldetail[0].City.ToString();
                            var flightDetail = _context.FlightDetailModel.Where(o => o.TravelRequestId == reqId && o.DestinationPlace == destination).ToList();
                            var flight = flightDetail[0].FlightNo;
                            var flightNo = flight.Substring(flight.LastIndexOf(',') + 1).Trim();
                            var eta = flightDetail[0].ArrivalTime.ToString();
                            var newList = new HotelRequestApprovalModel()
                            {
                                TravelRequestId = reqId,
                                HotelRequestId = hid,
                                HotelCode = elist[i].HotelCode,
                                RequestDate = DateTime.Now,
                                HotelName = elist[i].HotelName,
                                HotelAddress = elist[i].HotelAddress,
                                HotelPhone = elist[i].HotelPhone,
                                HotelType = elist[i].HotelType,
                                PrimaryEmail = elist[i].PrimaryEmail,
                                SecondaryEmail = elist[i].SecondaryEmail,
                                Remarks = elist[i].Remarks,
                                ApprovalDate = new DateTime(1900, 1, 1),
                                clubId = elist[i].clubId,
                                HotelPrice = elist[i].HotelPrice,
                                FlightNo = flightNo,
                                ETA = eta,
                                IsTaxIncluded = elist[i].IsTaxIncluded,
                                HotelCurrencyCode = elist[i].HotelCurrencyCode
                            };
                            var hodList1 = _context.NonContractualHotelApprovalMasterModel.Where(n => n.EmailId == hodemail1).ToList();
                            if (hodemail2.Length > 0)
                            {
                                var hodList2 = _context.NonContractualHotelApprovalMasterModel.Where(n => n.EmailId == hodemail2).ToList();
                                var applist = new HotelRequestHODFinancialApprovalModels()
                                {
                                    TravelRequestId = reqId,
                                    HotelRequestId = hid,
                                    ApproverEmpCodeLevel1 = hodList1[0].EmpCode,
                                    ApproverEmpCodeLevel2 = hodList2[0].EmpCode,
                                    ApproverNameLevel1 = hodList1[0].EmpName,
                                    ApproverNameLevel2 = hodList2[0].EmpName,
                                    ApprovalDateLevel1 = new DateTime(1900, 1, 1),
                                    ApprovalDateLevel2 = new DateTime(1900, 1, 1),
                                    ApprovalStatusLevel1 = 0,
                                    ApprovalStatusLevel2 = 0,
                                    ApprovalStatus = 0
                                };
                                _context.HotelRequestHODFinancialApprovalModel.Add(applist);
                            }
                            else
                            {
                                var applist = new HotelRequestHODFinancialApprovalModels()
                                {
                                    TravelRequestId = reqId,
                                    HotelRequestId = hid,
                                    ApproverEmpCodeLevel1 = hodList1[0].EmpCode,
                                    ApproverNameLevel1 = hodList1[0].EmpName,
                                    ApprovalDateLevel1 = new DateTime(1900, 1, 1),
                                    ApprovalDateLevel2 = new DateTime(1900, 1, 1),
                                    ApprovalStatusLevel1 = 0,
                                    ApprovalStatusLevel2 = 1,
                                    ApprovalStatus = 0
                                };
                                _context.HotelRequestHODFinancialApprovalModel.Add(applist);
                            }
                            _context.SaveChanges();

                            var existingAppList = _context.HotelRequestApprovalModel.Where(o => o.HotelRequestId == hid && o.TravelRequestId == reqId).ToList();
                            if (existingAppList == null || existingAppList.Count < 1)
                            {
                                _context.HotelRequestApprovalModel.Add(newList);
                            }
                            else
                            {
                                _context.HotelRequestApprovalModel.RemoveRange(existingAppList);
                                _context.HotelRequestApprovalModel.Add(newList);
                            }
                            var newIncList = new HotelInclusionNonContractualMasterModels()
                            {
                                TravelRequestId = reqId,
                                HotelRequestId = hid,
                                HotelName = inclist[i].HotelName,
                                Accomodation = inclist[i].Accomodation,
                                Food = inclist[i].Food,
                                AirportTransfers = inclist[i].AirportTransfers,
                                RoomService = inclist[i].RoomService,
                                BuffetTime = inclist[i].BuffetTime,
                                Laundry = inclist[i].Laundry
                            };
                            _context.HotelInclusionNonContractualMasterModel.Add(newIncList);
                        }
                        else
                        {
                            var updateItem = _context.OALHotelModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                            foreach (var p in updateItem)
                            {
                                p.HotelType = elist[i].HotelType;
                                p.Remarks_Status = elist[i].Remarks;
                                p.clubId = elist[i].clubId;
                                p.HodApprovalStatus = "0";
                                p.HotelPrice = elist[i].HotelPrice;
                            }
                            var hoteldetail = _context.OALHotelModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hid).ToList();
                            var destination = hoteldetail[0].City.ToString();
                            var flightDetail = _context.OALModels.Where(o => o.TravelRequestId == reqId && o.DestinationPlace == destination).ToList();
                            var flight = flightDetail[0].DepFlightNumber;
                            var flightNo = flight.Substring(flight.LastIndexOf(',') + 1).Trim();
                            var eta = hoteldetail[0].CheckinTime;
                            var newList = new HotelRequestApprovalOatModels()
                            {
                                TravelRequestId = reqId,
                                HotelRequestId = hid,
                                HotelCode = elist[i].HotelCode,
                                RequestDate = DateTime.Now,
                                HotelName = elist[i].HotelName,
                                HotelAddress = elist[i].HotelAddress,
                                HotelPhone = elist[i].HotelPhone,
                                HotelType = elist[i].HotelType,
                                PrimaryEmail = elist[i].PrimaryEmail,
                                SecondaryEmail = elist[i].SecondaryEmail,
                                Remarks = elist[i].Remarks,
                                ApprovalDate = new DateTime(1900, 1, 1),
                                clubId = elist[i].clubId,
                                HotelPrice = elist[i].HotelPrice,
                                FlightNo = flightNo,
                                ETA = eta,
                                IsTaxIncluded = elist[i].IsTaxIncluded
                            };
                            var hodList1 = _context.NonContractualHotelApprovalMasterModel.Where(n => n.EmailId == hodemail1).ToList();
                            if (hodemail2.Length > 0)
                            {
                                var hodList2 = _context.NonContractualHotelApprovalMasterModel.Where(n => n.EmailId == hodemail2).ToList();
                                var applist = new HotelRequestHODFinancialApprovalOatModels()
                                {
                                    TravelRequestId = reqId,
                                    HotelRequestId = hid,
                                    ApproverEmpCodeLevel1 = hodList1[0].EmpCode,
                                    ApproverEmpCodeLevel2 = hodList2[0].EmpCode,
                                    ApproverNameLevel1 = hodList1[0].EmpName,
                                    ApproverNameLevel2 = hodList2[0].EmpName,
                                    ApprovalDateLevel1 = new DateTime(1900, 1, 1),
                                    ApprovalDateLevel2 = new DateTime(1900, 1, 1),
                                    ApprovalStatusLevel1 = 0,
                                    ApprovalStatusLevel2 = 0,
                                    ApprovalStatus = 0
                                };
                                _context.HotelRequestHODFinancialApprovalOatModel.Add(applist);
                            }
                            else
                            {
                                var applist = new HotelRequestHODFinancialApprovalOatModels()
                                {
                                    TravelRequestId = reqId,
                                    HotelRequestId = hid,
                                    ApproverEmpCodeLevel1 = hodList1[0].EmpCode,
                                    ApproverNameLevel1 = hodList1[0].EmpName,
                                    ApprovalDateLevel1 = new DateTime(1900, 1, 1),
                                    ApprovalDateLevel2 = new DateTime(1900, 1, 1),
                                    ApprovalStatusLevel1 = 0,
                                    ApprovalStatusLevel2 = 1,
                                    ApprovalStatus = 0
                                };
                                _context.HotelRequestHODFinancialApprovalOatModel.Add(applist);
                            }
                            _context.HotelRequestApprovalOatModel.Add(newList);
                            var newIncList = new HotelInclusionNonContractualMasterOatModels()
                            {
                                TravelRequestId = reqId,
                                HotelRequestId = hid,
                                HotelName = inclist[i].HotelName,
                                Accomodation = inclist[i].Accomodation,
                                Food = inclist[i].Food,
                                AirportTransfers = inclist[i].AirportTransfers,
                                RoomService = inclist[i].RoomService,
                                BuffetTime = inclist[i].BuffetTime,
                                Laundry = inclist[i].Laundry
                            };
                            _context.HotelInclusionNonContractualMasterOatModel.Add(newIncList);
                        }
                        s = _context.SaveChanges();
                        dbTran.Commit();
                    }
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
        /// Get Hod Approver Name for Hotels
        /// </summary>
        /// <param name="hodemail"></param>
        /// <returns></returns>
        public List<NonContractualHotelApprovalMasterModels> GetHodApproverNameHotels(string hodemail)
        {
            var list = new List<NonContractualHotelApprovalMasterModels>();
            list = _context.NonContractualHotelApprovalMasterModel.Where(a => a.EmailId == hodemail).ToList();
            return list;
        }

        /// <summary>
        /// view detail for hod approval status
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        public List<HotelRequestHODFinancialApprovalModels> GetdetailHODApproval(Int64 trid, int hId, string sodoat)
        {
            if (sodoat == "SOD")
            {
                var list = new List<HotelRequestHODFinancialApprovalModels>();
                list = _context.HotelRequestHODFinancialApprovalModel.Where(a => a.TravelRequestId == trid && a.HotelRequestId == hId).ToList();
                return list;
            }
            else
            {
                var list = new List<HotelRequestHODFinancialApprovalOatModels>();
                list = _context.HotelRequestHODFinancialApprovalOatModel.Where(a => a.TravelRequestId == trid && a.HotelRequestId == hId).ToList();
                var listreturn = new List<HotelRequestHODFinancialApprovalModels>();
                listreturn.Add(new HotelRequestHODFinancialApprovalModels
                {
                    TravelRequestId = list[0].TravelRequestId,
                    ApproverEmpCodeLevel1 = list[0].ApproverEmpCodeLevel1,
                    ApproverEmpCodeLevel2 = list[0].ApproverEmpCodeLevel2,
                    ApprovalDateLevel1 = list[0].ApprovalDateLevel1,
                    ApprovalDateLevel2 = list[0].ApprovalDateLevel2,
                    ApproverNameLevel1 = list[0].ApproverNameLevel1,
                    ApproverNameLevel2 = list[0].ApproverNameLevel2,
                    ApprovalStatusLevel1 = list[0].ApprovalStatusLevel1,
                    ApprovalStatusLevel2 = list[0].ApprovalStatusLevel2,
                    ApprovalStatus = list[0].ApprovalStatus
                });
                return listreturn;
            }
        }

        /// <summary>
        /// update hotel status after request sent to hotel
        /// </summary>
        /// <param name="elist"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public int UpdateHotelStatus(List<HotelRequestApprovalModel> elist, string sodOat)
        {
            var s = 0;
            for (var i = 0; i < elist.Count; i++)
            {
                var reqId = Convert.ToInt64(elist[i].TravelRequestId);
                var hId = elist[i].HotelRequestId;
                if (sodOat == "SOD")
                {
                    var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hId).ToList();
                    foreach (var p in updateItem)
                    {
                        p.HotelStatus = "Pending from Hotel";
                    }

                    var newList = _context.HotelRequestApprovalModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hId).ToList();
                    foreach (var p in newList)
                    {
                        p.Status = "Pending";
                        p.RequestDate = DateTime.Now;
                    }
                }
                else
                {
                    var updateItem = _context.OALHotelModel.Where(o => o.TravelRequestId == reqId).ToList();
                    foreach (var p in updateItem)
                    {
                        p.HotelStatus = "Pending from Hotel";
                    }

                    var newList = _context.HotelRequestApprovalOatModel.Where(o => o.TravelRequestId == reqId).ToList();
                    foreach (var p in newList)
                    {
                        p.Status = "Pending";
                        p.RequestDate = DateTime.Now;
                    }
                }
            }
            s = _context.SaveChanges();
            return s;
        }

        /// <summary>
        /// Find City Code by city name
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        public List<SodCityCodeMasterModels> FindCityCode(string hotelcity)
        {
            var list = new List<SodCityCodeMasterModels>();
            list = _context.SodCityCodeMasterModel.Where(o => o.CityName == hotelcity || o.CityCode == hotelcity).ToList();
            return list;
        }


        /// <summary>
        /// Find City Name by city Code
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        public List<SodCityCodeMasterModels> FindCityName(string citycode)
        {
            var list = new List<SodCityCodeMasterModels>();
            list = _context.SodCityCodeMasterModel.Where(o => o.CityCode == citycode).ToList();
            return list;
        }

        /// <summary>
        /// find hotel inclusions of non contractual hotel
        /// </summary>
        /// <param name="travelreqid"></param>
        /// <returns></returns>
        public List<HotelInclusionNonContractualMasterModels> FindNonContractualHotelInclusions(Int64 travelreqid, int HotelRequestId)
        {
            var list = new List<HotelInclusionNonContractualMasterModels>();
            list = _context.HotelInclusionNonContractualMasterModel.Where(o => o.TravelRequestId == travelreqid && o.HotelRequestId == HotelRequestId).ToList();
            return list;
        }

        /// <summary>
        /// find hotel inclusions of non contractual hotel oat
        /// </summary>
        /// <param name="travelreqid"></param>
        /// <returns></returns>
        public List<HotelInclusionNonContractualMasterOatModels> FindNonContractualHotelInclusionsOat(Int64 travelreqid)
        {
            var list = new List<HotelInclusionNonContractualMasterOatModels>();
            list = _context.HotelInclusionNonContractualMasterOatModel.Where(o => o.TravelRequestId == travelreqid).ToList();
            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="empcode"></param>
        /// <returns></returns>
        public string GetFinancialHodDetailsEmail(string empcode)
        {
            var list = new List<NonContractualHotelApprovalMasterModels>();
            list = _context.NonContractualHotelApprovalMasterModel.Where(o => o.EmpCode == empcode).ToList();
            if (list[0].EmailId == null || list[0].EmailId == "")
            {
                return "";
            }
            else
            {
                return list[0].EmailId;
            }
        }

        /// <summary>
        /// find hotel data which is similar to already allocated person
        /// </summary>
        /// <param name="hotelname"></param>
        /// <param name="checkin"></param>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        public List<TravelRequestHotelDetailModels> FindSimilarHotelAllocationData(Int64 TravelRequestId, string hotelname, DateTime checkin, string hotelcity)
        {
            var list = new List<TravelRequestHotelDetailModels>();
            list = ADO.SodCommonServices.FindSimilarHotelAllocationData(TravelRequestId, hotelname, checkin, hotelcity);
            return list;
        }

        /// <summary>
        /// Get Sod Booking Request with Hotel Request :TravelDesk- for double occupancy (sorted)
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="empId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodEmployeeHotelList_TravelDesk_DoubleOccupancy(string newTrid, string existingTrid, string hotelrqstid, string exishotelid)
        {
            return ADO.SodCommonServices.GetSodEmployeeHotelList_TravelDesk_DoubleOccupancy(newTrid, existingTrid, hotelrqstid, exishotelid);
        }

        /// <summary>
        /// find if club id already exists or not
        /// </summary>
        /// <param name="elist"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public int FindIfClubIdExists(List<HotelRequestApprovalModel> elist, string sodOat)
        {
            foreach (var i in elist)
            {
                if (i.clubId == null || i.clubId == 0)
                {
                    return 0;
                }
                else
                {
                    return i.clubId;
                }
            }
            return 0;
        }


        /// <summary>
        /// Auto trigger mail to users after confirmation by hotel
        /// </summary>
        /// <param name="clubid"></param>
        /// <returns></returns>
        public List<TravelRequestHotelDetailModels> SendMailToUsers(string clubid)
        {
            var s = 0;
            var cID = Convert.ToInt32(clubid);
            var tridList = _context.TravelRequestHotelDetailModel.Where(o => o.clubId == cID).ToList();
            foreach (var i in tridList)
            {
                var reqId = i.TravelRequestId;
                var hId = i.HotelRequestId;
                var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == reqId && o.HotelRequestId == hId).ToList();
                foreach (var p in updateItem)
                {
                    p.IsAllocated = 1;
                }
            }
            s = _context.SaveChanges();
            return tridList;
        }

        /// <summary>
        /// get details of hotel and users by club id
        /// </summary>
        /// <param name="clubid"></param>
        /// <returns></returns>
        public List<TravelRequestHotelDetailModels> GetDetailsByClubId(string clubid)
        {
            var cID = Convert.ToInt32(clubid);
            var tridList = _context.TravelRequestHotelDetailModel.Where(o => o.clubId == cID).ToList();
            return tridList;
        }

        /// <summary>
        /// get requests with for checkin-checkout time confirmation by user & hotel
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetHotelDetailCheckTimeConflict()
        {
            return ADO.SodCommonServices.GetHotelDetailCheckTimeConflict();
        }

        /// <summary>
        /// Find Duplicate Hotel Data
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="checkin"></param>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        public bool FindDuplicateDataHotel(string empcode, DateTime checkin, string hotelcity)
        {
            var list = _context.TravelRequestHotelDetailModel.Where(o => o.EmployeeCode == empcode && o.CheckInDate == checkin && o.City == hotelcity && o.usercancellation == null).ToList();
            if (list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// get hotel detals by travel request id for exceptional cases
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetHotelInfoApprovalbyTrID(long travelReqId)
        {
            //var _context = new SodEntities();
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var hoteldetails = new List<HotelRequestApprovalModel>();
            var hotelData = new List<TravelRequestHotelDetailModels>();

            hoteldetails = _context.HotelRequestApprovalModel.Where(a => a.TravelRequestId == travelReqId).ToList();
            hotelData = _context.TravelRequestHotelDetailModel.Where(a => a.TravelRequestId == travelReqId).ToList();

            dicInfo.Add("hotelinfobyTrid", hoteldetails);
            dicInfo.Add("hotelData", hotelData);
            return dicInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelcode"></param>
        /// <returns></returns>
        public string GetCityCodeOfHotel(string hotelcode)
        {
            var hotelData = new List<SodHotelListDataModels>();
            hotelData = _context.SodHotelListDataModels.Where(a => a.HotelCode == hotelcode).ToList();
            return hotelData[0].StationCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="citycode"></param>
        /// <returns></returns>
        public List<HotelGstDetailModels> GetGstDetailsByCityCode(string citycode)
        {
            var gstData = new List<HotelGstDetailModels>();
            gstData = _context.HotelGstDetailModel.Where(a => a.CityCode == citycode).ToList();
            return gstData;
        }
        /// <summary>
        /// find existing city code
        /// </summary>
        /// <param name="citycode"></param>
        /// <returns></returns>
        public List<SodCityCodeMasterModels> checkCityCodeExist(string citycode)
        {
            var data = new List<SodCityCodeMasterModels>();
            data = _context.SodCityCodeMasterModel.Where(a => a.CityCode == citycode).ToList();
            return data;
        }


        /// <summary>
        /// update hotel dates by user
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="hotelid"></param>
        /// <param name="checkin"></param>
        /// <param name="checkout"></param>
        /// <returns></returns>
        public int updateHotelDatesByUser(string TravelRequestId, string hotelid, DateTime checkin, DateTime checkout)
        {
            var s = 0;
            var trid = Convert.ToInt64(TravelRequestId);
            var hid = Convert.ToInt32(hotelid);
            var data = new List<TravelRequestHotelDetailModels>();
            data = _context.TravelRequestHotelDetailModel.Where(a => a.TravelRequestId == trid && a.HotelRequestId == hid && a.HotelStatus == null).ToList();
            foreach (var i in data)
            {
                i.CheckInDate = checkin;
                i.CheckOutDate = checkout;
            }
            s = _context.SaveChanges();
            return s;
        }


        /// <summary>
        /// Undo cancellation request
        /// </summary>
        /// <param name="trid"></param>
        /// <param name="hid"></param>
        /// <param name="reason"></param>
        /// <param name="status"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int UndoCancelledRequest(Int64 trid, int hid, string status, string type)
        {
            var s = 0;
            if (status == "Allocated")
            {
                var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hid).ToList();
                foreach (var p in updateItem)
                {
                    p.usercancellation = null;
                    p.IsCancelled = 0;
                }
            }
            else
            {
                var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hid).ToList();
                foreach (var p in updateItem)
                {
                    p.usercancellation = null;
                    p.IsAllocated = 0;
                    p.IsCancelled = 0;
                }
            }
            s = _context.SaveChanges();
            return s;
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

        /// <summary>
        /// Dispose Method
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }


    }
}
