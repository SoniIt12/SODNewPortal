using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("tbl_designation")]
    public class DesignationModels
    {
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
         [Key]
         public int id { get; set; }
         public string designation_name { get; set; }
	     public int designation_level  { get; set; }
	     public int tbl_emp_dept_mapping_id { get; set; }
      }
	 
 }