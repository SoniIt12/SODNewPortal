using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;

namespace SOD.Services.Repository
{
    public class UserAccountRepository:IUserAccountRepository
    {
        private readonly SodEntities _context;

        public UserAccountRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
       
        /// <summary>
        /// Count User List in Login Db
        /// </summary>
        /// <param name="userLoginDal"></param>
        /// <returns></returns>
        public int GetUserLoginInfo(UserAccountModels userLoginDal)
        {
            var count = _context.SodUsersInfo.Count(u => u.UserName == userLoginDal.UserName && u.Password == userLoginDal.Password);
            return count;
        }

       
        /// <summary>
        /// Get Login User Common Info
        /// </summary>
        /// <param name="userLoginDal"></param>
        /// <returns></returns>
        public UserAccountModels GetLoginUserList(UserAccountModels userLoginDal)
        {
            var s = (from l in _context.SodUsersInfo
                    where l.UserName == userLoginDal.UserName && l.Password == userLoginDal.Password
                    orderby l.UserName 
                    select new
                    {
                        l.DepartmentId,
                        l.DesignationId,
                        l.UserName,
                        l.EmpCode
                    }).ToList();

           
            var lst=new UserAccountModels();

            if (s.Count > 0)
            {
                lst = new UserAccountModels
                {
                    DepartmentId = s[0].DepartmentId,
                    DesignationId = s[0].DesignationId,
                    UserName = s[0].UserName,
                    EmpCode = s[0].EmpCode
                };
            }

            return lst;
        }

       
        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="userLoginDal"></param>
        /// <returns></returns>
        public int ResetPassword(UserAccountModels userLoginDal)
        {
            var s = 0;
            using (_context)
            {
                var result = from u in _context.SodUsersInfo
                             where (u.UserName == userLoginDal.UserName) && (u.Password == userLoginDal.OldPassword)
                             select u;

                if (result.Count() ==1)
                {
                    var dbuser = result.First();
                    dbuser.Password = userLoginDal.Password;
                   s= _context.SaveChanges();   
                }
            }
            return s;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}