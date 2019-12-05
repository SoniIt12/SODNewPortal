using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    public class EmployeeModel
    {
        /// <summary>
        /// Common Property for Employee
        /// </summary>
        public int EmpId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; } 
        public string Phone { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public string EmployeeVertical { get; set; }
        public string EmployeeVerticleName { get; set; }

        [NotMapped]
        public int SodApproverID { get; set; }

    }



    /// <summary>
    /// To populate  Employee Details :bulk booking
    /// </summary>
    public class EmployeeCodewiseDetailModel
    {
        public string EmpCode       { get; set; }
        public string Title         { get; set; }
        public string FirstName     { get; set; }
        public string LastName      { get; set; }
        public string Gender        { get; set; }
        public string Designation   { get; set; }
        public string Department    { get; set; }
        public string PhoneNo      { get; set; }
        public string EmailId       { get; set; }
    }


    /// <summary>
    /// To get All  Employee Data
    /// </summary>
    public class EmployeeViewDetails
    {
        public int EntryID { get; set; }
        public int ID { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int DesignationID { get; set; }
        public string DesignationName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string EmployeeVertical { get; set; }
        public string EmpVerticalDescription { get; set; }

    }
    
}