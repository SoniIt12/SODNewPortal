using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;

namespace SOD.Services.Repository
{
     public class cityMasterRepository : IcityMasterRepository
    {
       
            /// <summary>
            /// Initialized Constructor
            /// </summary>
            private readonly SodEntities _context;
            public cityMasterRepository(SodEntities sodEntities)
            {
                this._context = sodEntities;
            }

            /// <summary>
            /// get city list data
            /// </summary>
            /// <returns></returns>
            public List<SodCityCodeMasterModels> GetCityListData()
            {
                var list = new List<SodCityCodeMasterModels>();
                list = _context.SodCityCodeMasterModel.ToList();
                return list;
            }
  
            /// <summary>
            /// Get city Info By Id
            /// </summary>
            /// <returns></returns>
            public Dictionary<string, object> GetCityInfoById(string id)
            {
                Dictionary<string, object> dicInfo = new Dictionary<string, object>();
                var intId = Convert.ToInt32(id);
                var list = new List<SodCityCodeMasterModels>();
                list = _context.SodCityCodeMasterModel.Where(o => o.Id == intId).ToList();         
                dicInfo.Add("cityPopupDetails", list);              
                return dicInfo;
            }


            /// <summary>
            /// update city master data
            /// </summary>
            /// <param name="elist"></param>
            /// <returns></returns>
            public int UpdateCityList(List<SodCityCodeMasterModels> elist)
            {
                var s = 0;
                var id = elist[0].Id;
                var hotelcode = elist[0].CityCode;
                var list = _context.SodCityCodeMasterModel.Where(o => o.Id == id).ToList();
                foreach (var i in list)
                {
                    i.CityName = elist[0].CityName;
                    i.CityCode = elist[0].CityCode;
                    i.Type = elist[0].Type;                    
                }              
                s = _context.SaveChanges();
                return s;
            }
        
            /// <summary>
            /// add city list
            /// </summary>
            /// <param name="elist"></param>
            /// <returns></returns>
            public int AddNewCity(List<SodCityCodeMasterModels> elist)
            {
                _context.SodCityCodeMasterModel.Add(elist[0]);              
               
                return _context.SaveChanges();
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
