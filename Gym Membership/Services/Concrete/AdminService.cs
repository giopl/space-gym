using Gym_Membership.Helpers;
using Gym_Membership.Models;
using Gym_Membership.Repositories.Abstract;
using Gym_Membership.Repositories.Concrete;
using Gym_Membership.Services.Abstract;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Gym_Membership.Services.Concrete
{
    public class AdminService : CoreService, IAdminService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(AdminService));
        public bool AddAdmin(Models.AdminUser user)
        {
            try
            {
                log.InfoFormat("AddAdmin");
                //Generate a temp password for the new user

                string newPassword = Guid.NewGuid().ToString();
                user.Password = Utils.base64Encode(newPassword);

                //Send email
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.SendEmail(user.EmailAddress, "Welcome to MyGym!", String.Format("Hi {0}<br/> Please find your new credentials, <br/> Please login with {1}", user.Fullname, newPassword));



                IAdminRepository adminRepository = new AdminRepository();
                var result = adminRepository.CreateAdmin(user);

                if(result)
                {

                    SaveAccessLog(new AccessLog { Username = UserSession.Current.Username, Operation = "ADD ADMIN", Details = string.Format("Adding new admin user {0}", user.Username), Type = "ADMIN" });
                }

                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("AddAdmin {0}", e.ToString());
                throw;
            }
        }



        public LoginState VerifyUser(LoginState loginState)
        {
            try
            {
                log.InfoFormat("ValidateUser - userId: {0}", loginState.Username);

                var result = LoginState.LoginStateEnum.Login_Successful;


                //check if either username or password is missing
                if (string.IsNullOrWhiteSpace(loginState.Username) || string.IsNullOrWhiteSpace(loginState.Password))
                {
                    result = LoginState.LoginStateEnum.Username_Or_Password_Missing;
                    return new LoginState { LoginStateValue = result };
                }


                //check against user access table
                IAdminRepository repository = new AdminRepository();
                var user = repository.FindAdmin(loginState);


                if (string.IsNullOrWhiteSpace(user.Username))
                {
                    result = LoginState.LoginStateEnum.Unauthorized_Access;


                    SaveAccessLog(new AccessLog { Username = loginState.Username, Operation = "LOGIN-FAILURE", Details = string.Format("Unauthorized Access for {0}", loginState.Username), Type = "USER" });

                    return new LoginState { LoginStateValue = result };
                }

                if (!user.IsActive)
                {
                    result = LoginState.LoginStateEnum.User_Access_Disabled;
                    SaveAccessLog(new AccessLog { Username = loginState.Username, Operation = "LOGIN-FAILURE", Details = string.Format("Access Disabled for {0}", loginState.Username), Type = "USER" });
                    return new LoginState { LoginStateValue = result };
                }

                //bool EnablePassword = HealthCheck.Properties.Settings.Default.EnablePassword;
                bool EnablePassword = ConfigurationHelper.GetIsPasswordEnabled();
                //bool UserValidated = false;



                //if authentication is enabled in web.config
                if (EnablePassword)
                {

                    //Check if password has been resetted.
                    if (user.IsTempPassword)
                    {
                        SaveAccessLog(new AccessLog { Username = loginState.Username, Operation = "TEMP PASSWORD", Details = string.Format("Logged in using temp password for {0}", loginState.Username), Type = "USER" });
                        result = LoginState.LoginStateEnum.Password_Reset;
                        UserSession.Current.Username = user.Username;
                        UserSession.Current.Fullname = user.Fullname;
                        UserSession.Current.ValidUser = true;
                        UserSession.Current.EncryptedPassword = user.Password;
                        UserSession.Current.UserAccessLevel = user.AccessLevelCode;

                        return new LoginState { LoginStateValue = result };
                    }

                    //authenticate against LDAP
                    var isValid = user.IsValid;

                    //Populate session variables as required.
                    if (isValid)
                    {
                        UserSession.Current.Username = user.Username;
                        UserSession.Current.Fullname = user.Fullname;
                        UserSession.Current.ValidUser = true;
                        UserSession.Current.UserAccessLevel = user.AccessLevelCode;


                        //if user is valid updates the count for the user access in DB
                        //IncrementUserAccess();

                        SaveAccessLog(new AccessLog("LOGIN", loginState.Browser, "USER"));
                        //if user is admin, leave system default if not admin, select the first system in the list        

                    }
                    else
                    {
                        SaveAccessLog(new AccessLog { Username = loginState.Username, Operation = "LOGIN-FAILURE", Details = string.Format("Authentication failed for {0}", loginState.Username), Type = "USER" });
                        result = LoginState.LoginStateEnum.Authentication_Failed;
                        return new LoginState { LoginStateValue = result };
                    }
                }
                //if authentication is not enabled, perform a check against web.config password
                else
                {
                    //Retrieve test password from web.config
                    //var TestPassword = HealthCheck.Properties.Settings.Default.TestPassword;
                    var TestPassword = ConfigurationHelper.GetTestPassword();
                    if (loginState.Password == TestPassword)
                    {


                        //UserSession.Current.IsSuperUser=loginState.LoginStateValue
                        try
                        {

                            UserSession.Current.Username = loginState.Username.ToUpper();

                            //UserSession.Current.Username = loginState.Username.ToUpper().Replace("MCB\\", "");
                            //UserSession.Current.Fullname = LDAPHelper.getUserCommonName("MCB\\" + loginState.Username.ToUpper());
                        }
                        //in case of failure (just in case) enter dummy data
                        catch
                        {
                            UserSession.Current.Username = loginState.Username.ToUpper();
                            if (string.IsNullOrWhiteSpace(UserSession.Current.Fullname))
                            {
                                UserSession.Current.Fullname = UserSession.Current.WindowsUser;

                            }

                        }


                        SaveAccessLog(new AccessLog("LOGIN-TEST", loginState.Browser, "USER"));


                    }
                    else
                    {
                        SaveAccessLog(new AccessLog { Username = loginState.Username, Operation = "FAILED LOGIN", Details = "AUTHENTICATION FAILED" });
                        result = LoginState.LoginStateEnum.Authentication_Failed;
                        return new LoginState { LoginStateValue = result };
                    }
                }

                UserSession.Current.ValidUser = true;
                //assigns the user change request history

                return new LoginState { LoginStateValue = result };

            }
            catch (Exception e)
            {
                log.ErrorFormat("CheckUser for userId: {0} - error {1} ", loginState.Username, e.ToString());
                throw;

            }

        }


        public bool ResetPassword(string userId, string email)
        {
            try
            {
                log.InfoFormat("ResetPassword - userId: {0}", userId);

                //First fetch the user to check if email entered is same as the one stored in DB
                IAdminRepository adminRepository = new AdminRepository();

                var user = adminRepository.FindAdmin(new LoginState { Username = userId });
                if (user != null)
                {
                    if (user.EmailAddress == email)
                    {
                        //emails match, can do the reset
                        string newPassword = Guid.NewGuid().ToString().Split('-')[1];

                        //Send email
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.SendEmail(email, "Password resetted!", String.Format("Hi {0}<br/> Your Password has been resetted, <br/> Please login with {1}", user.Fullname, newPassword));


                        //Updates the password in Db
                        //var success = adminRepository.ChangePassword(userId, Utils.base64Encode(newPassword), true);
                        var success = adminRepository.ChangePassword(null);


                        if (success)
                        {
                            SaveAccessLog(new AccessLog("RESET PASSWORD", string.Format("Resetting password for user id {0} ", userId), "PASSWORD"));
                        }
                        return success;
                    }
                }


                return false;
            }
            catch (Exception e)
            {
                log.ErrorFormat("ResetPassword for userId: {0} - error {1} ", userId, e.ToString());
                throw;
            }
        }


        public bool ChangePassword(ChangePasswordViewModel password)
        {
            try
            {
                log.InfoFormat("ChangePassword - userId: {0}", password.Username);

                //First fetch the user to check if email entered is same as the one stored in DB
                IAdminRepository adminRepository = new AdminRepository();

                
                var result = adminRepository.ChangePassword(password);


                if (result)
                {

                    SaveAccessLog(new AccessLog { Username = UserSession.Current.Username, Operation = "CHANGED PASSWORD", Details = string.Format("password changed for user {0}", password.Username), Type = "PASSWORD" });
                }

                return result;

            }
            catch (Exception e)
            {
                log.ErrorFormat("ResetPassword for userId: {0} - error {1} ", password.Username, e.ToString());
                throw;
            }
        }



        public bool AddMembership(Membership membership)
        {
            try
            {
                IAdminRepository adminRepository = new AdminRepository();
                var success = adminRepository.CreateMembership(membership);

                if (success)
                {
                    string CacheKey = CacheHelperKeys.CK_GET_MEMBERSHIP_TYPES;
                    Helpers.CacheHelper.RemoveObjectFromCache(CacheKey);
                    SaveAccessLog(new AccessLog("ADD MEMBERSHIP", string.Format("creating membership for type {0} ", membership.Name), "MEMBERSHIP"));
                }
                return success;

            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool SaveMembership(Membership membership)
        {
            try
            {
                IAdminRepository adminRepository = new AdminRepository();
                var success = adminRepository.UpdateMembership(membership);

                if (success)
                {
                    string CacheKey = CacheHelperKeys.CK_GET_MEMBERSHIP_TYPES;
                    Helpers.CacheHelper.RemoveObjectFromCache(CacheKey);
                    SaveAccessLog(new AccessLog("UPDATE MEMBERSHIP", string.Format("updaging membership for type {0} ", membership.Name), "MEMBERSHIP"));
                }
                return success;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Membership> GetMemberships(int? id, bool skipCache =false)
        {
            try
            {

                string CacheKey = CacheHelperKeys.CK_GET_MEMBERSHIP_TYPES;

                IList<Membership> items = new List<Membership>();

                if (ConfigurationHelper.UseCache())
                {
                    items = Helpers.CacheHelper.GetDataFromCache<List<Membership>>(CacheKey, false);

                }

                if (!ConfigurationHelper.UseCache() || items == null || skipCache)
                {

                    IAdminRepository adminRepository = new AdminRepository();
                    items = adminRepository.FetchMemberships(null);
                    Helpers.CacheHelper.AddObjectToCache(CacheKey, items, null, false, CacheExpiration.Midnight);
                }

                if (id.HasValue)
                    items = items.Where(x => x.MembershipId == id).ToList();

                return items;

            }
            catch (Exception)
            {

                throw;
            }

        }


        public Membership GetMembershipType(string code)
        {
            try
            {


                var memberships = GetMemberships(null);
                var membership = memberships.Where(x => x.MembershipCode == code).ToList();
                if (membership.Count == 1)
                {
                    return membership.FirstOrDefault();
                }
                return new Membership();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<AdminUser> GetAdmins()
        {
            try
            {

                string CacheKey = Helpers.CacheHelperKeys.CK_GET_ADMINS;

                IList<AdminUser> items = new List<AdminUser>();

                if (ConfigurationHelper.UseCache())
                {
                    items = Helpers.CacheHelper.GetDataFromCache<List<AdminUser>>(CacheKey, false);
                }

                if (!ConfigurationHelper.UseCache() || items == null)
                {
                    IAdminRepository adminRepository = new AdminRepository();
                    items = adminRepository.FetchAdmins();
                    Helpers.CacheHelper.AddObjectToCache(CacheKey, items, null, false, CacheExpiration.Midnight);
                }
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public int SaveAdmin(AdminUser user)
        {
            try
            {
                IAdminRepository adminRepository = new AdminRepository();

                string CacheKey = Helpers.CacheHelperKeys.CK_GET_ADMINS;
                Helpers.CacheHelper.RemoveObjectFromCache(CacheKey);
                var success = adminRepository.SaveOrCreateAdmin(user);

                if (success > 0)
                {
                    if (user.UserId > 0)
                    {
                        SaveAccessLog(new AccessLog("UPDATE USER", string.Format("updating user {0} - {1} ", user.UserId, user.Username), "USER"));

                    }
                    else
                    {

                        SaveAccessLog(new AccessLog("CREATE USER", string.Format("creating user {0} - {1} ", user.UserId, user.Username), "USER"));

                    }
                }
                return success;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public string SaveAdminDetails(string username, string item, string value)
        {
            try
            {
                IAdminRepository adminRepository = new AdminRepository();

                string CacheKey = Helpers.CacheHelperKeys.CK_GET_ADMINS;
                Helpers.CacheHelper.RemoveObjectFromCache(CacheKey);
                var result = adminRepository.UpdateAdminDetails(username, item, value);


                if (result.Contains("successfully"))
                {
                    SaveAccessLog(new AccessLog { Username = UserSession.Current.Username, Operation = "UPDATE ADMIN", Details = string.Format("Admin details updated for user {0} {1} set to {2}", username, item, value), Type = "ADMIN" });
                }

                return result;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool DeleteAdmin(string username)
        {
            try
            {
                IAdminRepository adminRepository = new AdminRepository();

                bool result = false;
                if (username != Helpers.UserSession.Current.Username)
                {
                    result = adminRepository.DeleteAdmin(username);


                    if (result)
                    {

                        SaveAccessLog(new AccessLog { Username = UserSession.Current.Username, Operation = "DELETE ADMIN", Details = string.Format("Admin user deleted {0}", username), Type = "ADMIN" });
                    }


                }
                if (result)
                {
                    string CacheKey = Helpers.CacheHelperKeys.CK_GET_ADMINS;
                    Helpers.CacheHelper.RemoveObjectFromCache(CacheKey);
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool DeleteMember(int memberId)
        {
            try
            {
                IAdminRepository adminRepository = new AdminRepository();
                var result = adminRepository.CascadeDeleteMember(memberId);


                if (result)
                {

                    SaveAccessLog(new AccessLog { Username = UserSession.Current.Username, Operation = "MEMBER DELETED", Details = string.Format("Member ERASED from system {0}", memberId), Type = "MEMBER" });
                }

                return result;
                    
            }
            catch (Exception)
            {
                
                throw;
            }
        }


        public bool ChangeMembersMembership(ChangeMembershipModel members)
        {
            try
            {
                IAdminRepository adminRepository = new AdminRepository();
                var result = adminRepository.UpdateMembersMembership(members);


                if (result)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(var om in members.MemberIdsList)
                    {
                        sb.Append(om.ToString());
                        sb.Append(",");
                    }
                    sb.Length--;
                    SaveAccessLog(new AccessLog { Username = UserSession.Current.Username, Operation = "MEMBERSHIP CHANGED", Details = string.Format("Membership changed for memberId {0} to {1} and related members {2} ", members.MainMemberId, members.MembershipCode, sb.ToString()), Type = "MEMBER" });
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool UpdateMembershipFee(string key, float value)
        {
            try
            {
                IAdminRepository adminRepository = new AdminRepository();
                        return adminRepository.UpdateMembershipFee(key, value);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Stat> GetMemberProfiles()
        {
            try
            {
                IAdminRepository adminRepository = new AdminRepository();
                return adminRepository.FetchMemberProfiles();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Stat> GetVisitDetails()
        {
            try
            {
                IAdminRepository AdminRepository = new AdminRepository();
                return AdminRepository.FetchVisitDetails();    

            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        public IList<Stat> GetRevenueDetails()
        {

            IAdminRepository AdminRepository = new AdminRepository();
            return AdminRepository.FetchRevenueDetails();

        }

        public IList<Stat> GetBudgetedRevenue()
        {
            IAdminRepository AdminRepository = new AdminRepository();
            return AdminRepository.FetchBudgetedRevenue();

        }


        public IList<ReceiptReport> GetReceiptReport(DateTime? from = default(DateTime?), DateTime? to = default(DateTime?))
        {
            try
            {



                IAdminRepository adminRepository = new AdminRepository();
                return adminRepository.FetchReceiptReport(from, to);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}