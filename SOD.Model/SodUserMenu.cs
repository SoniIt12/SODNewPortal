using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SodUserMenu")]
   public class SodUserMenu
    {
        [Key]
        public int Id { get; set; }
        public int MenuId { get; set; }

        public int ParentId { get; set; }
        
        public string MenuName { get; set; }

        public string MenuType { get; set; }

        public string MenuPath { get; set; }

        public bool IsActive { get; set; }

        public Int16? OrderPriority { get; set; }

    }
}
