using System;
using System.Collections.Generic;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface ISodApproverRepositorty : IDisposable
    {
        /// <summary>
        /// Get Sod Approver Status
        /// </summary>
        /// <param name="sodApproverModels"></param>
        /// <returns></returns>
        bool GetSodApprovalStatus(SodApproverModels sodApproverModels);


        /// <summary>
        /// Get Assigned Sod Approver List
        /// </summary>
        /// <returns></returns>
        IEnumerable<SodApproverModels> GetSodApprovelList();


        /// <summary>
        /// Save Sod Role
        /// </summary>
        /// <param name="sodApproverModels"></param>
        /// <returns></returns>
        int Save(SodApproverModels sodApproverModels);


        /// <summary>
        /// Get Sod List for Approval
        /// </summary>
        /// <returns></returns>
        IList<TravelRequestModels> GetSodBookingListForApproval(int departmentId, int designationId, int EmpId, int criteria, string hodEmailId);



        /// <summary>
        /// Get Sod Employee Booking History By Employee Code wise
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TravelRequestModels> GetSodEmployeeBookingHistoryList_ByEmployeeCode(string empcode, int criteria, bool IsVendorBooking, string fdate, string tdate, string Dept);

        IList<TravelRequestMasterModels> GetSodApproverBookingHistoryList_ByEmployeeCode(string empcode, int criteria, bool IsVendorBooking, string fdate, string tdate, string Dept);

        IList<TravelRequestModels> GetSodEmployeeBookingHistoryListbydate(DateTime fromdate, DateTime todate, Int32 EmpId, int criteria);

        /// <summary>
        /// Save and Approve Travel Booking request by Approver
        /// This method will also used for approve and reject case
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        int ApproveSodBookingRequest(TravelRequestApprovalModels travelRequestApprovalModels);

        /// <summary>
        /// Update Hotel Approval Status
        /// </summary>
        /// <param name="travelrequestid"></param>
        void UpdateHotelApprovalStatus(long travelrequestid);

        /// <summary>
        /// Update Only Hotel Approval Status
        /// </summary>
        /// <param name="travelrequestid"></param>
        void UpdateOnlyHotelApprovalStatus(long travelrequestid);

        /// <summary>
        /// Save and Reject Travel Booking request by Approver
        /// This method will also used for approve and reject case
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        int RejectSodBookingRequest(TravelRequestApprovalModels travelRequestApprovalModels);

        /// <summary>
        /// To check status of Hod 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        int checkApprovalStatusOfHod(int travelReqId);


        /// <summary>
        /// Get booking info for PNR Generation
        /// Booking Requester Info,Flight info,Passenger Info
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetSodBookingInfoForPNR(Int64 travelReqId);


        /// <summary>
        /// Update PNR
        /// </summary>
        /// <param name="pnr"></param>
        /// <param name="requestid"></param>
        /// <returns></returns>
        int UpdatePnr(string pnr, Int64 requestid);


        /// <summary>
        /// find hotel approval status
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        bool FindHotelApprovalStatus(long trid);

        /// <summary>
        /// Get PNR Amount and Time
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        string GetPNRAmountAndTime(Int64 requestid);

        /// <summary>
        /// Get Sod Employee Booking History List
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TravelRequestModels> GetSodEmployeeBookingHistoryList(int? departmentId, int? EmpId, int criteria);


        /// <summary>
        /// Get Sod Employee Booking Status
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<string> GetEmployeeBookingStatus(int reqId, int criteria);


        /// <summary>
        /// Update Booking Request :CXO Approval Level -1
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        int UpdateSodBookingRequestCXO_Level1(TravelRequestApprovalModels trApprovalModels);


        /// <summary>
        /// Update Booking Request :CXO Approval Level -2
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        int UpdateSodBookingRequestCXO_Level2(TravelRequestApprovalModels trApprovalModels);



        /// <summary>
        /// Check CXO-Level 1 Approval Status
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        bool CheckSodApprovalStatusCXO_Level1(Int64 requestid);


        /// <summary>
        /// Check CXO-Level 2 Approval Status
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        bool CheckSodApprovalStatusCXO_Level2(Int64 requestid);


        /// <summary>
        /// Check CXO Priority Level  When Logged In
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        Int16 CheckCXOApprover_Priority(int designationId, int departmentId, Int64 trRequestId);


        /// <summary>
        /// Update Booking Request :Revenue Approval 
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        int UpdateSodBookingRequest_Revenue(TravelRequestApprovalModels trApprovalModels);


        /// <summary>
        /// To Save :Reject Booking Request from Revenue
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        int RejectSodBookingRequest_Revenue(TravelRequestApprovalModels trApprovalModels);

        /// <summary>
        /// Close Booking Request After Rejection of the Request from HOD 
        /// Stand By Case
        /// </summary>
        /// <param name="trRequest"></param>
        /// <returns></returns>
        int CloseSodBookingRequest_HOD(TravelRequestMasterModels trRequest);


        /// <summary>
        /// To Get Priority L1 and L2 Status
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        bool CheckSodApprovalStatusCXO_LevelL1L2(Int64 requestid);



        /// <summary>
        /// Get CXO Email ID
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        string GetCXOMailId(string cl1, string cl2);


        /// <summary>
        /// Check CXO Priority Level  when approbed vio email
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        Int16 CheckCXOApprover_PriorityByEmpId(int EmpId, Int64 trRequestId);


        /// <summary>
        /// Rollback approval status by HOD
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        int RollBackApprovalByHOD(TravelRequestApprovalModels trApprovalModels);


        /// <summary>
        /// Rollback approval status by CXO1
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        int RollBackApprovalByCXO_Level1(TravelRequestApprovalModels trApprovalModels);



        /// <summary>
        /// Rollback approval status by CXO2
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        int RollBackApprovalByCXO_Level2(TravelRequestApprovalModels trApprovalModels);


        /// <summary>
        /// Get Comment from Revenue Dept.
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        string GetRevenueComment(Int64 reqId);



        /// <summary>
        /// Check Revenue Approval Status : YES or NO
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        string CheckRevenueApprovalStatus(Int64 reqId);


        /// <summary>
        /// Checking for User CXO Role
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        bool IsCXORole(Int32 EmpId);


        /// <summary>
        /// Checking for User HOD/Vertical Head Role
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        bool IsHODRole(Int32 EmpId, string verticalId, int deptId);

        /// <summary>
        /// Update Only Hotel Approval Status
        /// </summary>
        /// <param name="travelrequestid"></param>
        bool UpdateOnlyHotelRejectionStatus(long travelrequestid);

        /// <summary>
        /// To find hotel Rejection
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        bool FindHotelRejectionStatus(long trid);
        /// <summary>
        /// to approve sod booking request
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        int ApproveOnlyHotelSodBookingRequest(TravelRequestApprovalModels travelRequestApprovalModels);

        /// <summary>
        /// Save Save Reject with StandBy PNR from Revenue
        /// If Say "No & StandBy"
        /// </summary>
        /// <param name="objRevenueRejectModel"></param>
        /// <param name="strPrm"></param>
        /// <returns></returns>
        bool SaveRejectwithStandByPNR_Revenue(TravelRequestMasterModels_RejectwithStandByPNR objRevenueRejectModel);
    }
}