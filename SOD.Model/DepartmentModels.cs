using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
   
    [Table("tbl_department")]
    public class DepartmentModels
    {
        
          /// <summary>
         /// Columns Name are created as per table Column Name schema
         /// </summary>
         [Key]
         public int id { get; set; }
         public string dept_name { get; set; }
         public short dept_isactive { get; set; }
         public string vertical_name { get; set; }
         public string vertical_id { get; set; }
    }


    /// <summary>
    /// Vertical Info
    /// </summary>
    public class VerticalModels
    {
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
        public int DepartmentId { get; set; }
        public string VerticalID { get; set; }
        public string VerticalName { get; set; }
    }
}