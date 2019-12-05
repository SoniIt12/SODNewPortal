using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;


namespace SOD.Services.Interface
{
    public interface ITransportRepository:IDisposable
    {

        /// <summary>
        /// Get Sod Booking and Cab Info 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetSodBookingandCabInfo(Int64 travelReqId);


        /// <summary>
        /// Get Vendor List
        /// </summary>
        /// <param name="VendorCode"></param>
        /// <returns></returns>
        List<TransportVendorMasterModels> GetVendorList(string VendorCode);


        /// <summary>
        /// Get Sod Booking Request with Cab Request :TravelDesk
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Dictionary<string, object> GetSodEmployeeBookingHistoryList_TravelDesk(int? departmentId, int? EmpId, int criteria);

       /// <summary>
       /// 
       /// </summary>
       /// <param name="departmentId"></param>
       /// <param name="EmpId"></param>
       /// <param name="criteria"></param>
       /// <returns></returns>
        Dictionary<string, object> GetSodEmployeeHotelList_TravelDesk(int? departmentId, int? EmpId, int criteria);


        /// <summary>
        /// Get Sod Cab Booking Status
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<string> GetCabStatus(int reqId, int criteria);


        /// <summary>
        /// Approve cab Request 
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        int ApprovedCabRequest(Int64 reqId,string strRemarks);


        /// <summary>
        /// Reject Cab Request with Remarks
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        int RejectCabRequest(Int64 reqId, string strRemarks);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<string> GetHotelStatus(int reqId, int hId, int criteria);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetSodBookingandHotelInfo(long travelReqId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        int ApprovedHotelRequest(List<TravelRequestHotelDetailModels> elist);

        int ApprovedHotelRequestOat(List<OALHotelModel> elist);

        /// <summary>
        /// Reject Hotel Request
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        int RejectHotelRequest(Int64 reqId, string strRemarks);



        int RejectHotelRequestOat(Int64 reqId, string strRemarks);
         

        /// <summary>
        /// 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetSodHotelInfo(long travelReqId, int hId);

        Dictionary<string, object> GetSodHotelInfoOat(long travelReqId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Dictionary<string, object> hotelfilldropdown(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetHotelDetailbyTrID(long travelReqId, int hId);

        Dictionary<string, object> GetHotelDetailbyTrIDOat(long travelReqId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        Dictionary<string, object> GetHotelandUserInfo(List<String> requestList, List<String> hidList, string hotelname, string sodOat);


        /// <summary>
        /// get hotel and user details for responsive mail
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        Dictionary<string, object> GetHotelandUserInfoForResponsiveMail(List<String> requestList, List<String> hidList, string hotelname, string sodOat);

        /// <summary>
        /// 
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
        /// <returns></returns>
        int SaveHotelRequest(List<HotelRequestApprovalModel> elist, string hotelprice, string occupancy, string sodOat);

       
        /// <summary>
        /// Cancel Hotel Request
        /// </summary>
        /// <param name="trid"></param>
        /// <param name="reason"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int CancelHotelRequest(Int64 trid, int hid, string reason, string status, string type);

        /// <summary>
        /// Send cancellation request to hotel
        /// </summary>
        /// <param name="travReqstId"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        int SaveCancellationRequest(string travReqstId, int hid, string sodOat);

        int SaveCancellationRequestUser(string travReqstId, int hid, string sodOat);


        /// <summary>
        /// Get details of approval from hotel by travel request id 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        Dictionary<string, object> getApprovalHotelDetails(string travelReqId, int hId, string sodOat);

        /// <summary>
        /// get requests not approved by hod- exceptional cases
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> GetHotelInfoExceptional(string trid, int criteria);


        /// <summary>
        /// get hotel inclusion data
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        List<HotelInclusionMasterModels> findHotelInclusions(string hotelcity, string hotelname);

        /// <summary>
        /// get approver email id's for non contractual hotel approval
        /// </summary>
        /// <returns></returns>
        List<NonContractualHotelApprovalMasterModels> GetApproverIds();

        /// <summary>
        /// save approval request for non-contractual by HOD from traveldesk
        /// </summary>
        /// <param name="elist"></param>
        /// <param name="hodemail1"></param>
        /// <param name="hodemail2"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        int SaveHODFinancialApprovalRequest(List<HotelRequestApprovalModel> elist, string hodemail1, string hodemail2, List<HotelInclusionNonContractualMasterModels> inclist, string sodOat);

        /// <summary>
        /// Get Hod Approver Name for Hotels
        /// </summary>
        /// <param name="hodemail"></param>
        /// <returns></returns>
        List<NonContractualHotelApprovalMasterModels> GetHodApproverNameHotels(string hodemail);

        /// <summary>
        /// view detail for hod approval status
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        List<HotelRequestHODFinancialApprovalModels> GetdetailHODApproval(Int64 trid, int hId, string sodoat);

        /// <summary>
        /// update hotel status after request sent
        /// </summary>
        /// <param name="elist"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        int UpdateHotelStatus(List<HotelRequestApprovalModel> elist, string sodOat);

        /// <summary>
        /// Find City Code by city name
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        List<SodCityCodeMasterModels> FindCityCode(string hotelcity);

        /// <summary>
        /// get city name by city code
        /// </summary>
        /// <param name="citycode"></param>
        /// <returns></returns>
        List<SodCityCodeMasterModels> FindCityName(string citycode); 

        /// <summary>
        /// find hotel inclusions of non contractual hotel
        /// </summary>
        /// <param name="travelreqid"></param>
        /// <returns></returns>
        List<HotelInclusionNonContractualMasterModels> FindNonContractualHotelInclusions(Int64 travelreqid, int HotelRequestId);

        /// <summary>
        /// find hotel inclusions of non contractual hotel oat
        /// </summary>
        /// <param name="travelreqid"></param>
        /// <returns></returns>
        List<HotelInclusionNonContractualMasterOatModels> FindNonContractualHotelInclusionsOat(Int64 travelreqid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="empcode"></param>
        /// <returns></returns>
        string GetFinancialHodDetailsEmail(string empcode);

        /// <summary>
        /// find hotel data which is similar to already allocated person
        /// </summary>
        /// <param name="hotelname"></param>
        /// <param name="checkin"></param>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        List<TravelRequestHotelDetailModels> FindSimilarHotelAllocationData(Int64 TravelRequestId,string hotelname, DateTime checkin, string hotelcity);
        
        /// <summary>
        /// Get Sod Booking Request with Hotel Request :TravelDesk- for double occupancy (sorted)
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Dictionary<string, object> GetSodEmployeeHotelList_TravelDesk_DoubleOccupancy(string newTrid, string existingTrid, string hotelrqstid, string exishotelid);

        /// <summary>
        /// find if club id already exists or not
        /// </summary>
        /// <param name="elist"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        int FindIfClubIdExists(List<HotelRequestApprovalModel> elist, string sodOat);

        /// <summary>
        /// Auto trigger mail to users after confirmation by hotel
        /// </summary>
        /// <param name="clubid"></param>
        /// <returns></returns>
        List<TravelRequestHotelDetailModels> SendMailToUsers(string clubid);

        /// <summary>
        /// get requests with for checkin-checkout time confirmation by user & hotel
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> GetHotelDetailCheckTimeConflict();

        /// <summary>
        /// Find Duplicate Hotel Data
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="checkin"></param>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        bool FindDuplicateDataHotel(string empcode, DateTime checkin, string hotelcity);

        /// <summary>
        /// get hotel detals by travel request id for exceptional cases
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetHotelInfoApprovalbyTrID(long travelReqId);

        /// <summary>
        /// get details by club id
        /// </summary>
        /// <param name="clubid"></param>
        /// <returns></returns>
        List<TravelRequestHotelDetailModels> GetDetailsByClubId(string clubid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelcode"></param>
        /// <returns></returns>
        string GetCityCodeOfHotel(string hotelcode);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="citycode"></param>
        /// <returns></returns>
        List<HotelGstDetailModels> GetGstDetailsByCityCode(string citycode);

        /// <summary>
        /// find existing city code
        /// </summary>
        /// <param name="citycode"></param>
        /// <returns></returns>
        List<SodCityCodeMasterModels> checkCityCodeExist(string citycode);

        /// <summary>
        /// update hotel dates by user
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="hotelid"></param>
        /// <param name="checkin"></param>
        /// <param name="checkout"></param>
        /// <returns></returns>
        int updateHotelDatesByUser(string TravelRequestId, string hotelid, DateTime checkin, DateTime checkout);

        /// <summary>
        /// Undo cancellation request
        /// </summary>
        /// <param name="trid"></param>
        /// <param name="hid"></param>
        /// <param name="reason"></param>
        /// <param name="status"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        int UndoCancelledRequest(Int64 trid, int hid, string status, string type);

         /// <summary>
        /// Save SMS Log after the HOD approval
        /// </summary>
        /// <param name="smsLogModel"></param>
        /// <returns></returns>
        int SaveApproverSMSLog(SodApproverSMSLogModels smsLogModel);
    }
}
