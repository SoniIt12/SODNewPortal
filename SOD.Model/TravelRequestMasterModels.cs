using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("SodTravelRequestMaster")]
    public class TravelRequestMasterModels
    {
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
        [Key]
        public Int64 TravelRequestId { get; set; }
        public string TravelRequestCode { get; set; }
        public int TravelRequestTypeId { get; set; }
        public DateTime RequestDate { get; set; }
        public Int16 SodBookingTypeId { get; set; }
        public string BookingFor { get; set; }
        public short IsMandatoryTravel { get; set; }
        public string ReasonForMandatoryTravel { get; set; }
        public string Meals { get; set; }
        public string ReasonForTravel { get; set; }
        public int RequestedEmpId { get; set; }
        public string RequestedEmpCode { get; set; }
        public string RequestedEmpName { get; set; }
        public string RequestedEmpDesignation { get; set; }
        public string RequestedEmpDept { get; set; }
        public string EmailId { get; set; }
        public string Phno { get; set; }
        public Int16 Passengers { get; set; }
        public string Pnr { get; set; }
        public decimal PnrAmount { get; set; }
        public string PassEmailId { get; set; }
        public string PassAddressLine1 { get; set; }
        public string PassAddressLine2 { get; set; }
        public string PassCity { get; set; }
        public string PassCountry { get; set; }
        public string CLevelApprover1 { get; set; }
        public string CLevelApprover2 { get; set; }
        public string BookingStatus { get; set; }
        public DateTime StatusDate { get; set; }
        public string Title { get; set; }
        public bool IsCabRequired { get; set; }
        public bool IsAmountPaidByTraveller { get; set; }
        public bool IsHotelRequired { get; set; }
        public bool IsOKtoBoard { get; set; }
        public bool? IsVendorBooking { get; set; }
        public bool? IsSJSC { get; set; }
        public bool HotelApproval { get; set; }
        public string SJSCHodEmailId { get; set; }
        [NotMapped]
        public string ArrivalTime { get; set; }
        [NotMapped]
        public string DepartureTime { get; set; }
        [NotMapped]
        public string FlightName { get; set; }
        [NotMapped]
        public string FlightNo { get; set; }
        [NotMapped]
        public string FlightTypes { get; set; }
        [NotMapped]
        public string TravelDate { get; set; }
        [NotMapped]
        public string OriginPlace { get; set; }
        [NotMapped]
        public string DestinationPlace { get; set; }
        [NotMapped]
        public string EmployeeVertical { get; set; }
        [NotMapped]
        public string DesignationID { get; set; }
        [NotMapped]
        public string EmpVerticalDescription { get; set; }
        [NotMapped]
        public int DepartmentID { get; set; }
        //[NotMapped]
        //public ICollection<FlightDetailModels> FlightDetailModels { get; set; }
    }

    public class TravelRequestMasterModel
    {
        public ICollection<TravelRequestMasterModels> TravelRequestMaster { get; set; }
        public ICollection<FlightDetailModels> FlightDetailModels { get; set; }
    }
    }
    