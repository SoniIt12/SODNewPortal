using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("PassengerMealAllocationDetail")]
    public class PassengerMealAllocationModels
    {
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
        [Key]
        public Int64 Id              { get; set; }
        public Int64 TravelRequestId { get; set; }
        public Int64 PassengerId     { get; set; }
        public string Sector         { get; set; }
        public string MealType       { get; set; }

        [NotMapped]
        public string Beverages { get; set; }
    }
}
