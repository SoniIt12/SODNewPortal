using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Interface
{
    public interface IBulkUploadRepository : IDisposable
    {
        /// <summary>
        /// Save Bulk Upload data in Temp File
        /// </summary>
        /// <param name="bulkUploadModels"></param>
        /// <returns></returns>
        int SaveBulkUploadTemp(BulkUploadMasterModels bulkUploadMasterModels, List<BulkUploadModels> bulkUploadDetailsModels);

        /// <summary>
        /// Update Bulk Booking PNR Status
        /// </summary>
        /// <param name="s"></param>
        /// <param name="bulkUploadDetailsModels"></param>
        /// <returns></returns>
        int UpdatePNRStatusList(int s, List<BulkEmployeeList> bulkEmployeeList);


       /// <summary>
       /// Validate Employee Code
       /// </summary>
       /// <param name="empList"></param>
       /// <returns></returns>
        List<string> ValidateEmployeeCode(List<string> empList);


        /// <summary>
        /// Get Current Uploaded Excel File Data : After Generating PNR
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        List<BulkUpload_ExcelExport> GetCurrentBulkBookingData_ExportToExcel(int TrId);



        /// <summary>
        /// Get Bulk Booking MAster Data
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <returns></returns>
        List<BulkUploadMasterModels> GetBulkBookingMasterData(DateTime fromdate, DateTime todate, Int32 DepartmentId, string VerticalCode,string BookingType);


        /// <summary>
        /// Bulk Upload Details Data
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        List<BulkUploadModels> GetBulkBookinDetailsData(string TrId);

        /// <summary>
        /// Get hotel details for a travelRequestId
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        List<BulkUploadModels> GetHotelListPopup(string TrId);

        /// <summary>
        /// View traveldesk hotel status
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        int GetViewstatushotel(string TrId);


        /// <summary>
        /// approve hotel request by traveldesk
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        int ApprovedHotelRequest(string empcode, string TravelRequestId);

        /// <summary>
        /// get hotel details for user mail
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        List<BulkTravelRequestHotelDetailModels> GetHotelDetailForMail(string empcode, string TravelRequestId);
        
        
        /// <summary>
        /// Find hotelStatus
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        int ChangeHotelStatus(string empcode, string TravelRequestId);

        /// <summary>
        /// get user details for mail
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <returns></returns>
        List<BulkUploadModels> GetUserDetailForMail(string empcode, string TravelRequestId);

        /// <summary>
        /// Get Shared User Details
        /// </summary>
        /// <param name="sharingid"></param>
        /// <param name="clubid"></param>
        /// <param name="empcode"></param>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        List<BulkUploadModels> GetSharedUserDetails(Int16 sharingid, Int32 clubid, string empcode, string TravelRequestId);

        /// <summary>
        /// Get Shared User Details
        /// </summary>
        /// <param name="sharingid"></param>
        /// <param name="clubid"></param>
        /// <param name="empcode"></param>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        List<BulkUploadModels>  GetSharedUserDetailsNew(Int32 clubId , Int64 travelrequestid );

        /// <summary>
        /// Get Bulk Booking Details : PNR wise
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        List<BulkUploadModels> GetBulkBookingPNRWiseDetails(DateTime fromdate, DateTime todate, string empcode, int criteria);

        /// <summary>
        /// Emport PNR wise details
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        List<BulkUpload_ExcelExportPNRWise> GetBulkBookingPNRWiseDetails_ExportToExcel(DateTime fromdate, DateTime todate, string empcode, int criteria);
   
    
        /// <summary>
        /// Validatee Employee Verticals
        /// </summary>
        /// <param name="empList"></param>
        /// <returns></returns>
        List<string> ValidateEmployeeVerticals(List<string> empVerticalList, string verticalList);



        /// <summary>
        /// Get Employee Code wise Details 
        /// </summary>
        /// <param name="empList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        List<EmployeeCodewiseDetailModel> GetEmployeeCodewiseDetails(List<string> empList, int criteria);


        /// <summary>
        /// Get Employee Booking Agency Rights
        /// </summary>
        /// <param name="empCode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        List<string> GetEmployeeBookingAgencyRight(string empCode, int criteria);
        /// <summary>
        /// Get Employee Booking Department Rights
        /// </summary>
        /// <param name="empCode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        List<EmployeeDepartmentRights> GetEmployeeDepartmentRight(string empCode, int criteria);
        


       /// <summary>
       /// To save Approved/Reject Bulk Booking Request
       /// </summary>
       /// <param name="travelRequestApprovalModels"></param>
       /// <returns></returns>
       int ApproveBulkSodBookingRequest(BulkBookingRequestApprovalModels bulkBookingRequestApprovalModel);
        /// <summary>
        /// To save Approved/Reject Bulk Booking Request Selective
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// modified by Soni
        /// <returns></returns>
        int ApproveBulkSodBookingRequestSelective(List<BulkBookingRequestApprovalModels> bulkBookingRequestApprovalModel);

        /// <summary>
        /// to check how many employee is rest for approval
        /// </summary>
        /// <param name="travelRequest"></param>
        /// <param name="AddNo"></param>
        /// <returns></returns>
        List<BulkUploadModels>IsEmpRestForApproval(int travelRequest,int AddNo);
        /// <summary>
        /// to check how many employee is rest for approval
        /// </summary>
        /// <param name="travelRequest"></param>
        /// <param name="AddNo"></param>
        /// <returns></returns>
        List<BulkUploadModels> IsEmpRestForApproval(int travelRequest);
        /// Get Bulk Booking Info For PNR Generation
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetBulkBookingInfoForPNRGeneration(Int64 travelReqId);

        /// <summary>
        /// Get Bulk Booking info for Pnr Generation when Add no is added
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="AddNo"></param>
        /// <returns></returns>
        Dictionary<string, object> GetBulkBookingInfoForPNRGeneration_edit(Int64 travelReqId,Int32 AddNo);


        /// <summary>
        /// to get bulk booking info for selective option
        /// </summary>
        /// <param name="vlist"></param>
        /// <returns></returns>
        Dictionary<string, object> GetBulkBookingInfoForPNRGenerationSelective(List<long> vlist);

       /// <summary>
       /// To save Reject Booking Request
       /// </summary>
       /// <param name="BulkBookingRequestApprovalModels"></param>
       /// <returns></returns>
       int RejectSodBulkBookingRequest(BulkBookingRequestApprovalModels bulkBookingRequestApprovalModel);

        int RejectSodBulkBookingRequest_edit(BulkBookingRequestApprovalModels bulkBookingRequestApprovalModel, Int32 AddNo);

       /// <summary>
       /// Rollback Bulk Booking approval status by HOD
       /// </summary>
       /// <param name="bulkBookingRequestApprovalModel"></param>
       /// <returns></returns>
       int RollBackApprovalByHOD(BulkBookingRequestApprovalModels bulkBookingRequestApprovalModel);



       /// <summary>
       /// Get Bulk Booking Master For HOD Approval
       /// </summary>
       /// <returns></returns>
       List<BulkUploadMasterModels> GetBulkBookingHODApprovalList_MasterData(String empcode, int criteria);

        /// <summary>
        /// To find SPoc detail which upload the excel sheet
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<BulkUploadMasterModels> FindSpocDetails( string travelrequestid);



        /// <summary>
        /// Get hotel bulk booking data for traveldesk
        /// </summary>
        /// <returns></returns>
        List<BulkUploadMasterModels> GetBulkBookingHotelListData(string bookingType);

       /// <summary>
       /// Reject & Close Bulk Booking Request
       /// </summary>
       /// <param name="TravelRequestId"></param>
       /// <param name="Criteria"></param>
       /// <returns></returns>
       int  RejectionCloseBulkBookingRequest(Int64 TravelRequestId,int Criteria);

        /// <summary>
        /// Update rejection status in bulk bookingDetails table
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="Criteria"></param>
        /// <returns></returns>
        int UpdateStatusOnRejection(List<long> breqIdList,string trID , string type);


        /// <summary>
        /// Check duplicate Bulk Booking List
        /// </summary>
        /// <param name="blist"></param>
        /// <returns></returns>
        List<string> ValidateDuplicateBulkBooking(List<BulkUploadModels> blist);


        /// <summary>
        /// Save bulk hotel booking data
        /// </summary>
        /// <param name="hlist"></param>
        /// <returns></returns>
       int SaveBulkHotelUpload(List<BulkTravelRequestHotelDetailModels> hlist);

        /// <summary>
        /// save financial approval data
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        int SaveHODFinancialApprovalRequest(List<BulkTravelRequestHotelDetailModels> hlist, string  hodemail1, string hodemail2,List<BulkHotelInclusionNonContractualMasterModels> inclist,  string sodOat);


        /// <summary>
       /// get bulk user info for hotel
        /// </summary>
        /// <param name="travelRequestID"></param>
        /// <param name="clubid"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
       Dictionary<string, object> GetBulkHotelandUserInfo(Int64 travelRequestID, Int32 clubid, string hotelname,int criteria);

        /// <summary>
       /// find clubid list from bulk booking details
       /// </summary>
       /// <param name="empcode"></param>
       /// <param name="TravelRequestId"></param>
       /// <returns></returns>
       List<BulkTravelRequestHotelDetailModels> FindClubid(string empcode, string TravelRequestId);


        /// <summary>
       /// Find bulk List By ClubId
       /// </summary>
       /// <param name="clubid"></param>
       /// <returns></returns>
       List<BulkTravelRequestHotelDetailModels> FindListByClubId(string clubid);

        /// <summary>
        /// Updated the booking list 
        /// </summary>
        /// <param name="oldEcode"></param>
        /// <param name="upDatedrow"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        Int64 UpdateBulkDetail(string oldEcode, List<BulkUploadModels> upDatedrow, Int64 trid );

        /// <summary>
        /// Update the hotel details after changing details in already booking generated booking request
        /// </summary>
        /// <param name="oldEcode"></param>
        /// <param name="upDatedrow"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        Int64 UpdateHotelDetails(string oldEcode, List<BulkUploadModels> upDatedrow, Int64 trid);

        /// <summary>
        /// fetch the details of employee by employee code and department id
        /// </summary>
        /// <param name="newEcode"></param>
        /// <param name="dept"></param>
        /// <returns></returns>
        List<EmdmAPIModels> fetchdetails(string newEcode , int dept);


        /// <summary>
        /// fetch the details of employee by employee code 
        /// </summary>
        /// <param name="Empcode"></param>
        /// <returns></returns>
        List<EmdmAPIModels> fetchEmpdetails(string Empcode);

        /// <summary>
        /// to check employee code is present in same booking or not
        /// </summary>
        /// <param name="newEcode"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        string check_duplicacy(string newEcode, Int64 trid);

        /// <summary>
        /// to add new row in booking list
        /// </summary>
        /// <param name="DetalsList"></param>
        /// <returns></returns>
        int saveBulk_newRow(List<BulkUploadModels> DetalsList);
        //Dictionary<string, object> SaveCancellationRequestUser(string travReqstId, string empCode);

        /// <summary>
        /// to find add number which show no of times booking request is added in same booking request no
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        int FindAddNo(Int64 trid);

        /// <summary>
        /// to cancel booking from user end
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
         int CancelBookingRequest(String empcode, Int64 trid , String ReasonForCancellation);

        /// <summary>
        /// To approve hod financial approval
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="hid"></param>
        /// <param name="clubId"></param>
        /// <param name="approverEmpcCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        int BulkApproveNonContractualHotelRequest(string travelReqID, string hid, int clubId, string approverEmpcCode, string type);

        /// <summary>
        /// to reject hod financial approval
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="hid"></param>
        /// <param name="clubid"></param>
        /// <param name="approverEmpcCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        int BulkRejectNonContractualHotelRequest(string travelReqID, string hid, int clubid, string approverEmpcCode, string type);

        /// <summary>
        /// FindNonContractualHotelInclusions
        /// </summary>
        /// <param name="travelreqid"></param>
        /// <param name="HotelRequestId"></param>
        /// <returns></returns>
        List<BulkHotelInclusionNonContractualMasterModels> FindNonContractualHotelInclusions(Int64 travelreqid, int HotelRequestId);

        /// <summary>
        /// getting list of approval request
        /// </summary>
        /// <param name="trid"></param>
        /// <param name="hId"></param>
        /// <param name="sodoat"></param>
        /// <returns></returns>
        List<BulkBookingHODFinancialApprovalModels> GetdetailHODApproval(Int64 trid, int hId, string sodoat);

        /// <summary>
        /// GetBulkhotel By trid
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetBulkHotelDetailbyTrID(long travelReqId);

        /// <summary>
        /// Update hotel status after sending mail to hotel on approval
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        int  UpdateHotelStatus(Int64 TravelRequestId , string clubId);
       
		/// <summary>
		/// Get HOD/Approver details as Dept & Vertical wise
		/// </summary>
		/// <param name="dept"></param>
		/// <param name="verticalCode"></param>
		/// <returns></returns>
        string getHodEmployee(int deptId, string verticalCode);

        /// <summary>
        /// Save SMS Log after the HOD approval
        /// </summary>
        /// <param name="smsLogModel"></param>
        /// <returns></returns>
        int SaveApproverSMSLog(SodApproverSMSLogModels smsLogModel);
    }
}
