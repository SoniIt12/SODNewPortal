using System;
using System.Collections;
using System.Collections.Generic;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface IDepartmentRepository:IDisposable
    {
        IList<DepartmentModels> GetDepartmentList();

        IList<DepartmentModels> GetDepartmentVerticalList();
    }
}