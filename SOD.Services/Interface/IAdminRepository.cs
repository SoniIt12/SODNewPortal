using System;
using System.Collections.Generic;
using SOD.Model;
using System.Threading.Tasks;

namespace SOD.Services.Interface
{
    public interface IAdminRepository:IDesignationRepository,IDepartmentRepository
    {
          /// <summary>
          /// Save Method for Sod Approvers
          /// </summary>
          /// <param name="sodApproverModels"></param>
          /// <returns></returns>
          int SaveSodApprover(List<SodApproverModels> sodApproverModels);

          
          /// <summary>
          /// Save Method for Sod Blanket Approvers
          /// </summary>
          /// <param name="sodBlanketApprovalModels"></param>
          /// <returns></returns>
          int SaveSodBlanketApprover(SodBlanketApprovalModels sodBlanketApprovalModels);



          /// <summary>
          /// Get Employee Common Info
          /// </summary>
          /// <param name="EmpId"></param>
          /// <returns></returns>
          IList<EmployeeModel> GetEmployeeList(int EmpId);


           /// <summary>
           /// Get SodBlanket Approver List
           /// </summary>
           /// <param name="deptId"></param>
           /// <param name="criteria"></param>
           /// <returns></returns>
          IList<EmployeeModel> GetSodBlanketApproverList(int? deptId,int criteria);

            
         /// <summary>
         /// Get Employee Common info
         /// </summary>
         /// <param name="empCode"></param>
         /// <param name="criteria"></param>
         /// <returns></returns>
         IList<EmployeeModel> GetEmployeeInfo(string empCode, int criteria);

         /// <summary>
         /// Get Sod  Approver List
         /// </summary>
         /// <param name="deptId"></param>
         /// <param name="criteria"></param>
         /// <returns></returns>
         IList<EmployeeModel> GetSodApproverList(int? deptId, int criteria);


        /// <summary>
        /// Remove Blanket Approver Rights
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        int RemoveBlanketApproverRights(int EmpId);

        
        /// <summary>
        /// Remove Approver Rights
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="degId"></param>
        /// <returns></returns>
        int RemoveApproverRight(int deptId, int degId);

        /// <summary>
        /// Remove Approver Rights
        /// By Approver ID
        /// <returns></returns>
        int RemoveApproverRightByID(int approverId);


        /// <summary>
        /// Get CXO Dept Mapping List
        /// </summary>
        /// <param name="cxoName"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<SodCXODeptMappingModels> GetcxoDeptMappingList(string cxoName, int criteria);



        /// <summary>
        /// Remove Cxo Dept Mapping List
        /// </summary>
        /// <param name="depId"></param>
        /// <returns></returns>
        int RemoveCxoDeptMappingList(int depId);

        
        /// <summary>
        /// Save: Allocate CXO Dept Mapping
        /// </summary>
        /// <param name="objcxodeptmapping"></param>
        /// <returns></returns>
        int AllocateCxoDeptMappingList(SodCXODeptMappingModels objcxodeptmapping);


        
        /// <summary>
        /// Save Employee Info in MDM DB
        /// </summary>
        /// <param name="eList"></param>
        /// <returns></returns>
        int SaveEMDMAPI_EmployeeInfo(List<EmdmAPIModels> eList);


        /// <summary>
        /// Get Vertical List
        /// </summary>
        /// <returns></returns>
        IList<VerticalModels> GetDeptVerticals();


        /// <summary>
        /// Get Emp Roles
        /// </summary>
        /// <param name="Empcode"></param>
        /// <returns></returns>
        RoleViewModel GetEmpRoles(string Empcode);

        /// <summary>
        /// Save Emp Role
        /// </summary>
        /// <param name="Empcode"></param>
        /// <param name="Roles"></param>
        /// <returns></returns>
        int SaveEmpRole(string Empcode, string Roles);

        /// <summary>
        /// Get All Menus
        /// </summary>
        /// <returns></returns>
        IEnumerable<SodUserMenu> GetAllMenus();

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        IEnumerable<SODUserRoleMaster> GetAllRoles();

        /// <summary>
        /// Get SOD Menus
        /// </summary>
        /// <returns></returns>
        List<MenuViewModel> GetSODMenus(int Role);

        /// <summary>
        /// Save SOD Menu Rights
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="Menuids"></param>
        /// <returns></returns>
        int SaveSODMenuRights(int RoleID, string Menuids);
    }
}