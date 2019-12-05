using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SOD.Services.Interface
{
    public interface IcityMasterRepository : IDisposable
    {      

        /// <summary>
        /// get city list data
        /// </summary>
        /// <returns></returns>
         List<SodCityCodeMasterModels> GetCityListData();


        /// <summary>
        /// get city info by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
         Dictionary<string, object> GetCityInfoById(string id);

        


         /// <summary>
         /// update city master
         /// </summary>
         /// <param name="elist"></param>
         /// <returns></returns>
        int UpdateCityList(List<SodCityCodeMasterModels> elist);

       

        /// <summary>
        /// add city data to city master
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
         int AddNewCity(List<SodCityCodeMasterModels> elist);

       

       
    }
}
