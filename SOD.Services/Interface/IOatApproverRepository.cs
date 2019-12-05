using System;
using System.Collections.Generic;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface IOatApproverRepository:IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="travelRequestApprovalModels"></param>
        /// <returns></returns>
        int ApproveOatBookingRequest(OatTravelRequestApprovalModel travelRequestApprovalModels);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trRequest"></param>
        /// <returns></returns>
        int CloseOatBookingRequest_HOD(OALTravelRequestMasterModel trRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetOatBookingInfoForPNR(Int64 travelReqId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trApprovalModels"></param>
        /// <returns></returns>
        int RejectOatBookingRequest(OatTravelRequestApprovalModel trApprovalModels); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="designationId"></param>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<OALTravelRequestMasterModel> GetOatBookingListForApproval(int departmentId, int designationId, int EmpId, int criteria);
    }
}