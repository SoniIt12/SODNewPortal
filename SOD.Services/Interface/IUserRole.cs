using System;

namespace SOD.Services.Interface
{
    /// <summary>
    /// This Interface will be user for Manage the User Role
    /// </summary>
    public interface IUserRole:IDisposable
    {

        /// <summary>
        /// Check Blanket Approver Role
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        bool IsBlanketApproverRole(int EmpId);


        /// <summary>
        /// Sod Approver/Hod
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        bool IsSodApproverHodRole(int employeeId);


        /// <summary>
        /// Check CXO Level Approver Role
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        bool IsCXOApproverRole(int employeeId);

    }
}