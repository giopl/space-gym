using Gym_Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Membership.Services.Abstract
{
    interface IAdminService
    {


        bool AddAdmin(AdminUser user);
      //  bool SaveAdmin(AdminUser user);

        LoginState VerifyUser(LoginState user);

        void SaveAccessLog(AccessLog log);

        bool ResetPassword(string userId, string email);

        bool ChangePassword(ChangePasswordViewModel password);

        bool AddMembership(Membership membership);
        bool SaveMembership(Membership membership);

        IList<Membership> GetMemberships(int? id, bool skipCache = false);
        Membership GetMembershipType(string code);

        IList<AdminUser> GetAdmins();

        int SaveAdmin(AdminUser user);

        string SaveAdminDetails(string username, string item, string value);

        bool DeleteAdmin(string username);

        bool DeleteMember(int memberId);

        bool ChangeMembersMembership(ChangeMembershipModel members);

        //     IList<Configuration> GetConfigurations();

        //   bool UpdateConfiguration(string key, string value);

        bool UpdateMembershipFee(string key, float value);

        IList<Stat> GetMemberProfiles();

        IList<Stat> GetVisitDetails();

        IList<Stat> GetRevenueDetails();

         IList<Stat> GetBudgetedRevenue();

        IList<ReceiptReport> GetReceiptReport(DateTime? from = null, DateTime? to = null);

    }
}
