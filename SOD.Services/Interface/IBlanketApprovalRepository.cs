using System;
using System.Collections;
using System.Collections.Generic;
using SOD.Model;


namespace SOD.Services.Interface
{
    public interface IBlanketApprovalRepository : IDisposable
    {
        /// <summary>
        /// Get Blanket Approver Status
        /// </summary>
        /// <param name="blanketApproval"></param>
        /// <returns></returns>
        bool GetBlanketApprovalStatus(SodBlanketApprovalModels blanketApproval);

        /// <summary>
        /// Get Blanket Approver List
        /// </summary>
        /// <returns></returns>
        IEnumerable<SodBlanketApprovalModels> GetBlanketApprovelList();


        /// <summary>
        /// Save Blanket Approver Rights
        /// </summary>
        /// <param name="blanketApproval"></param>
        /// <returns></returns>
        int Save(SodBlanketApprovalModels blanketApproval);


        
        /// <summary>
        /// Remove Blanket Approver Rights
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        int RemoveBlanketApproverRights(int EmpId);
    }
}