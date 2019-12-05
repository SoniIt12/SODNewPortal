using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("SodBookingType")]
    public class SodBookingTypeModels
    {
        /// <summary>
        /// Sod Booking Type Properties
        /// </summary>
        [Key]
        public short SodBookingTypeId { get; set; }
        public int SodBookingTypeName { get; set; }
        
    }
}