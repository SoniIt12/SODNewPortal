using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("SodHotelGSTDetails")]
    public class HotelGstDetailModels
    {
        
            [Key]
            public Int64 ID { get; set; }
            public string CityCode { get; set; }
            public string State { get; set; }
            public string PlaceOfBusiness { get; set; }
            public string GSTIN { get; set; }
            public string ARNNo { get; set; }
         
    }
}
