using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SOD.Model;

namespace SOD.Services.EntityFramework
{
    public class SodEntities : DbContext
    {
        public SodEntities()
            : base("name=SodEntities")
        {

        }
        public DbSet<TravelRequestTypeModels> SodTravelRequestType { get; set; }
        //public   DbSet<TravelRequestModels> SodTravelRequest { get; set; }
        public DbSet<UserAccountModels> SodUsersInfo { get; set; }
        public DbSet<SodBookingTypeModels> SodBookingType { get; set; }
        public DbSet<SodApproverModels> SodApprovers { get; set; }
        public DbSet<SodApproverOnlyStandbyModels> SodApproverOnlyStandbyModel { get; set; }

        public DbSet<SodBlanketApprovalModels> SodBlanketApprovals { get; set; }
        public DbSet<TravelRequestMasterModels> TravelRequestMasterModel { get; set; }
        public DbSet<FlightDetailModels> FlightDetailModel { get; set; }
        public DbSet<PassengerDetailModels> PassengerDetailModel { get; set; }
        public DbSet<TravelRequestApprovalModels> TravelRequestApprovalModels { get; set; }
        public DbSet<SodCXOLevelModels> SodCXOLevelModel { get; set; }
        public DbSet<LoggingModels> LoggingModel { get; set; }
        public DbSet<SodCXODeptMappingModels> SodCXODeptMappingModel { get; set; }
        public DbSet<PassengerMealAllocationModels> PassengerMealAllocationModel { get; set; }

        //Bulk Upload
        public DbSet<BulkUploadModels> BulkUploadModel { get; set; }
        public DbSet<BulkUploadMasterModels> BulkUploadMasterModel { get; set; }
        public DbSet<BulkBookingRequestApprovalModels> BulkBookingRequestApprovalModel { get; set; }
        public DbSet<EmployeeDepartmentRights> EmployeeDepartmentRight { get; set; }

        //EMDM API Table
        public DbSet<EmdmAPIModels> EmdmAPIModel { get; set; }
        public DbSet<TravelRequestCabDetailModels> TravelRequestCabDetailModel { get; set; }
        public DbSet<TravelRequestHotelDetailModels> TravelRequestHotelDetailModel { get; set; }

        public DbSet<TransportVendorMasterModels> TransportVendorMasterModel { get; set; }

        //OAL
        public DbSet<OALModels> OALModels { get; set; }
        public DbSet<OALPassengerModel> OALPassengerModel { get; set; }
        public DbSet<OALHotelModel> OALHotelModel { get; set; }
        public DbSet<OALCabModel> OALCabModel { get; set; }
        public DbSet<OALTravelRequestMasterModel> OALTravelRequestMasterModel { get; set; }
        public DbSet<OatTravelRequestApprovalModel> OatTravelRequestApprovalModel { get; set; }
        public DbSet<OALUploadModel> OALUploadModel { get; set; }
        public DbSet<TravelRequestMasterModels_RejectwithStandByPNR> TravelRequestMasterModels_RejectwithStandByPNR { get; set; }

        //Hotel 
        public DbSet<SodHotelListDataModels> SodHotelListDataModels { get; set; }
        public DbSet<HotelRequestApprovalModel> HotelRequestApprovalModel { get; set; }
        public DbSet<HotelRequestRejectionModel> HotelRequestRejectionModel { get; set; }
        public DbSet<HotelRequestApprovalOatModels> HotelRequestApprovalOatModel { get; set; }
        public DbSet<HotelRequestCancellationModels> HotelRequestCancellationModel { get; set; }
        public DbSet<HotelRequestCancellationOatModels> HotelRequestCancellationOatModel { get; set; }
        public DbSet<HotelInclusionMasterModels> HotelInclusionMasterModel { get; set; }
        public DbSet<SodHotelPriceListMasterModels> SodHotelPriceListMasterModel { get; set; }
        public DbSet<NonContractualHotelApprovalMasterModels> NonContractualHotelApprovalMasterModel { get; set; }
        public DbSet<HotelRequestHODFinancialApprovalModels> HotelRequestHODFinancialApprovalModel { get; set; }
        public DbSet<HotelRequestHODFinancialApprovalOatModels> HotelRequestHODFinancialApprovalOatModel { get; set; }
        public DbSet<HotelInclusionNonContractualMasterModels> HotelInclusionNonContractualMasterModel { get; set; }
        public DbSet<HotelInclusionNonContractualMasterOatModels> HotelInclusionNonContractualMasterOatModel { get; set; }
        public DbSet<HotelGstDetailModels> HotelGstDetailModel { get; set; }
        public DbSet<HotelCancellationByTraveldeskModel> HotelCancellationByTraveldeskModel { get; set; }

        //Bulk Hotel
        public DbSet<BulkTravelRequestHotelDetailModels> BulkTravelRequestHotelDetailModel { get; set; }
        public DbSet<BulkTravelRequestHotelRejectionDetailModels> BulkTravelRequestHotelRejectionDetailModel { get; set; }
       

        public DbSet<BulkBookingHODFinancialApprovalModels> BulkBookingHODFinancialApprovalModel { get; set; }
        public DbSet<BulkHotelInclusionNonContractualMasterModels> BulkHotelInclusionNonContractualMasterModel { get; set; }

        //OATt ITH model
        public DbSet<IthVendorListDataModels> IthVendorListDataModel { get; set; }
        public DbSet<ITHRequestApprovalModels> ITHRequestApprovalModel { get; set; }
        public DbSet<ITHResponseDetailModels> ITHResponseDetailModel { get; set; }
        public DbSet<ITHFinancialApprovalHODModels> ITHFinancialApprovalHODModel { get; set; }
        public DbSet<ITACodeMasterModel> ITACodeMasterModels { get; set; }

        //City Codes
        public DbSet<SodCityCodeMasterModels> SodCityCodeMasterModel { get; set; }

        //sms
        public DbSet<SmsLoggingModels> SmsLoggingModel { get; set; }

        //SJSC
        public DbSet<SJSCUserMasterModels> SJSCUserMasterModel { get; set; }
        public DbSet<SJSCVerticalMasterModels> SJSCVerticalMasterModel { get; set; }
        public DbSet<HotelCurrencyMasterModels> HotelCurrencyMasterModel { get; set; }

        //Change Request
        public DbSet<SODEmployeeChangeRequestMaster> SODEmployeeChangeRequestMaster { get; set; }
        public DbSet<SODEmployeeChangeRequestDetails> SODEmployeeChangeRequestDetails { get; set; }
        public DbSet<HRDepartmentsRight> HRDepartmentsRight { get; set; }
        public DbSet<SodUserProfileDp> SodUserProfileDp { get; set; }
        public DbSet<CRFinanceUpdateRight> CRFinanceUpdateRight { get; set; }
        public DbSet<CRHRApprovalStatus> CRHRApprovalStatus { get; set; }
        public DbSet<SodUserMenu> SodUserMenu { get; set; }
        public DbSet<SodMenuRight> SodMenuRight { get; set; }
        public DbSet<SODIntegratedApplicationMaster> SODIntegratedApplicationMaster { get; set; }
        public DbSet<SODIntegratedApplicationUserAllocation> SODIntegratedApplicationUserAllocation { get; set; }
        public DbSet<SODUserRoleAllocation> SODUserRoleAllocation { get; set; }
        public DbSet<SODUserRoleMaster> SODUserRoleMaster { get; set; }

        //SOD Vendor Booking 
        public DbSet<VendorModels> NONSODVendorMaster { get; set; }
        public DbSet<VendorHODDetails> NONSODVendorApproverMaster { get; set; }
        public DbSet<VendorApprovallog> NONSODVendorApprovalLog { get; set; }

        //OAT
        public DbSet<OATTravelRequestMasterModal> OATTravelRequestMasterModals { get; set; }
        public DbSet<OATTravelRequestFlightDetailModal> OATTravelRequestFlightDetailModal { get; set; }
        public DbSet<OATTravelRequestPassengerDetailModal> OATTravelRequestPassengerDetailModal { get; set; }
        public DbSet<OATPAXTyepeMasterModal> OATPAXTyepeMasterModal { get; set; }

        public DbSet<OATBookingRightModal> OATBookingRightModal { get; set; }
        public DbSet<OATFlightNoShowReportModal> OATFlightNoShowReportModal { get; set; }
        

        public DbSet<OATUploadItenaryModal> OATUploadItenaryModal { get; set; }

        public DbSet<OATFinancialApprovalMaster_RoisteringModal> OATFinancialApprovalMaster_RoisteringModal { get; set; }
        public DbSet<OATFinancialApprovalDetail_RoisteringModal> OATFinancialApprovalDetail_RoisteringModal { get; set; }

        ///ITH
        public DbSet<ITHVenderModal> ITHVenderModal { get; set; }
        public DbSet<ITHTransactionMasterModal> ITHTransactionMasterModal { get; set; }
        public DbSet<ITHTransactionRejectionModal> ITHTransactionRejectionModal { get; set; }
        public DbSet<ITHTransactionDetailModal> ITHTransactionDetailModal { get; set; }
        public DbSet<ITHTransactionDetailLogModal> ITHTransactionDetailLogModal { get; set; }
        // SMS Notification Log
        public DbSet<SodApproverSMSLogModels> SodApproverSMSLogModel { get; set; }
    }
}