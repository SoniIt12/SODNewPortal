using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{

     [Table("SodTravelRequestPassengerDetail")]
    public class PassengerDetailModels
    {
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string Title { get; set; }
        public string TravelerFirstName { get; set; }
        public string TravelerLastName { get; set; }
        public string TravelerGender { get; set; }
      
    }
}