using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SodCXO_Dept_Mapping")]
    public class SodCXODeptMappingModels
    {

        /// <summary>
        /// CXO Department Mapping
        /// </summary>

        public int Id { get; set; }
        public string CXOName { get; set; }
        public int AllocatedDeptId { get; set; }
        [NotMapped]
        public string AllocatedDeptName { get; set; }
        public short IsActive { get; set; }
    }
}
