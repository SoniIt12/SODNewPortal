using System;
using System.Collections.Generic;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface ITravelRequestTypeRepository:IDisposable
    {
        bool GetTravelRequestTypeName(Int16 travelRequestTypeId);

        IEnumerable<TravelRequestTypeModels> GetTravelRequestTypeList();

        int Save(TravelRequestTypeModels travelRequestTypeModel); 
    }
}