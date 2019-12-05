using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
     [Table("SodTravelRequestType")]
    public class TravelRequestTypeModels
    {
         /// <summary>
        /// Travel Request Type Master Properties
        /// </summary>
        [Key]
        public short TravelRequestTypeId { get; set; }
        public string TravelRequestTypeName { get; set; }
    }
}