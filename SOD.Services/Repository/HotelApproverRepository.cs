using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;
using SOD.Services.Interface;
using SOD.Services.EntityFramework;
using System.Globalization;

namespace SOD.Services.Repository
{
    public class HotelApproverRepository : IHotelApproverRepository
    {

        #region "Constructor Initialization"
        /// <summary>
        /// Constructor Initilization
        /// </summary>
        private readonly SodEntities _context;
        public HotelApproverRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
        #endregion


        /// <summary>
        /// Approve hotel booking request
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="confirmationNo"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public int ApproveHotelBookingRequest(string clubId, string confirmationNo, string hotelname, string sodOat)
        {
            var s = 0;
            var cId = Convert.ToInt32(clubId);

            if (sodOat == "SOD")
            {
                var updateItem = _context.HotelRequestApprovalModel.Where(b => b.clubId == cId && b.HotelName.ToLower().Trim() == hotelname.ToLower().Trim()).ToList();
                foreach (var p in updateItem)
                {
                    p.Status = "Approved";
                    p.HotelConfirmationNo = confirmationNo;
                    p.ApprovalDate = DateTime.Now;
                }

                var hoteldetails = new List<HotelRequestApprovalModel>();
                hoteldetails = _context.HotelRequestApprovalModel.Where(b => b.clubId == cId && b.HotelName.ToLower().Trim() == hotelname.ToLower().Trim()).ToList();

                //var hCode = hoteldetails[0].HotelCode;
                //var priceList = new List<SodHotelPriceListMasterModels>();
                //priceList = _context.SodHotelPriceListMasterModel.Where(b => b.HotelCode == hCode).ToList();

                var updateItem2 = _context.TravelRequestHotelDetailModel.Where(o => o.clubId == cId).ToList();
                foreach (var p in updateItem2)
                {
                    p.HotelStatus = "Approved by Hotel";
                    p.HotelCode = hoteldetails[0].HotelCode;
                    p.HotelName = hoteldetails[0].HotelName;
                    p.HotelAddress = hoteldetails[0].HotelAddress;
                    p.HotelPhoneNo = hoteldetails[0].HotelPhone;
                    p.PrimaryEmail = hoteldetails[0].PrimaryEmail;
                    p.SecondaryEmail = hoteldetails[0].SecondaryEmail;
                    p.HotelType = hoteldetails[0].HotelType;
                    p.HotelConfirmationNo = confirmationNo;
                    p.IsAllocated = 1;
                    //if (hoteldetails[0].HotelType == "Contractual")
                    //{
                    //    if (updateItem.Count > 1)
                    //    {
                    //        p.HotelPrice = priceList[0].DoublePrice;
                    //    }
                    //    else
                    //    {
                    //        p.HotelPrice = priceList[0].SinglePrice;
                    //    }
                    //}
                }

                //if (hoteldetails[0].HotelType == "Contractual")
                //{
                //    if (updateItem.Count > 1)
                //    {
                //        foreach (var p in updateItem)
                //        {
                //            p.HotelPrice = priceList[0].DoublePrice;
                //        }
                //    }
                //    else
                //    {
                //        foreach (var p in updateItem)
                //        {
                //            p.HotelPrice = priceList[0].SinglePrice;
                //        }
                //    }
                //}
            }
            else
            {
                var updateItem = _context.HotelRequestApprovalOatModel.Where(b => b.clubId == cId && b.HotelName.ToLower().Trim() == hotelname.ToLower().Trim()).ToList();
                foreach (var p in updateItem)
                {
                    p.Status = "Approved";
                    p.HotelConfirmationNo = confirmationNo;
                    p.ApprovalDate = DateTime.Now;
                }

                var hoteldetails = new List<HotelRequestApprovalOatModels>();
                hoteldetails = _context.HotelRequestApprovalOatModel.Where(b => b.clubId == cId && b.HotelName.ToLower().Trim() == hotelname.ToLower().Trim()).ToList();

                //var hCode = hoteldetails[0].HotelCode;
                //var priceList = new List<SodHotelPriceListMasterModels>();
                //priceList = _context.SodHotelPriceListMasterModel.Where(b => b.HotelCode == hCode).ToList();

                var updateItem2 = _context.OALHotelModel.Where(o => o.clubId == cId);
                foreach (var p in updateItem2)
                {
                    p.HotelStatus = "Approved by Hotel";
                    p.HotelCode = hoteldetails[0].HotelCode;
                    p.HotelName = hoteldetails[0].HotelName;
                    p.HotelAddress = hoteldetails[0].HotelAddress;
                    p.HotelPhoneNo = hoteldetails[0].HotelPhone;
                    p.PrimaryEmail = hoteldetails[0].PrimaryEmail;
                    p.SecondaryEmail = hoteldetails[0].SecondaryEmail;
                    p.HotelType = hoteldetails[0].HotelType;
                    p.HotelConfirmationNo = confirmationNo;
                }
            }
            s = _context.SaveChanges();


            return s;
        }


        /// <summary>
        /// reject hotel booking request
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="confirmationNo"></param>
        /// <returns></returns>
        public int RejectHotelBookingRequest(string clubId, string hotelname, string sodOat)
        {
            var s = 0;
            var cId = Convert.ToInt32(clubId);


            if (sodOat == "SOD")
            {
                var updateItem = _context.HotelRequestApprovalModel.Where(b => b.clubId == cId && b.HotelName == hotelname).ToList();

                for (var i = 0; i < updateItem.Count; i++)
                {
                    var newList = new HotelRequestRejectionModel()
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
                        Status = "Rejected",
                        ApprovalDate = DateTime.Now,
                        clubId = updateItem[i].clubId,
                        FlightNo = updateItem[i].FlightNo,
                        ETA = updateItem[i].ETA,
                        IsTaxIncluded = updateItem[i].IsTaxIncluded,
                        HotelConfirmationNo = updateItem[i].HotelConfirmationNo
                    };
                    _context.HotelRequestRejectionModel.Add(newList);
                }

                _context.HotelRequestApprovalModel.RemoveRange(updateItem);

                //var hoteldetails = new List<HotelRequestApprovalModel>();
                //hoteldetails = _context.HotelRequestApprovalModel.Where(b => b.clubId == cId && b.HotelName == hotelname).ToList();

                var updateItem2 = _context.TravelRequestHotelDetailModel.Where(o => o.clubId == cId);
                foreach (var p in updateItem2)
                {
                    p.HotelStatus = "Rejected by Hotel";
                    p.HotelType = null;
                    p.Remarks_Status = null;
                    p.clubId = 0;
                    p.HotelConfirmationNo = null;
                    p.IsAllocated = 0;
                }

            }
            else
            {
                var updateItem = _context.HotelRequestApprovalOatModel.Where(b => b.clubId == cId && b.HotelName == hotelname).ToList();
                foreach (var p in updateItem)
                {
                    p.Status = "Rejected";
                    p.ApprovalDate = DateTime.Now;
                }

                var hoteldetails = new List<HotelRequestApprovalOatModels>();
                hoteldetails = _context.HotelRequestApprovalOatModel.Where(b => b.clubId == cId && b.HotelName == hotelname).ToList();

                var updateItem2 = _context.OALHotelModel.Where(o => o.clubId == cId);
                foreach (var p in updateItem2)
                {
                    p.HotelStatus = "Rejected by Hotel";
                }
            }
            s = _context.SaveChanges();


            return s;
        }


        /// <summary>
        /// get passenger info for hotel mail
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetInfoForPassenger(Int64 travelReqId, int hid, string sodOat)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            if (sodOat == "SOD")
            {
                var sodRequestsList = new List<HotelRequestApprovalModel>();
                var pasList = new List<TravelRequestMasterModels>();
                sodRequestsList = (_context.HotelRequestApprovalModel.GroupBy(a => a.TravelRequestId).Select(a => a.OrderByDescending(x => x.RequestDate).FirstOrDefault()).Where(o => o.TravelRequestId == travelReqId && o.HotelRequestId == hid)).ToList();
                pasList = _context.TravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();
                dicInfo.Add("hotelConfirmInfo", sodRequestsList);
                dicInfo.Add("pasList", pasList);
            }
            else if (sodOat == "OAT")
            {
                var sodRequestsList = new List<HotelRequestApprovalOatModels>();
                var pasList = new List<OALTravelRequestMasterModel>();
                sodRequestsList = (_context.HotelRequestApprovalOatModel.GroupBy(a => a.TravelRequestId).Select(a => a.OrderByDescending(x => x.RequestDate).FirstOrDefault()).Where(o => o.TravelRequestId == travelReqId && o.HotelRequestId == hid)).ToList();
                pasList = _context.OALTravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();
                dicInfo.Add("hotelConfirmInfo", sodRequestsList);
                dicInfo.Add("pasList", pasList);
            }
            else
            {

            }
            return dicInfo;
        }


        /// <summary>
        /// To find confirmation no. of existing TravelRequestId
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public Dictionary<string, object> FindExistingTrId(string clubid, string types, string hotelname)
        {
            var cId = Convert.ToInt32(clubid);

            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            if (types == "SOD")
            {
                var rejectedlist = new List<HotelRequestRejectionModel>();
                rejectedlist = _context.HotelRequestRejectionModel.Where(b => b.clubId == cId && b.HotelName == hotelname).ToList();
                dicInfo.Add("rejectedlist", rejectedlist);

                var existingList = new List<TravelRequestHotelDetailModels>();
                existingList = _context.TravelRequestHotelDetailModel.Where(b => b.clubId == cId && b.HotelName == hotelname).ToList();
                dicInfo.Add("existingList", existingList);
            }
            else if (types == "OAT")
            {
                var existingList = new List<OALHotelModel>();
                existingList = _context.OALHotelModel.Where(b => b.clubId == cId && b.HotelName == hotelname).ToList();
                dicInfo.Add("existingList", existingList);
            }
            else
            {
                var existingList = new List<BulkTravelRequestHotelDetailModels>();
                existingList = _context.BulkTravelRequestHotelDetailModel.Where(b => b.clubId == cId && b.HotelName == hotelname).ToList();
                dicInfo.Add("existingList", existingList);
            }
            return dicInfo;
        }


        /// <summary>
        /// update existing confirmation no.
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="confirmationNo"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public int UpdateExistingCNo(string clubid, string types, string newconfirmNo, string oldconfirmNo)
        {
            var s = 0;
            var cId = Convert.ToInt32(clubid);

            {
                if (types == "SOD")
                {
                    var updateItem = _context.HotelRequestApprovalModel.Where(o => o.clubId == cId && o.HotelConfirmationNo == oldconfirmNo).ToList();
                    foreach (var p in updateItem)
                    {
                        p.HotelConfirmationNo = newconfirmNo;
                    }

                    var updateItem2 = _context.TravelRequestHotelDetailModel.Where(o => o.clubId == cId).ToList();
                    foreach (var p in updateItem2)
                    {
                        p.HotelConfirmationNo = newconfirmNo;
                    }
                }
                else if (types == "OAT")
                {
                    var updateItem = _context.HotelRequestApprovalOatModel.Where(o => o.clubId == cId && o.HotelConfirmationNo == oldconfirmNo).ToList();
                    foreach (var p in updateItem)
                    {
                        p.HotelConfirmationNo = newconfirmNo;
                    }

                    var updateItem2 = _context.OALHotelModel.Where(o => o.clubId == cId).ToList();
                    foreach (var p in updateItem2)
                    {
                        p.HotelConfirmationNo = newconfirmNo;
                    }
                }
                else
                {
                    var updateItem2 = _context.BulkTravelRequestHotelDetailModel.Where(o => o.clubId == cId).ToList();
                    foreach (var p in updateItem2)
                    {
                        p.HotelConfirmationNo = newconfirmNo;
                    }
                }
                s = _context.SaveChanges();
            }
            return s;
        }


        /// <summary>
        /// Approve bulk booking request by hotel
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="confirmationNo"></param>
        /// <returns></returns>
        public int ApproveHotelBookingRequestBulk(string clubId, string confirmationNo)
        {
            var s = 0;
            var cId = Convert.ToInt32(clubId);
            var updateItem = _context.BulkTravelRequestHotelDetailModel.Where(b => b.clubId == cId).ToList();
            foreach (var p in updateItem)
            {
                p.HotelStatus = "Approved by Hotel";
                p.HotelConfirmationNo = confirmationNo;
            }

            var hCode = updateItem[0].HotelCode;
            var priceList = new List<SodHotelPriceListMasterModels>();
            priceList = _context.SodHotelPriceListMasterModel.Where(b => b.HotelCode == hCode).ToList();
            //Need to check the code
            if (priceList.Count > 0)
            {
                if (updateItem.Count > 1)
                {
                    foreach (var p in updateItem)
                    {
                        p.HotelPrice = priceList[0].DoublePrice;
                        p.Occupancy = "Double";
                    }
                }
                else
                {
                    foreach (var p in updateItem)
                    {
                        p.HotelPrice = priceList[0].SinglePrice;
                        p.Occupancy = "Single";
                    }
                }
                s = _context.SaveChanges();
            }

            return s;
        }


        /// <summary>
        /// Reject bulk booking request by hotel
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public int RejectHotelBookingRequestBulk(string clubId)
        {
            var s = 0;
            var k = new List<BulkTravelRequestHotelRejectionDetailModels>();
            var cId = Convert.ToInt32(clubId);
            var updateItem = _context.BulkTravelRequestHotelDetailModel.Where(b => b.clubId == cId).ToList();
            foreach (var x in updateItem)
            {
                var modal = new BulkTravelRequestHotelRejectionDetailModels();
                modal.TravelRequestId = x.TravelRequestId;
                modal.HotelReferenceID = x.HotelReferenceID;
                modal.City = x.City;
                modal.EmployeeCode = x.EmployeeCode;
                //modal.CheckInDate = x.CheckInDate;
               // modal.CheckOutDate = x.CheckOutDate;
                modal.NoOfGuest = x.NoOfGuest;
                //modal.EntryDate = x.EntryDate;
                modal.HotelCode = x.HotelCode;
                modal.HotelName = x.HotelName;
                modal.HotelAddress = x.HotelAddress;
                //HotelPhoneNo = x.HotelPhoneNo,
                //IsAllocated = x.IsAllocated,
                //Remarks_Status = x.Remarks_Status,
                //CheckinTime = x.CheckinTime,
                //CheckoutTime = x.CheckoutTime,
                //PrimaryEmail = x.PrimaryEmail,
                //SecondaryEmail = x.SecondaryEmail,
                //HotelType = x.SecondaryEmail,
                //HotelStatus = x.HotelStatus,
                //HotelConfirmationNo = x.HotelConfirmationNo,
                //AirportTransport = x.AirportTransport,
                //usercancellation = x.usercancellation,
                //IsCancelled = x.IsCancelled,
                //clubId = x.clubId,
                //sharingId = x.sharingId,
                //HotelPrice = x.HotelPrice,
               /// Occupancy = x.Occupancy,
               // BReqId = x.BReqId,
               // HotelCurrencyCode = x.HotelCurrencyCode,
               // HodApprovalStatus = x.HodApprovalStatus,
               //SubmittedBy = x.SubmittedBy
                k.Add(modal);

            }
            //var dfd = updateItem.ConvertAll(x => new BulkTravelRequestHotelRejectionDetailModels
            //{
                
            //});
            //k = dfd.ToList();

            _context.BulkTravelRequestHotelRejectionDetailModel.AddRange(k);
            _context.SaveChanges();
            _context.BulkTravelRequestHotelDetailModel.RemoveRange(updateItem);
            //foreach (var p in updateItem)
            //{
            //    p.HotelStatus = "Rejected by Hotel";
            //}
            s = _context.SaveChanges();

            return s;
        }

        /// <summary>
        /// Approve Non-Contractual Hotel Request by hod
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="approverEmpcCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int ApproveNonContractualHotelRequest(string travelReqID, string hid, string approverEmpcCode, string type)
        {
            var s = 0;
            var IdList = travelReqID.Split(',');
            var hidList = hid.Split(',');
            for (var i = 0; i < IdList.Length - 1; i++)
            {
                var trid = Convert.ToInt64(IdList[i]);
                var hId = Convert.ToInt32(hidList[i]);
                var findIfRejected = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hId).ToList();
                foreach (var f in findIfRejected)
                {
                    if (f.HodApprovalStatus == "2")
                    {
                        return s;
                    }
                }
                var updateItem2 = _context.HotelRequestHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel1 == approverEmpcCode && o.HotelRequestId == hId).ToList();
                foreach (var p in updateItem2)
                {
                    p.ApprovalDateLevel1 = DateTime.Now;
                    p.ApprovalStatusLevel1 = 1;
                }

                var updateItem3 = _context.HotelRequestHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel2 == approverEmpcCode && o.HotelRequestId == hId).ToList();
                foreach (var p in updateItem3)
                {
                    if (p.ApproverEmpCodeLevel1 == approverEmpcCode && p.ApprovalStatusLevel1 == 1)
                    {
                        return s = -1;
                    }
                    else if (p.ApproverEmpCodeLevel2 == approverEmpcCode && p.ApprovalStatusLevel2 == 1)
                    {
                        return s = -1;
                    }
                    else
                    {
                        p.ApprovalDateLevel2 = DateTime.Now;
                        p.ApprovalStatusLevel2 = 1;
                    }
                }

                var updateItem4 = _context.HotelRequestHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hId).ToList();
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
        /// Reject Non-Contractual Hotel Request by hod
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="approverEmpcCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int RejectNonContractualHotelRequest(string travelReqID, string hid, string approverEmpcCode, string type)
        {
            var s = 0;
            var IdList = travelReqID.Split(',');
            var hidList = hid.Split(',');
            for (var i = 0; i < IdList.Length - 1; i++)
            {
                var trid = Convert.ToInt64(IdList[i]);
                var hID = Convert.ToInt32(hidList[i]);
                var findIfRejected = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hID).ToList();
                foreach (var f in findIfRejected)
                {
                    if (f.HodApprovalStatus == "2")
                    {
                        return s = -1;
                    }
                }

                var findIfApproved = _context.HotelRequestHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hID).ToList();
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
                var updateItem = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hID).ToList();
                foreach (var p in updateItem)
                {
                    p.HodApprovalStatus = "2";
                }

                var updateItem2 = _context.HotelRequestHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel1 == approverEmpcCode && o.HotelRequestId == hID).ToList();
                foreach (var p in updateItem2)
                {
                    p.ApprovalDateLevel1 = DateTime.Now;
                    p.ApprovalStatusLevel1 = 2;
                }

                var updateItem3 = _context.HotelRequestHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel2 == approverEmpcCode && o.HotelRequestId == hID).ToList();
                foreach (var p in updateItem3)
                {
                    p.ApprovalDateLevel2 = DateTime.Now;
                    p.ApprovalStatusLevel2 = 2;
                }

                var updateItem4 = _context.HotelRequestHODFinancialApprovalModel.Where(o => o.TravelRequestId == trid && o.HotelRequestId == hID).ToList();
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
        /// Approve Non-Contractual Hotel Request by hod (OAT)
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="approverEmpcCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int ApproveNonContractualHotelRequestOat(string travelReqID, string approverEmpcCode, string type)
        {
            var s = 0;
            var IdList = travelReqID.Split(',');
            for (var i = 0; i < IdList.Length - 1; i++)
            {
                var trid = Convert.ToInt64(IdList[i]);
                var findIfRejected = _context.OALHotelModel.Where(o => o.TravelRequestId == trid).ToList();
                foreach (var f in findIfRejected)
                {
                    if (f.HodApprovalStatus == "2")
                    {
                        return s;
                    }
                }

                var updateItem2 = _context.HotelRequestHODFinancialApprovalOatModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel1 == approverEmpcCode).ToList();
                foreach (var p in updateItem2)
                {
                    p.ApprovalDateLevel1 = DateTime.Now;
                    p.ApprovalStatusLevel1 = 1;
                }

                var updateItem3 = _context.HotelRequestHODFinancialApprovalOatModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel2 == approverEmpcCode).ToList();
                foreach (var p in updateItem3)
                {
                    p.ApprovalDateLevel2 = DateTime.Now;
                    p.ApprovalStatusLevel2 = 1;
                }

                var updateItem4 = _context.HotelRequestHODFinancialApprovalOatModel.Where(o => o.TravelRequestId == trid).ToList();
                foreach (var p in updateItem4)
                {
                    var updateItem = _context.OALHotelModel.Where(o => o.TravelRequestId == trid).ToList();
                    if (p.ApprovalStatusLevel1 == 1 && p.ApprovalStatusLevel2 == 1)
                    {
                        p.ApprovalStatus = 1;
                        foreach (var u in updateItem)
                        {
                            u.HodApprovalStatus = "1";
                        }
                    }
                    else
                    {
                        foreach (var u in updateItem)
                        {
                            u.HodApprovalStatus = "0";
                        }
                    }
                }
            }
            s = _context.SaveChanges();
            return s;

        }

        /// <summary>
        /// Reject Non-Contractual Hotel Request by hod (OAT)
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="approverEmpcCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int RejectNonContractualHotelRequestOat(string travelReqID, string approverEmpcCode, string type)
        {
            var s = 0;
            var IdList = travelReqID.Split(',');
            for (var i = 0; i < IdList.Length - 1; i++)
            {
                var trid = Convert.ToInt64(IdList[i]);
                var findIfApproved = _context.HotelRequestHODFinancialApprovalOatModel.Where(o => o.TravelRequestId == trid).ToList();
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

                var updateItem = _context.OALHotelModel.Where(o => o.TravelRequestId == trid).ToList();
                foreach (var p in updateItem)
                {
                    p.HodApprovalStatus = "2";
                }

                var updateItem2 = _context.HotelRequestHODFinancialApprovalOatModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel1 == approverEmpcCode).ToList();
                foreach (var p in updateItem2)
                {
                    p.ApprovalDateLevel1 = DateTime.Now;
                    p.ApprovalStatusLevel1 = 2;
                }

                var updateItem3 = _context.HotelRequestHODFinancialApprovalOatModel.Where(o => o.TravelRequestId == trid && o.ApproverEmpCodeLevel2 == approverEmpcCode).ToList();
                foreach (var p in updateItem3)
                {
                    p.ApprovalDateLevel2 = DateTime.Now;
                    p.ApprovalStatusLevel2 = 2;
                }

                var updateItem4 = _context.HotelRequestHODFinancialApprovalOatModel.Where(o => o.TravelRequestId == trid).ToList();
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
        /// Update check-in and check-out time by hotel
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="type"></param>
        /// <param name="checkin"></param>
        /// <param name="checkout"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        public int UpdateCheckinCheckout(string clubId, string type, string checkin, string checkout, string hotelname)
        {
            var s = 0;
            var cId = Convert.ToInt32(clubId);
            if (type == "SOD")
            {
                var updateItem = _context.TravelRequestHotelDetailModel.Where(b => b.clubId == cId).ToList();
                foreach (var p in updateItem)
                {
                    p.CheckinTimeHotel = checkin;
                    p.CheckoutTimeHotel = checkout;
                }
                s = _context.SaveChanges();
            }
            else
            {
                var updateItem = _context.OALHotelModel.Where(b => b.clubId == cId).ToList();
                foreach (var p in updateItem)
                {
                    p.CheckinTime = checkin;
                    p.CheckoutTime = checkout;
                }
                s = _context.SaveChanges();
            }
            return s;
        }


        /// <summary>
        /// Update check-in and check-out time by user
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="type"></param>
        /// <param name="checkin"></param>
        /// <param name="checkout"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        public string UpdateUserCheckinCheckout(string trid, string type, string checkin, string checkout, string hid)
        {
            var s = 0;
            var travelRqId = Convert.ToInt64(trid);
            var hId = Convert.ToInt32(hid);
            if (type == "SOD")
            {
                var updateItem = _context.TravelRequestHotelDetailModel.Where(b => b.TravelRequestId == travelRqId && b.HotelRequestId == hId).ToList();
                foreach (var p in updateItem)
                {
                    if (p.UserCheckinCheckoutUpdate == false)
                    {
                        if (p.IsCancelled != 1)
                        {
                            p.CheckinTime = checkin;
                            p.CheckoutTime = checkout;
                            p.UserCheckinCheckoutUpdate = true;
                        }
                        else
                        {
                            return "Cancelled";
                        }
                    }
                    else
                    {
                        return "AlreadyUpdated";
                    }
                }
                s = _context.SaveChanges();
            }
            else
            {

            }
            return "Saved";
        }



        /// <summary>
        /// Object Dispose
        /// </summary>
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
