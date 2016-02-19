using Gym_Membership.Helpers;
using Gym_Membership.Models;
using Gym_Membership.Repositories.Abstract;
using Gym_Membership.Repositories.Concrete;
using Gym_Membership.Services.Abstract;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Services.Concrete
{

    public class UserService : CoreService, IUserService
    {

        ILog log = log4net.LogManager.GetLogger(typeof(UserService));
        public Models.AdminUser CheckUser(Models.AdminUser user)
        {
            throw new NotImplementedException();
        }

        public bool CheckInMember(int memberId, bool isPass=false)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                
                return userRepository.InsertVisit(memberId, isPass);
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Models.GymMember> FindMemberByNameOrId(string name)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                var users = userRepository.SearchMemberByNameOrId(name);

                return users;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public int SavePass(GymMember member)
        {
            try 
	            {
                    IUserRepository userRepository = new UserRepository();
                    var success  = userRepository.SavePass(member);

                IAdminService adminService = new AdminService();
                var prices = adminService.GetMemberships(null).Where(x => x.IsPass).ToList();
                var oneDay = prices.Where(x => x.MembershipCode== "1DAY").ToList().FirstOrDefault().Fee;
                var tenDay = prices.Where(x => x.MembershipCode == "10VS").ToList().FirstOrDefault().Fee;

                var tran = new Transaction
                {
                    Member = new GymMember { MemberId = success , Membership = new Membership { MembershipCode = member.IsDayPass ? "1DAY" : "10VS" } },
                    //Member.Membership = new Membership { MembershipCode = member.IsDayPass ? "1DAY" : "10VS" },
                    //Comment = member.Comments,
                    PaymentMethodForm = member.PaymentMethod,
                    LastTransactionId = 0,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    PaidAmount = member.IsDayPass ? oneDay : tenDay,
                    IsPass = true
                    

                };

                    if(success > 0)
                    userRepository.SaveTransaction(tran);
       

                if(member.IsDayPass)
                    {

                    userRepository.InsertVisit(success, true);
                    }

                    return success;
	            }
	            catch (Exception e )
	            {
		
		            throw;
	            }
        }


        public IList<Models.GymMember> FindMemberByNameOrId(string name, bool isCheckin = false)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                var users = userRepository.SearchMemberByNameOrId(name);

                if (users.Count == 1 && isCheckin)
                {
                    var usr = users.FirstOrDefault();
                    if (usr.IsActive)
                    {
                        CheckInMember(usr.MemberId);
                    }

                }

                return users;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Models.GymMember GetMemberById(int id, bool isLive = false)
        {
            try
            {
                if (isLive)
                {

                    IUserRepository userRepository = new UserRepository();
                    var items = userRepository.FetchMembers(id, null);

                    if (items.Count > 0)
                    {

                        return items[0];
                    }

                }
                else
                {

                    var members = GetMembers();
                    var member = members.Where(x => x.MemberId == id).ToList();

                    if (member.Count == 1)
                    {

                        var mem = member.FirstOrDefault();
                        mem.Comments = GetCommentsByMember(mem.MemberId);
                        return mem;
                    }
                }



                return new GymMember();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Models.GymMember> GetMembersByDate(DateTime? dt = null)
        {
            throw new NotImplementedException();
        }



        public bool SaveMember(Models.GymMember member)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                var result = userRepository.SaveMemberDetails(member);

                if (result)
                {
                    SaveAccessLog(new AccessLog { Username = UserSession.Current.Username, Operation = "SAVE MEMBER", ItemId = member.MemberId, Details = string.Format("Save/Update Member details for {0} {1}", member.MemberId, member.FullnameFromFirstAndLastName), Type = "MEMBER" });

                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<Models.GymMember> GetMembers(int? id = null, DateTime? dt = null, bool liveOnly = false)
        {
            try
            {

                string CacheKey =

                    id.HasValue ? Helpers.CacheHelperKeys.CK_GET_MEMBER_BY_ID((int)id) :
                    (dt.HasValue ? Helpers.CacheHelperKeys.CK_GET_MEMBERS_BY_DATE((DateTime)dt)
                    : Helpers.CacheHelperKeys.CK_GET_MEMBERS);


                if(id.HasValue)
                {
                    SaveAccessLog(new AccessLog("VIEW", "Visit page of Member", "MEMBER", (int)id));
                }


                IList<GymMember> items = new List<GymMember>();

                if (ConfigurationHelper.UseCache())
                {
                    items = Helpers.CacheHelper.GetDataFromCache<List<GymMember>>(CacheKey, false);

                }

                if (!ConfigurationHelper.UseCache() || items == null || liveOnly)
                {
                    IUserRepository userRepository = new UserRepository();
                    items = userRepository.FetchMembers(id, dt);
                    Helpers.CacheHelper.AddObjectToCache(CacheKey, items, null, false, CacheExpiration.Midnight);
                }
                return items;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool RemoveVisit(int visitId, int memberId)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                var success = userRepository.DeleteVisit(visitId, memberId);
                if (success)
                {
                    SaveAccessLog(new AccessLog("CANCEL CHECK-IN", string.Format("Check-in cancelled for member id {0} ", memberId), "CHECKIN", memberId));
                }
                return success;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public int AddMembers(AddMemberViewModel members)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                var success = userRepository.CreateMembers(members);
                if (success > 0)
                {
                    SaveAccessLog(new AccessLog("CREATE MEMBER", string.Format("created member(s) id {0} ", success), "MEMBER", success));
                }
                return success;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public IList<Transaction> GetTransactionsByMember(int memberId, bool onlyLast)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.FetchTransactionsByMember(memberId, onlyLast);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public IList<MemberComment> GetCommentsByMember(int memberId)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.FetchMemberComments(memberId);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool SaveTransaction(Transaction transaction)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();

                var success = userRepository.SaveTransaction(transaction);

                if (success)
                {
                    SaveAccessLog(new AccessLog("SAVE TRANSACTION", string.Format("Transaction recorded for member id {0} ", transaction.Member.MemberId), "TRANSACTION",transaction.Member.MemberId));
                }
                return success;
            }

            catch (Exception)
            {

                throw;
            }
        }


        public Transaction GetTransactionById(int transactionId)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.FetchTransactionById(transactionId);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public RelatedMembers GetRelatedMembers(int memberId)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.FetchRelatedMembers(memberId);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool CancelTransaction(int transactionId, int memberId)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();

                var result = userRepository.DeleteTransaction(transactionId);

                if (result)
                {
                    SaveAccessLog(new AccessLog("DELETED TRANSACTION", string.Format("Transaction with id {0} deleted from member {1}", transactionId, memberId), "TRANSACTION",memberId ));

                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<Receipt> GetReceiptsByMember(int memberId)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.FetchReceiptsByMember(memberId);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<MemberComment> GetMemberComments(int memberId)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.FetchMemberComments(memberId);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public Transaction GetTransactionDetails(int transactionId)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.FetchTransactionDetails(transactionId);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool SaveComment(MemberComment comment)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.InsertComment(comment);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<GymMember> CheckIfMemberIdExist(AddMemberViewModel membersToCheck)
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.CheckIfMemberIdExist(membersToCheck).ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<Transaction> GetStandingOrders()
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.FetchStandingOrders();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool SaveMemberProfilePic(int memberId, HttpPostedFileBase file)
        {
            //Check if there is a photo.
            bool hasPhoto = file != null && file.ContentLength > 0;
            bool hasSavedPhoto = false;
            string thumbnailPath = Helpers.DirectoryHelper.GetPath("~/Images/MemberImages");

            if (hasPhoto)
            {
                IUserRepository userRepository = new UserRepository();
                string thumbnailExt = System.IO.Path.GetExtension(file.FileName).ToLower();
                //Get the file extension and check if it is in authorised list
                if (Helpers.ConfigurationHelper.AuthorizedImagesExt().Contains(thumbnailExt))
                {
                    //Update the member with the pic extension
                    hasSavedPhoto = userRepository.UpdateMemberProfilePic(memberId, thumbnailExt);
                    if (hasSavedPhoto)
                    {
                        //Save the image

                        //Delete any existing photo first
                        DirectoryHelper.DeleteFilesByMask(thumbnailPath, string.Concat(memberId, ".*"));

                    string path = string.Concat(thumbnailPath, memberId, thumbnailExt);
                    file.SaveAs(path);
                }
            }
            }
            else
            {
                IUserRepository userRepository = new UserRepository();
                hasSavedPhoto = userRepository.UpdateMemberProfilePic(memberId, "");
                SaveAccessLog(new AccessLog("ADD PICTURE", string.Format("added photo for member {0}", memberId), "PICTURE", memberId));

                if (hasSavedPhoto)
                {
                    DirectoryHelper.DeleteFilesByMask(thumbnailPath, string.Concat(memberId, ".*"));
                }
            }

            return hasSavedPhoto;
        }


        //public Pass GetPassVisitor(int id)
        //{
        //    try
        //    {
        //        IUserRepository userRepository = new UserRepository();
        //        return userRepository.FetchPassVisitor(id);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}



        public IList<ListItem> GetHistory()
        {
            try
            {
                IUserRepository userRepository = new UserRepository();
                return userRepository.FetchHistory();
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}