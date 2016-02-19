using Gym_Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Membership.Repositories.Abstract
{
    interface IAdminRepository
    {
        bool CreateAdmin(AdminUser user);
        bool UpdateAdmin(AdminUser user);

        bool ChangePassword(ChangePasswordViewModel password);

        AdminUser FindAdmin(LoginState user);

        bool InsertAccessLog(AccessLog log);

        bool CreateMembership(Membership membership);
        bool UpdateMembership(Membership membership);

        IList<Membership> FetchMemberships(int? id);

        IList<AdminUser> FetchAdmins();
        int SaveOrCreateAdmin(AdminUser user);

        string UpdateAdminDetails(string username, string item, string value);

        bool DeleteAdmin(string userName);

        bool CascadeDeleteMember(int memberId);

        bool UpdateMembersMembership(ChangeMembershipModel model);

     //   IList<Configuration> FetchConfigurations();

        // bool UpdateConfiguration(string key, string value);

        bool UpdateMembershipFee(string key, float value);

        IList<Stat> FetchMemberProfiles();

        IList<ReceiptReport> FetchReceiptReport(DateTime? from = null, DateTime? to = null);

        IList<Stat> FetchVisitDetails();

        IList<Stat> FetchRevenueDetails();
        IList<Stat> FetchBudgetedRevenue();
    }
}
