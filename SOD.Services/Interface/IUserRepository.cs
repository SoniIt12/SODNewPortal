using System;
using System.Collections.Generic;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface IUserRepository : IUserRole,  IDisposable
    {
        
        
        /// <summary>
        /// Save Travel Request Sod Booking
        /// </summary>
        /// <param name="travelRequestModels"></param>
        /// <returns></returns>
        int SaveSodBookingRequest(TravelRequestModels travelRequestModels);



        /// <summary>
        /// Get Employee Common Info
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        IList<EmployeeModel> GetEmployeeList(int EmpId);


        /// <summary>
        /// Save Travel Request Sod Booking
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="passengerList"></param>
        /// <returns></returns>
        int SaveBookingRequest(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> passengerList, List<PassengerMealAllocationModels> passengerMealsList, List<TravelRequestCabDetailModels> cabDetailList, List<TravelRequestHotelDetailModels> hotelDetailList);

        /// <summary>
        /// Update PNR in Request Master Table
        /// </summary>
        /// <param name="pnr"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        int UpdatePnr(string pnr, int requestId);


        /// <summary>
        /// save hotel approval status
        /// </summary>
        /// <param name="travelrequestid"></param>
        void UpdateHotelApprovalStatus(long travelrequestid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="travelrequestid"></param>
        void UpdateOnlyHotelApprovalStatus(long travelrequestid);

		/// <summary>
        /// Save Travel Request Sod Booking
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="passengerList"></param>
        /// <returns></returns>
        Int64 SaveHotelRequest(List<TravelRequestHotelDetailModels> hotelDetailList);


        /// <summary>
        /// Get PNR Amount and Time
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        string GetPNRAmountAndTime(Int64 requestid);



        /// <summary>
        /// Get HOD/Approver Email id
        /// </summary>
        /// <param name="verticalId"></param>
        /// <returns></returns>
        string GetHODEmailId(string verticalId);

        
        /// <summary>
        /// Get CXO Approver List
        /// </summary>
        /// <returns></returns>
        List<string> GetCXO_ApproverList();

        
        /// <summary>
        /// Get CXO Approver List 
        /// OverLoad Method
        /// </summary>
        /// <returns></returns>
        List<string> GetCXO_ApproverList(Int16 deptId, Int16 bookingfor);

        /// <summary>
        /// Get Employee Id
        /// </summary>
        /// <param name="empCode"></param>
        /// <returns></returns>
        int GetLoginEmployeeID(string empCode);


         /// <summary>
        /// Get Approver Dept & Designation
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        string GetApproverDept_Desig(int EmpId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetSodHotelInfo(long travelReqId);


        /// <summary>
        /// Get only Standby HOD/Approver Email id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        string GetHODEmailId_OnlyStandby(string verticalId);



        /// <summary>
        /// Get CXO Email id 
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        string GetCXO_EmailCC(int deptId);

        
        /// <summary>
        /// Save master data, flight data and hotel data from "Only Hotel" link page 
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="hotelDetailList"></param>
        /// <returns></returns>
        int SaveOnlyHotelBookingRequest(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<TravelRequestHotelDetailModels> hotelDetailList);

        
        /// <summary>
        /// For managing Changes Request Area
        /// </summary>
        /// <param name="verticalId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        string GetHODEmailIdDetails(string verticalId, int criteria);

        /// <summary>
        /// For managing Changes Request Area
        /// </summary>
        /// <param name="EmployeeCode"></param>
        /// <returns></returns>
        List<SODIntegratedApplicationMaster>GetDropdownData(string EmployeeCode);

        // Get Employee Role
        string GetRoleOFEmp(string empcode);

        /// <summary>
        /// Save SMS Log after the HOD approval
        /// </summary>
        /// <param name="smsLogModel"></param>
        /// <returns></returns>
        int SaveApproverSMSLog(SodApproverSMSLogModels smsLogModel);

        /// <summary>
        /// Get booking List which not yet approved by hod
        /// </summary>
        /// <returns></returns>
        List<TravelRequestHotelDetailModels> GetReminderListTosendApproval(int criteria);

        /// <summary>
        /// GET LIST OF TRAVELREQUESTID AND HOTELREQUESTID TO SEND TRIGGER TO FINANCIAL APPROVAR
        /// </summary>
        /// <returns></returns>
       // List<TravelRequestHotelDetailModels> GetListTosendTriggerFinancalApproval(); 


        /// <summary
        /// gethodemailidBYTravelReqID
        /// </summary>
        /// <param name="TravelReqId"></param>
        /// <returns></returns>
        string  GetHODEmailIdByTravelReqID(String TravelReqId);


    }
}