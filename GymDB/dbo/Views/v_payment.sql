
CREATE view [dbo].[v_payment] as
                    select p.year_month, sum(p.fees) bill, sum(p.paid) paid, sum(p.written_off) wo, sum(p.discounted) disc, 
                    sum(p.due) due, sum(mb.fee / (mb.month_terms * mb.num_members)) as per_month, count(t.transaction_id) [count], r.payment_method, 
					year(r.received_on)*100+ month(r.received_on) as receipt_year_month,
                    mb.code, mb.name,mb.category ,dbo.GetAgeGroup(m.dob) age_group, m.gender, due.fees_due, due.active_members, reg.reg_amt

                    from tbl_payment p
                    inner join tbl_transaction t 
                    on p.transaction_id = t.transaction_id
                    
                    inner join tbl_membership mb
                    on mb.code = t.membership_code

                    inner join tbl_member m
                    on t.member_id = m.member_id


                    inner join tbl_receipt r
                    on t.transaction_id = r.transaction_id

                    inner join
                    (
                    
					select 
						sum (

						case when m.membership_type = 'CUST' then m.custom_monthly_fee  else (mb.fee / (mb.month_terms * mb.num_members)) end
						) fees_due,
						max(case when m.membership_type = 'CUST' then m.custom_monthly_fee  else (mb.fee / (mb.month_terms * mb.num_members)) end) fee_pax,
						 count(1) active_members,
						mb.code, m.gender, dbo.GetAgeGroup(m.dob) ageGroup
						 from tbl_member m
						inner join tbl_membership mb
						on m.membership_type = mb.code
						where m.is_active = 'Y'
						group by mb.code, mb.code, m.gender, dbo.GetAgeGroup(m.dob) 


                    ) as due
                    on mb.code = due.code 
                    and cast(dbo.GetAgeGroup(m.dob) as char(1)) = cast(due.ageGroup as char(1))
                    and m.gender = due.gender

					left join
					(
				
						select  ((year(t.period_start_date) *100)+ month(t.period_start_date)) as yearMth,
						m.gender, m.membership_type, dbo.GetAgeGroup(m.dob) ageGroup, r.payment_method,
						(t.amount_registration) reg_amt

						from tbl_transaction t
						inner join tbl_member m 
						on t.member_id = m.member_id

						inner join tbl_receipt r
						on t.transaction_id = r.transaction_id
						where t.amount_registration > 0
				 
				) reg
				on m.gender = reg.gender
				and m.membership_type = reg.membership_type
				and dbo.GetAgeGroup(m.dob) = reg.ageGroup
				and p.year_month = reg.yearMth
				and r.payment_method = reg.payment_method

                    where coalesce(r.transaction_cancelled,'N') = 'N'
                    group by p.year_month , r.payment_method, mb.code, mb.name, mb.category, dbo.GetAgeGroup(m.dob) , m.gender,
                    due.fees_due, due.active_members, due.ageGroup, year(r.received_on)*100+ month(r.received_on), reg.reg_amt