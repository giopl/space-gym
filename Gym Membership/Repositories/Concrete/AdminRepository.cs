using Gym_Membership.Helpers;
using Gym_Membership.Models;
using Gym_Membership.Repositories.Abstract;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Gym_Membership.Repositories.Concrete
{ 
    public class AdminRepository : DataAccess, IAdminRepository
    {
        ILog log = log4net.LogManager.GetLogger(typeof(AdminRepository));



        public bool CreateAdmin(AdminUser user)
        {
            try
            {
                log.Info("[CreateAdmin]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@" 
                        INSERT INTO {0}
           ([username]
           ,[fullname]
           ,[password]
           ,[is_active]
           ,[last_login]
           ,[access_level]
           ,[num_logins]
           ,[is_temp_password]
           ,[email_addr])
            VALUES
                            (@username, @fullname, @password, @is_active, null, @access_level, 0, 'Y', @email_addr)
                ", TableMappings.TBL_ADMIN
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@username", SqlDbType.VarChar, user.Username, 30));
                parameters.Add(CreateParameter("@fullname", SqlDbType.VarChar, user.Fullname, 50));
                parameters.Add(CreateParameter("@password", SqlDbType.VarChar, user.Password, 200));
                parameters.Add(CreateParameter("@is_active", SqlDbType.Int, user.IsActive ? "Y" : "N", 1));
                parameters.Add(CreateParameter("@access_level", SqlDbType.Int, user.AccessLevel));
                parameters.Add(CreateParameter("@email_addr", SqlDbType.VarChar, user.EmailAddress, 200));


                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[CreateAdmin] Error: {0}", e.ToString());

                throw;
            }
        }

        public bool UpdateAdmin(AdminUser user)
        {
            try
            {
                log.Info("[UpdateAdmin]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@" 
                       UPDATE {0}
   SET  
       [fullname] = @fullname
      ,[is_active] = @is_active
      ,[access_level] = @access_level
      ,[email_addr] = @email
 WHERE [username] = @username
                ", TableMappings.TBL_ADMIN
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@fullname", SqlDbType.VarChar, user.Fullname, 50));
                parameters.Add(CreateParameter("@is_active", SqlDbType.Int, user.IsActive ? "Y" : "N", 1));
                parameters.Add(CreateParameter("@access_level", SqlDbType.Int, user.AccessLevel));
                parameters.Add(CreateParameter("@email_addr", SqlDbType.VarChar, user.EmailAddress, 200));
                parameters.Add(CreateParameter("@username", SqlDbType.VarChar, user.Username, 30));

                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[UpdateAdmin] Error: {0}", e.ToString());

                throw;
            }
        }

        public bool ChangePassword(ChangePasswordViewModel password)
        {

            try
            {
                log.Info("[ChangePassword]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                        UPDATE {0}
                       SET [is_temp_password] = @isTempPassword
                          ,[password] = @password
                          
                     WHERE [username] = @user
               
                        ", TableMappings.TBL_ADMIN
                 );

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@password", SqlDbType.VarChar, password.EncryptedPassword, 200));
                parameters.Add(CreateParameter("@user", SqlDbType.VarChar, password.Username, 30));
                parameters.Add(CreateParameter("@isTempPassword", SqlDbType.VarChar, password.IsTempPassword ? "Y" : "N", 1));


                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[ChangePassword] Error: {0}", e.ToString());
                throw;
            }


        }

        public AdminUser FindAdmin(LoginState user)
        {
            try
            {
                log.Info("[FindAdmin]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"

			       select user_id, username, fullname, password, is_active, last_login, access_level, num_logins, email_addr, is_temp_password
                     from {0}
                    where username =  @username

                "
             , TableMappings.TBL_ADMIN
                 );

                DBConnect();

                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add(CreateParameter("@username", SqlDbType.VarChar, user.Username, 20));

                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                AdminUser item = new AdminUser();

                while (reader.Read())
                {
                    int i = 0;
                    item.UserId = ReadInt(ref reader, ref i);
                    item.Username = ReadString(ref reader, ref i);
                    item.Fullname = ReadString(ref reader, ref i);
                    item.Password = ReadString(ref reader, ref i);

                    item.IsValid = item.Password == user.Password;


                    item.IsActive = ReadString(ref reader, ref i) == "Y";
                    item.LastLogin = ReadDate(ref reader, ref i);

                    //use code below if enum is string in database
                    //item.AccessLevel = (AdminUser.AccessLevelEnum)Enum.Parse(typeof(AdminUser.AccessLevelEnum), ReadString(ref reader, ref i));
                    item.AccessLevelCode = ReadInt(ref reader, ref i);
                    item.AccessLevel = (AdminUser.AccessLevelEnum)item.AccessLevelCode;
                    item.NumLogins = ReadInt(ref reader, ref i);
                    item.EmailAddress = ReadString(ref reader, ref i);
                    item.IsTempPassword = ReadString(ref reader, ref i) == "Y";
                }

                reader.Dispose();
                DBDisconnect();

                if(item.UserId>0)
                {
                    updateAdminAccess(item.UserId);
                }

                return item;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[FindUser] Error: {0}", e.ToString());
                throw e;
            }
        }

        private void updateAdminAccess(int id)
        {
            try
            {
                log.Info("[InsertAccessLog]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@" 
                        UPDATE {0} SET [last_login] = getDate(), num_logins = num_logins + 1 WHERE user_id =@userId 
                ", TableMappings.TBL_ADMIN
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@userId", SqlDbType.Int, id));

                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();

            }
            catch (Exception e)
            {
                log.ErrorFormat("[updateAdminAccess] Error: {0}", e.ToString());

                throw;
            }

        }

        public bool InsertAccessLog(AccessLog accesslog)
        {
            try
            {
                log.Info("[InsertAccessLog]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@" 
                        INSERT INTO {0} ([username],[operation],[item_type],[item_id],[log_timestamp],[description])
                            VALUES(@username, @operation,@itemType, @itemId, getDate(), @description)
                ", TableMappings.TBL_LOG
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@username", SqlDbType.VarChar, accesslog.Username, 30));
                parameters.Add(CreateParameter("@operation", SqlDbType.VarChar, accesslog.Operation, 100));
                parameters.Add(CreateParameter("@itemtype", SqlDbType.VarChar, accesslog.Type, 100));
                parameters.Add(CreateParameter("@itemId", SqlDbType.Int, accesslog.ItemId));
                parameters.Add(CreateParameter("@description", SqlDbType.VarChar, accesslog.Details, 1000));

                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[InsertAccessLog] Error: {0}", e.ToString());

                throw;
            }
        }


        public bool CreateMembership(Membership membership)
        {
            try
            {
                log.Info("[CreateMembership]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                        INSERT INTO {0}
                                   ([name]
                                    ,[category]                
,[code]
                
                                     ,[display_order]
                                   ,[description]
                                   ,[registration_fee]
                                   ,[fee]
                                   ,[month_terms]
                                   ,[num_members]
                                   ,[updated_on]
                                   ,[updated_by]
                                ,[membership_rules] )
                        OUTPUT Inserted.membership_id

                        values (@name, @category,@code, @displayOrder, @desc, @regfee, @fee, @months, @nummem, getDate(), @user,@rules)			     
               
                        ", TableMappings.TBL_MEMBERSHIP
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@name", SqlDbType.VarChar, membership.Name, 150));
                parameters.Add(CreateParameter("@category", SqlDbType.VarChar, membership.Category, 20));
                parameters.Add(CreateParameter("@code", SqlDbType.VarChar, membership.MembershipCode, 20));
                parameters.Add(CreateParameter("@displayOrder", SqlDbType.Int, membership.DisplayOrder));

                parameters.Add(CreateParameter("@desc", SqlDbType.VarChar, membership.Description, 500));
                parameters.Add(CreateParameter("@regfee", SqlDbType.Float, membership.RegistrationFee));
                parameters.Add(CreateParameter("@fee", SqlDbType.Float, membership.Fee));
                parameters.Add(CreateParameter("@months", SqlDbType.Int, membership.MonthTerms));
                parameters.Add(CreateParameter("@nummem", SqlDbType.Int, membership.NumberMembers));
                parameters.Add(CreateParameter("@user", SqlDbType.VarChar, UserSession.Current.Username, 50));
                parameters.Add(CreateParameter("@rules", SqlDbType.VarChar, membership.MembershipRules, 1000));

                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[CreateMembership] Error: {0}", e.ToString());
                throw;
            }

        }

        public bool UpdateMembership(Membership membership)
        {
            try
            {
                log.Info("[UpdateMembership]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                        UPDATE {0}
                       SET [name] = @name
                          ,[code] = @code
                          ,[category] = @category
                          ,[display_order] = @displayOrder

                          ,[description] = @desc
                          ,[membership_rules] = @rules
                          ,[registration_fee] = @regfee
                          ,[fee] = @fee
                          ,[month_terms] = @months
                          ,[num_members] = @nummem
                          ,[is_active] = @active
                          ,[updated_on] = getDate()
                          ,[updated_by] = @user

                     WHERE membership_id = @membershipid
                   

		     
               
                        ", TableMappings.TBL_MEMBERSHIP
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@membershipid", SqlDbType.Int, membership.MembershipId));
                parameters.Add(CreateParameter("@name", SqlDbType.VarChar, membership.Name, 150));
                parameters.Add(CreateParameter("@displayOrder", SqlDbType.Int, membership.DisplayOrder));
                parameters.Add(CreateParameter("@code", SqlDbType.VarChar, membership.MembershipCode, 20));
                parameters.Add(CreateParameter("@category", SqlDbType.VarChar, membership.Category, 20));

                parameters.Add(CreateParameter("@desc", SqlDbType.VarChar, membership.Description, 500));
                parameters.Add(CreateParameter("@rules", SqlDbType.VarChar, membership.MembershipRules, 1000));
                parameters.Add(CreateParameter("@regfee", SqlDbType.Float, membership.RegistrationFee));
                parameters.Add(CreateParameter("@fee", SqlDbType.Float, membership.Fee));
                parameters.Add(CreateParameter("@months", SqlDbType.Int, membership.MonthTerms));
                parameters.Add(CreateParameter("@nummem", SqlDbType.Int, membership.NumberMembers));
                parameters.Add(CreateParameter("@user", SqlDbType.VarChar, UserSession.Current.Username, 50));
                parameters.Add(CreateParameter("@active", SqlDbType.VarChar, membership.IsActive ? "Y" : "N", 1));


                DBConnect();
                
                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[UpdateMembership] Error: {0}", e.ToString());
                throw;
            }

        }

        public bool DeleteAdmin(string userName)
        {
            try
            {
                log.Info("[DeleteAdmin]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                        delete from {0} where username = @username
                   

		     
               
                        ", TableMappings.TBL_ADMIN
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@username", SqlDbType.VarChar, userName, 30));

                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();

                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[DeleteAdmin] Error: {0}", e.ToString());
                throw;
            }

        }

        public IList<Membership> FetchMemberships(int? id)
        {
            log.Info("[FetchMemberships]");
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			     SELECT [membership_id]
                  ,[name]
                  ,[code]
                  ,[category]
                  ,[description]
                   ,membership_rules
                  ,[registration_fee]
                  ,[fee]
                  ,[month_terms]
                  ,[num_members]
                  ,[is_active]
                  ,[updated_on]
                  ,[updated_by]
                ,[display_order]
,[is_system]
,[is_pass]
              FROM {0}
                {1}

                    ", TableMappings.TBL_MEMBERSHIP
                     , id.HasValue ? "where membership_id  = @membershipId" : ""
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@membershipId", SqlDbType.Int, id));

                IList<Membership> list = new List<Membership>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    Membership item = new Membership();

                    item.MembershipId = ReadInt(ref reader, ref i);
                    item.Name = ReadString(ref reader, ref i);
                    item.MembershipCode = ReadString(ref reader, ref i);
                    item.Category= ReadString(ref reader, ref i);
                    item.Description = ReadString(ref reader, ref i);
                    item.MembershipRules = ReadString(ref reader, ref i);
                    item.RegistrationFee = ReadDouble(ref reader, ref i);
                    item.Fee = ReadDouble(ref reader, ref i);
                    item.MonthTerms = ReadInt(ref reader, ref i);
                    item.NumberMembers = ReadInt(ref reader, ref i);
                    item.IsActive = ReadString(ref reader, ref i) == "Y";
                    item.UpdatedOn = ReadDate(ref reader, ref i);
                    item.UpdatedBy = ReadString(ref reader, ref i);
                    int dis_ord = ReadInt(ref reader, ref i);
                    item.DisplayOrder = dis_ord == 0 ? 99 : dis_ord;
                    item.IsSystem= ReadString(ref reader, ref i) == "Y";

                    item.IsPass = ReadString(ref reader, ref i) == "Y";


                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();


                return list;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchMemberships] Error: {0}", e.ToString());
                throw e;
            }
        }

        public int SaveOrCreateAdmin(AdminUser user)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                var isInsert = user.UserId == 0;

                if (isInsert)
                {

                    query.AppendFormat(@"
		

                    INSERT INTO {0}
                               ([username]
                               ,[fullname]
                               ,[password]
                               ,[is_active]
                               ,[access_level]
                            ,[num_logins]
                               ,[is_temp_password]
                               ,[email_addr])
 OUTPUT Inserted.user_id
                         VALUES
                               (@username
                               ,@fullname 
                               ,@password 
                               ,'Y'
                               ,@accessLevel
                               ,0
                               ,'Y'
                              ,@emailAddr)



                    ", TableMappings.TBL_ADMIN
                     );


                }
                else
                {
                    query.AppendFormat(@"
                        UPDATE {0}
                           SET [username] = @username
                              ,[fullname] = @fullname
                              ,[password] = @password 
                              ,[is_active] = @isActive
                              ,[last_login] = getDate()
                              ,[access_level] = @accessLevel
                              ,[is_temp_password] = @isTempPassword
                              ,[email_addr] = ,@emailAddr
                         WHERE user_id = @userId
                    ", TableMappings.TBL_ADMIN);

                }


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@userId", SqlDbType.Int, user.UserId));

                parameters.Add(CreateParameter("@username", SqlDbType.VarChar, user.Username, 30));
                parameters.Add(CreateParameter("@fullname", SqlDbType.VarChar, user.Fullname, 50));


                parameters.Add(CreateParameter("@password", SqlDbType.VarChar, user.EncryptedPassword, 200));
                parameters.Add(CreateParameter("@accessLevel", SqlDbType.Int, user.AccessLevelCode));

                parameters.Add(CreateParameter("@emailAddr", SqlDbType.VarChar, user.EmailAddress,200));

                parameters.Add(CreateParameter("@isActive", SqlDbType.VarChar, user.IsActive ? "Y" : "N", 1));              
                parameters.Add(CreateParameter("@isTempPassword", SqlDbType.VarChar, user.IsTempPassword ? "Y" : "N", 1));



                DBConnect();
                var result = 0;
                if (isInsert)
                {
                    result = InsertWithReturnId(query.ToString(), parameters);
                }
                else
                {
                    var success = CreateUpdateDelete(query.ToString(), parameters) > 0;
                    result = user.UserId;
                }

                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[SaveOrCreateAdmin] Error: {0}", e.ToString());
                throw;
            }
        }
        public IList<AdminUser> FetchAdmins()
        {
            try
            {
                  StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			        SELECT [user_id]
                          ,[username]
                          ,[fullname]
                          ,[password]
                          ,[is_active]
                          ,[last_login]
                          ,[access_level]
                          ,[num_logins]
                          ,[is_temp_password]
                          ,[email_addr]
                      FROM {0}
                    ", TableMappings.TBL_ADMIN
                     
                 );


                IList<AdminUser> list = new List<AdminUser>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString());

                while (reader.Read())
                {
                    int i = 0;
                    AdminUser item = new AdminUser();

                    item.UserId = ReadInt(ref reader, ref i);
                    item.Username = ReadString(ref reader, ref i);
                    item.Fullname = ReadString(ref reader, ref i);
                    item.Password = ReadString(ref reader, ref i);
                    item.IsActive = ReadString(ref reader, ref i) == "Y";
                    item.LastLogin = ReadDate(ref reader, ref i);
                    item.AccessLevelCode = ReadInt(ref reader, ref i);
                    item.NumLogins = ReadInt(ref reader, ref i);
                    item.IsTempPassword = ReadString(ref reader, ref i) == "Y";
                    item.EmailAddress = ReadString(ref reader, ref i);

                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();


                return list;
            }
            catch (Exception)
            {
                
                throw;
            }
        }


        public string UpdateAdminDetails(string username, string item, string value)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                var isPassword = item == "password";
                var isAccess = item == "access_level";

              
                    query.AppendFormat(@"
		
                            update {0} set {1} = @value {2} where username = @username

                    ", TableMappings.TBL_ADMIN, 
                     item,
                     isPassword ? ",is_temp_password='Y' " : ""
                     );


              


                List<SqlParameter> parameters = new List<SqlParameter>();


                if (isPassword)
                {
                    value = Helpers.Utils.base64Encode(value);
                }

                if (isAccess)
                {
                    parameters.Add(CreateParameter("@value", SqlDbType.Int, value));
                    
                }
                else
                {
                parameters.Add(CreateParameter("@value", SqlDbType.VarChar, value));


                }
                parameters.Add(CreateParameter("@username", SqlDbType.VarChar, username, 30));

                DBConnect();
         
                    var success = CreateUpdateDelete(query.ToString(), parameters) > 0;

                DBDisconnect();

                string result = string.Concat(item, " could not be updated");

                if (success)
                {
                    result = string.Concat(item, " updated successfully");

                }

                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[SaveOrCreateAdmin] Error: {0}", e.ToString());
                throw;
            }
        }


        public bool CascadeDeleteMember(int memberId)
        {
            try
            {
                
                log.Info("[CascadeDeleteMember]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
               

                    delete from {0} where member_id = @memberId ;

                    delete from {1} where member_id = @memberId;

                    delete from {2} where member_id = @memberId;

                    delete from {3} 
                    where transaction_id in 
                    (
                    select transaction_id from {4}
                    where member_id = @memberId
                    ) ;

                    delete from {5}
                    where transaction_id in 
                    (
                    select transaction_id from {4}
                    where member_id = @memberId
                    ) ;


                    delete from {5}
                    where transaction_id in 
                    (
                    select transaction_id from {4} 
                    where member_id = @memberId
                    ) ;

                    delete from {4} where member_id = @memberId;

                    delete from {7} where member_id = @memberId;
               
                        ", TableMappings.TBL_VISIT // 0
                         ,TableMappings.TBL_COMMENT
                         , TableMappings.TBL_RELATIONSHIP
                         ,TableMappings.TBL_PAYMENT //3
                         , TableMappings.TBL_TRANSACTION
                         , TableMappings.TBL_RECEIPT
                         , TableMappings.TBL_FACILITY
                         , TableMappings.TBL_MEMBER //7

                 );

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberId));


                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[CascadeDeleteMember] Error: {0}", e.ToString());
                throw;
            }
        }


        public bool UpdateMembersMembership(ChangeMembershipModel model)
        {
            try
            {
                var MainMember = model.MainMemberId;

                DiscardRelationship(MainMember);

                var memberIds = model.MemberIdsList;
                memberIds.Add(MainMember);

                int[] memberarray = memberIds.ToArray();
                var Ids = string.Join(",", memberIds);

                Int32? relId = null;

                if(memberIds.Count() >1)
                {
                    IUserRepository userRepository = new UserRepository();
                    relId = userRepository.FetchNextRelationshipRecord();

                    foreach(var memberId in memberIds)
                    {
                        userRepository.InsertRelationship((Int32)relId, memberId);

                    }
                }

                StringBuilder query = new StringBuilder();
                query.AppendFormat(
                    @" update {0} SET membership_type = @membership , relationship_id = @relId where member_id in ({1}) "
                    , TableMappings.TBL_MEMBER
                     ,Ids);

                  List<SqlParameter> parameters = new List<SqlParameter>();
                  parameters.Add(CreateParameter("@membership", SqlDbType.VarChar, model.MembershipCode));       
                parameters.Add(CreateParameter("@relId", SqlDbType.Int, relId));



                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;

            }
            catch (Exception e)
            {
                log.ErrorFormat("[UpdateMembersMembership] Error: {0}", e.ToString());
                
                throw;
            }
        }


        private bool DiscardRelationship(int id)
        {
            try
            {
                   StringBuilder query = new StringBuilder();

                   query.AppendFormat(@"

            DELETE FROM {0}
            where relationship_id in
            (
            select relationship_id from {1}
            where member_id = @memberId
            )
                ;

                UPDATE {1} SET [membership_type] = 'STDM', relationship_id = null 
                where relationship_id in
                (

                select relationship_id from {1}
                where member_id = @memberId
                )
                    ", TableMappings.TBL_RELATIONSHIP
                       , TableMappings.TBL_MEMBER
                       );

                   List<SqlParameter> parameters = new List<SqlParameter>();
                   parameters.Add(CreateParameter("@memberId", SqlDbType.Int, id));


                   DBConnect();

                   var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                   DBDisconnect();


                   return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[DiscardRelationship] Error: {0}", e.ToString());
                
                throw;
            }


        }

        public bool UpdateMembershipFee(string key, float value)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                    UPDATE {0} SET fee =  @value where code = @key

                        ", TableMappings.TBL_MEMBERSHIP
                    );

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@value", SqlDbType.Float, value));
                parameters.Add(CreateParameter("@key", SqlDbType.VarChar, key, 20));


                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[UpdateConfiguration] Error: {0}", e.ToString());

                throw;
            }

        }

        public IList<Stat> FetchMemberProfiles()
        {
            try
            {

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"   
                    SELECT 
                    COUNT(1) AS [COUNT],  m.MEMBERSHIP_TYPE, mb.name, mb.category, m.GENDER,

                    {2}(dob) age_Group
                    FROM {0} m

                    inner join {1} mb
                    on m.membership_type = mb.code

                    GROUP BY 
                     GENDER,
                    m.mEMBERSHIP_TYPE, mb.name, mb.category,
                    {2}(dob)
                    ", TableMappings.TBL_MEMBER
                    , TableMappings.TBL_MEMBERSHIP
                    ,TableMappings.FUNC_GET_AGE_GROUP
                 );


                IList<Stat> list = new List<Stat>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString());

                while (reader.Read())
                {
                    int i = 0;
                    Stat item = new Stat();

                    item.Count = ReadInt(ref reader, ref i);
                    item.Membership = ReadString(ref reader, ref i);
                    item.MembershipName = ReadString(ref reader, ref i);
                    item.MembershipCategory = ReadString(ref reader, ref i);

                    item.Gender = ReadString(ref reader, ref i);
                    item.AgeGroup = ReadString(ref reader, ref i);

                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();


                return list;

            }
            catch (Exception)
            {


                throw;
            }

        }

        public IList<Stat> FetchVisitDetails()
        {

            try
            {

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@" select  cast( v.check_in as date) dt, datepart(HH, v.check_in) hr, 
                          m.membership_type, mb.name, mb.category, m.gender, {3}(m.dob) ageGrp 
                            from {0} v
                            inner join {1} m
                            on m.member_id = v.member_id
                    
left join {2} mb
on m.membership_type = mb.code
  
                                        ", TableMappings.TBL_VISIT,
                                        TableMappings.TBL_MEMBER
                    , TableMappings.TBL_MEMBERSHIP
                                        ,TableMappings.FUNC_GET_AGE_GROUP

                 );


                IList<Stat> list = new List<Stat>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString());

                while (reader.Read())
                {
                    int i = 0;
                    Stat item = new Stat();

              //      item.Count = ReadInt(ref reader, ref i);
                    item.VisitDate = ReadDate(ref reader, ref i);
                    item.VisitHour = ReadInt(ref reader, ref i);
                    item.Membership = ReadString(ref reader, ref i);
                    item.MembershipName = ReadString(ref reader, ref i);
                    item.MembershipCategory = ReadString(ref reader, ref i);

                    item.Gender = ReadString(ref reader, ref i);
                    item.AgeGroup = ReadString(ref reader, ref i);

                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();


                return list;

            }
            catch (Exception)
            {


                throw;
            }
        }

        public IList<Stat> FetchRevenueDetails()
        {
            try
            {

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@" 


                        SELECT [year_month]
                              ,[bill]
                              ,[paid]
                              ,[wo]
                              ,[disc]
                              ,[due]
                              ,[per_month]
                              ,[count]
                              ,[payment_method]
                              ,[receipt_year_month]
                              ,[code]
                              ,[name]
                              ,[category]
                              ,[age_group]
                              ,[gender]
                              ,[fees_due]
                              ,[active_members]
                                ,[reg_amt]
                          FROM {0}
                        order by 1

                                          ", TableMappings.VW_PAYMENT
                                        
                 );


                IList<Stat> list = new List<Stat>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString());

                while (reader.Read())
                {
                    int i = 0;
                    Stat item = new Stat();

                    //      item.Count = ReadInt(ref reader, ref i);
                    item.YearMonthPayment= ReadInt(ref reader, ref i);
                    item.Bill = ReadDouble(ref reader, ref i);
                    item.Paid = ReadDouble(ref reader, ref i);
                    item.WrittenOff = ReadDouble(ref reader, ref i);
                    item.Discounted = ReadDouble(ref reader, ref i);
                    item.InstallmentDue = ReadDouble(ref reader, ref i);
                    item.DuePerPerson = ReadDouble(ref reader, ref i);

                    item.MembersPaid = ReadInt(ref reader, ref i);
                    item.PaymentMethod = ReadString(ref reader, ref i);
                    item.ReceiptYearMonth = ReadInt(ref reader, ref i);

                    item.Membership = ReadString(ref reader, ref i);
                    item.MembershipName = ReadString(ref reader, ref i);
                    item.MembershipCategory  = ReadString(ref reader, ref i);
                    item.AgeGroup = ReadString(ref reader, ref i);
                    item.Gender = ReadString(ref reader, ref i);
                    item.PotentialRevenue = ReadDouble(ref reader, ref i);
                    item.TotalMembers = ReadInt(ref reader, ref i);
                    item.RegistrationAmount = ReadDouble(ref reader, ref i);


                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();


                return list;


            }
            catch (Exception)
            {

                throw;
            }
        }

        
        public IList<Stat> FetchBudgetedRevenue()
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@" 
                    SELECT [fees_due]
                      ,[fee_pax]
                      ,[active_members]
                      ,[code]
                      ,[category]
                      ,[gender]
                      ,[age_group]
                  FROM {0}
                       ", TableMappings.VW_BUDGETED
                 );

                IList<Stat> list = new List<Stat>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString());

                while (reader.Read())
                {
                    int i = 0;
                    Stat item = new Stat();

                    item.PotentialRevenue = ReadDouble(ref reader, ref i);
                    item.DuePerPerson = ReadDouble(ref reader, ref i);
                    item.TotalMembers = ReadInt(ref reader, ref i);
                    item.Membership = ReadString(ref reader, ref i);
                    item.MembershipCategory = ReadString(ref reader, ref i);
                    item.Gender = ReadString(ref reader, ref i);
                    item.AgeGroup = ReadString(ref reader, ref i);

                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();

                return list;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ReceiptReport> FetchReceiptReport(DateTime? from = default(DateTime?), DateTime? to = default(DateTime?))
        {
            try
            {

                StringBuilder query = new StringBuilder();

                var fromDate = from.HasValue ? (DateTime)from : new DateTime(2000, 1, 1);
                var toDate = to.HasValue ? (DateTime)to : new DateTime(2100, 1, 1);

                
                TimeSpan tsStart = new TimeSpan(0, 0, 0);
                fromDate = fromDate.Date + tsStart;


                TimeSpan tsEnd= new TimeSpan(23, 59, 59);
                toDate = toDate.Date + tsEnd;


                query.AppendFormat(@" 
                    select r.receipt_id, r.member_id, m.fullname, ms.name, r.amount_received,r.payment_method, r.received_by, r.received_on, r.transaction_cancelled, t.transaction_id
                    ,t.amount_paid, t.amount_discounted, t.amount_registration, t.amount_writtenoff, t.amount_unpaid, t.period_start_date, t.period_end_date
                    from {0} r
                    inner join {1} m
                    on r.member_id = m.member_id
                    inner join {2} ms
                    on m.membership_type = ms.code
                    left join {3} t
                    on r.transaction_id = t.transaction_id
                    where r.received_on between @from and @to

                       ", TableMappings.TBL_RECEIPT
                       , TableMappings.TBL_MEMBER
                       ,TableMappings.TBL_MEMBERSHIP
                       , TableMappings.TBL_TRANSACTION

                 );

                IList<ReceiptReport> list = new List<ReceiptReport>();

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@from", SqlDbType.DateTime, fromDate));
                parameters.Add(CreateParameter("@to", SqlDbType.DateTime, toDate));



                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    ReceiptReport item = new ReceiptReport();

                    item.ReceiptId = ReadInt(ref reader, ref i);
                    item.MemberId = ReadInt(ref reader, ref i);
                    item.MemberName = ReadString(ref reader, ref i);
                    item.Membership = ReadString(ref reader, ref i);
                    item.ReceiptAmount = ReadDouble(ref reader, ref i);
                    item.PaymentMethod = ReadString(ref reader, ref i);
                    item.ReceivedByUser = ReadString(ref reader, ref i);
                    item.ReceiptDate = ReadDate(ref reader, ref i);
                    item.IsCancelled = ReadString(ref reader, ref i) == "Y";
                    item.TransactionId = ReadInt(ref reader, ref i);

                    item.PaidAmount = ReadDouble(ref reader, ref i);
                    item.DiscountedAmount = ReadDouble(ref reader, ref i);
                    item.RegistrationAmount = ReadDouble(ref reader, ref i);
                    item.WrittenOffAmount = ReadDouble(ref reader, ref i);
                    item.DiscountedAmount = ReadDouble(ref reader, ref i);
                    item.PeriodStart = ReadDate(ref reader, ref i);
                    item.PeriodEnd= ReadDate(ref reader, ref i);

                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();

                return list;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}