using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("SodCityCodeMaster")]
    public class SodCityCodeMasterModels
    { 
        
          /// <summary>
         /// Columns Name are created as per table Column Name schema
         /// </summary>
         [Key]
         public Int64 Id { get; set; }
         public string CityName { get; set; }
         public string CityCode { get; set; }
         public string Type { get; set; } 

    }
}