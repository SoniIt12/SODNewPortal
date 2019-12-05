using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Repository
{
    public class InclusionRepository : IInclusionRepository
    {
        
        /// <summary>
        /// Initialized Constructor
        /// </summary>
        private readonly SodEntities _context;
        public InclusionRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }


        /// <summary>
        /// get hotel inclusion master list
        /// </summary>
        /// <returns></returns>
        public List<HotelInclusionMasterModels> GetHotelInclusionList()
        {
            var list = new List<HotelInclusionMasterModels>();
            list = _context.HotelInclusionMasterModel.ToList();
            return list;
        }


        /// <summary>
        /// get hotel list data
        /// </summary>
        /// <returns></returns>
        public List<SodHotelListDataModels> GetHotelListData()
        {
            var list = new List<SodHotelListDataModels>();
            list = _context.SodHotelListDataModels.ToList();
            return list;
        }

        /// <summary>
        /// Get Inclusion Info By Id
        /// </summary>
        /// <returns></returns>
        public List<HotelInclusionMasterModels> GetInclusionInfoById(string id)
        {
            var intId = Convert.ToInt32(id);
            var list = new List<HotelInclusionMasterModels>();
            list = _context.HotelInclusionMasterModel.Where(o=> o.Id==intId).ToList();
            return list;
        }

        /// <summary>
        /// Get hotel Info By Id
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetHotelInfoById(string id)
        {
            Dictionary<string, object> dicInfo = new Dictionary<string, object>();
            var intId = Convert.ToInt32(id);
            var list = new List<SodHotelListDataModels>();
            list = _context.SodHotelListDataModels.Where(o => o.ID == intId).ToList();

            var hotelcode= list[0].HotelCode;
            var priceList = _context.SodHotelPriceListMasterModel.Where(o => o.HotelCode == hotelcode).ToList();

            dicInfo.Add("hotelPopupDetails", list);
            dicInfo.Add("hotelPriceList", priceList);
            return dicInfo;
        }

        /// <summary>
        /// update inclusion master data
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        public int UpdateHotelInclusion(List<HotelInclusionMasterModels> elist)
        {
            var s = 0;
            var id = elist[0].Id;
            var list = _context.HotelInclusionMasterModel.Where(o => o.Id == id).ToList();
            foreach (var i in list)
            {
                i.HotelName = elist[0].HotelName;
                i.Accomodation = elist[0].Accomodation;
                i.AirportTransfers = elist[0].AirportTransfers;
                i.Amenities = elist[0].Amenities;
                i.BuffetTime = elist[0].BuffetTime;
                i.Discount = elist[0].BuffetTime;
                i.DrinkingWater = elist[0].DrinkingWater;
                i.Food = elist[0].Food;
                i.Laundry = elist[0].Laundry;
                i.Location = elist[0].Location;
                i.Newspaper = elist[0].Newspaper;
                i.RoomService = elist[0].RoomService;
                i.TeaMaker = elist[0].TeaMaker;
                i.WiFi = elist[0].WiFi;
                i.CheckinOutHours = elist[0].CheckinOutHours;
                i.RetentionCancellation = elist[0].RetentionCancellation;
                i.SpouseStay = elist[0].SpouseStay;
            }
            s= _context.SaveChanges();
            return s;
        }


        /// <summary>
        /// update hotel master data
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        public int UpdateHotelList(List<SodHotelListDataModels> elist, List<SodHotelPriceListMasterModels> plist)
        {
            var s = 0;
            var id = elist[0].ID;
            var hotelcode = elist[0].HotelCode;
            var list = _context.SodHotelListDataModels.Where(o => o.ID == id).ToList();
            foreach (var i in list)
            {
                i.HotelName = elist[0].HotelName;
                i.Address = elist[0].Address;
                i.City = elist[0].City;
                i.HotelCode = elist[0].HotelCode;
                i.Phone = elist[0].Phone;
                i.PrimaryEmail = elist[0].PrimaryEmail;
                i.SecondaryEmail = elist[0].SecondaryEmail;
                i.StationCode = elist[0].StationCode;
                i.Status = elist[0].Status;
                i.GMname = elist[0].GMname;
                i.ContractStartDate = elist[0].ContractStartDate;
                i.ContractEndDate = elist[0].ContractEndDate;
                i.IsTaxIncluded = elist[0].IsTaxIncluded;
            }
            var adddate = DateTime.Now;
            var pricelist = _context.SodHotelPriceListMasterModel.Where(o => o.HotelId == id).ToList();
            if(pricelist.Count > 0)
            {
                adddate = pricelist[0].AddDate;
                _context.SodHotelPriceListMasterModel.RemoveRange(pricelist);
            }
            
            foreach (var i in plist)
            {
                i.AddDate = adddate;
                i.ModifiedDate = DateTime.Now;
            }
            _context.SodHotelPriceListMasterModel.AddRange(plist);
            s = _context.SaveChanges();
            return s;
        }


        /// <summary>
        /// add inclusion list
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        public int AddHotelInclusion(List<HotelInclusionMasterModels> incList)
        {            
            _context.HotelInclusionMasterModel.Add(incList[0]);
            return  _context.SaveChanges();
        }

        /// <summary>
        /// add hotel list
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        public int AddNewHotel(List<SodHotelListDataModels> elist, List<SodHotelPriceListMasterModels> plist) 
        {
            _context.SodHotelListDataModels.Add(elist[0]);
            _context.SaveChanges();

            var hotelcode= elist[0].HotelCode;
            var hotellist=  _context.SodHotelListDataModels.Where(o => o.HotelCode == hotelcode).ToList();
            var hotelId= hotellist[0].ID;
            foreach (var i in plist)
            {
                i.AddDate = DateTime.Now;
                i.ModifiedDate = DateTime.Now;
                i.HotelId = hotelId;
            }
            _context.SodHotelPriceListMasterModel.AddRange(plist);
            return _context.SaveChanges();
        }


        /// <summary>
        /// hotel list data
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        public List<SodHotelListDataModels> FindHotelListByNameAndCode(string hotelcity, string hotelname)
        {
            var list = new List<SodHotelListDataModels>();
            list = _context.SodHotelListDataModels.Where(o => o.HotelName == hotelname && o.StationCode == hotelcity).ToList();
            return list;
        }

        /// <summary>
        /// get all inclusions to export to excel
        /// </summary>
        /// <returns></returns>
        public List<HotelInclusionMasterModels> GetHotelInclusionDetails_ExportToExcel()
        {
            var list = new List<HotelInclusionMasterModels>();
            list = _context.HotelInclusionMasterModel.ToList();
            return list;
        }

        /// <summary>
        /// get all hotel data to export to excel
        /// </summary>
        /// <returns></returns>
        public List<SodHotelListDataModels> GetHotelDataDetails_ExportToExcel()
        {
            var list = new List<SodHotelListDataModels>();
            list = _context.SodHotelListDataModels.ToList();
            return list;
        }

        /// <summary>
        /// Save hotel contract file
        /// </summary>
        /// <param name="hotelCode"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public int SaveUploadData(string hotelCode, string filename)
        {
            var list = new List<SodHotelListDataModels>();
            list = _context.SodHotelListDataModels.Where(o => o.HotelCode== hotelCode).ToList();
            foreach (var i in list)
            {
                i.ContractFile = filename;
            }
            return _context.SaveChanges();
        }

        /// <summary>
        /// get Currency list data
        /// </summary>
        /// <returns></returns>
        public List<HotelCurrencyMasterModels> GetCurrencyList()
        {
            var list = new List<HotelCurrencyMasterModels>();
            list = _context.HotelCurrencyMasterModel.ToList();
            return list;
        }

        /// <summary>
        /// Dispose Method
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
