using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;

namespace SOD.Services.Repository
{
    public class SodBookingTypeRepository:ISodBookingTypeRepository
    {
        private readonly SodEntities _context;

        public SodBookingTypeRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }


        public bool GetSodBookingTypeName(short sodBookingTypeId)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<SodBookingTypeModels> GetSodBookingTypeList()
        {
            throw new System.NotImplementedException();
        }

        public int Save(SodBookingTypeModels sodApproverModels)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}