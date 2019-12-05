using System.Collections.Generic;
using System.Linq;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using System;
using System.Threading.Tasks;

namespace SOD.Services.Repository
{
    public class AdminRepository : IAdminRepository
    {

        #region "Constructor Initialization"
        /// <summary>
        /// Constructor Initilization
        /// </summary>
        private readonly SodEntities _context;
        public AdminRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }

        #endregion

        /// <summary>
        /// Dispose Object
        /// </summary>
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        #region "Save Operation"
        /// <summary>
        /// Save Sod Approver 
        /// </summary>
        /// <param name="sodApproverModels"></param>
        /// <returns></returns>
        public int SaveSodApprover(List<SodApproverModels> sodApproverModels)
        {
            var counter = 0;
            foreach (var item in sodApproverModels)
            {
                var s = _context.SodApprovers.Where(id => id.DepartmentId == item.DepartmentId && id.EmployeeId == item.EmployeeId && id.VerticleId == item.VerticleId).ToList();
                if (s.Count() > 0) counter++;
            }
            if (counter > 0) { return 0; }

            _context.SodApprovers.AddRange(sodApproverModels);
            return _context.SaveChanges();
        }


        /// <summary>
        /// Save Sod Blanket Approvers
        /// </summary>
        /// <param name="sodBlanketApprovalModels"></param>
        /// <returns></returns>
        public int SaveSodBlanketApprover(SodBlanketApprovalModels sodBlanketApprovalModels)
        {
            var s = _context.SodBlanketApprovals.Where(id => id.EmployeeId == sodBlanketApprovalModels.EmployeeId).ToList();

            if (s.Count() > 0) { return 0; }

            _context.SodBlanketApprovals.Add(sodBlanketApprovalModels);
            return _context.SaveChanges();
        }

        #endregion

        #region "Fetch Data Methods"

        /// <summary>
        /// Get Department List
        /// </summary>
        /// <returns></returns>
        public IList<DepartmentModels> GetDepartmentList()
        {
            var lstdept = ADO.SodCommonServices.GetDepartmentList(0, 1);
            return lstdept.ToList();
        }

        /// <summary>
        /// Get Department List
        /// </summary>
        /// <returns></returns>
        public IList<DepartmentModels> GetDepartmentVerticalList()
        {
            var lstdept = ADO.SodCommonServices.GetDepartmentVerticalList(0, 4);
            return lstdept.ToList();
        }



        /// <summary>
        /// Get Vertical List
        /// </summary>
        /// <returns></returns>
        public IList<VerticalModels> GetDeptVerticals()
        {
            var lstdept = ADO.SodCommonServices.GetDeptVerticals(0, 3);
            return lstdept.ToList();
        }



        /// <summary>
        /// Get Employee Designations List
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public IList<DesignationModels> GetDesignationsList(int deptId)
        {
            //_context.DesignationModel.ToList();
            //return _context.DesignationModel.Where(id=>id.tbl_emp_dept_mapping_id==deptId).ToList();
            var lstdesig = ADO.SodCommonServices.GetDesignationList(deptId, 2);
            return lstdesig;
        }

        /// <summary>
        /// Get Employee common info List
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public IList<EmployeeModel> GetEmployeeList(int EmpId)
        {
            var lstEmployee = ADO.SodCommonServices.GetEmployeeCommonDetails(EmpId);
            return lstEmployee.ToList();
        }


        /// <summary>
        /// Get Employee common info using emp Code
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public IList<EmployeeModel> GetEmployeeInfo(string empCode, int criteria)
        {
            var lstEmployee = ADO.SodCommonServices.GetEmployeeCommonInfo(empCode, criteria);
            return lstEmployee.ToList();
        }


        /// <summary>
        /// Get Blanket Approver List
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<EmployeeModel> GetSodBlanketApproverList(int? deptId, int criteria)
        {
            var lstEmployee = ADO.SodCommonServices.GetSodBlanketApprovals(deptId, criteria);
            return lstEmployee.ToList();
        }


        /// <summary>
        /// Get Sod Approver List
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<EmployeeModel> GetSodApproverList(int? deptId, int criteria)
        {
            var lstEmployee = ADO.SodCommonServices.GetSodApprovals(deptId, criteria);
            return lstEmployee.ToList();
        }


        /// <summary>
        /// Get Employee Dept. Mapping List
        /// </summary>
        /// <returns></returns>
        public IList<SodCXODeptMappingModels> GetcxoDeptMappingList(string cxoName, int criteria)
        {
            var lstcxo = ADO.SodCommonServices.GetcxoDeptMappingList(cxoName, criteria);
            return lstcxo.ToList();
        }

        /// <summary>
        /// Remove Blanket Approver Rights
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public int RemoveBlanketApproverRights(int EmpId)
        {
            var obj = _context.SodBlanketApprovals.FirstOrDefault(e => e.EmployeeId == EmpId);
            _context.SodBlanketApprovals.Remove(obj);
            return _context.SaveChanges();
        }


        /// <summary>
        /// Remove Cxo Dept Mapping List
        /// </summary>
        /// <param name="depId"></param>
        /// <returns></returns>
        public int RemoveCxoDeptMappingList(int depId)
        {
            var obj = _context.SodCXODeptMappingModel.FirstOrDefault(e => e.AllocatedDeptId == depId);
            _context.SodCXODeptMappingModel.Remove(obj);
            return _context.SaveChanges();
        }

        /// <summary>
        /// Remove Approver Rights
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="degId"></param>
        /// <returns></returns>
        public int RemoveApproverRight(int deptId, int degId)
        {
            var obj = _context.SodApprovers.FirstOrDefault(e => e.DepartmentId == deptId && e.DesignationId == degId);
            _context.SodApprovers.Remove(obj);
            return _context.SaveChanges();
        }


        /// <summary>
        /// Approver ID
        /// </summary>
        /// <param name="approverId"></param>
        /// <returns></returns>
        public int RemoveApproverRightByID(int approverId)
        {
            var obj = _context.SodApprovers.FirstOrDefault(e => e.SodApproverId == approverId);
            _context.SodApprovers.Remove(obj);
            return _context.SaveChanges();
        }


        /// <summary>
        /// Save: Allocate CXO Dept Mapping
        /// </summary>
        /// <param name="objcxodeptmapping"></param>
        /// <returns></returns>
        public int AllocateCxoDeptMappingList(SodCXODeptMappingModels objcxodeptmapping)
        {
            _context.SodCXODeptMappingModel.Add(objcxodeptmapping);
            return _context.SaveChanges();
        }


        #endregion

        #region "Save Data from API to EMDM DataBase"

        /// <summary>
        /// Save Employee EMDM Common Info in DB from API
        /// </summary>
        /// <param name="eList"></param>
        /// <returns></returns>
        public int SaveEMDMAPI_EmployeeInfo(List<EmdmAPIModels> eList)
        {
            var s = 0;
            try
            {
                if (eList.Count > 0)
                {
                    _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [SodEmployeeCommonDetails]");
                    _context.EmdmAPIModel.AddRange(eList);
                    s = _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                s = -1;
                throw;
            }
            return s;
        }
        #endregion

        /// <summary>
        /// Get Emp Roles
        /// </summary>
        /// <param name="Empcode"></param>
        /// <returns></returns>
        public RoleViewModel GetEmpRoles(string Empcode)
        {
            RoleViewModel model = null;
            model = new RoleViewModel
            {
                TotalRoles = _context.SODUserRoleMaster.Any() ? _context.SODUserRoleMaster.Where(x => x.IsActive == true).ToList() : new List<SODUserRoleMaster>() { new SODUserRoleMaster { Id = 1, RoleId = 1, RoleName = "User" } },
                ExistingRoles = _context.SODUserRoleAllocation.Any() && _context.SODUserRoleAllocation.Where(x => x.EmployeeCode == Empcode && x.IsActive == true).FirstOrDefault() != null ? _context.SODUserRoleAllocation.Where(x => x.EmployeeCode == Empcode && x.IsActive == true).Select(x => x.RoleId).FirstOrDefault() + ",1" : "1"
            };
            return model;
        }

        /// <summary>
        /// Save Emp Role
        /// </summary>
        /// <param name="Empcode"></param>
        /// <param name="Roles"></param>
        /// <returns></returns>
        public int SaveEmpRole(string Empcode, string Roles)
        {
            int Id = 0;
            var data = _context.SODUserRoleAllocation.Where(x => x.EmployeeCode == Empcode && x.IsActive == true).FirstOrDefault();
            if (data != null && Roles != "")
            {
                data.RoleId = Roles;
                Id = data.Id;
                _context.SaveChanges();
            }
            else if (data == null && Roles != "")
            {
                SODUserRoleAllocation model = new SODUserRoleAllocation
                {
                    RoleId = Roles,
                    EmployeeCode = Empcode,
                    CreatedDate = DateTime.Now,
                    CreatedByUser = "admin",
                    IsActive = true
                };
                _context.SODUserRoleAllocation.Add(model);
                _context.SaveChanges();
                Id = model.Id;
            }
            else
            {
                Id = data.Id;
                _context.SODUserRoleAllocation.Remove(data);
                _context.SaveChanges();
            }
            return Id;
        }

        /// <summary>
        /// Get All Menus
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SodUserMenu> GetAllMenus()
        {
            var allmenu = _context.SodUserMenu.Where(x => x.IsActive == true).OrderBy(x => x.OrderPriority).ToList();
            return allmenu;
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SODUserRoleMaster> GetAllRoles()
        {
            return _context.SODUserRoleMaster.Where(x => x.IsActive == true);
        }

        /// <summary>
        /// Get SOD Menus
        /// </summary>
        /// <returns></returns>
        public List<MenuViewModel> GetSODMenus(int Role)
        {
            //var allmenus = (from a in _context.SodUserMenu
            //                join b in _context.SodMenuRight on a.MenuId equals b.MenuId into c
            //                from d in c.DefaultIfEmpty()
            //                where a.IsActive == true
            //                orderby a.OrderPriority
            //                select new
            //                {
            //                    a.Id,
            //                    a.MenuId,
            //                    a.MenuPath,
            //                    a.MenuType,
            //                    a.MenuName,
            //                    a.ParentId,
            //                    RoleId = d.RoleId.ToString() == null ? "" : d.RoleId.ToString()
            //                }).ToList().GroupBy(x => x.Id).Select(x => x.First());

            var allmenus = from a in _context.SodUserMenu
                           join b in from c in _context.SodMenuRight where c.RoleId == Role select c
on a.MenuId equals b.MenuId into d
                           from e in d.DefaultIfEmpty()
                           where a.IsActive == true
                           orderby a.OrderPriority
                           select new
                           {
                               a.Id,
                               a.MenuId,
                               a.MenuPath,
                               a.MenuType,
                               a.MenuName,
                               a.ParentId,
                               RoleId = e.RoleId.ToString() == null ? "" : e.RoleId.ToString()
                           };

            List<MenuViewModel> modeldata = new List<MenuViewModel>();
            if (allmenus != null && allmenus.Count() > 0)
            {
                foreach (var data in allmenus)
                {
                    MenuViewModel model = new MenuViewModel();
                    model.Id = data.Id;
                    model.MenuId = data.MenuId;
                    model.MenuName = data.MenuName;
                    model.MenuPath = data.MenuPath;
                    model.ParentId = data.ParentId;
                    model.MenuType = data.MenuType;
                    model.RoleID = data.RoleId == "" ? "" : data.RoleId;
                    modeldata.Add(model);
                }
            }
            return modeldata;
        }

        /// <summary>
        /// Save SOD Menu Rights
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="Menuids"></param>
        /// <returns></returns>
        public int SaveSODMenuRights(int RoleID, string Menuids)
        {
            int RowsAffected = 0;
            IList<SodMenuRight> sodmenulist = new List<SodMenuRight>();
            var tableid = _context.SodMenuRight.Any() ? _context.SodMenuRight.Select(x => x.Id).Max() + 1 : 1;
            string[] Ids = Menuids.Split(',');
            var deletedata = _context.SodMenuRight.Where(x => x.RoleId == RoleID).ToList();
            if (deletedata.Count > 0 && deletedata != null)
            {
                _context.SodMenuRight.RemoveRange(deletedata);
                _context.SaveChanges();
            }
            foreach (var menuid in Ids)
            {
                SodMenuRight model = new SodMenuRight()
                {
                    RoleId = RoleID,
                    MenuId = Convert.ToInt32(menuid)
                };
                sodmenulist.Add(model);
                tableid++;
            }
            _context.SodMenuRight.AddRange(sodmenulist);
            RowsAffected = _context.SaveChanges();
            return RowsAffected;
        }
    }
}