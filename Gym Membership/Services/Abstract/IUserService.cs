using Gym_Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Membership.Services.Abstract
{
    interface IUserService
    {
        AdminUser CheckUser(AdminUser user);

        bool CheckInMember(int memberId, bool isPass=false);

        IList<GymMember> FindMemberByNameOrId(string name, bool isCheckin = false);

        IList<GymMember> FindMemberByNameOrId(string name);


        GymMember GetMemberById(int id, bool isLive=false);

        IList<GymMember> GetMembersByDate(DateTime? dt = null);


        List<GymMember> CheckIfMemberIdExist(AddMemberViewModel membersToAdd);

        bool SaveMember(GymMember member);

        IList<GymMember> GetMembers(int? id=null, DateTime? dt = null, bool liveOnly = false);

        
        bool RemoveVisit(int visitId, int memberId);

        int AddMembers(AddMemberViewModel members);


        IList<ListItem> GetHistory();

        /// <summary>
        /// returns a list of transaction by member, 
        /// if OnlyLast will return the last one with its payment breakdown per month
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="OnlyLast"></param>
        /// <returns></returns>
        IList<Transaction> GetTransactionsByMember(int memberId, bool onlyLast=false);

        bool SaveTransaction(Transaction transaction);

        Transaction GetTransactionById(int transactionId);

        RelatedMembers GetRelatedMembers(int memberId);

        bool CancelTransaction(int transactionId, int memberId);

        IList<Receipt> GetReceiptsByMember(int memberId);

        IList<MemberComment> GetMemberComments(int memberId);

        Transaction GetTransactionDetails(int transactionId);

        bool SaveComment(MemberComment comment);

        IList<Transaction> GetStandingOrders();

        bool SaveMemberProfilePic(int memberId, System.Web.HttpPostedFileBase file);

        int SavePass(GymMember member);


        //Pass GetPassVisitor(int id);


    }
}
