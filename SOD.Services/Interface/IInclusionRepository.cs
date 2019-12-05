using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Interface
{
    public interface IInclusionRepository : IDisposable
    {         
        /// <summary>
        /// get hotel inclusion master list
        /// </summary>
        /// <returns></returns>
        List<HotelInclusionMasterModels> GetHotelInclusionList();

        /// <summary>
        /// get hotel list data
        /// </summary>
        /// <returns></returns>
        List<SodHotelListDataModels> GetHotelListData();

        /// <summary>
        /// Get Inclusion Info By Id
        /// </summary>
        /// <returns></returns>
        List<HotelInclusionMasterModels> GetInclusionInfoById(string id);

        /// <summary>
        /// get hotel info by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Dictionary<string, object> GetHotelInfoById(string id);

        /// <summary>
        /// update inclusion master data
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        int UpdateHotelInclusion(List<HotelInclusionMasterModels> elist);

        /// <summary>
        /// update hotel master
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        int UpdateHotelList(List<SodHotelListDataModels> elist, List<SodHotelPriceListMasterModels> plist);

        /// <summary>
        /// add inclusion list
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        int AddHotelInclusion(List<HotelInclusionMasterModels> elist);

        /// <summary>
        /// add hotel data to hotel master
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        int AddNewHotel(List<SodHotelListDataModels> elist, List<SodHotelPriceListMasterModels> plist);

        /// <summary>
        /// get hotel list by name and code
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        List<SodHotelListDataModels> FindHotelListByNameAndCode(string hotelcity, string hotelname);

        /// <summary>
        /// get all inclusions to export to excel
        /// </summary>
        /// <returns></returns>
        List<HotelInclusionMasterModels> GetHotelInclusionDetails_ExportToExcel();

        /// <summary>
        /// get all hotel data to export to excel
        /// </summary>
        /// <returns></returns>
        List<SodHotelListDataModels> GetHotelDataDetails_ExportToExcel();

        /// <summary>
        /// save uploaded hotel contract
        /// </summary>
        /// <param name="hotelCode"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        int SaveUploadData(string hotelCode, string filename);

        List<HotelCurrencyMasterModels> GetCurrencyList();

    }
}
