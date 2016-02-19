USE [GMDB]
GO
/****** Object:  UserDefinedFunction [dbo].[CapitalizeFirstLetter]    Script Date: 26-Jan-16 4:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[CapitalizeFirstLetter]
(
--string need to format
@string VARCHAR(200)--increase the variable size depending on your needs.
)
RETURNS VARCHAR(200)
AS

BEGIN
--Declare Variables
DECLARE @Index INT,
@ResultString VARCHAR(200)--result string size should equal to the @string variable size
--Initialize the variables
SET @Index = 1
SET @ResultString = ''
--Run the Loop until END of the string

WHILE (@Index <LEN(@string)+1)
BEGIN
IF (@Index = 1)--first letter of the string
BEGIN
--make the first letter capital
SET @ResultString =
@ResultString + UPPER(SUBSTRING(@string, @Index, 1))
SET @Index = @Index+ 1--increase the index
END

-- IF the previous character is space or '-' or next character is '-'

ELSE IF ((SUBSTRING(@string, @Index-1, 1) =' 'or SUBSTRING(@string, @Index-1, 1) ='-' or SUBSTRING(@string, @Index+1, 1) ='-') and @Index+1 <> LEN(@string))
BEGIN
--make the letter capital
SET
@ResultString = @ResultString + UPPER(SUBSTRING(@string,@Index, 1))
SET
@Index = @Index +1--increase the index
END
ELSE-- all others
BEGIN
-- make the letter simple
SET
@ResultString = @ResultString + LOWER(SUBSTRING(@string,@Index, 1))
SET
@Index = @Index +1--incerase the index
END
END--END of the loop

IF (@@ERROR
<> 0)-- any error occur return the sEND string
BEGIN
SET
@ResultString = @string
END
-- IF no error found return the new string
RETURN @ResultString
END

GO
/****** Object:  UserDefinedFunction [dbo].[GetAgeGroup]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetAgeGroup] 
(
	-- Add the parameters for the function here
	@dob Date
	
)
RETURNS char(1)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @result char(1)

select @result = case
	when year(GetDate()) - year(@dob) <12 then 'A'

when year(GetDate()) - year(@dob) between 12 and 17 then 'B'
     when year(GetDate()) - year(@dob) between 18 and 25 then 'C'
     when year(GetDate()) - year(@dob) between 26 and 35 then 'D'

     when year(GetDate()) - year(@dob) between 36 and 45 then 'E'
     when year(GetDate()) - year(@dob) between 46 and 55 then 'F'

     when year(GetDate()) - year(@dob) between 56 and 65 then 'G'
     when year(GetDate()) - year(@dob) > 65 then 'H'
	 END 


	-- Add the T-SQL statements to compute the return value here
	
	-- Return the result of the function
	RETURN @result

END
GO
/****** Object:  UserDefinedFunction [dbo].[InitCap]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[InitCap] ( @InputString varchar(4000) ) 
RETURNS VARCHAR(4000)
AS
BEGIN

DECLARE @Index          INT
DECLARE @Char           CHAR(1)
DECLARE @PrevChar       CHAR(1)
DECLARE @OutputString   VARCHAR(255)

SET @OutputString = LOWER(@InputString)
SET @Index = 1

WHILE @Index <= LEN(@InputString)
BEGIN
    SET @Char     = SUBSTRING(@InputString, @Index, 1)
    SET @PrevChar = CASE WHEN @Index = 1 THEN ' '
                         ELSE SUBSTRING(@InputString, @Index - 1, 1)
                    END

    IF @PrevChar IN (' ', ';', ':', '!', '?', ',', '.', '_', '-', '/', '&', '''', '(')
    BEGIN
        IF @PrevChar != '''' OR UPPER(@Char) != 'S'
            SET @OutputString = STUFF(@OutputString, @Index, 1, UPPER(@Char))
    END

    SET @Index = @Index + 1
END

RETURN @OutputString

END


GO
/****** Object:  UserDefinedFunction [dbo].[RemoveNonASCII]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[RemoveNonASCII] 
(
    @nstring nvarchar(255)
)
RETURNS varchar(255)
AS
BEGIN

    DECLARE @Result varchar(255)
    SET @Result = ''

    DECLARE @nchar nvarchar(1)
    DECLARE @position int

    SET @position = 1
    WHILE @position <= LEN(@nstring)
    BEGIN
        SET @nchar = SUBSTRING(@nstring, @position, 1)
        --Unicode & ASCII are the same from 1 to 255.
        --Only Unicode goes beyond 255
        --0 to 31 are non-printable characters
        IF UNICODE(@nchar) between 32 and 255
            SET @Result = @Result + @nchar
        SET @position = @position + 1
    END

    RETURN @Result

END


GO
/****** Object:  Table [dbo].[tbl_admin]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_admin](
	[user_id] [int] IDENTITY(100,1) NOT NULL,
	[username] [nvarchar](30) NOT NULL,
	[fullname] [nvarchar](50) NULL,
	[password] [nvarchar](200) NULL,
	[is_active] [nchar](1) NULL,
	[last_login] [datetime] NULL,
	[access_level] [int] NULL,
	[num_logins] [int] NULL CONSTRAINT [DF_tbl_admin_num_logins]  DEFAULT ((0)),
	[is_temp_password] [char](1) NULL DEFAULT ('N'),
	[email_addr] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_comment]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_comment](
	[comment_id] [int] IDENTITY(100,1) NOT NULL,
	[member_id] [int] NOT NULL,
	[comment] [nvarchar](2000) NULL,
	[comment_date] [datetime] NULL,
	[followup_date] [date] NULL,
	[comment_type] [nvarchar](50) NULL,
	[Inputter] [nvarchar](30) NULL,
	[comment_status] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[comment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_facility]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_facility](
	[facility_id] [int] IDENTITY(100,1) NOT NULL,
	[transaction_id] [int] NOT NULL,
	[due_date] [date] NULL,
	[due_amount] [float] NULL,
	[is_paid] [char](1) NULL,
	[installment_num] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_log]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[tbl_log](
	[log_id] [int] IDENTITY(100,1) NOT NULL,
	[username] [varchar](30) NULL,
	[operation] [varchar](100) NULL,
	[item_type] [varchar](100) NULL,
	[item_id] [int] NULL,
	[log_timestamp] [datetime] NULL,
	[description] [varchar](1000) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_member]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_member](
	[member_id] [int] NOT NULL,
	[title] [nvarchar](20) NOT NULL,
	[firstname] [nvarchar](50) NULL,
	[lastname] [nvarchar](100) NULL,
	[fullname] [nvarchar](100) NULL,
	[gender] [nchar](1) NULL,
	[dob] [date] NULL,
	[age] [int] NULL,
	[address_street] [nvarchar](100) NULL,
	[address_town] [nvarchar](50) NULL,
	[email_address] [nvarchar](100) NULL,
	[home_phone] [nvarchar](20) NULL,
	[office_phone] [nvarchar](20) NULL,
	[mobile_phone] [nvarchar](20) NULL,
	[profile_pic_ext] [varchar](50) NULL,
	[club] [nvarchar](20) NULL,
	[registration_date] [date] NULL,
	[is_reg_paid] [char](1) NULL,
	[is_active] [nchar](1) NULL,
	[membership_type] [nvarchar](30) NULL,
	[custom_registration_fee] [float] NULL,
	[custom_monthly_fee] [float] NULL,
	[max_visits] [int] NULL,
	[visits_left] [int] NULL,
	[last_transaction_id] [int] NULL,
	[is_part_payment] [nchar](1) NULL,
	[installment_date] [date] NULL,
	[relationship_id] [int] NULL,
	[heard_about_us] [nvarchar](50) NULL,
	[employer_name] [nvarchar](50) NULL,
	[occupation] [varchar](200) NULL,
	[payment_until] [date] NULL,
	[reason_for_leaving] [nvarchar](500) NULL,
	[num_visits] [int] NULL,
	[last_visit] [datetime] NULL,
	[created_by] [nvarchar](30) NULL,
	[updated_by] [nvarchar](30) NULL,
	[last_updated_on] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[member_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_membership]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_membership](
	[membership_id] [int] IDENTITY(100,1) NOT NULL,
	[code] [nvarchar](20) NOT NULL,
	[name] [nvarchar](150) NOT NULL,
	[description] [nvarchar](500) NULL,
	[membership_rules] [varchar](1000) NULL,
	[registration_fee] [float] NULL,
	[fee] [float] NULL,
	[month_terms] [int] NULL,
	[num_members] [int] NULL,
	[is_active] [char](1) NULL DEFAULT ('Y'),
	[updated_on] [datetime] NULL,
	[updated_by] [varchar](50) NULL,
	[display_order] [int] NULL,
	[is_system] [char](1) NULL DEFAULT ('N'),
	[max_visits] [int] NULL,
	[is_pass] [char](1) NULL,
	[category] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_pass]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_pass](
	[pass_id] [int] IDENTITY(100000,1) NOT NULL,
	[title] [nvarchar](20) NOT NULL,
	[fullname] [nvarchar](100) NULL,
	[gender] [nchar](1) NULL,
	[dob] [date] NULL,
	[age] [int] NULL,
	[address] [nvarchar](200) NULL,
	[email] [nvarchar](100) NULL,
	[contact_no] [nvarchar](100) NULL,
	[club] [nvarchar](20) NULL,
	[visits_allowed] [int] NOT NULL,
	[visits_left] [int] NULL,
	[comments] [nvarchar](1000) NULL,
	[created_on] [datetime] NULL DEFAULT (getdate()),
	[last_visit] [datetime] NULL DEFAULT (getdate()),
	[created_by] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[pass_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_payment]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_payment](
	[transaction_id] [int] NOT NULL,
	[year_month] [int] NOT NULL,
	[fees] [float] NULL,
	[paid] [float] NULL,
	[written_off] [float] NULL,
	[discounted] [float] NULL,
	[due] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[transaction_id] ASC,
	[year_month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_receipt]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_receipt](
	[receipt_id] [int] IDENTITY(10000,1) NOT NULL,
	[transaction_id] [int] NOT NULL,
	[received_on] [datetime] NULL DEFAULT (getdate()),
	[received_by] [nvarchar](30) NULL,
	[payment_method] [nvarchar](50) NULL,
	[amount_received] [float] NULL,
	[transaction_cancelled] [char](1) NULL DEFAULT ('N'),
	[member_id] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_relationship]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_relationship](
	[relationship_id] [int] NOT NULL,
	[member_id] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_transaction]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_transaction](
	[transaction_id] [int] IDENTITY(100,1) NOT NULL,
	[member_id] [int] NOT NULL,
	[membership_code] [nvarchar](20) NULL,
	[transaction_date] [datetime] NOT NULL DEFAULT (getdate()),
	[period_start_date] [date] NOT NULL,
	[period_end_date] [date] NOT NULL,
	[amount_paid] [float] NULL,
	[amount_discounted] [float] NULL,
	[amount_registration] [float] NULL,
	[amount_unpaid] [float] NULL,
	[last_transaction_id] [int] NULL,
	[num_facilities_orig] [int] NULL,
	[num_facilities_left] [int] NULL,
	[comment] [nvarchar](1000) NULL,
	[amount_writtenoff] [float] NULL,
	[amount_first_installment] [float] NULL,
	[amount_second_installment] [float] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_visit]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_visit](
	[visit_id] [int] IDENTITY(100,1) NOT NULL,
	[member_id] [int] NOT NULL,
	[check_in] [datetime] NULL,
	[check_out] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[visit_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[v_budgeted]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  View [dbo].[v_payment]    Script Date: 26-Jan-16 4:02:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
               




GO
ALTER TABLE [dbo].[tbl_facility] ADD  DEFAULT ('N') FOR [is_paid]
GO
