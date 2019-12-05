
﻿using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Interface
{
    public interface IOATrepository : IDisposable
    {
        /// <summary>
        /// get userdeatil for sending message
        /// </summary>
        /// <param name="oatRequestId"></param>
        /// <returns></returns>
        OATTravelRequestMasterModal GetUserDetail(Int64 oatRequestId);

        /// <summary>
        /// To submit OAT  booking Request 
        /// </summary>
        /// <param name="personalInfo"></param>
        /// <param name="passangerInfo"></param>
        /// <param name="flightInfo"></param>
        /// <returns></returns>
        Int64 submitDataForsendingToIth(List<OATTravelRequestMasterModal> personalInfo, List<OATTravelRequestPassengerDetailModal> passangerInfo, List<OATTravelRequestFlightDetailModal> flightInfo);

        /// <summary>
        /// To submit Flight Detail From History Page
        /// </summary>
        /// <param name="flightInfo"></param>
        /// <returns></returns>
        Int64 submitDataForOnlyFlight(List<OATTravelRequestFlightDetailModal> flightInfo);

        /// <summary>
        /// To update TravelRequestId On OaTRequestMaster 
        /// </summary>
        /// <param name="trnID"></param>
        /// <returns></returns>
        int UpdateTrnIdOnOatMaster(Int64 trnID,Int64 oatReqNo);

        /// <summary>
        /// To update oatTravelRequestPassengerDetail for requesting hotel or flight from history Page
        /// </summary>
        /// <param name="OatReqID"></param>
        /// <param name="PassID"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        int UpdatePassengerModal(Int64 OatReqID, Int64 PassID, int criteria);

        /// <summary>
        /// To get all IOTA List
        /// </summary>
        /// <param name="ITACode"></param>
        /// <returns></returns>
        List<ITACodeMasterModel> fetchIOTAList( string LikeITACode,string unlikeIotaCode,string IsDomestic);

        /// <summary>
        /// To get all Pax Detail
        /// </summary>
        /// <returns></returns>
        List<OATPAXTyepeMasterModal> GetPaxType();

        /// <summary>
        /// get BookingRight Of Logged Employee
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        List<OATBookingRightModal> getOATBookingRight( String EmpCode);

        List<OATTravelRequestMasterModal> getOatMasterlist();

        /// <summary>
        /// TO get all booking rquest as per EMPID
        /// </summary>
        /// <param name="empCode"></param>
        /// <returns></returns>
        List<OATTravelRequestMasterModal> getOatMasterlist(String empCode);

        /// <summary>
        /// To get all  detail of Passenger , Flight, Hotel   as per OatReqId For History On one List Modal
        /// </summary>
        /// <param name="oatReqId"></param>
        /// <param name="trreqId"></param>
        /// <param name="bookingfor"></param>
        /// <returns></returns>
        List<viewOatDetailsModal>  getselecteddetails(Int64 oatReqId, Int64 trreqId, String bookingfor);

        int updateFlightDetail(OATTravelRequestFlightDetailModal Detail);

        /// <summary>
        /// To get detail of all type of Detail as per Oat ReuestId
        /// </summary>
        /// <param name="oatReqID"></param>
        /// <returns></returns>
        Dictionary<string, object> getOatDetailsPerReqId(long oatReqID);

        /// <summary>
        /// to get all Ith List 
        /// </summary>
        /// <returns></returns>
        List<String> getIthListName();

        /// <summary>
        /// To get all detail Of Ith as per name
        /// </summary>
        /// <param name="ithName"></param>
        /// <returns></returns>
        List<ITHVenderModal> getIthDetailPerName( String ithName);

        /// <summary>
        ///  To get View Hotel Detail as per TrId For User History
        /// </summary>
        /// <param name="trId"></param>
        /// <returns></returns>
        List<viewOatHotelDetailsModal> getViewOatHotelDetail( String trId);

        /// <summary>
        /// To get View Flight Detail as per OAtReqId and Id For User History
        /// </summary>
        /// <param name="oatReqID"></param>
        /// <param name="passengerID"></param>
        /// <returns></returns>
        List<OATTravelRequestFlightDetailModal> getViewOatFlightDetail(Int64 oatReqID, Int64 ID);

        string CheckOnlyflightBooking(List<OATTravelRequestFlightDetailModal> flightInfo);
        /// <summary>
        /// get passenger Detail according to PassengerID
        /// </summary>
        /// <param name="oatReqID"></param>
        /// <param name="PassengerID"></param>
        /// <returns></returns>
        List<OATTravelRequestPassengerDetailModal> getPassengerDetail(Int64 oatReqID, Int64 PassengerID);

        /// <summary>
        /// To send Req To Ith Of OAT Booking on OAT Desk
        /// </summary>
        /// <param name="modal"></param>
        /// <param name="transactionDetailList"></param>
        /// <returns></returns>
        /// 
        Int64 saveIthReqDetail(ITHTransactionMasterModal  modal, List<ITHTransactionDetailModal> transactionDetailList);

        /// <summary>
        /// To get all List in their Modal Format as Per OatReqId
        /// </summary>
        /// <param name="OatReqID"></param>
        /// <returns></returns>
        Dictionary<string, object> getIthDetailAsPerReqID(long OatReqID);

        /// <summary>
        /// To get all List in their Modal Format as Per OatReqId and TransactionId
        /// </summary>
        /// <param name="OatReqId"></param>
        /// <param name="ithTransactionID"></param>
        /// <returns></returns>
        Dictionary<string, object>  GetDetailForIthResponse(Int64 OatReqId, Int64 ithTransactionID ,string type);

        /// <summary>
        /// To submit Ith Detail Response
        /// </summary>
        /// <param name="newIthDetail"></param>
        /// <returns></returns>
        int SubmitIthDetailResponse(List<ITHTransactionDetailModal> newIthDetail);

        /// <summary>
        /// to submit booked itenary
        /// </summary>
        /// <param name="UploadedItenary"></param>
        /// <returns></returns>
        int SubmitItenaryListOfBooking(List<OATUploadItenaryModal> UploadedItenary);

       

        /// <summary>
        /// To get image as base64 string as per ID
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        string viewIthAttachedSrc(long Id, int criteria);

        /// <summary>
        /// To Confirm Ith Resource 
        /// </summary>
        /// <param name="SelectedResponse"></param>
        /// <returns></returns>
        int ConfirmIThResponse(List<ITHTransactionDetailModal> SelectedResponse);

        /// <summary>
        /// To Revert all the Ith Response by oat desk 
        /// </summary>
        /// <param name="revertToIthSrc"></param>
        /// <returns></returns>
        int RevertToITh(ITHTransactionDetailLogModal revertToIthSrc);

        ///// <summary>
        /////  To getEMpId As per Req ID
        ///// </summary>
        ///// <param name="oatReqID"></param>
        ///// <returns></returns>
        //int getEmpIdAsPerReqId(Int64 oatReqID);

        /// <summary>
        /// To Submit Hod Response 
        /// </summary>
        /// <param name="SelectedResponse"></param>
        /// <returns></returns>
        int SubmitHodResponse(List<ITHTransactionDetailModal> SelectedResponse);

        /// <summary>
        /// To Submit User Response 
        /// </summary>
        /// <param name="SelectedResponse"></param>
        /// <returns></returns>
        int SubmitUserResponse(List<ITHTransactionDetailModal> SelectedResponse);


        /// <summary>
        /// To Send hold request to all booking of OatreqId 
        /// </summary>
        /// <param name="oatReqId"></param>
        /// <returns></returns>
        int sendHoldRequestToIth(Int64 oatReqId,string holdBy);

        int undoHoldRequestToIth(Int64 oatReqId ,string holdBy);
        

        /// <summary>
        /// To Send cancel request to all booking of OatreqId 
        /// </summary>
        /// <param name="oatReqId"></param>
        /// <returns></returns>
        int cancelFlightReqAsPerReqId(Int64 oatReqId);

        /// <summary>
        /// To Send cancel request to all booking of flightId 
        /// </summary>
        /// <param name="oatReqId"></param>
        /// <returns></returns>
        Int64 cancelFlightReq(OATTravelRequestFlightDetailModal cancellationsrc, int criteria);

        /// <summary>
        /// Check status of IthResponse it send response or not
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        String checkstatusOfOthBookingResponse(string types , Int64 oatreqId);

        /// <summary>
        /// to check req sed to ITh or not
        /// </summary>
        /// <param name="oatReqId"></param>
        /// <returns></returns>
        bool ISReqSentToIth(Int64 oatReqId);

        /// <summary>
        /// To reject response Of Ith Via ITH or Hod or User
        /// </summary>
        /// <param name="types"></param>
        /// <param name="oatreqId"></param>
        /// <returns></returns>
        String RejectIthResponse(string types, Int64 oatreqId);

        /// <summary>
        /// accept response without any modification
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        List<ITHTransactionDetailModal> acceptReponseByHodOrUserAsPre (string types, Int64 oatReqId);

        Dictionary<string, object> GetDetailForIthResponse(Int64 oatReqId);       

        string getBookingType(Int64 oatReqId);
        string getClevelApproverMail(Int64 oatReqId);

        bool getAmountForFinancialApproval(Int64 oatReqId);
        int saveFinacialDetailForApproval(List<OATFinancialApprovalDetail_RoisteringModal> ListOfFinancialApproval);
        OATFinancialApprovalDetail_RoisteringModal GetFinacialDetailForApproval(Int64 oatReqId);
        List<OATFinancialApprovalMaster_RoisteringModal> getFinacialMasterDetail(Int64 oatReqId);

        String ApproveOrRejectFinancialApproval(string types, Int64 OATReqId);

        String SubmitCancelAcknwledgement(string types, Int64 oatreqId, int criteria);
        String SubmitHoldAcknwledgement(string types, Int64 oatreqId);
        String SubmitCLevelAcknowledgemnt(string types, Int64 oatreqId);

        int UpdateClevelNotification(Int64 oatReqId);
        
        String GetManageNoShowDetails(ITHTransactionDetailModal mangeShowList,DateTime date);

        List<OATFlightNoShowReportModal> getFlightNoShowList();

        List<OATFinancialApprovalDetail_RoisteringModal> GetListToSendFinancialApprover();

        /// <summary>
        /// getDataFortrigger mail to user
        /// </summary>
        /// <returns></returns>
        List<OATUploadItenaryModal> GetListOfAllBookedOATFlightDetail();
        List<OATTravelRequestFlightDetailModal> GetListOfRoisteringBooking(List<OATTravelRequestFlightDetailModal> flightInfo);
    }
}
