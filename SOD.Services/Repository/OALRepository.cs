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
    public class OALRepository : IOALRepository
    {
       
        #region "Constructor Initialization"
        /// <summary>
        /// Constructor Initilization
        /// </summary>
        private readonly SodEntities _context;
        public OALRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
        #endregion
        
        /// <summary>
        /// Save OAL details
        /// </summary>
        /// <param name="flightList"></param>
        /// <param name="passengerList"></param>
        /// <param name="hotelList"></param>
        /// <param name="cabList"></param>
        /// <param name="masterList"></param>
        /// <returns></returns>
        public Int64 SaveOALData(List<OALModels> oalList, List<OALPassengerModel> plist, List<OALHotelModel> hlist, List<OALCabModel> clist, List<OALTravelRequestMasterModel> mlist) 
        {
            Int64 requestMaxId = 0;
            using (System.Data.Entity.DbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    //Master List
                    _context.OALTravelRequestMasterModel.Add(mlist[0]);
                    _context.SaveChanges();

                    requestMaxId = _context.OALTravelRequestMasterModel.DefaultIfEmpty().Max(x => x == null ? 1 : x.TravelRequestId);

                    //Update Code for OAT Travel Request Info
                    var mlists = _context.OALTravelRequestMasterModel.Where(o => o.TravelRequestId == requestMaxId);
                    foreach (var list in mlists)
                    {
                        list.TravelRequestId = requestMaxId;
                        list.TravelRequestCode = "OAT-" + requestMaxId;
                    }

                    //For OAT Details
                    foreach (var list in oalList)
                    {
                        list.TravelRequestId = requestMaxId;
                    }

                    _context.OALModels.AddRange(oalList);

                    //For Passenger List
                    foreach (var list in plist)
                    {
                        list.TravelRequestId = requestMaxId;
                    }
                    _context.OALPassengerModel.AddRange(plist);

                    //For Hotel List
                    if (hlist != null)
                    {
                        foreach (var list in hlist)
                        {
                            list.TravelRequestId = requestMaxId;
                            list.HotelReferenceId = "SH" + requestMaxId;
                        }
                        _context.OALHotelModel.AddRange(hlist);
                    }

                    //For Cab Request List
                    if (clist != null)
                    {
                        foreach (var list in clist)
                        {
                            list.TravelRequestId = requestMaxId;
                        }
                        _context.OALCabModel.AddRange(clist);
                    }
                     _context.SaveChanges();

                    dbTran.Commit();
                }
                catch(Exception ex)
                {
                    //Rollback transaction if exception occurs
                    dbTran.Rollback();
                    requestMaxId = -1;
                    throw;
                }
            }
            return requestMaxId;
        }


        /// <summary>
        /// Get employee details from database
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public IList<EmployeeModel> GetEmployeeList(int EmpId)
        {
            var lstEmployee = ADO.SodCommonServices.GetEmployeeCommonDetails(EmpId);
            return lstEmployee.ToList();
        }


        /// <summary>
        /// Get OAT Data by EmpId
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetOatData(string EmpId, int criteria)
        {
            return ADO.SodCommonServices.GetOatData(EmpId, criteria);
        } 


        /// <summary>
        /// Dispose Object
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Maximum Travel Request Id
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        public int getMaxRequestId(List<OALModels> elist)
        {
            var requestMaxId = _context.OALTravelRequestMasterModel.DefaultIfEmpty().Max(x => x == null ? 1 : x.TravelRequestId);
            int s = Convert.ToInt32(requestMaxId) + 1;
            return s;
        }

        /// <summary>
        /// get oat data 
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetOATDetailsData(string TrId)
        {
            var dicList = new Dictionary<string, object>();

            var blist = new List<OALModels>();
            var plist = new List<OALPassengerModel>();
            var alist = new List<OALUploadModel>();
            var hlist = new List<OALHotelModel>();
            var mlist = new List<OALTravelRequestMasterModel>();

            var travelRequestId = TrId.Split('-');
            long trid = Convert.ToInt64(travelRequestId[1]);

                blist = _context.OALModels.Where(x => x.TravelRequestId == trid).ToList();
                plist = _context.OALPassengerModel.Where(x => x.TravelRequestId == trid).ToList();
                alist = _context.OALUploadModel.Where(x => x.TravelRequestId == trid).ToList();
                hlist = _context.OALHotelModel.Where(x => x.TravelRequestId == trid).ToList();
                mlist = _context.OALTravelRequestMasterModel.Where(x => x.TravelRequestId == trid).ToList();
                            
            dicList.Add("flightList", blist);
            dicList.Add("pasgList", plist);
            dicList.Add("pnrList", alist);
            dicList.Add("hotelList", hlist);
            dicList.Add("masterList", mlist);

            return dicList;
        }

        
        /// <summary>
        /// get login user info
        /// </summary>
        /// <param name="userLoginDal"></param>
        /// <returns></returns>
        public UserAccountModels GetLoginUserList(UserAccountModels userLoginDal)
        {
            var s = (from l in _context.SodUsersInfo
                     where l.UserName == userLoginDal.UserName && l.Password == userLoginDal.Password
                     orderby l.UserName
                     select new
                     {
                         l.DepartmentId,
                         l.DesignationId,
                         l.UserName,
                         l.EmpCode
                     }).ToList();


            var lst = new UserAccountModels();

            if (s.Count > 0)
            {
                lst = new UserAccountModels
                {
                    DepartmentId = s[0].DepartmentId,
                    DesignationId = s[0].DesignationId,
                    UserName = s[0].UserName,
                    EmpCode = s[0].EmpCode
                };
            }

            return lst;
        }


        /// <summary>
        /// get approved requests from hod for traveldesk
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetApprovedRequests(string EmpId, int criteria)
        {
            return ADO.SodCommonServices.GetApprovedRequests(EmpId, criteria); 
        }


        /// <summary>
        /// get oat requests for travel history
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetOatHistoryRequests(string EmpId, int criteria)
        {            
            return ADO.SodCommonServices.GetOatHistoryRequests(EmpId, criteria);
        }


        /// <summary>
        /// save oat data
        /// </summary>
        /// <param name="oatList"></param>
        /// <returns></returns>
        public int SaveUploadData(List<OALUploadModel> oatList)
        {
            _context.OALUploadModel.AddRange(oatList);
            var i = _context.SaveChanges();
            return i;
        }


        /// <summary>
        /// Close Booking Request After Rejection/Approval         
        /// </summary>
        /// <param name="trRequest"></param>
        /// <returns></returns>
        public int CloseOatBookingRequest(OALTravelRequestMasterModel trRequest)
        { 
            //Booking Request Master for Close Request
            var closeItem = _context.OALTravelRequestMasterModel.Where(o => o.TravelRequestId == trRequest.TravelRequestId).ToList();
            if (closeItem.Count() > 0)
            {
                foreach (var p in closeItem)
                {
                    p.BookingStatus = trRequest.BookingStatus;
                    p.StatusDate = trRequest.StatusDate;
                    p.Pnr = trRequest.Pnr;
                }
            }
            return _context.SaveChanges();
        }

        /// <summary>
        /// get oat booking info for pnr creation
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetOatBookingInfoForPNR(Int64 travelReqId)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var sodRequestsList = new List<OALTravelRequestMasterModel>();
            var sodflightList = new List<OALModels>();
            var passengerList = new List<OALPassengerModel>();
            var approvalList = new List<OatTravelRequestApprovalModel>();
            var hotelList= new List<OALHotelModel>();

            sodRequestsList = _context.OALTravelRequestMasterModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            sodflightList = _context.OALModels.Where(b => b.TravelRequestId == travelReqId).ToList();
            passengerList = _context.OALPassengerModel.Where(b => b.TravelRequestId == travelReqId).ToList();
            approvalList = _context.OatTravelRequestApprovalModel.Where(b => b.TravelRequestId == travelReqId && b.RevenueApprovedStatus == 2).ToList();
            hotelList = _context.OALHotelModel.Where(b => b.TravelRequestId == travelReqId).ToList();

            dicInfo.Add("bookingInfo", sodRequestsList);
            dicInfo.Add("flightInfo", sodflightList);
            dicInfo.Add("passInfo", passengerList);
            dicInfo.Add("approvalInfo", approvalList);
            dicInfo.Add("hotelInfo", hotelList);
            return dicInfo;
        }

        /// <summary>
        /// travel house details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Dictionary<string, object> ithfilldropdown(string name)
        {
            var _context = new SodEntities();
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var ithdetails = new List<IthVendorListDataModels>();
            ithdetails = _context.IthVendorListDataModel.Where(a => a.IthName == name).ToList();
            dicInfo.Add("ithList", ithdetails);
            return dicInfo;
        }

        /// <summary>
        /// save ITH request 
        /// </summary>
        /// <param name="oatList"></param>
        /// <returns></returns>
        public int SaveITHRequestData(List<ITHRequestApprovalModels> hlist)
        {
            var _context = new SodEntities();
            _context.ITHRequestApprovalModel.AddRange(hlist);
            var trid = hlist[0].TravelRequestId;
            var updateItem = _context.OALTravelRequestMasterModel.Where(b => b.TravelRequestId == trid).ToList();
            foreach (var p in updateItem)
            {
                p.ITHResponseStatus = "Response Pending from ITH";
            }
            var i = _context.SaveChanges();
            return i;
        }


        /// <summary>
        /// save ith response data
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public int SaveITHResponseData(List<ITHResponseDetailModels> ithList )
        {
            var _context = new SodEntities();
            _context.ITHResponseDetailModel.AddRange(ithList);
            var trid = ithList[0].TravelRequestId;
            var updateItem = _context.OALTravelRequestMasterModel.Where(b => b.TravelRequestId == trid).ToList();
            foreach (var p in updateItem)
            {
                p.ITHResponseStatus = "Response Received";
            }
            var i = _context.SaveChanges();
            return i;
        }

        /// <summary>
        /// get ith response data 
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        public Dictionary<string, object> viewITHStatusList(string TrId)
        {
            var dicList = new Dictionary<string, object>();
            var alist = new List<ITHResponseDetailModels>();
            var mlist = new List<OALTravelRequestMasterModel>();
            var travelRequestId = TrId.Split('-');
            long trid = Convert.ToInt64(travelRequestId[1]);
            alist = _context.ITHResponseDetailModel.OrderBy(n => n.OriginPlace).Where(x => x.TravelRequestId == trid).ToList();
            mlist = _context.OALTravelRequestMasterModel.Where(x => x.TravelRequestId == trid).ToList();
            dicList.Add("ithList", alist);
            dicList.Add("masterList", mlist);
            return dicList;
        }


        /// <summary>
        /// save hod status
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int saveHODStatus(long TravelRequestId, string status)
        {
            var _context = new SodEntities();
            var updateItem = _context.OALTravelRequestMasterModel.Where(b => b.TravelRequestId == TravelRequestId).ToList();
            foreach (var p in updateItem)
            {
                p.FinancialApproval = status;
            }
            var i = _context.SaveChanges();
            return i;
        }

        /// <summary>
        /// save hod response
        /// </summary>
        /// <param name="IdList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public int saveHODResponseData(string IdList, int criteria)
        {
            List<ITHResponseDetailModels> ithdetails = new List<ITHResponseDetailModels>();
            ithdetails = ADO.SodCommonServices.saveHODResponseData(IdList, criteria);

            List<ITHFinancialApprovalHODModels> hlist = new List<ITHFinancialApprovalHODModels>();
            var _context = new SodEntities();
            foreach (var item in ithdetails)
            {
                var lst = new ITHFinancialApprovalHODModels();
                lst.TravelRequestId = item.TravelRequestId;
                lst.ResponseDate = item.ResponseDate;
                lst.OriginPlace = item.OriginPlace;
                lst.DestinationPlace = item.DestinationPlace;
                lst.TravelDate = item.TravelDate;
                lst.FlightNo = item.FlightNo;
                lst.FlightName = item.FlightName;
                lst.DepartureTime = item.DepartureTime;
                lst.ArrivalTime = item.ArrivalTime;
                lst.Amount = item.Amount;
                lst.FlightType = item.FlightType;
                hlist.Add(lst);                
            }
            _context.ITHFinancialApprovalHODModel.AddRange(hlist);

            //save approval status in ith details table
            for (var j = 0; j < IdList.Split(',').Length - 1; j++)
            {
                var id = Convert.ToInt64(IdList.Split(',')[j]);
                var updateith = _context.ITHResponseDetailModel.Where(b => b.ID == id).ToList();
                foreach (var p in updateith)
                {
                    p.HodApproval = "Approved";
                }
            }
            //save approval status in master table
            var trid = ithdetails[0].TravelRequestId;
            var updateItem = _context.OALTravelRequestMasterModel.Where(b => b.TravelRequestId == trid).ToList();
            foreach (var p in updateItem)
            {
                p.FinancialApproval = "Approved";
            }
            var i = _context.SaveChanges();
            return i;
        }


        /// <summary>
        /// display hod financial approval response 
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        public Dictionary<string, object> viewHODResponse(string TrId)
        {
            var dicList = new Dictionary<string, object>();
            var alist = new List<ITHFinancialApprovalHODModels>();
            var travelRequestId = TrId.Split('-');
            long trid = Convert.ToInt64(travelRequestId[1]);
            alist = _context.ITHFinancialApprovalHODModel.OrderBy(n => n.OriginPlace).Where(x => x.TravelRequestId == trid).ToList();
            dicList.Add("hodResponseList", alist);
            return dicList;
        }
        

        /// <summary>
        /// Get Financially Approved Requests Data
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetFinanciallyApprovedRequests(string TrId)
        {
            var dicList = new Dictionary<string, object>();
            var flist = new List<ITHFinancialApprovalHODModels>();
            var plist = new List<OALPassengerModel>();
            var mlist = new List<OALTravelRequestMasterModel>();

            var travelRequestId = TrId.Split('-');
            long trid = Convert.ToInt64(travelRequestId[1]);

            flist = _context.ITHFinancialApprovalHODModel.Where(x => x.TravelRequestId == trid).ToList();
            plist = _context.OALPassengerModel.Where(x => x.TravelRequestId == trid).ToList();
            mlist = _context.OALTravelRequestMasterModel.Where(x => x.TravelRequestId == trid).ToList();

            dicList.Add("finApprovedList", flist);
            dicList.Add("pasgList", plist);
            dicList.Add("masterList", mlist);

            return dicList;
        }
        

    }
}
