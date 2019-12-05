using System;
using System.Collections.Generic;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface IRevenueRepository:IDisposable 
    {
        /// <summary>
        /// Get Sod List for Approval
        /// </summary>
        /// <returns></returns>
        IList<TravelRequestModels> GetSodBookingListForApproval(int departmentId, int designationId,int EmpId, int criteria); 
    }
}