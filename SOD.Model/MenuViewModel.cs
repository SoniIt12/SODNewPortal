using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    public class MenuViewModel
    {
        
        public int Id { get; set; }
        public int MenuId { get; set; }

        public int ParentId { get; set; }

        public string MenuName { get; set; }

        public string MenuType { get; set; }

        public string MenuPath { get; set; }

        public bool IsActive { get; set; }

        public Int16? OrderPriority { get; set; }

        public string RoleID { get; set; }

    }
}
