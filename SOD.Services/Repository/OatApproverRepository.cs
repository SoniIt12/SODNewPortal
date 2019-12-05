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
    public class OatApproverRepository : IOatApproverRepository
    {
       
        #region "Constructor Initialization"
        /// <summary>
        /// Constructor Initilization
        /// </summary>
        private readonly SodEntities _context;
        public OatApproverRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
        #endregion

        public int ApproveOatBookingRequest(OatTravelRequestApprovalModel travelRequestApprovalModels)
        {
            var c = _context.OatTravelRequestApprovalModel.Where(o => o.TravelRequestId == travelRequestApprovalModels.TravelRequestId).ToList();
            if (c.Count()>0)
                return 1;

            _context.OatTravelRequestApprovalModel.Add(travelRequestApprovalModels);
                var s = _context.SaveChanges();
            
            return s;
        }


        /// <summary>
        /// Close Booking Request After Rejection of the Request from HOD 
        /// Stand By Case
        /// </summary>
        /// <param name="trRequest"></param>
        /// <returns></returns>
        public int CloseOatBookingRequest_HOD(OALTravelRequestMasterModel trRequest)
        { 
            //Booking Request Master for Close Request
            var closeItem = _context.OALTravelRequestMasterModel.Where(o => o.TravelRequestId == trRequest.TravelRequestId).ToList();
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
        /// Get SodBookingInfo For PNR
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
            var hotelList = new List<OALHotelModel>();

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
        /// To save Reject Booking Request
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        public int RejectOatBookingRequest(OatTravelRequestApprovalModel trApprovalModels)
        { 
            var s = 0;
            //For Travel Request Approval
            var updateItem = _context.OatTravelRequestApprovalModel.Where(o => o.TravelRequestId == trApprovalModels.TravelRequestId).ToList();
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
                _context.OatTravelRequestApprovalModel.Add(trApprovalModels);
            }
            return _context.SaveChanges();
        }


        /// <summary>
        /// Get Oat Booking List for Approval
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="designationId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<OALTravelRequestMasterModel> GetOatBookingListForApproval(int departmentId, int designationId, int EmpId, int criteria)
        {
            return ADO.SodCommonServices.GetOatBookingListForApproval(departmentId, designationId, EmpId, criteria);
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
