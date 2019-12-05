using System.Collections.Generic;
using System.Linq;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;

namespace SOD.Services.Repository
{
    public class DepartmentRepository:IDepartmentRepository
    {
        /// <summary>
        /// Constructor for Connection initialization
        /// </summary>
        private readonly SodEntities _context;

        public DepartmentRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }


        /// <summary>
        /// Get Department List
        /// </summary>
        /// <returns></returns>
        public IList<DepartmentModels> GetDepartmentList()
        {
            //return _context.DepartmentModel.ToList();
            var lstdept = ADO.SodCommonServices.GetDepartmentList(0, 1);
            return lstdept.ToList();
        }

        /// <summary>
        /// Get Department List
        /// </summary>
        /// <returns></returns>
        public IList<DepartmentModels> GetDepartmentVerticalList()
        {
            //return _context.DepartmentModel.ToList();
            var lstdept = ADO.SodCommonServices.GetDepartmentVerticalList(0, 4);
            return lstdept.ToList();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}