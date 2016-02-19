using Gym_Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Membership.Repositories.Abstract
{
    interface IUserRepository
    {

        #region visit
        bool InsertVisit(int memberId, bool isPass = false);
        //bool InsertPassVisit(int passId);

        bool DeleteVisit(int visitId, int memberId);
     //   bool DeletePassVisit(int visitId, int passId);

        #endregion

        #region member

        IList<GymMember> SearchMemberByNameOrId(string name);

        IList<GymMember> FetchMembers(int? id = null, DateTime? dt = null);
        
        //IList<Stat> FetchStats(int? memberId = null);

        //int CreateMember(GymMember member);
        int CreateMembers(AddMemberViewModel members);

        RelatedMembers FetchRelatedMembers(int memberId);

        bool SaveMemberDetails(GymMember member);

        #endregion

        #region payment
        IList<Transaction> FetchTransactionsByMember(int memberId, bool onlyLast);

        bool SaveTransaction(Transaction transaction);

        bool DeleteTransaction(int  transactionId);

        IList<Payment> FetchPaymentsByTransaction(int transactionId);

        Transaction FetchTransactionById(int transactionId);

        #endregion

        IList<MemberComment> FetchMemberComments(int memberId);
        IList<ListItem> FetchHistory();

        bool SaveMemberComment(MemberComment comment);

        bool DeleteMemberComment(int commentId);

        IList<Receipt> FetchReceiptsByMember(int memberId);


        Transaction FetchTransactionDetails(int transaction);

        bool InsertComment(MemberComment comment);

        IList<GymMember> CheckIfMemberIdExist(AddMemberViewModel members);

        IList<Transaction> FetchStandingOrders();


        int FetchNextRelationshipRecord();

        bool InsertRelationship(int relId, int memberId);

        bool UpdateMemberProfilePic(int memberId, string photoExt);

        int SavePass(GymMember member);

        //Pass FetchPassVisitor(int id);

        //IList<MemberVisit> FetchVisitsByMember(int id);

    }
}
