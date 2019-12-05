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
    public  class LoggingRepository:ILoggingRepository
    {
        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly SodEntities _context;
        public LoggingRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }


        /// <summary>
        /// Save logg/Error into database
        /// </summary>
        /// <param name="loggingModel"></param>
        /// <returns></returns>
        public string SaveDBLogg(LoggingModels loggingModel)
        {
            this._context.LoggingModel.Add(loggingModel);
            _context.SaveChanges();
            return "ER001";
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
