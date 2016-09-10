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
    public class UserRepository : DataAccess, IUserRepository
    {
        ILog log = log4net.LogManager.GetLogger(typeof(UserRepository));

        #region "Visit Management"
        public bool InsertVisit(int memberId, bool isPass= false)
        {
            try
            {
                log.Info("[FindAdmin]");

                StringBuilder query = new StringBuilder();

                var lastvisit = FetchLastVisit(memberId);

                TimeSpan span = DateTime.Now - lastvisit;
                bool NoDuplicatedCheckin = span.TotalMinutes > (isPass?60*6:120);
                bool result = false;
                if (NoDuplicatedCheckin)
                {
                    query.AppendFormat(@"
			     
                insert into {0} (member_id, check_in) values (@memberId, getDate())

                update {1} set num_visits = coalesce(num_visits,0)+1 , last_visit = (Select max(check_in) from {0} where member_id = @memberId) 
                ,visits_left = case when max_visits > 0 then visits_left -1 else null end
                where member_id = @memberId
                ", TableMappings.TBL_VISIT
                     , TableMappings.TBL_MEMBER
                     );

                    //if(isPass)
                    //{
                    //    query.AppendFormat(@" update {0} set visits_left = visits_left -1 where member = @memberId ", TableMappings.TBL_MEMBER);
                    //}


                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberId));

                    DBConnect();

                    result = CreateUpdateDelete(query.ToString(), parameters) > 1;
                    DBDisconnect();

                }
                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[CheckInMember] Error: {0}", e.ToString());
                throw;
            }
        }

        public bool DeleteVisit(int visitId, int memberId)
        {
            try
            {
                log.Info("[DeleteVisit]");


                StringBuilder query = new StringBuilder();

                var result = false;
                var lastVisit = FetchLastVisit(memberId);
                GymMember g = new GymMember { MemberId = memberId, LastVisit = lastVisit };

                if (g.IsPresent)
                {
                    query.AppendFormat(@"Delete from {0} where visit_id = @visitId ;
                    UPDATE {1} set num_visits = num_visits -1, last_visit = 
                        ( select max(check_in) from {0} where member_id = @memberId)
                            
                     ,visits_left = (case when max_visits > 0 then visits_left + 1 else null end)

                            where member_id = @memberId
                    "
                        , TableMappings.TBL_VISIT
                        , TableMappings.TBL_MEMBER
                        );

                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberId));
                    parameters.Add(CreateParameter("@visitId", SqlDbType.Int, visitId));

                    DBConnect();

                    result = CreateUpdateDelete(query.ToString(), parameters) > 1;
                    DBDisconnect();


                }

                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[CheckInMember] Error: {0}", e.ToString());
                throw;
            }
        }

        //public bool DeletePassVisit(int visitId, int passId)
        //{
        //    try
        //    {
        //        log.Info("[DeleteVisit]");


        //        StringBuilder query = new StringBuilder();

        //        var result = false;
        //        var lastVisit = FetchLastPassVisit(passId);
        //        GymMember g = new GymMember { MemberId = passId, LastVisit = lastVisit };

        //        if (g.IsPresent)
        //        {
        //            query.AppendFormat(@"Delete from {0} where visit_id = @visitId ;
        //            UPDATE {1} set max_visits = max_visits +1, last_visit = 
        //                ( select max(check_in) from {0} where member_id = @memberId)
        //                    where member_id = @memberId
        //            "
        //                , TableMappings.TBL_VISIT
        //                , TableMappings.TBL_MEMBER
        //                );

        //            List<SqlParameter> parameters = new List<SqlParameter>();
        //            parameters.Add(CreateParameter("@memberId", SqlDbType.Int, passId));
        //            parameters.Add(CreateParameter("@visitId", SqlDbType.Int, visitId));

        //            DBConnect();

        //            result = CreateUpdateDelete(query.ToString(), parameters) > 1;
        //            DBDisconnect();


        //        }

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        log.ErrorFormat("[CheckInMember] Error: {0}", e.ToString());
        //        throw;
        //    }
        //}

        //public bool InsertPassVisit(int passId)
        //{
        //    try
        //    {
        //        log.Info("[FindAdmin]");

        //        StringBuilder query = new StringBuilder();

        //        var lastvisit = FetchLastPassVisit(passId);

        //        TimeSpan span = DateTime.Now - lastvisit;
        //        bool NoDuplicatedCheckin = span.TotalMinutes > 60*8;
        //        bool result = false;
        //        if (NoDuplicatedCheckin)
        //        {
        //            query.AppendFormat(@"
			     
        //        insert into {0} (member_id, check_in) values (@passId, getDate())

        //        update {1} set visits_left = coalesce(visits_left,0)-1 , last_visit = (Select max(check_in) from {0} where member_id = @passId) where pass_id = @passId
        //        ", TableMappings.TBL_VISIT,
        //           TableMappings.TBL_MEMBER
        //             );


        //            List<SqlParameter> parameters = new List<SqlParameter>();
        //            parameters.Add(CreateParameter("@passId", SqlDbType.Int, passId));

        //            DBConnect();

        //            result = CreateUpdateDelete(query.ToString(), parameters) > 1;
        //            DBDisconnect();

        //        }
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        log.ErrorFormat("[CheckInMember] Error: {0}", e.ToString());
        //        throw;
        //    }
        //}


        private DateTime FetchLastVisit(int memberId)
        {
            try
            {
                log.Info("[FetchLastVisit]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			     
                select max(check_in) from {0} where member_id = @memberId

                    ", TableMappings.TBL_VISIT
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberId));



                DateTime dt = new DateTime(2000, 1, 1);

                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    dt = ReadDate(ref reader, ref i);
                }
                reader.Dispose();
                DBDisconnect();


                return dt;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchLastVisit] Error: {0}", e.ToString());
                throw e;
            }
        }


        private DateTime FetchLastPassVisit(int passId)
        {
            try
            {
                log.Info("[FetchLastPassVisit]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			     
                select max(check_in) from {0} where member_id = @passId and is_pass = 'Y'

                    ", TableMappings.TBL_VISIT
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@passId", SqlDbType.Int, passId));



                DateTime dt = new DateTime(2000, 1, 1);

                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    dt = ReadDate(ref reader, ref i);
                }
                reader.Dispose();
                DBDisconnect();


                return dt;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchLastPassVisit] Error: {0}", e.ToString());
                throw e;
            }
        }

        #endregion

        #region "Member Management"

 //       private List<GymMember> FetchPassVisits(DateTime? dt= null)
 //       {


 //           try
 //           {
 //               StringBuilder query = new StringBuilder();

 //               query.AppendFormat(@"

 //                       select 
 //                       p.pass_id,
 //                        p.title,
 //                        p.fullname,
 //                        p.gender,
 //                        case visits_allowed when 1 then 'ONED' ELSE 'TEND' end membership,
 //                        case visits_allowed when 1 then 'One Day Pass' ELSE '10 Visit voucher' end membership_name,


 //                           count(v.visit_id) as num_visits,
 //                        p.last_visit


 //                       from {0} p
 //                       inner join {1} v
 //                       on p.pass_id = v.member_id
                        
 //                       where cast(v.check_in  as date) = cast(@p_date as date)

 //                       group by 
 //                       p.pass_id,
 //                       p.title,
 //                        p.fullname,
 //                        p.gender,
                         
 //                        case visits_allowed when 1 then 'ONED' ELSE 'TEND' end ,
 //case visits_allowed when 1 then 'One Day Pass' ELSE '10 Visit voucher' end ,
 //                        p.last_visit

 //                       ", TableMappings.TBL_PASS , TableMappings.TBL_VISIT);


 //               List<SqlParameter> parameters = new List<SqlParameter>();

 //                if (dt.HasValue)
 //               {
 //                   parameters.Add(CreateParameter("@p_date", SqlDbType.DateTime, dt));
 //               }
 //               else
 //               {
 //                   parameters.Add(CreateParameter("@p_date", SqlDbType.DateTime, DateTime.Now));
 //               }

                



 //               SqlDataReader reader = ReadQuery(query.ToString(), parameters);

 //               List<GymMember> list = new List<GymMember>();

 //               while (reader.Read())
 //               {
 //                   GymMember item = new GymMember();
 //                   int i = 0;
 //                   item.MemberId = ReadInt(ref reader, ref i);
 //                   item.Title = ReadString(ref reader, ref i);
 //                   item.Firstname = ReadString(ref reader, ref i);
 //                   item.Gender = ReadString(ref reader, ref i);
 //                   item.Membership.MembershipCode = ReadString(ref reader, ref i);
 //                   item.Membership.Name = ReadString(ref reader, ref i);
 //                   item.NumberVisits = ReadInt(ref reader, ref i);
 //                   item.LastVisit = ReadDate(ref reader, ref i);

 //                   list.Add(item);


 //               }
 //               return list;
 //           }
 //           catch (Exception)
 //           {

 //               throw;
 //           }

 //       }


        public IList<Models.GymMember> FetchMembers(int? id = null, DateTime? dt = null)
        {
            try
            {
                log.Info("[FindAdmin]");

                StringBuilder query = new StringBuilder();
                StringBuilder periodFilter = new StringBuilder();
                StringBuilder userFilter = new StringBuilder();
                StringBuilder passQuery = new StringBuilder();

                query.AppendFormat(@"
			     
                    SELECT m.[member_id]
                          ,m.[title]
                          ,m.[firstname]
                          ,m.[lastname]
                         ,m.[fullname]
                          ,m.[gender]
                          ,m.[dob]
                          ,m.[address_street]
                          ,m.[address_town]
                          ,m.[email_address]
                          ,m.[home_phone]
                          ,m.[office_phone]
                          ,m.[mobile_phone]
                          ,m.[club]
                          ,m.[registration_date]
                          ,m.[is_active]
                          ,m.[membership_type]
              
                                    ,case when m.membership_type = 'CUST' then 'Custom' when m.membership_type = 'TEMP' then 'Temporary' else mb.[name] end
                          ,mb.[membership_rules]
                        ,case when m.membership_type in ( 'CUST', 'TEMP') then m.custom_registration_fee else mb.[registration_fee] end
                          ,case when m.membership_type in ( 'CUST', 'TEMP') then m.custom_monthly_fee else mb.[fee] end
                          ,case when m.membership_type in ( 'CUST', 'TEMP') then 1 else mb.[month_terms] end
                          ,case when m.membership_type in ( 'CUST', 'TEMP') then 1 else mb.[num_members] end
                            
                          ,m.[heard_about_us]
                          ,m.[employer_name]
                          ,coalesce(m.[payment_until], m.registration_date)
                          ,m.[reason_for_leaving]
                          ,m.[created_by]
                          ,m.[updated_by]
                          ,m.[last_updated_on]
                          --,m.[registration_fee]
                          --,m.[monthly_fee]
                        ,m.[num_visits]
                        ,lv.check_in
                        ,m.[occupation]
                        ,m.is_reg_paid
                        ,lv.last_visit_id
                    ,m.last_transaction_id
                    ,m.is_part_payment
                    ,m.installment_date
                    ,m.relationship_id
                    ,m.profile_pic_ext
,m.age
,m.max_visits
,m.visits_left

,case when m.membership_type <>'CUST' and mb.MONTH_terms = 1 then (fee * 12)/365 
           when m.membership_type <>'CUST' and mb.month_Terms = 12 then fee/365
		   when m.membership_type = 'CUST' then (m.custom_monthly_fee * 12) / 365
		   else 0 end * datediff(day,coalesce(m.payment_until,registration_date),getDate()) due_amt_to_date

                      FROM {0} m


                     left join {2} mb
                        on m.membership_type = mb.code

                    left join 
                    (
                    select max(visit_id) last_visit_id, max(check_in) check_in, member_id from {1}
                    where cast(check_in  as date) <= cast(@p_date as date)
                    group by member_id

                    ) lv
                    on m.member_id = lv.member_id

                ", TableMappings.TBL_MEMBER
                 , TableMappings.TBL_VISIT
                 , TableMappings.TBL_MEMBERSHIP

                 );


                periodFilter.AppendFormat(@"
                      left join 
                      (
                      select max(visit_id) visit_id, member_id, max(check_in) check_in, max(check_out) check_out
                      from {0} v
                        where cast(v.check_in  as date) <= cast(@p_date as date)

                      group by member_id
                      ) v
                      on m.member_id = v.member_id

                        where cast(v.check_in  as date) = cast(@p_date as date)
                    ", TableMappings.TBL_VISIT);

                userFilter.Append(@" where m.member_id = @p_member ");

                DBConnect();

                List<SqlParameter> parameters = new List<SqlParameter>();
                if (id.HasValue)
                {
                    query.Append(userFilter);
                    parameters.Add(CreateParameter("@p_member", SqlDbType.Int, id));
                    parameters.Add(CreateParameter("@p_date", SqlDbType.DateTime, DateTime.Now));
                }
                else if (dt.HasValue)
                {
                    query.Append(periodFilter);
                    parameters.Add(CreateParameter("@p_date", SqlDbType.DateTime, dt));

                }
                else if (!dt.HasValue)
                {

                    parameters.Add(CreateParameter("@p_date", SqlDbType.DateTime, DateTime.Now));
                }



                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                List<GymMember> list = new List<GymMember>();

                while (reader.Read())
                {
                    GymMember item = new GymMember();
                    int i = 0;
                    item.MemberId = ReadInt(ref reader, ref i);
                    item.Title = ReadString(ref reader, ref i);
                    item.Firstname = ReadString(ref reader, ref i);
                    item.Lastname = ReadString(ref reader, ref i);
                    item.Fullname = ReadString(ref reader, ref i);

                    item.Gender = ReadString(ref reader, ref i);
                    item.DateOfBirth = ReadDate(ref reader, ref i);
                    item.StreetAddress = ReadString(ref reader, ref i);
                    item.Town = ReadString(ref reader, ref i);
                    item.EmailAddress = ReadString(ref reader, ref i);
                    item.HomePhone = ReadString(ref reader, ref i);
                    item.OfficePhone = ReadString(ref reader, ref i);
                    item.MobilePhone = ReadString(ref reader, ref i);
                    item.Club = ReadString(ref reader, ref i);
                    item.RegistrationDate = ReadDate(ref reader, ref i);
                    item.IsActive = ReadString(ref reader, ref i) == "Y";

                    item.Membership.MembershipCode = ReadString(ref reader, ref i);
                    item.Membership.Name = ReadString(ref reader, ref i);
                    item.Membership.MembershipRules = ReadString(ref reader, ref i);
                    item.Membership.RegistrationFee = ReadDouble(ref reader, ref i);

                    item.Membership.Fee = ReadDouble(ref reader, ref i);
                    item.Membership.MonthTerms = ReadInt(ref reader, ref i);
                    item.Membership.NumberMembers = ReadInt(ref reader, ref i);


                    item.HowYouHeardAboutUs = ReadString(ref reader, ref i);
                    item.Company = ReadString(ref reader, ref i);
                    item.PaymentUntilDate = ReadDate(ref reader, ref i);
                    item.ReasonForLeaving = ReadString(ref reader, ref i);
                    item.CreatedBy.Username = ReadString(ref reader, ref i);
                    item.UpdatedBy.Username = ReadString(ref reader, ref i);
                    item.UpdatedOn = ReadDate(ref reader, ref i);

                    item.NumberVisits = ReadInt(ref reader, ref i);
                    item.LastVisit = ReadDate(ref reader, ref i);
                    item.Occupation = ReadString(ref reader, ref i);
                    item.IsRegistrationPaid = ReadString(ref reader, ref i) == "Y";
                    item.LastVisitId = ReadInt(ref reader, ref i);
                    item.LastTransactionId = ReadInt(ref reader, ref i);
                    item.IsPartPayment = ReadString(ref reader, ref i) == "Y";
                    item.InstallmentDate = ReadDate(ref reader, ref i);
                    item.RelatedMembers.RelationshipId = ReadInt(ref reader, ref i);
                    item.ProfilePicExt = ReadString(ref reader, ref i);

                    item.AgeInputted = ReadInt(ref reader, ref i);
                    item.MaxVisits = ReadInt(ref reader, ref i);
                    item.VisitsLeft = ReadInt(ref reader, ref i);
                    item.AmountDueToDate = ReadDouble(ref reader, ref i);

                    list.Add(item);


                }


                reader.Dispose();

                //if(!id.HasValue)
                //{
                //    var passVisits = FetchPassVisits(dt);
                //    list.AddRange(passVisits);

                //}


                DBDisconnect();

                return list;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[FetchMembers] Error: {0}", e.ToString());
                throw e;
            }
        }

        public IList<GymMember> SearchMemberByNameOrId(string name)
        {
            try
            {
                log.Info("[SearchMemberByNameOrId]");

                int memberid;

                var success = Int32.TryParse(name, out memberid);

                List<SqlParameter> parameters = new List<SqlParameter>();
                IList<GymMember> Found = new List<GymMember>();
                StringBuilder query = new StringBuilder();
                if (success)
                {
                    query.AppendFormat(
                        @"
                        select member_id, firstname, lastname, fullname, is_active, last_visit , 
                        max_visits , visits_left
                        from {0} 
                        T
                        where T.member_id = @memberId
                

                            ", TableMappings.TBL_MEMBER
                            
                            );

                    parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberid));

                    DBConnect();
                    SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                    while (reader.Read())
                    {
                        GymMember item = new GymMember();
                        int i = 0;
                        item.MemberId = ReadInt(ref reader, ref i);
                        item.Firstname = ReadString(ref reader, ref i);
                        item.Lastname = ReadString(ref reader, ref i);
                        item.Fullname = ReadString(ref reader, ref i);
                        item.IsActive = ReadString(ref reader, ref i) == "Y";
                        item.LastVisit = ReadDate(ref reader, ref i);
                        item.MaxVisits = ReadInt(ref reader, ref i);
                        item.VisitsLeft = ReadInt(ref reader, ref i) ;
                        if (item.VisitsLeft > 0)
                            item.IsActive = true;
                        Found.Add(item);
                    }
                    reader.Dispose();
                    DBDisconnect();

                }

                else
                {
                    query.AppendFormat("select member_id, firstname, lastname, fullname, is_active, last_visit, max_visits, visits_left from {0} where 1=1 ", TableMappings.TBL_MEMBER);
                    var parts = name.Split(' ');
                    var n = 1;

                    foreach (var p in parts)
                    {
                        n++;
                        parameters.Add(CreateParameter("@p" + n, SqlDbType.VarChar, string.Concat("%", p.ToUpper(), "%"), 100));
                        //parameters.Add(CreateParameter("@p" + n, SqlDbType.VarChar, string.Concat("%", p.ToUpper(), "%"), 100));
                        query.AppendFormat(" and upper(fullname) LIKE @p{0}  ", n);
                    }

                    DBConnect();
                    SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                    while (reader.Read())
                    {
                        GymMember item = new GymMember();
                        int i = 0;
                        item.MemberId = ReadInt(ref reader, ref i);
                        item.Firstname = ReadString(ref reader, ref i);
                        item.Lastname = ReadString(ref reader, ref i);
                        item.Fullname = ReadString(ref reader, ref i);

                        item.IsActive = ReadString(ref reader, ref i) == "Y";
                        item.LastVisit = ReadDate(ref reader, ref i);
                        item.MaxVisits = ReadInt(ref reader, ref i);
                        item.VisitsLeft = ReadInt(ref reader, ref i);
                        if(item.VisitsLeft > 0)
                        {
                            item.IsActive = true;
                        }

                        Found.Add(item);
                    }
                    reader.Dispose();
                    DBDisconnect();

                }



                return Found;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[SearchMemberByNameOrId] Error: {0}", e.ToString());
                throw e;
            }
        }


        public RelatedMembers FetchRelatedMembers(int memberId)
        {
            log.Info("[FetchRelatedMembers]");
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			     select 
                member_id, title, firstname, lastname, gender, dob, email_address, office_phone, mobile_phone, 
                payment_until, last_visit, is_Reg_paid, last_transaction_id, relationship_id
                 from {0} m
                where member_id in 

               (select member_id from {1} where relationship_id = (select relationship_id from {0} where member_id = @memberId))
                and member_id <> @memberId
              

                    ", TableMappings.TBL_MEMBER
                     , TableMappings.TBL_RELATIONSHIP
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberId));

                IList<GymMember> list = new List<GymMember>();

                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    GymMember item = new GymMember();

                    item.MemberId = ReadInt(ref reader, ref i);
                    item.Title = ReadString(ref reader, ref i);
                    item.Firstname = ReadString(ref reader, ref i);
                    item.Lastname = ReadString(ref reader, ref i);
                    item.Gender = ReadString(ref reader, ref i);
                    item.DateOfBirth = ReadDate(ref reader, ref i);
                    item.EmailAddress = ReadString(ref reader, ref i);
                    item.OfficePhone = ReadString(ref reader, ref i);
                    item.MobilePhone = ReadString(ref reader, ref i);
                    item.PaymentUntilDate = ReadDate(ref reader, ref i);
                    item.LastVisit = ReadDate(ref reader, ref i);
                    item.IsRegistrationPaid = ReadString(ref reader, ref i) == "Y";
                    item.LastTransactionId = ReadInt(ref reader, ref i);
                    item.RelatedMembers.RelationshipId = ReadInt(ref reader, ref i);

                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();

                RelatedMembers result = new RelatedMembers { Members = list };
                return result;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchRelatedMembers] Error: {0}", e.ToString());
                throw e;
            }

        }


        public int CreateMembers(AddMemberViewModel members)
        {
            try
            {
                log.Info("[CreateMembers]");

                var multiple = members.NumMembers > 1;

                var relId = 0;
                if (multiple)
                {
                    relId = FetchNextRelationshipRecord();

                }
                int mainMemberId = 0;
                int i = 0;
                foreach (var m in members.GymMembers)
                {
                    if (members.NumMembers > i)
                    {

                        if (relId > 0)
                        {
                            m.RelatedMembers.RelationshipId = relId;
                            m.IsRegistrationPaid = false;
                            //if (i > 0)
                            //{
                            //    m.IsRegistrationPaid = false;
                            //}
                            //else
                            //{
                            //    m.IsRegistrationPaid = false;
                            //}
                        }

                        m.MemberId = CreateMember(m);

                        if (multiple)
                        {
                            InsertRelationship(relId, m.MemberId);

                        }
                        if (i == 0)
                        {
                            mainMemberId = m.MemberId;
                        }

                    }
                    i++;
                }



                return mainMemberId;// result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[CreateMembers] Error: {0}", e.ToString());
                throw;
            }
        }

        public bool UpdateMemberProfilePic(int memberId, string photoExt)
        {
            try
            {
                log.Info("[UpdateMemberProfilePic]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@" 
                       UPDATE {0}
   SET  
       profile_pic_ext = @photoExt
 WHERE [member_id] = @member_id
                ", TableMappings.TBL_MEMBER
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@photoExt", SqlDbType.VarChar, photoExt, 50));
                parameters.Add(CreateParameter("@member_id", SqlDbType.Int, memberId));

                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[UpdateMemberProfilePic] Error: {0}", e.ToString());

                throw;
            }
        }

        public bool InsertRelationship(int relId, int memberId)
        {


            try
            {
                log.Info("[CreateMember]");

                StringBuilder query = new StringBuilder();

                //TODO implement
                query.AppendFormat(@"
			  
                    INSERT INTO {0} (relationship_id, member_id) 
                    VALUES (@relid, @memberId)
                    ", TableMappings.TBL_RELATIONSHIP
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add(CreateParameter("@relid", SqlDbType.Int, relId));
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberId));



                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[InsertRelationship] Error: {0}", e.ToString());
                throw;
            }
        }

        public int FetchNextRelationshipRecord()
        {
            try
            {
                log.Info("[CreateMember]");

                StringBuilder query = new StringBuilder();

                //TODO implement
                query.AppendFormat(@"
			  
                    select max(relationship_id)+1 from {0}
                    ", TableMappings.TBL_RELATIONSHIP
                 );


                DBConnect();

                SqlDataReader reader = ReadQuery(query.ToString());

                var result = 0;
                while (reader.Read())
                {
                    int i = 0;
                    result = ReadInt(ref reader, ref i);
                }


                reader.Dispose();
                DBDisconnect();



                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[CreateRelationshipRecord] Error: {0}", e.ToString());
                throw;
            }
        }

        private int CreateMember(GymMember member)
        {
            try
            {
                log.Info("[CreateMember]");

                StringBuilder query = new StringBuilder();
                var maxmember = string.Format("(select max(member_id) + 1 from {0} where member_id < {1})", TableMappings.TBL_MEMBER
                    , ConfigurationHelper.BoundaryLimitForVoucherId());
                //TODO implement
                query.AppendFormat(@"
			  
                    INSERT INTO {0}
                               (
                                [member_id],
                                [title]
                               ,[firstname]
                               ,[lastname]
                               ,[fullname]

                               ,[gender]
                               ,[dob]
                               ,[address_street]
                               ,[address_town]
                               ,[email_address]
                               ,[home_phone]
                               ,[office_phone]
                               ,[mobile_phone]
                               ,[club]
                               ,[registration_date]
                               ,[is_active]
                               ,[membership_type]
                               ,[heard_about_us]
                               ,[employer_name]
                               ,[payment_until]
                               ,[created_by]
        
                               ,[last_updated_on]
                               ,[custom_registration_fee]
                               ,[custom_monthly_fee]
                                ,[num_visits]
                             

                               ,[occupation]
                                ,[is_reg_paid]
                            ,relationship_id)
  
                         VALUES ({2} , @title,@firstname,@lastname,@fullname,@gender,@dob,@street,@town,@email,
                                 @home,@office,@mobile,@club, getDate(), 'Y', @type, @heard, @employer, @paymentUntil,
                              @createdBy, getDate(), 
                                @regfee,
                                @monthlyfee,
                                0,
                                @occupation, @isRegPaid ,@relationshipId
                                )
                    


					select member_id  from {0}
					where last_updated_on = (select max(last_updated_on) from {0})

                    ", TableMappings.TBL_MEMBER, TableMappings.TBL_MEMBERSHIP
                     , member.MemberId > 0 ? "@memberid" : maxmember
                     );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@memberid", SqlDbType.Int, member.MemberId));
                parameters.Add(CreateParameter("@title", SqlDbType.VarChar, Utils.ToTitleCase(member.Title), 20));
                parameters.Add(CreateParameter("@firstname", SqlDbType.VarChar, Utils.ToTitleCase(member.Firstname), 50));
                parameters.Add(CreateParameter("@lastname", SqlDbType.VarChar, Utils.ToTitleCase(member.Lastname), 50));

                parameters.Add(CreateParameter("@fullname", SqlDbType.VarChar, string.Concat(Utils.ToTitleCase(member.Firstname), " ", Utils.ToTitleCase(member.Lastname)), 100));


                parameters.Add(CreateParameter("@gender", SqlDbType.VarChar, member.Gender, 1));
                parameters.Add(CreateParameter("@dob", SqlDbType.Date, member.DateOfBirth));
                parameters.Add(CreateParameter("@street", SqlDbType.VarChar, Utils.ToTitleCase(member.StreetAddress), 100));
                parameters.Add(CreateParameter("@town", SqlDbType.VarChar, member.Town, 50));

                parameters.Add(CreateParameter("@email", SqlDbType.VarChar, member.EmailAddress, 100));
                parameters.Add(CreateParameter("@home", SqlDbType.VarChar, member.HomePhone, 20));
                parameters.Add(CreateParameter("@office", SqlDbType.VarChar, member.OfficePhone, 20));
                parameters.Add(CreateParameter("@mobile", SqlDbType.VarChar, member.MobilePhone, 20));
                parameters.Add(CreateParameter("@club", SqlDbType.VarChar, member.Club, 20));
                parameters.Add(CreateParameter("@type", SqlDbType.VarChar, member.Membership.MembershipCode, 20));

                //parameters.Add(CreateParameter("@registrationDate", SqlDbType.Date, member.RegistrationDate));
                parameters.Add(CreateParameter("@heard", SqlDbType.VarChar, member.HowYouHeardAboutUs, 50));
                parameters.Add(CreateParameter("@employer", SqlDbType.VarChar, member.Company, 50));

                var paymentDate = DateTime.Now;
                if (member.Membership.MembershipCode == "FREE")
                {
                    paymentDate = new DateTime(2030, 1, 1);

                }
                parameters.Add(CreateParameter("@paymentUntil", SqlDbType.DateTime, paymentDate));
                //parameters.Add(CreateParameter("@PaymentUntilDate", SqlDbType.Date, member.RegistrationDate));

                parameters.Add(CreateParameter("@createdBy", SqlDbType.VarChar, UserSession.Current.Username, 50));
                parameters.Add(CreateParameter("@regfee", SqlDbType.Float, member.CustomRegistrationFee));
                parameters.Add(CreateParameter("@monthlyfee", SqlDbType.Float, member.CustomMonthlyFee));
                parameters.Add(CreateParameter("@occupation", SqlDbType.VarChar, member.Occupation, 50));
                parameters.Add(CreateParameter("@isRegPaid", SqlDbType.VarChar, member.IsRegistrationPaid ? "Y" : "N", 1));

                parameters.Add(CreateParameter("@relationshipId", SqlDbType.Int, member.RelatedMembers.RelationshipId));




                DBConnect();

                var memberId = 0;
                var success = CreateUpdateDelete(query.ToString(), parameters) > 0;
                if (success)
                {

                    memberId = FetchInsertedMemberId();
                }

                DBDisconnect();


                return memberId;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[CreateMember] Error: {0}", e.ToString());
                throw;
            }
        }


        private int FetchInsertedMemberId(bool isPass=false)
        {
            try
            {                
                var query = string.Format("(select max(member_id)  from {0} where member_id < {1} )"
                    , TableMappings.TBL_MEMBER
                    , isPass ? Int32.MaxValue : ConfigurationHelper.BoundaryLimitForVoucherId()
                    );
                SqlDataReader reader = ReadQuery(query);
                var memberId = 0;
                while (reader.Read())
                {
                    int i = 0;
                    memberId = ReadInt(ref reader, ref i);
                }

                reader.Dispose();
                return memberId;
            }
            catch (Exception)
            {

                throw;
            }
        }



        //private int FetchInsertedPassId()
        //{
        //    try
        //    {
        //        var query = string.Format("(select max(pass_id)  from {0})", TableMappings.TBL_PASS);
        //        SqlDataReader reader = ReadQuery(query);
        //        var passId = 0;
        //        while (reader.Read())
        //        {
        //            int i = 0;
        //            passId = ReadInt(ref reader, ref i);
        //        }

        //        reader.Dispose();
        //        return passId;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        #endregion

        #region "Payment Management"

        public IList<Payment> FetchPaymentsByTransaction(int transactionId)
        {
            log.Info("[FetchPaymentsByTransaction]");
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                    SELECT [transaction_id]
                          ,[year_month]
                          ,[fees]
                          ,[paid]
                          ,[written_off]
                          ,[discounted]
                          ,[due]
                      FROM {0}
                    where transaction_id = @transactionId

                    "
                    , TableMappings.TBL_PAYMENT

                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@transactionId", SqlDbType.Int, transactionId));

                IList<Payment> list = new List<Payment>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    Payment item = new Payment();
                    item.TransactionId = ReadInt(ref reader, ref i);
                    item.YearMonth = ReadInt(ref reader, ref i);
                    item.FeeAmount = ReadDouble(ref reader, ref i);
                    item.PaidAmount = ReadDouble(ref reader, ref i);
                    item.WriteOffAmount = ReadDouble(ref reader, ref i);
                    item.DiscountedAmount = ReadDouble(ref reader, ref i);
                    item.RemainingBalanceAmount = ReadDouble(ref reader, ref i);



                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();


                return list;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchPaymentsByTransaction] Error: {0}", e.ToString());
                throw e;
            }
        }
        public IList<Transaction> FetchTransactionsByMember(int memberId, bool onlyLast)
        {
            log.Info("[FetchTransactionsByMember]");
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			  
              SELECT {0} [transaction_id]
               
                  ,t.[member_id]
	              ,m.title, m.firstname, m.lastname
	              ,m.address_street, m.address_town, m.email_address
	              ,m.home_phone, m.office_phone, m.mobile_phone
	              ,m.payment_until
	              
	              ,mb.code, mb.name, coalesce(m.custom_monthly_fee,mb.fee) fee, mb.month_terms
                  ,t.[transaction_date]
                  ,t.[period_start_date]
                  ,t.[period_end_date]
                 
                  ,t.[amount_paid]
                  ,t.[amount_discounted]
                  ,t.[amount_registration]
                    ,t.amount_writtenoff
                  ,t.[comment]
                   
                   ,t.[amount_unpaid]
                 ,t.[num_facilities_orig]
                  ,t.[num_facilities_left]

                    ,t.amount_first_installment
                    ,t.amount_second_installment


              FROM {1} t
              inner join {2} m
              on t.member_id = m.member_id

              inner join {3} mb
              on m.membership_type = mb.code


            where t.member_id = @memberId
            order by transaction_id desc


                    "
                     , onlyLast ? "TOP 1" : ""
                    , TableMappings.TBL_TRANSACTION
                    , TableMappings.TBL_MEMBER
                    , TableMappings.TBL_MEMBERSHIP

                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberId));

                IList<Transaction> list = new List<Transaction>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    Transaction item = new Transaction();
                    item.TransactionId = ReadInt(ref reader, ref i);
                    //item.ReceiptId = ReadInt(ref reader, ref i);
                    item.Member.MemberId = ReadInt(ref reader, ref i);
                    item.Member.Title = ReadString(ref reader, ref i);
                    item.Member.Firstname = ReadString(ref reader, ref i);
                    item.Member.Lastname = ReadString(ref reader, ref i);
                    item.Member.StreetAddress = ReadString(ref reader, ref i);
                    item.Member.Town = ReadString(ref reader, ref i);
                    item.Member.EmailAddress = ReadString(ref reader, ref i);
                    item.Member.HomePhone = ReadString(ref reader, ref i);
                    item.Member.OfficePhone = ReadString(ref reader, ref i);
                    item.Member.MobilePhone = ReadString(ref reader, ref i);
                    item.Member.PaymentUntilDate = ReadDate(ref reader, ref i);
                    item.Member.Membership.MembershipCode = ReadString(ref reader, ref i);
                    item.Member.Membership.Name = ReadString(ref reader, ref i);
                    item.Member.Membership.Fee = ReadDouble(ref reader, ref i);
                    item.Member.Membership.MonthTerms = ReadInt(ref reader, ref i);

                    item.TransactionDate = ReadDate(ref reader, ref i);

                    item.StartDate = ReadDate(ref reader, ref i);
                    item.EndDate = ReadDate(ref reader, ref i);
                    //item.ReceivedOn = ReadDate(ref reader, ref i);
                    //item.ReceivedBy.Username = ReadString(ref reader, ref i);
                    //item.PaymentMethod = ReadString(ref reader, ref i);
                    item.PaidAmount = ReadDouble(ref reader, ref i);
                    item.DiscountAmount = ReadDouble(ref reader, ref i);
                    item.RegistrationAmount = ReadDouble(ref reader, ref i);
                    item.WrittenOffAmount = ReadDouble(ref reader, ref i);


                    item.Comment = ReadString(ref reader, ref i);
                    item.UnpaidAmount = ReadDouble(ref reader, ref i);
                    item.NumInstallments = ReadInt(ref reader, ref i);
                    item.NumInstallmentsLeft = ReadInt(ref reader, ref i);

                    item.FirstInstallmentAmount= ReadDouble(ref reader, ref i);
                    item.SecondInstallmentAmount = ReadDouble(ref reader, ref i);



                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();
                if (onlyLast && list.Count > 0)
                {
                    list[0].PaymentDueForm = FetchPaymentsByTransaction(list[0].TransactionId);
                    //list[0].ExistingFacilities = FetchFacilitiesByTransaction(list[0].TransactionId);
                    list[0].Receipts = FetchReceiptsByTransaction(list[0].TransactionId);
                }

                return list;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchTransactionsByMember] Error: {0}", e.ToString());
                throw e;
            }
        }

        public Transaction FetchTransactionDetails(int transactionId)
        {
            log.Info("[FetchTransactionsByMember]");
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			  
              SELECT [transaction_id]
               
                  ,t.[member_id]
	              ,m.title, m.firstname, m.lastname, m.fullname
	              ,m.address_street, m.address_town, m.email_address
	              ,m.home_phone, m.office_phone, m.mobile_phone
	              ,m.payment_until
	              
	              ,mb.code, mb.name, coalesce(m.custom_monthly_fee,mb.fee) fee, mb.month_terms
                  ,t.[transaction_date]
                  ,t.[period_start_date]
                  ,t.[period_end_date]
                 
                  ,t.[amount_paid]
                  ,t.[amount_discounted]
                  ,t.[amount_registration]
,t.amount_writtenoff
                  ,t.[comment]
                   
                   ,t.[amount_unpaid]
                 ,t.[num_facilities_orig]
                  ,t.[num_facilities_left]
                  ,t.[amount_first_installment]
                  ,t.[amount_second_installment]



              FROM {0} t
              inner join {1} m
              on t.member_id = m.member_id

              inner join {2} mb
              on m.membership_type = mb.code


            where t.transaction_id = @transactionId


                    "
                    , TableMappings.TBL_TRANSACTION
                    , TableMappings.TBL_MEMBER
                    , TableMappings.TBL_MEMBERSHIP

                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@transactionId", SqlDbType.Int, transactionId));

                Transaction item = new Transaction();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    item.TransactionId = ReadInt(ref reader, ref i);
                    //item.ReceiptId = ReadInt(ref reader, ref i);
                    item.Member.MemberId = ReadInt(ref reader, ref i);
                    item.Member.Title = ReadString(ref reader, ref i);
                    item.Member.Firstname = ReadString(ref reader, ref i);
                    item.Member.Lastname = ReadString(ref reader, ref i);
                    item.Member.Fullname = ReadString(ref reader, ref i);

                    item.Member.StreetAddress = ReadString(ref reader, ref i);
                    item.Member.Town = ReadString(ref reader, ref i);
                    item.Member.EmailAddress = ReadString(ref reader, ref i);
                    item.Member.HomePhone = ReadString(ref reader, ref i);
                    item.Member.OfficePhone = ReadString(ref reader, ref i);
                    item.Member.MobilePhone = ReadString(ref reader, ref i);
                    item.Member.PaymentUntilDate = ReadDate(ref reader, ref i);
                    item.Member.Membership.MembershipCode = ReadString(ref reader, ref i);
                    item.Member.Membership.Name = ReadString(ref reader, ref i);
                    item.Member.Membership.Fee = ReadDouble(ref reader, ref i);
                    item.Member.Membership.MonthTerms = ReadInt(ref reader, ref i);

                    item.TransactionDate = ReadDate(ref reader, ref i);

                    item.StartDate = ReadDate(ref reader, ref i);
                    item.EndDate = ReadDate(ref reader, ref i);
                    //item.ReceivedOn = ReadDate(ref reader, ref i);
                    //item.ReceivedBy.Username = ReadString(ref reader, ref i);
                    //item.PaymentMethod = ReadString(ref reader, ref i);
                    item.PaidAmount = ReadDouble(ref reader, ref i);
                    item.DiscountAmount = ReadDouble(ref reader, ref i);
                    item.RegistrationAmount = ReadDouble(ref reader, ref i);
                    item.WrittenOffAmount = ReadDouble(ref reader, ref i);


                    item.Comment = ReadString(ref reader, ref i);
                    item.UnpaidAmount = ReadDouble(ref reader, ref i);
                    item.NumInstallments = ReadInt(ref reader, ref i);
                    item.NumInstallmentsLeft = ReadInt(ref reader, ref i);
                    item.FirstInstallmentAmount = ReadDouble(ref reader, ref i);
                    item.SecondInstallmentAmount = ReadDouble(ref reader, ref i);

                }
                reader.Dispose();
                DBDisconnect();

                item.PaymentDueForm = FetchPaymentsByTransaction(item.TransactionId);
                //list[0].ExistingFacilities = FetchFacilitiesByTransaction(list[0].TransactionId);
                item.Receipts = FetchReceiptsByTransaction(item.TransactionId);


                return item;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchTransactionDetails] Error: {0}", e.ToString());
                throw e;
            }
        }



        public IList<Receipt> FetchReceiptsByTransaction(int transactionId)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                    SELECT [receipt_id]
                          ,[transaction_id]
                          ,[received_on]
                          ,[received_by]
                          ,[payment_method]
                          ,[amount_received]
                           ,transaction_cancelled
, member_id
                      FROM {0}
                       WHERE transaction_id = @transactionId
                    "
                    , TableMappings.TBL_RECEIPT

                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@transactionId", SqlDbType.Int, transactionId));

                IList<Receipt> list = new List<Receipt>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    Receipt item = new Receipt();
                    item.ReceiptId = ReadInt(ref reader, ref i);
                    item.TransactionId = ReadInt(ref reader, ref i);

                    item.ReceivedOn = ReadDate(ref reader, ref i);
                    item.ReceivedBy.Username = ReadString(ref reader, ref i);
                    item.PaymentMethod = ReadString(ref reader, ref i);

                    item.AmountReceived = ReadDouble(ref reader, ref i);
                    item.TransactionCancelled = ReadString(ref reader, ref i) == "Y";
                    item.MemberId = ReadInt(ref reader, ref i);

                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();


                return list;

            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchPaymentsByTransaction] Error: {0}", e.ToString());
                throw e;
            }

        }

        private bool InsertTransaction(Transaction transaction)
        {
            try
            {
                log.Info("[InsertTransaction]");

                StringBuilder queryTransaction = new StringBuilder();

                queryTransaction.AppendFormat(@"
                        INSERT INTO {0}
                                   (
                                
                                   [member_id]
                                   ,[membership_code]
                                    ,transaction_date
                                   ,[period_start_date]
                                   ,[period_end_date]                              
                                   ,[amount_paid]
                                   ,[amount_discounted]
                                    ,[amount_registration]
                                    ,[amount_writtenoff]
                                    ,[amount_unpaid]
                                    ,[last_transaction_id]
                                    ,num_facilities_orig
                                    ,num_facilities_left
                                   ,[comment]
                                    )
                             VALUES			     
                        (
                        @memberId,
                        @membershipCode,
                        getDate(),
                        @startDate,
                        @endDate,
                        @amountPaid,
                        @amountDiscounted,
                        @amountRegistration,
                        @amountWrittenoff,
                        @amountUnpaid,
                        @lastTransactionId,
                        @numFacilities,
                        @installmentLeft,
                        @comment

                        );

                        SELECT SCOPE_IDENTITY()
                        ", TableMappings.TBL_TRANSACTION
                                             );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, transaction.Member.MemberId));
                parameters.Add(CreateParameter("@membershipCode", SqlDbType.VarChar, transaction.Member.Membership.MembershipCode, 20));
                parameters.Add(CreateParameter("@comment", SqlDbType.VarChar, transaction.Comment, 1000));
                parameters.Add(CreateParameter("@lastTransactionId", SqlDbType.Int, transaction.LastTransactionId));



                var pStartDate = transaction.StartDate;
                var pEndDate = transaction.EndDate;
                var pAmountPaid = 0d;

                var pAmountDisc = 0d;
                var pAmountReg = 0d;
                var pAmountUnpaid = 0d;
                var pAmountWrittenOff = 0d;
                var pNumLeft = 0;


                var pNumFacilities = 0;

                if (transaction.IsPass)
                {

                     pStartDate = transaction.StartDate;
                     pEndDate = transaction.EndDate;
                     pAmountPaid = transaction.PaidAmount;
                   

                }
                else {

                    var isYearly = transaction.IsYearly;

                    var yearlyDiscount = transaction.DiscountAmount;
                    var yearlyRegistration = transaction.RegistrationDue;
                    if (transaction.DiscountAmount > transaction.CalculatedFeeIncludingOverdue)
                    {
                        //disc 11000, yearly fee = 10000 then disc = 10000
                        yearlyDiscount = transaction.CalculatedFeeIncludingOverdue;
                        yearlyRegistration = transaction.RegistrationDue - (transaction.DiscountAmount - transaction.CalculatedFeeIncludingOverdue);
                    }


                    // pStartDate = isYearly ? transaction.StartDate : transaction.CalculatedStartingPeriodDate;
                    // pEndDate = isYearly ? ( transaction.HasLongDues? transaction.EndDate : transaction.StartDate.AddYears(1).AddDays(-1) ) : transaction.CalculatedEndingPeriodDate;
                    //// pAmountPaid = isYearly ? transaction.InitialDownpayment - transaction.RegistrationFeeForm : transaction.CalculatedTotalPaid;
                    // pAmountPaid = isYearly ? transaction.InitialDownpayment - yearlyRegistration : transaction.CalculatedTotalPaid;
                    // //pAmountDisc = isYearly ? transaction.RegistrationAmountDue - transaction.RegistrationFeeForm : transaction.CalculatedTotalDiscount;
                    // pAmountDisc = isYearly ? yearlyDiscount : transaction.CalculatedTotalDiscount;
                    // //pAmountReg = transaction.RegistrationFeeForm;
                    // pAmountReg = isYearly ? yearlyRegistration : transaction.RegistrationAmount;
                    // //pAmountUnpaid = isYearly ? transaction.Membership.Fee - (transaction.InitialDownpayment - transaction.RegistrationFeeForm) : 0;
                    // pAmountUnpaid = isYearly ? (transaction.RegistrationDue + transaction.CalculatedFeeIncludingOverdue) - (transaction.InitialDownpayment + transaction.DiscountAmount ) : 0;
                    // pAmountWrittenOff = isYearly ? 0 : transaction.CalculatedWriteOffs;

                    pStartDate = transaction.OverridenStartDate;
                    pEndDate = isYearly ? (transaction.HasLongDues ? transaction.OverridenEndDate : transaction.OverridenStartDate.AddYears(1).AddDays(-1)) : transaction.OverridenEndDate;
                    pAmountPaid = isYearly  && transaction.PaidAmountOverriden ==  (transaction.InitialDownpayment - yearlyRegistration) ? transaction.InitialDownpayment - yearlyRegistration : transaction.PaidAmountOverriden;
                    pAmountDisc = isYearly ? yearlyDiscount : transaction.DiscountAmountOverriden;
                    pAmountReg = isYearly ? yearlyRegistration : transaction.RegistrationAmountOverriden;
                    pAmountUnpaid = isYearly ? (transaction.RegistrationDue + transaction.CalculatedFeeIncludingOverdue) - (transaction.InitialDownpayment + transaction.DiscountAmount) : 0;
                    pAmountWrittenOff = isYearly ? 0 : transaction.WrittenOffAmountOverriden;

                    pNumFacilities = transaction.NumInstallments;
                     pNumLeft = transaction.NumInstallments;

                    //if(transaction.PartPaymentForm)
                    //{
                    //    pStartDate = transaction.StartDate;
                    //    pEndDate = transaction.EndDate;
                    //    pAmountDisc = 0;
                    //    pAmountReg = 0;
                    //    pAmountWrittenOff = 0;

                    //     pAmountPaid = transaction.InitialDownpayment;
                    //    pAmountUnpaid = transaction.UnpaidAmount - transaction.InitialDownpayment;
                    //     pNumLeft = pAmountUnpaid == 0 ? 0 : transaction.NumInstallmentsLeft - 1;


                    //}


                  
                }

                parameters.Add(CreateParameter("@startDate", SqlDbType.Date, pStartDate));
                parameters.Add(CreateParameter("@endDate", SqlDbType.Date, pEndDate));
                parameters.Add(CreateParameter("@amountPaid", SqlDbType.Float, pAmountPaid));
                parameters.Add(CreateParameter("@amountDiscounted", SqlDbType.Float, pAmountDisc));
                parameters.Add(CreateParameter("@amountRegistration", SqlDbType.Float, pAmountReg));
                parameters.Add(CreateParameter("@amountUnpaid", SqlDbType.Float, pAmountUnpaid));
                parameters.Add(CreateParameter("@amountWrittenoff", SqlDbType.Int, pAmountWrittenOff));
                parameters.Add(CreateParameter("@numFacilities", SqlDbType.Int, pNumFacilities));
                parameters.Add(CreateParameter("@installmentLeft", SqlDbType.Int, pNumLeft));





                DBConnect();

                var transactionId = InsertWithReturnId(queryTransaction.ToString(), parameters);
                DBDisconnect();

                var paymentSuccess = false;
                if (transactionId > 0)
                {
                    transaction.TransactionId = transactionId;
                    var received = pAmountPaid + pAmountReg;

                    transaction.Receipts.Add(CreateReceipt(new Receipt { TransactionId = transactionId, PaymentMethod = transaction.IsStandingOrder ? "Standing Order" : transaction.PaymentMethodForm, AmountReceived = received, MemberId = transaction.Member.MemberId }));

                    paymentSuccess = InsertPayment(transaction);
                }

                var updatePaymentDateSuccess = false;
                if(paymentSuccess)
                {
                    updatePaymentDateSuccess = UpdatePaymentDate(transaction);
                    }


                return updatePaymentDateSuccess;

            }
            catch (Exception e)
            {
                log.ErrorFormat("[InsertTransaction] Error: {0}", e.ToString());
                throw e;
            }
        }

        private bool UpdateTransaction(Transaction transaction)
        {
            try
            {
                log.Info("[UpdateTransaction]");

                StringBuilder queryTransaction = new StringBuilder();
                StringBuilder queryPayment = new StringBuilder();
                StringBuilder queryFacility = new StringBuilder();
                StringBuilder queryMember = new StringBuilder();

                queryTransaction.AppendFormat(@"
                       UPDATE {0}
                       SET 
                          {1} = @amountPaid
                          ,[comment] = @comment
                          ,[amount_unpaid] = @amountUnpaid
                            ,num_facilities_left = @numInstallmentLeft
                     WHERE transaction_id =@transactionId
                        ", TableMappings.TBL_TRANSACTION
                          , transaction.InstallmentNum == 1 ? "amount_first_installment" : "amount_second_installment" );
                

                var pAmountPaid = transaction.InitialDownpayment;
                var pAmountUnpaid = transaction.UnpaidAmount - transaction.InitialDownpayment;
                var pInstLeft = pAmountUnpaid == 0 ? 0 :  1;

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@comment", SqlDbType.VarChar, transaction.Comment, 1000));
                parameters.Add(CreateParameter("@transactionId", SqlDbType.Int, transaction.TransactionId));
                parameters.Add(CreateParameter("@amountPaid", SqlDbType.Float, pAmountPaid));
                parameters.Add(CreateParameter("@amountUnpaid", SqlDbType.Float, pAmountUnpaid));

                parameters.Add(CreateParameter("@numInstallmentLeft", SqlDbType.Float, pInstLeft));


                DBConnect();

                var success = CreateUpdateDelete(queryTransaction.ToString(), parameters) > 0;
                var successPayment = false;
                DBDisconnect();

                if (success)
                {

                    queryPayment.AppendFormat(@"
                       UPDATE {0}
                       SET 
                          [paid] = paid + @amountPaidPerMonth
                          ,[due] = @amountUnpaidPerMonth
                     WHERE transaction_id =@transactionId
                        ", TableMappings.TBL_PAYMENT);

                    parameters.Add(CreateParameter("@amountPaidPerMonth", SqlDbType.Float, pAmountPaid / transaction.TotalNumberOfMonths));
                    parameters.Add(CreateParameter("@amountUnpaidPerMonth", SqlDbType.Float, pAmountUnpaid / transaction.TotalNumberOfMonths));
                    DBConnect();
                    successPayment = CreateUpdateDelete(queryPayment.ToString(), parameters) > 0;
                    DBDisconnect();

                }


                var sucessFinal = false;


                if (successPayment)
                {
                    transaction.Receipts.Add(CreateReceipt(new Receipt { TransactionId = transaction.TransactionId, PaymentMethod = transaction.IsStandingOrder ? "Standing Order" : transaction.PaymentMethodForm, AmountReceived = transaction.InitialDownpayment, MemberId = transaction.Member.MemberId }));

                    if (pAmountUnpaid == 0)
                    {

                        queryTransaction.AppendFormat(@"
                       UPDATE {0} SET installment_date = null, is_part_payment = 'N' where member_id = @memberId

                    "
                     , TableMappings.TBL_MEMBER);

                        parameters.Add(CreateParameter("@memberId", SqlDbType.Int, transaction.Member.MemberId));

                        DBConnect();
                        sucessFinal = CreateUpdateDelete(queryTransaction.ToString(), parameters) > 0;
                        DBDisconnect();


                    }
                    else
                    {
                        queryTransaction.AppendFormat(@"
                     
                       UPDATE {0} SET installment_date = @nxtinstallment where member_id = @memberId

                    "
                     , TableMappings.TBL_MEMBER);

                        parameters.Add(CreateParameter("@memberId", SqlDbType.Int, transaction.Member.MemberId));
                        var pNxtInstallment = Utils.GetLastDayOfMonth(transaction.TransactionDate.AddMonths(2));

                        parameters.Add(CreateParameter("@nxtinstallment", SqlDbType.Date, pNxtInstallment));


                        DBConnect();
                        sucessFinal = CreateUpdateDelete(queryTransaction.ToString(), parameters) > 0;
                        DBDisconnect();


                    }

                }


                return sucessFinal;

            }
            catch (Exception e)
            {
                log.ErrorFormat("[UpdateTransaction] Error: {0}", e.ToString());
                throw e;
            }
        }


        private Receipt CreateReceipt(Receipt receipt)
        {
            try
            {

                log.Info("[CreateReceipt]");

                StringBuilder queryTransaction = new StringBuilder();

                queryTransaction.AppendFormat(@"
                   
                        INSERT INTO {0}
                                   ([transaction_id]
                                   ,[received_on]
                                   ,[received_by]
                                   ,[payment_method]
                                   ,[amount_received]
                                     ,member_id)
                             VALUES
                                   (@transactionId, getDate(),@adminUser,@paymentMethod,@amount, @memberId)
                     

                        SELECT SCOPE_IDENTITY()
                        ", TableMappings.TBL_RECEIPT
                                             );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@transactionId", SqlDbType.Int, receipt.TransactionId));
                parameters.Add(CreateParameter("@adminUser", SqlDbType.VarChar, UserSession.Current.Username, 30));
                parameters.Add(CreateParameter("@paymentMethod", SqlDbType.VarChar, receipt.PaymentMethod, 50));
                parameters.Add(CreateParameter("@amount", SqlDbType.Float, receipt.AmountReceived));
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, receipt.MemberId));


                DBConnect();

                receipt.ReceiptId = InsertWithReturnId(queryTransaction.ToString(), parameters);
                DBDisconnect();

                return receipt;

            }
            catch (Exception e)
            {

                log.ErrorFormat("[CreateReceipt] Error: {0}", e.ToString());
                throw e;
            }
        }


        /// <summary>
        /// inserts transaction details per year month code in payment table
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        private bool InsertPayment(Transaction transaction)
        {
            try
            {
                log.Info("[InsertPayment]");

                StringBuilder query = new StringBuilder();
                StringBuilder paramSB = new StringBuilder();



                query.AppendFormat(@"
                       INSERT INTO {0}
                       ([transaction_id]
                       ,[year_month]
                       ,[fees]
                       ,[paid]
                       ,[written_off]
                       ,[discounted]
                       ,[due])
                 VALUES ", TableMappings.TBL_PAYMENT);

                if (transaction.IsYearly || transaction.PartPaymentForm)
                {
                    foreach (var due in transaction.CalculatedYearlyDue)
                    {
                        paramSB.AppendFormat("({0},{1},{2},{3},{4},{5},{6}),",
                            transaction.TransactionId, due.YearMonth, due.FeeAmount, due.PaidAmount, due.CalculatedWrittenOffAmount, due.DiscountedAmount, due.RemainingBalanceAmount);
                    }

                }
                else if(transaction.IsPass)
                {

                    var dueMonth = Helpers.Utils.YearMonthCode(DateTime.Now);

                    paramSB.AppendFormat("({0},{1},{2},{3},{4},{5},{6}),",
                            transaction.TransactionId, dueMonth, transaction.PaidAmount, transaction.PaidAmount, 0.0, 0.0, 0.0);

                }
                else
                {


                    foreach (var due in transaction.CalculatedPaymentDueForm)
                    {

                        if(due.YearMonth <= (transaction.OverridenEndDate.Year*100) + transaction.OverridenEndDate.Month)
                        {
                            paramSB.AppendFormat("({0},{1},{2},{3},{4},{5},{6}),",
                            transaction.TransactionId, due.YearMonth, due.FeeAmount, due.PaidAmount, due.CalculatedWrittenOffAmount, due.DiscountedAmount, due.RemainingBalanceAmount);
                        }
                        
                    }
                    foreach (var adv in transaction.CalculatedPaymentAdvances)
                    {
                        if (adv.YearMonth <= (transaction.OverridenEndDate.Year * 100) + transaction.OverridenEndDate.Month)
                        {
                            paramSB.AppendFormat("({0},{1},{2},{3},{4},{5},{6}),",
                            transaction.TransactionId, adv.YearMonth, adv.FeeAmount, adv.PaidAmount, adv.CalculatedWrittenOffAmount, adv.DiscountedAmount, adv.RemainingBalanceAmount);
                        }
                    }


                }
                paramSB.Length--;
                query.Append(paramSB);

                DBConnect();
                var result = CreateUpdateDelete(query.ToString()) > 0;
                //var transactionId = InsertWithReturnId(query.ToString(), parameters);
                DBDisconnect();

                //if (transaction.Membership.IsYearly && transaction.Facilities.Count >0)
                //{
                //     UpdateFacility(transaction);
                //}


                return result;

            }
            catch (Exception e)
            {
                log.ErrorFormat("[InsertPayment] Error: {0}", e.ToString());
                throw e;
            }
        }



        //private bool UpdatePartPaymentDate(Transaction transaction)
        //{
        //    try
        //    {

        //        /* section for part payment */

        //        StringBuilder queryTransaction = new StringBuilder();
        //        StringBuilder queryUnpaid = new StringBuilder();

        //        queryUnpaid.AppendFormat(@"Select amount_unpaid from {0} where transaction_id = @current_tranid ", TableMappings.TBL_TRANSACTION);



        //        //    var pAmountUnpaid = transaction.CalculatedFeeIncludingOverdue - (transaction.InitialDownpayment + transaction.PaidAmount - transaction.RegistrationAmount + transaction.DiscountAmount);
        //        List<SqlParameter> parameters = new List<SqlParameter>();
        //        parameters.Add(CreateParameter("@current_tranid", SqlDbType.Int, transaction.TransactionId));
        //        var pAmountUnpaid = 0D;

        //        DBConnect();

        //        SqlDataReader reader = ReadQuery(queryUnpaid.ToString(), parameters);

        //        while (reader.Read())
        //        {
        //            int i = 0;

        //            pAmountUnpaid = ReadDouble(ref reader, ref i);
        //        }
        //        reader.Dispose();
        //        DBDisconnect();

        //        var successTran = false;
        //        if (pAmountUnpaid == 0)
        //            {

        //                queryTransaction.AppendFormat(@"
        //               UPDATE {0} SET installment_date = null, is_part_payment = 'N' where member_id = @memberId "
        //             , TableMappings.TBL_MEMBER);

        //                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, transaction.Member.MemberId));

        //                DBConnect();
        //                 successTran = CreateUpdateDelete(queryTransaction.ToString(), parameters) > 0;
        //                DBDisconnect();


        //            }
        //            else
        //            {
        //                queryTransaction.AppendFormat(@" 
        //               UPDATE {0} SET installment_date = @nxtinstallment, last_transaction_id = @tranId where member_id = @memberId "
        //             , TableMappings.TBL_MEMBER);

        //                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, transaction.Member.MemberId));
        //                parameters.Add(CreateParameter("@tranId", SqlDbType.Int, transaction.TransactionId));
        //            var pNxtInstallment = Utils.GetLastDayOfMonth(transaction.TransactionDate.AddMonths(2));

        //                parameters.Add(CreateParameter("@nxtinstallment", SqlDbType.Date, pNxtInstallment));


        //        DBConnect();

        //            successTran = CreateUpdateDelete(queryTransaction.ToString(), parameters) > 0;
        //                DBDisconnect();

        //            }
        //        return successTran;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


        //private bool UpdatePaymentDate(int memberId, int lastTransactionId, DateTime paymentDate, bool IsPartPayment)
        private bool UpdatePaymentDate(Transaction transaction)
        {
            try
            {
                log.Info("[UpdatePaymentDate]");

                //var paymentDate = transaction.CalculatedEndingPeriodDate;
                var paymentDate = transaction.OverridenEndDate;
                DateTime? installmentDate = null;
                if (transaction.IsYearly)
                {

                    if (transaction.hasDues)
                    {
                        paymentDate = transaction.EndDate;
                        installmentDate = transaction.NumInstallments > 0 ? Utils.GetLastDayOfMonth(DateTime.Now.AddMonths(1)) : transaction.EndDate;

                    }
                    else
                    {
                        paymentDate = transaction.Member.PaymentUntilDate.AddYears(1);

                    }

                } 




                StringBuilder query = new StringBuilder();
                query.AppendFormat(@"
                       UPDATE {0} SET payment_until = @paymentUntil, installment_date = @installmentDate, is_reg_paid = 'Y' , last_transaction_id=@lastTransactionId, is_part_payment = @isPartPayment where member_id = @memberId
                 ", TableMappings.TBL_MEMBER);

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, transaction.Member.MemberId));
                parameters.Add(CreateParameter("@paymentUntil", SqlDbType.Date, paymentDate));
                parameters.Add(CreateParameter("@installmentDate", SqlDbType.Date, installmentDate));
                parameters.Add(CreateParameter("@lastTransactionId", SqlDbType.Int, transaction.TransactionId));
                parameters.Add(CreateParameter("@isPartPayment", SqlDbType.VarChar, transaction.NumInstallmentsLeft > 0 ? "Y" : "N", 1));


                DBConnect();
                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                //var transactionId = InsertWithReturnId(query.ToString(), parameters);
                DBDisconnect();

          

                return result;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[UpdatePaymentDate] Error: {0}", e.ToString());
                throw e;
            }

        }

        /*
        private IList<Facility> FetchFacilitiesByTransaction(int transactionId)
        {
            try
            {
                log.Info("[FetchFacilitiesByTransaction]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			  
                 SELECT [facility_id]
                  ,[transaction_id]
                  ,[due_date]
                  ,[due_amount]
                  ,[is_paid]
                  ,[installment_num]
              FROM {0}
              WHERE transaction_id = @transactionId
                    "
                    , TableMappings.TBL_FACILITY
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@transactionId", SqlDbType.Int, transactionId));

                IList<Facility> list = new List<Facility>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    Facility item = new Facility();
                    item.FacilityId = ReadInt(ref reader, ref i);
                    item.TransactionId = ReadInt(ref reader, ref i);
                    item.DueDate = ReadDate(ref reader, ref i);
                    item.DueAmount = ReadDouble(ref reader, ref i);
                    item.IsPaid = ReadString(ref reader, ref i) == "Y";
                    item.InstallmentNum = ReadInt(ref reader, ref i);
                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();
                return list;

            }
            catch (Exception e)
            {
                
                  log.ErrorFormat("[FetchFacilitiesByTransaction] Error: {0}", e.ToString());
                throw e;
            }
        }

        private bool UpdateFacility(Transaction transaction)
        {
            try
            {
                log.Info("[UpdateFacility]");


                StringBuilder query = new StringBuilder();
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!transaction.IsPartPayment && transaction.Facilities.Count > 0)

                    query.AppendFormat(@"

                        INSERT into {0} (transaction_id, due_date, due_amount, is_paid, installment_num)
                        VALUES 
                        
                 ", TableMappings.TBL_FACILITY);


                    foreach(var facility in transaction.Facilities)
                    {
                        query.AppendFormat(@" (@transactionId{0}, @dueDate{0}, @dueAmount{0}, @isPaid{0}, @instalmentNum{0}),",facility.InstallmentNum);
                        
                        parameters.Add(CreateParameter(string.Format("@transactionId{0}",facility.InstallmentNum), SqlDbType.Int, transaction.TransactionId));
                        parameters.Add(CreateParameter(string.Format("@dueDate{0}", facility.InstallmentNum), SqlDbType.Date, facility.DueDate));
                        parameters.Add(CreateParameter(string.Format("@dueAmount{0}", facility.InstallmentNum), SqlDbType.Float, facility.DueAmount));
                        parameters.Add(CreateParameter(string.Format("@isPaid{0}", facility.InstallmentNum), SqlDbType.Char, facility.IsPaid ? "Y" : "N", 1));
                        parameters.Add(CreateParameter(string.Format("@instalmentNum{0}", facility.InstallmentNum), SqlDbType.Int, facility.InstallmentNum));
                        
                    }
                    query.Length--;



                DBConnect();
                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                //var transactionId = InsertWithReturnId(query.ToString(), parameters);
                DBDisconnect();

                return result;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[UpdateFacility] Error: {0}", e.ToString());
                throw e;
            }

        }
        */



        public bool DeleteTransaction(int transactionId)
        {
            try
            {
                log.Info("[UpdatePaymentDate]");

                StringBuilder query = new StringBuilder();



                query.AppendFormat(@"
                       UPDATE {0} SET 
                        payment_until = (select   dateadd(Day,-1,period_start_date)  from dbo.tbl_transaction where transaction_id = @transactionId), 
                        is_reg_paid = (select case when amount_registration > 0 then 'N' else 'Y' end from {1} where transaction_id = @transactionId) ,
                        --is_part_payment = (select case when num_facilities_orig > 0 then 'Y' else 'N' end from {1} where transaction_id = @transactionId), 
                        --installment_date = (select case when num_facilities_orig > 0 then dateadd(MONTH, -1 , installment_date) else null end from {1} where transaction_id = @transactionId),
                    is_part_payment = null, installment_date = null,    
                    last_transaction_id = (select last_transaction_id from {1} where transaction_id = @transactionId) 
                     where member_id = (select member_id from {1} where transaction_id = @transactionId);

                       DELETE FROM {2} WHERE transaction_id = @transactionId;
 
                       DELETE FROM {1} WHERE transaction_id  = @transactionId;

                       DELETE FROM {3} WHERE transaction_id  = @transactionId;

                        UPDATE {4} SET transaction_cancelled = 'Y' where transaction_id = @transactionId;


                 ", TableMappings.TBL_MEMBER, TableMappings.TBL_TRANSACTION, TableMappings.TBL_PAYMENT, TableMappings.TBL_FACILITY, TableMappings.TBL_RECEIPT);



                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@transactionId", SqlDbType.Int, transactionId));

                DBConnect();
                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                //var transactionId = InsertWithReturnId(query.ToString(), parameters);
                DBDisconnect();
                return result;

            }
            catch (Exception e)
            {
                log.ErrorFormat("[DeleteTransaction] Error: {0}", e.ToString());

                throw;
            }
        }


        public Transaction FetchTransactionById(int transactionId)
        {
            log.Info("[FetchTransactionById]");
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			     
                              SELECT t.[transaction_id]
                          ,t.[member_id]
                          ,t.[membership_code]
                        ,t.[transaction_date]
                            ,t.[period_start_date]
                          ,t.[period_end_date]
                          ,t.[amount_paid]
                          ,t.[amount_discounted]
                          ,t.[amount_registration]
                          ,t.[amount_unpaid]
                          ,t.[last_transaction_id]
                          ,t.[num_facilities_orig]
                          ,t.[num_facilities_left]
                          ,t.[comment] 
                          ,t.[amount_first_installment]
                          ,t.[amount_second_installment]

                      FROM {0} t
 
                    where transaction_id = @transactionId

                    ", TableMappings.TBL_TRANSACTION
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@transactionId", SqlDbType.Int, transactionId));

                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                Transaction item = new Transaction();
                while (reader.Read())
                {
                    int i = 0;

                    item.TransactionId = ReadInt(ref reader, ref i);
                    item.Member.MemberId = ReadInt(ref reader, ref i);
                    item.Member.Membership.MembershipCode = ReadString(ref reader, ref i);
                    item.TransactionDate = ReadDate(ref reader, ref i);

                    item.StartDate = ReadDate(ref reader, ref i);
                    item.EndDate = ReadDate(ref reader, ref i);
                    item.PaidAmount = ReadDouble(ref reader, ref i);
                    item.DiscountAmount = ReadDouble(ref reader, ref i);
                    item.RegistrationAmount = ReadDouble(ref reader, ref i);
                    item.UnpaidAmount = ReadDouble(ref reader, ref i);
                    item.LastTransactionId = ReadInt(ref reader, ref i);
                    item.NumInstallments = ReadInt(ref reader, ref i);
                    item.NumInstallmentsLeft = ReadInt(ref reader, ref i);
                    item.Comment = ReadString(ref reader, ref i);
                    item.FirstInstallmentAmount = ReadDouble(ref reader, ref i);
                    item.SecondInstallmentAmount = ReadDouble(ref reader, ref i);

                }
                reader.Dispose();
                DBDisconnect();

                item.Receipts = FetchReceiptsByTransaction(transactionId);
                return item;
            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchStats] Error: {0}", e.ToString());
                throw;
            }
        }


        public bool SaveTransaction(Transaction transaction)
        {
            try
            {

                // means part payment
                if (transaction.TransactionId > 0)
                {
                    return UpdateTransaction(transaction);
                }
                return InsertTransaction(transaction);
            }
            catch (Exception e)
            {
                log.ErrorFormat("[SaveTransaction] Error: {0}", e.ToString());
                throw;
            };
        }

        #endregion


        public bool SaveMemberDetails(GymMember member)
        {
            try
            {
                log.Info("[UpdateTransaction]");

                StringBuilder query = new StringBuilder();


                query.AppendFormat(@"

                    UPDATE {0}
                       SET [title] = @title
                          ,[firstname] = @firstname
                          ,[lastname] = @lastname
                          ,[fullname] = @fullname
                          ,[gender] = @gender
                          ,[dob] =@dob
                          ,[address_street] = @street
                          ,[address_town] = @town
                          ,[email_address] = @email
                          ,[home_phone] = @homePhone
                          ,[office_phone] = @officePhone
                          ,[mobile_phone] = @mobilePhone
                          ,[club] = @club
                          ,[is_active] = @isActive
                          ,[heard_about_us] = @heard
                          ,[employer_name] = @employer
                          ,[reason_for_leaving] = @reason
                          ,[updated_by] = @updatedBy
                          ,[last_updated_on] = getDate()
                          ,[custom_registration_fee] = @customRegFee
                          ,[custom_monthly_fee] = @customMthFee
                          ,[occupation] = @occupation
                            ,[registration_date] = @registrationDate
                            ,[payment_until] = @paymentUntil
                        ,is_part_payment = @ispartpayment

                     WHERE Member_id = @memberId

                        ", TableMappings.TBL_MEMBER);


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@title", SqlDbType.VarChar, Utils.ToTitleCase(member.Title), 20));

                parameters.Add(CreateParameter("@firstname", SqlDbType.VarChar, Utils.ToTitleCase(member.Firstname), 50));
                parameters.Add(CreateParameter("@lastname", SqlDbType.VarChar, Utils.ToTitleCase(member.Lastname), 50));
                var fullname = string.Concat(Utils.ToTitleCase(member.Firstname), " ", Utils.ToTitleCase(member.Lastname));
                parameters.Add(CreateParameter("@fullname", SqlDbType.VarChar, fullname, 100));
                parameters.Add(CreateParameter("@gender", SqlDbType.VarChar, member.Gender, 1));
                parameters.Add(CreateParameter("@dob", SqlDbType.Date, member.DateOfBirth));
                parameters.Add(CreateParameter("@street", SqlDbType.VarChar, member.StreetAddress, 100));
                parameters.Add(CreateParameter("@town", SqlDbType.VarChar, member.Town, 50));
                parameters.Add(CreateParameter("@email", SqlDbType.VarChar, member.EmailAddress, 100));
                parameters.Add(CreateParameter("@homePhone", SqlDbType.VarChar, member.HomePhone, 20));
                parameters.Add(CreateParameter("@officePhone", SqlDbType.VarChar, member.OfficePhone, 20));
                parameters.Add(CreateParameter("@mobilePhone", SqlDbType.VarChar, member.MobilePhone, 20));
                parameters.Add(CreateParameter("@club", SqlDbType.VarChar, member.Club, 20));

                parameters.Add(CreateParameter("@isActive", SqlDbType.VarChar, member.IsActive ? "Y" : "N", 1));
                parameters.Add(CreateParameter("@heard", SqlDbType.VarChar, member.HowYouHeardAboutUs, 50));
                parameters.Add(CreateParameter("@employer", SqlDbType.VarChar, member.Company, 50));
                parameters.Add(CreateParameter("@reason", SqlDbType.VarChar, member.ReasonForLeaving, 500));
                parameters.Add(CreateParameter("@updatedBy", SqlDbType.VarChar, UserSession.Current.Username, 50));
                parameters.Add(CreateParameter("@customMthFee", SqlDbType.Float, member.CustomMonthlyFee));
                parameters.Add(CreateParameter("@customRegFee", SqlDbType.Float, member.CustomRegistrationFee));
                parameters.Add(CreateParameter("@occupation", SqlDbType.VarChar, member.Occupation, 200));
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, member.MemberId));
                parameters.Add(CreateParameter("@registrationDate", SqlDbType.Date, member.RegistrationDate));
                parameters.Add(CreateParameter("@paymentUntil", SqlDbType.Date, member.PaymentUntilDate));
                parameters.Add(CreateParameter("@isPartPayment", SqlDbType.VarChar, member.IsPartPayment? "Y" : "N", 1));


                DBConnect();

                var success = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();
                return success;

            }
            catch (Exception e)
            {
                log.ErrorFormat("[SaveMemberDetails] Error: {0}", e.ToString());
                throw;
            }
        }


        public IList<MemberComment> FetchMemberComments(int memberId)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                    SELECT [comment_id]
                      ,[member_id]
                      ,[comment]
                      ,[comment_date]
                      ,[followup_date]
                      ,[comment_type]
                      ,[Inputter]
                      ,[comment_status]
                  FROM {0}

                where member_id = @memberId
                order by comment_Date desc
                                    "
                                    , TableMappings.TBL_COMMENT

                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberId));

                IList<MemberComment> list = new List<MemberComment>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    MemberComment item = new MemberComment();
                    item.CommentId = ReadInt(ref reader, ref i);
                    item.MemberId = ReadInt(ref reader, ref i);
                    item.CommentDescription = ReadString(ref reader, ref i);

                    item.CommentDate = ReadDate(ref reader, ref i);
                    item.FollowupDate = ReadDate(ref reader, ref i);

                    item.CommentType = ReadString(ref reader, ref i);
                    item.Inputter.Username = ReadString(ref reader, ref i);
                    item.CommentStatus = ReadString(ref reader, ref i);

                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();


                return list;

            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchMemberComments] Error: {0}", e.ToString());
                throw e;
            }

        }




        public bool SaveMemberComment(MemberComment comment)
        {
            try
            {
                log.Info("[UpdateTransaction]");

                StringBuilder query = new StringBuilder();


                query.AppendFormat(@"


            INSERT INTO {0}
                       ([member_id]
                       ,[comment]
                       ,[comment_date]
                       ,[followup_date]
                       ,[comment_type]
                       ,[Inputter]
                       ,[comment_status])
                 VALUES
                       (@memberId, @comment, getDate(), @followUpDate, @type, @inputter, @status)

                        ", TableMappings.TBL_COMMENT);


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@member_id", SqlDbType.Int, comment.MemberId));
                parameters.Add(CreateParameter("@comment", SqlDbType.VarChar, comment.CommentDescription, 2000));

                parameters.Add(CreateParameter("@followUpDate", SqlDbType.Date, comment.FollowupDate));
                parameters.Add(CreateParameter("@type", SqlDbType.VarChar, comment.CommentType, 50));
                parameters.Add(CreateParameter("@inputter", SqlDbType.VarChar, UserSession.Current.Username, 30));
                parameters.Add(CreateParameter("@status", SqlDbType.VarChar, comment.CommentStatus, 20));

                DBConnect();

                var success = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();
                return success;

            }
            catch (Exception e)
            {
                log.ErrorFormat("[SaveMemberDetails] Error: {0}", e.ToString());
                throw;
            }
        }

        public bool DeleteMemberComment(int commentId)
        {
            try
            {
                log.Info("[DeleteMemberComment]");

                StringBuilder query = new StringBuilder();


                query.AppendFormat(@" DELETE FROM {0} WHERE comment_id = @commentId
 
                        ", TableMappings.TBL_COMMENT);


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@commentId", SqlDbType.Int, commentId));

                DBConnect();

                var success = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();
                return success;

            }
            catch (Exception e)
            {
                log.ErrorFormat("[DeleteMemberComment] Error: {0}", e.ToString());
                throw;
            }
        }


        public IList<Receipt> FetchReceiptsByMember(int memberId)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                    SELECT [receipt_id]
                          ,[transaction_id]
                          ,[received_on]
                          ,[received_by]
                          ,[payment_method]
                          ,[amount_received]
                           ,transaction_cancelled , member_id

                      FROM {0}
                       WHERE member_id = @memberId
order by received_on desc
                    "
                    , TableMappings.TBL_RECEIPT

                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, memberId));

                IList<Receipt> list = new List<Receipt>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    Receipt item = new Receipt();
                    item.ReceiptId = ReadInt(ref reader, ref i);
                    item.TransactionId = ReadInt(ref reader, ref i);

                    item.ReceivedOn = ReadDate(ref reader, ref i);
                    item.ReceivedBy.Username = ReadString(ref reader, ref i);
                    item.PaymentMethod = ReadString(ref reader, ref i);

                    item.AmountReceived = ReadDouble(ref reader, ref i);
                    item.TransactionCancelled = ReadString(ref reader, ref i) == "Y";
                    item.MemberId = ReadInt(ref reader, ref i);



                    list.Add(item);
                }
                reader.Dispose();
                DBDisconnect();


                return list;

            }
            catch (Exception e)
            {

                log.ErrorFormat("[FetchReceiptsByMember] Error: {0}", e.ToString());
                throw e;
            }

        }


        public bool InsertComment(MemberComment comment)
        {


            try
            {
                log.Info("[InsertComment]");

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
			  
                  INSERT INTO {0}
           ([member_id]
           ,[comment]
           ,[comment_date]
           ,[followup_date]
           ,[comment_type]
           ,[Inputter]
           ,[comment_status])
     VALUES
          (@memberId, @comment, getDate(), @followupDate, @type, @inputter, @status)
                    ", TableMappings.TBL_COMMENT
                 );


                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add(CreateParameter("@memberId", SqlDbType.Int, comment.MemberId));
                parameters.Add(CreateParameter("@comment", SqlDbType.VarChar, comment.CommentDescription, 2000));
                parameters.Add(CreateParameter("@followupDate", SqlDbType.Date, comment.FollowupDate));
                parameters.Add(CreateParameter("@type", SqlDbType.VarChar, comment.CommentType, 50));
                parameters.Add(CreateParameter("@inputter", SqlDbType.VarChar, UserSession.Current.Username, 30));
                parameters.Add(CreateParameter("@status", SqlDbType.VarChar, comment.CommentStatus, 20));



                DBConnect();

                var result = CreateUpdateDelete(query.ToString(), parameters) > 0;
                DBDisconnect();


                return result;
            }
            catch (Exception e)
            {
                log.ErrorFormat("[InsertComment] Error: {0}", e.ToString());
                throw;
            }
        }





        public IList<GymMember> CheckIfMemberIdExist(AddMemberViewModel members)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendFormat(@"
                select member_id, firstname, lastname from {0}
                    where member_id in (
                ", TableMappings.TBL_MEMBER);
                var countMem = 0;
                foreach (var m in members.GymMembers)
                {
                    if (m.MemberId > 0)
                    {
                        countMem++;
                        query.AppendFormat("{0},", m.MemberId);

                    }

                }

                if (countMem == 0)
                {

                    return new List<GymMember>();
                }

                query.Length--;
                query.Append(")");

                DBConnect();

                SqlDataReader reader = ReadQuery(query.ToString());

                IList<GymMember> list = new List<GymMember>();

                while (reader.Read())
                {
                    GymMember item = new GymMember();
                    int i = 0;
                    item.MemberId = ReadInt(ref reader, ref i);

                    item.Firstname = ReadString(ref reader, ref i);
                    item.Lastname = ReadString(ref reader, ref i);


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


        public IList<Transaction> FetchStandingOrders()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendFormat(@"
               
                    select m.member_id, m.firstname, m.lastname, m.membership_type, 

                    case when m.membership_type in ('CUST','TEMP') then m.custom_monthly_fee else 
                    mb.fee/mb.num_members end as [fee_pp], 
                    m.payment_until,
                    t.period_start_date

                    ,cast( DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0, getDate()) + 1, 0))as date) as payment_until
                    ,t.last_transaction_id

                    from {0} r
                    inner join 

                    (
                     select member_id, max(received_on) rec_on from {0}
                     group by member_id
                     ) latest
                     on r.member_id = latest.member_id
                     and latest.rec_on = r.received_on

                     inner join {1} t
                     on r.transaction_id = t.transaction_id

                     inner join {2} m
                     on t.member_id = m.member_id

                     inner join {3} mb
                     on m.membership_type = mb.code

                    where r.payment_method = 'Standing Order'

                ", TableMappings.TBL_RECEIPT
                 , TableMappings.TBL_TRANSACTION
                    , TableMappings.TBL_MEMBER
                    , TableMappings.TBL_MEMBERSHIP
                    );

                DBConnect();

                SqlDataReader reader = ReadQuery(query.ToString());

                IList<Transaction> list = new List<Transaction>();

                while (reader.Read())
                {
                    Transaction item = new Transaction();
                    int i = 0;
                    item.Member.MemberId = ReadInt(ref reader, ref i);

                    item.Member.Firstname = ReadString(ref reader, ref i);
                    item.Member.Lastname = ReadString(ref reader, ref i);
                    item.Member.Membership.MembershipCode = ReadString(ref reader, ref i);

                    item.PaidAmount = ReadDouble(ref reader, ref i);
                    item.Member.PaymentUntilDate = ReadDate(ref reader, ref i);
                    item.StartDate = ReadDate(ref reader, ref i);
                    item.EndDate = ReadDate(ref reader, ref i);
                    item.LastTransactionId = ReadInt(ref reader, ref i);

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


        public int SavePass(GymMember member)
        {
            try
            {
                log.Info("[SaveDayPass]");

                StringBuilder query = new StringBuilder();                
                query.AppendFormat(@"

        INSERT INTO {0}
                     (

[member_id],
[title]
           ,[fullname]
           ,[gender]
           ,[dob]
           ,[age]
           ,[address_street]
           ,[address_town]
           ,[email_address]
           ,[home_phone]
           ,[club]
           ,[max_visits]
           ,[visits_left]       
           ,[membership_type]
           ,[last_updated_on]
           ,[created_by])
             VALUES
        ((select max(case when member_id < {1} then {1} else member_id end  ) + 1 from {0})
, @title,  @fullname, @gender, @dob, @age, @street, @town, @email, @contact, @club, @allowed, @left, @membership, getDate(), @createdby)
           
                    

                        ", TableMappings.TBL_MEMBER
                        , ConfigurationHelper.BoundaryLimitForVoucherId());


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@title", SqlDbType.VarChar, Utils.ToTitleCase(member.Title), 20));

                parameters.Add(CreateParameter("@fullname", SqlDbType.VarChar, Utils.ToTitleCase(member.Fullname), 100));
                parameters.Add(CreateParameter("@gender", SqlDbType.VarChar, member.Gender, 1));

                DateTime? dob = member.DateOfBirth < new DateTime(1900, 1, 1) ? null : (DateTime?)member.DateOfBirth;

                parameters.Add(CreateParameter("@dob", SqlDbType.Date, dob));
                parameters.Add(CreateParameter("@age", SqlDbType.Int, member.AgeInputted));

                parameters.Add(CreateParameter("@street", SqlDbType.VarChar, member.StreetAddress, 200));
                parameters.Add(CreateParameter("@town", SqlDbType.VarChar, member.Town, 200));

                parameters.Add(CreateParameter("@email", SqlDbType.VarChar, member.EmailAddress, 100));
                parameters.Add(CreateParameter("@contact", SqlDbType.VarChar, member.HomePhone, 20));
                parameters.Add(CreateParameter("@club", SqlDbType.VarChar, member.Club, 20));
                parameters.Add(CreateParameter("@allowed", SqlDbType.Int, member.MaxVisits));

                var membership = member.MaxVisits == 1 ? "1DAY" : "10VS";
                parameters.Add(CreateParameter("@membership", SqlDbType.VarChar, membership, 20));



                //defaults to max visits since its creation time
                parameters.Add(CreateParameter("@left", SqlDbType.Int, member.MaxVisits));
                
                parameters.Add(CreateParameter("@createdby", SqlDbType.VarChar, UserSession.Current.Username, 50));
                
                DBConnect();
                var passid = 0;
                var success = CreateUpdateDelete(query.ToString(), parameters) > 0;
                if (success)
                {
                    passid = FetchInsertedMemberId(true);
                }

                DBDisconnect();
                
                return passid;
                
            }
            catch (Exception e)
            {
                log.ErrorFormat("[SaveDayPass] Error: {0}", e.ToString());   
                throw;
            }
        }

        //public Pass FetchPassVisitor(int id)
        //{
        //    try
        //    {
        //        StringBuilder query = new StringBuilder();
        //        query.AppendFormat(@"
        //        SELECT [pass_id]
        //              ,[title]
        //              ,[fullname]
        //              ,[gender]
        //              ,[dob]
        //              ,[age]
        //              ,[address]
        //              ,[email]
        //              ,[contact_no]
        //              ,[club]
        //              ,[visits_allowed]
        //              ,[visits_left]
        //              ,[comments]
        //              ,[created_on]
        //              ,[last_visit]
        //              ,[created_by]
        //          FROM {0}
        //          where pass_id = @passId
               

                    
        //        ", TableMappings.TBL_PASS
                
            
        //            );



        //        List<SqlParameter> parameters = new List<SqlParameter>();
        //        parameters.Add(CreateParameter("@passId", SqlDbType.Int,id));

        //        DBConnect();

        //        SqlDataReader reader = ReadQuery(query.ToString(), parameters);


        //            Pass item = new Pass();
        //        while (reader.Read())
        //        {
        //            int i = 0;
        //            item.PassId = ReadInt(ref reader, ref i);

        //            item.Title = ReadString(ref reader, ref i);
        //            item.Fullname = ReadString(ref reader, ref i);
        //            item.Gender = ReadString(ref reader, ref i);
        //            item.Dob = ReadDate(ref reader, ref i);
        //            item.Age= ReadInt(ref reader, ref i);
        //            item.Address = ReadString(ref reader, ref i);
        //            item.EmailAddress = ReadString(ref reader, ref i);
        //            item.Phone = ReadString(ref reader, ref i);
        //            item.Club = ReadString(ref reader, ref i);
        //            item.VisitsAllowed = ReadInt(ref reader, ref i);
        //            item.VisitsLeft = ReadInt(ref reader, ref i);

        //            item.Comments= ReadString(ref reader, ref i);

        //            item.CreatedOn = ReadDate(ref reader, ref i);
        //            item.LastVisit = ReadDate(ref reader, ref i);
        //            item.CreatedBy = ReadString(ref reader, ref i);
                    

        //        }

        //        reader.Dispose();

        //        item.Visits = FetchVisitsByMember(id);

        //        DBDisconnect();

        //        return item;

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}


        private IList<MemberVisit> FetchVisitsByMember(int id)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"

                SELECT [visit_id]
                      ,[member_id]
                      ,[check_in]
                      ,[check_out]
                      ,[is_pass]
                  FROM {0}
                where member_id = @memberId
 
                        ", TableMappings.TBL_VISIT);


                List<SqlParameter> parameters = new List<SqlParameter>();
                
                    parameters.Add(CreateParameter("@memberId", SqlDbType.Int, id));
                
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                List<MemberVisit> list = new List<MemberVisit>();

                while (reader.Read())
                {
                    MemberVisit item = new MemberVisit();
                    int i = 0;
                    item.VisitId = ReadInt(ref reader, ref i);
                    item.MemberId = ReadInt(ref reader, ref i);
                    
                    item.CheckInTime = ReadDate(ref reader, ref i);
                    item.CheckOutTime = ReadDate(ref reader, ref i);
                    item.IsPass = ReadString(ref reader, ref i) == "Y";

                    list.Add(item);


                }
                return list;

            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<ListItem> FetchHistory()
        {
            try
            {

                StringBuilder query = new StringBuilder();

                query.AppendFormat(@"
                 

                        select  top {0} l.item_id , max(l.log_timestamp) logtimestamp, m.title+' '+ m.firstname+' '+ m.lastname
                        from {1} l
                        inner join {2} m
                        on l.item_id = m.member_id
                        where username=@user
                        and l.item_id is not null
                        group by l.item_id, m.title+' '+ m.firstname+' '+ m.lastname
                        order by 2 desc "
                    , ConfigurationHelper.GetHistorySize()
                   , TableMappings.TBL_LOG
                   , TableMappings.TBL_MEMBER

                 );


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(CreateParameter("@user", SqlDbType.VarChar, UserSession.Current.Username,30));

                IList<ListItem> list = new List<ListItem>();
                DBConnect();
                SqlDataReader reader = ReadQuery(query.ToString(), parameters);

                while (reader.Read())
                {
                    int i = 0;
                    ListItem item = new ListItem();
                    item.id = ReadInt(ref reader, ref i);
                    item.date = ReadDate(ref reader, ref i);
                    item.name = ReadString(ref reader, ref i);

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