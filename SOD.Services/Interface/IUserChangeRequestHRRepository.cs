using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;

namespace SOD.Services.Interface
{
  public  interface IUserChangeRequestHRRepository
    {
       Int64 SaveChangeRequest(SODEmployeeChangeRequestMaster sodChangeMaster, SODEmployeeChangeRequestDetails sodChangeDetails);

      // SODEmployeeChangeRequestMaster getdata(int idd,string status);

         SodChangeRequestView CRMailData(Int64 idd, string status,string remarks);
        List<SodChangeRequestView> getchangeRequestData(string EmpCode);
        void updateconfirmationMailRemarks(string HRremarks,Int64 ReqID);

        SodChangeRequestView ResendMailtoHRData(Int64 ReqId);

        string GetHREmail(int DeptId);

        List<SodChangeRequestView> ChangeRequestHRRights(string empCode);

        Int64 UserSetDp(SodUserProfileDp model,string EmpCode);

        SodUserProfileDp DisplayDp(string EmpCode);

        List<SodChangeRequestView> ChangeRequestFinanceRights();

        Int64 SaveHRApproval(Int64 ReqId,string status);

        void RemoveDP(string EmpCode);

        bool checkdp(string EmpCode);

        //List<SodUserMenu> AllMenus(int Role);

        //List<SodUserMenu> AllMenus();
        List<SodUserMenu> AllMenus(string Role);

        string GetFinanceEmailId(int DeptId);

        List<SodChangeRequestView> ChangeRequestHRRightsHistory(string empCode);
        List<SodChangeRequestView> ChangeRequestFinanceRightsHistory();
        bool SendMAiltoUserCheck(Int64 Reqid);

        string CheckRoleOfUser(string EmpCOde);
    }
}
