using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;

namespace SOD.Services.Repository
{
    public class BlanketApprovalRepository:IBlanketApprovalRepository
    {

        private readonly SodEntities _context;

        public BlanketApprovalRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }


        public bool GetBlanketApprovalStatus(SodBlanketApprovalModels blanketApproval)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<SodBlanketApprovalModels> GetBlanketApprovelList()
        {
            throw new System.NotImplementedException();
        }

        public int Save(SodBlanketApprovalModels blanketApproval)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }


        public int RemoveBlanketApproverRights(int EmpId)
        {
            throw new System.NotImplementedException();
        }
    }
}