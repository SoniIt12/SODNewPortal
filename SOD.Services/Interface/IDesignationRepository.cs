using System;
using System.Collections;
using System.Collections.Generic;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface IDesignationRepository:IDisposable
    {
        IList<DesignationModels> GetDesignationsList(int deptId);
    }
}