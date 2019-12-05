using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;

namespace SOD.Services.Interface
{
    public interface ISjSisConcernRepository : IDisposable
    {
        /// <summary>
        /// To create new user
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        Int64 registerUser(SJSCUserMasterModels UserData);

        /// <summary>
        /// to update the profile of user
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        Int64 UpdateUserInfo(SJSCUserMasterModels UserData);

        string validateUserIdAndPwd(string userId, string Pwd);

        /// <summary>
        /// to check user id is existed or not
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        Int64 checkUserId(SJSCUserMasterModels UserData);

        /// <summary>
        /// to get the info of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        SJSCUserMasterModels GetUserData(string userId);

        /// <summary>
        /// to get list of verticals
        /// </summary>
        /// <returns></returns>
        List<SJSCVerticalMasterModels> GetVerticals();

        /// <summary>
        /// check duplicacy of hotel booking
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="checkin"></param>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        bool FindDuplicateDataHotel(string userId, DateTime checkin, string hotelcity);

        /// <summary>
        /// to verify user id from email
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Int64 verifyUserID(string userId);

        /// <summary>
        /// to validate password
        /// </summary>
        /// <param name="Pwd"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        string ValidatePwd(string Pwd, string userID);

        /// <summary>
        /// to update password
        /// </summary>
        /// <param name="UserData"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        Int64 UpdatePassword(PasswordModal UserData, string userID);

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="UserData"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        Int64 ResetPassword(PasswordModal UserData, string userID);

        /// <summary>
        /// to set the resetpwd is equal to true
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Int64 modifyPwd(string userID);

        /// <summary>
        /// to check password is reset or not
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        string IsPwdreset(string userID);

        /// <summary>
        /// To check duplicacy of user via userID and EmpCOde
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>

        Int64 checkUserIdWithEmpCode(string userID, string EmpCode);

        List<HodDetails> GetHodDetails(string userID);
    }
}
