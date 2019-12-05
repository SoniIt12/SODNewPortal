using System;
using System.Collections.Generic;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface IHotelApproverRepository:IDisposable
    {
        int ApproveHotelBookingRequest(string clubId, string confirmationNo, string hotelname, string sodOat);

        int RejectHotelBookingRequest(string clubId, string hotelname, string sodOat); 

        Dictionary<string, object> GetInfoForPassenger(Int64 travelReqId, int hid, string sodOat);

        Dictionary<string, object> FindExistingTrId(string clubid, string types, string hotelname);

        int UpdateExistingCNo(string clubid, string types, string newconfirmNo, string oldconfirmNo);

        int ApproveHotelBookingRequestBulk(string clubId, string confirmationNo);

        int RejectHotelBookingRequestBulk(string clubId);

        int ApproveNonContractualHotelRequest(string travelReqID, string hid, string approverEmpcCode, string type);

        int RejectNonContractualHotelRequest(string travelReqID, string hid, string approverEmpcCode, string type);

        int ApproveNonContractualHotelRequestOat(string travelReqID, string approverEmpcCode, string type);

        int RejectNonContractualHotelRequestOat(string travelReqID, string approverEmpcCode, string type);

        int UpdateCheckinCheckout(string clubId, string type, string checkin, string checkout, string hotelname);

        string UpdateUserCheckinCheckout(string trid, string type, string checkin, string checkout, string hid);

    }
}
