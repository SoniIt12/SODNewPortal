using System.Collections.Generic;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;

namespace SOD.Services.Repository
{
    public class TravelRequestTypeRepository:ITravelRequestTypeRepository
    {
        private readonly SodEntities _context;

        public TravelRequestTypeRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
       
        public bool GetTravelRequestTypeName(short travelRequestTypeId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TravelRequestTypeModels> GetTravelRequestTypeList()
        {
            throw new System.NotImplementedException();
        }

        public int Save(TravelRequestTypeModels travelRequestTypeModel)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}