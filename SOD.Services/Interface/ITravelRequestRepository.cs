using System;
using System.Collections.Generic;
using SOD.Model;
using SOD.Services.Repository;

namespace SOD.Services.Interface
{
    public interface ITravelRequestRepository:IDisposable
    {
        List<TravelRequestModels> GetTravelRequestDetails();

        void InsertTravelRequest(TravelRequestModels travelRequestModels);

        void Save(TravelRequestModels travelRequestModels);

        
    }
}