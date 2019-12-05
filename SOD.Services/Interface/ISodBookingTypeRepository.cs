using System;
using System.Collections.Generic;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface ISodBookingTypeRepository:IDisposable
    {
        bool GetSodBookingTypeName(Int16 sodBookingTypeId);

        IEnumerable<SodBookingTypeModels> GetSodBookingTypeList();

        int Save(SodBookingTypeModels sodApproverModels); 
    }
}