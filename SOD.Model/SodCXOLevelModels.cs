using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SodCXOLevel")]
    public class SodCXOLevelModels
    {

    /// <summary>
    /// To Mapped SodCXOLevel Property columns
    /// </summary>
   
    public int   Id             {get;set;}
	public string CXOName       {get;set;}
	public int DesignationId    {get;set;}
	public int DepartmentId     {get;set;}
	public bool IsActive        {get;set;}
    public string EmailId       {get;set;}
    public int EmpId            {get;set;}
  }
}
