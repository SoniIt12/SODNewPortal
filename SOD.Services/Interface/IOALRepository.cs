using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Interface
{
    public interface IOALRepository : IDisposable
    {
        /// <summary>
        /// Save OAL details
        /// </summary>
        /// <param name="flightList"></param>
        /// <param name="passengerList"></param>
        /// <param name="hotelList"></param>
        /// <param name="cabList"></param>
        /// <param name="masterList"></param>
        /// <returns></returns>
        Int64 SaveOALData(List<OALModels> oalList, List<OALPassengerModel> plist, List<OALHotelModel> hlist, List<OALCabModel> clist, List<OALTravelRequestMasterModel> mlist);


        /// <summary>
        /// Get employee details from database
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        IList<EmployeeModel> GetEmployeeList(int EmpId);

        /// <summary>
        /// Get All Employee Data
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Dictionary<string, object> GetOatData(string EmpId, int criteria);

        /// <summary>
        /// Get Maximum Travel Request Id
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        int getMaxRequestId(List<OALModels> elist);


        /// <summary>
        /// get oat data
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetOATDetailsData(string TrId);


        /// <summary>
        /// get logged in user details
        /// </summary>
        /// <param name="userLoginDal"></param>
        /// <returns></returns>
        UserAccountModels GetLoginUserList(UserAccountModels userLoginDal);


        /// <summary>
        /// get approved requests from hod for traveldesk panel
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Dictionary<string, object> GetApprovedRequests(string EmpId, int criteria);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        int SaveUploadData(List<OALUploadModel> ulists);

        /// <summary>
        /// Close Booking Request After Rejection/Approval 
        /// </summary>
        /// <param name="trRequest"></param>
        /// <returns></returns>
        int CloseOatBookingRequest(OALTravelRequestMasterModel trRequest);

        /// <summary>
        /// get oat booking info for pnr creation
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetOatBookingInfoForPNR(Int64 travelReqId);

        /// <summary>
        /// get oat requests for travel history
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Dictionary<string, object> GetOatHistoryRequests(string EmpId, int criteria);


        /// <summary>
        /// get ITH details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Dictionary<string, object> ithfilldropdown(string name);

        /// <summary>
        /// save ith request
        /// </summary>
        /// <param name="hlist"></param>
        /// <returns></returns>
        int SaveITHRequestData(List<ITHRequestApprovalModels> hlist);

        /// <summary>
        /// save ith response
        /// </summary>
        /// <param name="ithList"></param>
        /// <returns></returns>
        int SaveITHResponseData(List<ITHResponseDetailModels> ithList);

        /// <summary>
        /// get ith response data
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        Dictionary<string, object> viewITHStatusList(string TrId);

        /// <summary>
        /// save hod status
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int saveHODStatus(long TravelRequestId, string status);

        /// <summary>
        /// save hod response for financial approval
        /// </summary>
        /// <param name="IdList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        int saveHODResponseData(string IdList, int criteria);

        /// <summary>
        /// display hod financial approval response 
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        Dictionary<string, object> viewHODResponse(string TrId);

        /// <summary>
        /// Get Financially Approved Requests Data
        /// </summary>
        /// <param name="TrId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetFinanciallyApprovedRequests(string TrId);
    }
}
