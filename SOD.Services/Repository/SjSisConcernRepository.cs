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
    public class SjSisConcernRepository : ISjSisConcernRepository
    {
        /// <summary>
        /// Initialized Constructor
        /// </summary>
        private readonly SodEntities _context;
        public SjSisConcernRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        public Int64 registerUser(SJSCUserMasterModels UserData)
        {
            SJSCUserMasterModels regData = new SJSCUserMasterModels();
            regData.EmpCode = UserData.EmpCode.Trim();
            regData.Title = UserData.Title.Trim();
            regData.FirstName = UserData.FirstName.Trim();
            regData.LastName = UserData.LastName.Trim();
            regData.Designation = UserData.Designation.Trim();
            regData.Department = UserData.Department.Trim();
            regData.EmailID = UserData.EmailID.Trim();
            regData.MobileNo = UserData.MobileNo.Trim();
            regData.SJSCVerticalID = UserData.SJSCVerticalID;
            regData.Pwd = UserData.Pwd.Trim();
            regData.HodTitle = UserData.HodTitle.Trim();
            regData.HodName = UserData.HodName.Trim();
            regData.HodEmailId = UserData.HodEmailId.Trim();
            regData.HodMobileNo = UserData.HodMobileNo.Trim();
            regData.UserId = UserData.EmailID;
            regData.IsApprover = UserData.EmailID.Trim().Equals(UserData.HodEmailId.Trim()) ? true : false;
            regData.IsActive = true;
            regData.CreateDate = DateTime.Now;
            regData.ModifiedDate = DateTime.Parse("01/01/1900");
            _context.SJSCUserMasterModel.Add(regData);
            //manage sjsc role 
            _context.SaveChanges();
            return regData.ID;
        }

        /// <summary>
        /// Update User info
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        public Int64 UpdateUserInfo(SJSCUserMasterModels UserData)
        {
            var s = 0;
            var getData = _context.SJSCUserMasterModel.Where(x => x.UserId == UserData.EmailID.Trim()).FirstOrDefault();
            getData.ModifiedDate = DateTime.Now;
            getData.Title = (UserData.Title == null) ? getData.Title.Trim() : UserData.Title.Trim();
            getData.FirstName = (UserData.FirstName == null) ? getData.FirstName.Trim() : UserData.FirstName.Trim();
            getData.LastName = (UserData.LastName == null) ? getData.LastName.Trim() : UserData.LastName.Trim();
            getData.EmpCode = (UserData.EmpCode == null) ? getData.EmpCode.Trim() : UserData.EmpCode.Trim();
            getData.SJSCVerticalID = (UserData.SJSCVerticalID == 0) ? getData.SJSCVerticalID : UserData.SJSCVerticalID;
            getData.Department = (UserData.Department == null) ? getData.Department.Trim() : UserData.Department.Trim();
            getData.Designation = (UserData.Designation == null) ? getData.Designation.Trim() : UserData.Designation.Trim();
            getData.MobileNo = (UserData.MobileNo == null) ? getData.MobileNo.Trim() : UserData.MobileNo.Trim();
            getData.HodEmailId = (UserData.HodEmailId == null) ? getData.HodEmailId.Trim() : UserData.HodEmailId.Trim();
            getData.HodName = (UserData.HodName == null) ? getData.HodName.Trim() : UserData.HodName.Trim();
            getData.HodTitle = (UserData.HodTitle == null) ? getData.HodTitle.Trim() : UserData.HodTitle.Trim();
            s = _context.SaveChanges();
            return s;
        }


        /// <summary>
        /// Update Password
        /// </summary>
        /// <param name="UserData"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Int64 UpdatePassword(PasswordModal UserData, string userID)
        {
            var getData = _context.SJSCUserMasterModel.Where(x => x.UserId == userID.Trim() && x.Pwd == UserData.OldPwd.Trim()).FirstOrDefault();
            getData.Pwd = UserData.NewPwd.Trim();
            getData.ModifiedDate = DateTime.Now;
            _context.SaveChanges();
            return getData.ID;
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="UserData"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Int64 ResetPassword(PasswordModal UserData, string userID)
        {
            var s = 0;
            SJSCUserMasterModels getData = _context.SJSCUserMasterModel.Where(x => x.UserId == userID.Trim() && x.IsActive == true).FirstOrDefault();
            if (getData.ResetPwd == false)
            {
                getData.Pwd = UserData.NewPwd.Trim();
                getData.ResetPwd = true;
                getData.ModifiedDate = DateTime.Now;
                s = _context.SaveChanges();
            }
            return s;
        }

        /// <summary>
        /// set the resetPwd to 0
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public Int64 modifyPwd(string userID)
        {
            var s = 0;
            var getData = _context.SJSCUserMasterModel.Where(x => x.UserId == userID && x.IsActive == true).FirstOrDefault();
            getData.ResetPwd = false;
            return s = _context.SaveChanges();
        }

        /// <summary>
        /// to check password is reset or not
        /// </summary>
        public string IsPwdreset(string userID)
        {
            var s = 0;
            var getData = _context.SJSCUserMasterModel.Where(x => x.UserId == userID.Trim() && x.IsActive == true).FirstOrDefault();
            if (getData.ResetPwd == true)
            {
                return "yes";
            }
            else
            {
                return "No";
            }
        }

        /// <summary>
        /// validate User-Id and Pwd
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Pwd"></param>
        /// <returns></returns>
        public string validateUserIdAndPwd(string userId, string Pwd)
        {
            var s = string.Empty;
            var datacheck = _context.SJSCUserMasterModel.Where(x => x.UserId == userId.Trim() && x.Pwd == Pwd.Trim()).FirstOrDefault();
            if (datacheck != null)
            {
                if (datacheck.IsVarified != true)
                {
                    return s = "Not varified";
                }
                return s = "Valid";
            }
            else
            {
                return s = "Invalid";
            }
        }

        /// <summary>
        /// Validate Pwd
        /// </summary>
        /// <param name="Pwd"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string ValidatePwd(string Pwd, string userID)
        {
            var s = string.Empty;
            var datacheck = _context.SJSCUserMasterModel.Where(x => x.UserId == userID.Trim() && x.Pwd == Pwd.Trim()).FirstOrDefault();
            if (datacheck != null)
            {
                return s = "Valid";
            }
            else
            {
                return s = "Invalid";
            }
        }

        /// <summary>
        /// check User Id
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        public Int64 checkUserId(SJSCUserMasterModels UserData)
        {
            var datacheck = _context.SJSCUserMasterModel.Where(x => x.EmailID == UserData.EmailID.Trim()).FirstOrDefault();
            return datacheck != null ? datacheck.ID : 0;
        }
        /// <summary>
        /// To check duplicacy of user via userID and EmpCOde
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public Int64 checkUserIdWithEmpCode(string userID, string EmpCode)
        {
            var datacheck = _context.SJSCUserMasterModel.Where(x => x.EmailID == userID.Trim() || x.EmpCode == EmpCode).FirstOrDefault();
            return datacheck != null ? datacheck.ID : 0;
        }

        /// <summary>
        /// Find Duplicate Hotel Data
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="checkin"></param>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        public bool FindDuplicateDataHotel(string userId, DateTime checkin, string hotelcity)
        {
            var list = _context.TravelRequestHotelDetailModel.Where(o => o.EmployeeCode.Trim() == userId.Trim() &&
                o.CheckInDate == checkin && o.City.Trim() == hotelcity.Trim() && o.IsCancelled == 0).ToList();
            if (list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get User Data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SJSCUserMasterModels GetUserData(string userId)
        {
            var userdata = new SJSCUserMasterModels();
            userdata = _context.SJSCUserMasterModel.Where(x => x.UserId == userId.Trim()).FirstOrDefault();
            return userdata;
        }

        /// <summary>
        /// get verticals
        /// </summary>
        /// <returns></returns>
        public List<SJSCVerticalMasterModels> GetVerticals()
        {
            var list = new List<SJSCVerticalMasterModels>();
            list = _context.SJSCVerticalMasterModel.ToList();
            return list;
        }

        /// <summary>
        /// verify User ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Int64 verifyUserID(string userId)
        {
            int s = 0;
            var userdata = _context.SJSCUserMasterModel.Where(x => x.UserId == userId.Trim()).ToList();
            if (userdata.Count > 0)
            {
                if (userdata[0].IsVarified == true)
                {
                    return s = -1;
                }
                else
                {
                    userdata[0].IsVarified = true;
                    s = _context.SaveChanges();
                }
            }
            return s;
        }

        /// <summary>
        /// Get HOD Details
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<HodDetails> GetHodDetails(string HodMailId)
        {
            var list = new List<HodDetails>();
            var lst = new HodDetails();
            var data = _context.SJSCUserMasterModel.Where(a => a.HodEmailId == HodMailId).ToList();
            lst.HodEmail = data[0].HodEmailId;
            lst.HodMobileNo = data[0].HodMobileNo;
            lst.HoDName = data[0].HodName;
            lst.HoDTitle = data[0].HodTitle;
            list.Add(lst);
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
