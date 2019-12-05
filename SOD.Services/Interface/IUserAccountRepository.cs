using System;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface IUserAccountRepository : IDisposable
    {
        
        /// <summary>
        /// Get User Account Info
        /// </summary>
        /// <param name="userLoginDal"></param>
        /// <returns></returns>
        int GetUserLoginInfo(UserAccountModels userLoginDal);



        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="userLoginDal"></param>
        /// <returns></returns>
        int ResetPassword(UserAccountModels userLoginDal);

        /// <summary>
        /// Get Login User Common Info
        /// </summary>
        /// <param name="userLoginDal"></param>
        /// <returns></returns>
        UserAccountModels GetLoginUserList(UserAccountModels userLoginDal);
    }
}