CREATE view [dbo].[v_budgeted] as
select 
						sum (

						case when m.membership_type = 'CUST' then m.custom_monthly_fee  else (mb.fee / (mb.month_terms * mb.num_members)) end
						) fees_due,
						max(case when m.membership_type = 'CUST' then m.custom_monthly_fee  else (mb.fee / (mb.month_terms * mb.num_members)) end) fee_pax,
						 count(1) active_members,
						mb.code, mb.category, m.gender,  dbo.GetAgeGroup(m.dob) age_group
						 from tbl_member m
						inner join tbl_membership mb
						on m.membership_type = mb.code
						where m.is_active = 'Y'
						group by mb.code, mb.category, m.gender, dbo.GetAgeGroup(m.dob)