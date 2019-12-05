using System.Collections.Generic;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;

namespace SOD.Services.Repository
{
    public class TravelRequestRepository:ITravelRequestRepository
    {
        private readonly SodEntities _context;

        public TravelRequestRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }

        public List<TravelRequestModels> GetTravelRequestDetails()
        {
            throw new System.NotImplementedException();
        }

        public void InsertTravelRequest(TravelRequestModels travelRequestModels)
        {
            throw new System.NotImplementedException();
        }

        public void Save(TravelRequestModels travelRequestModels)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}