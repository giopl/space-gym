USE [master]
GO
/****** Object:  Database [SpaceGymDB]    Script Date: 1/26/2016 6:33:23 PM ******/
CREATE DATABASE [SpaceGymDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SpaceGymDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\SpaceGymDB.mdf' , SIZE = 6208KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'SpaceGymDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\SpaceGymDB_log.ldf' , SIZE = 43968KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [SpaceGymDB] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SpaceGymDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SpaceGymDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SpaceGymDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SpaceGymDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SpaceGymDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SpaceGymDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [SpaceGymDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [SpaceGymDB] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [SpaceGymDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SpaceGymDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SpaceGymDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SpaceGymDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SpaceGymDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SpaceGymDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SpaceGymDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SpaceGymDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SpaceGymDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [SpaceGymDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SpaceGymDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SpaceGymDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SpaceGymDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SpaceGymDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SpaceGymDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SpaceGymDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SpaceGymDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SpaceGymDB] SET  MULTI_USER 
GO
ALTER DATABASE [SpaceGymDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SpaceGymDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SpaceGymDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SpaceGymDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [SpaceGymDB]
GO
/****** Object:  User [spaceuser]    Script Date: 1/26/2016 6:33:23 PM ******/
CREATE USER [spaceuser] FOR LOGIN [spaceuser] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [spaceuser]
GO
/****** Object:  Schema [memadmin]    Script Date: 1/26/2016 6:33:23 PM ******/
CREATE SCHEMA [memadmin]
GO
/****** Object:  StoredProcedure [dbo].[test_populate_visits]    Script Date: 1/26/2016 6:33:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[test_populate_visits]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
declare @id int 
select @id = 1
while @id >=1 and @id <= 50000
begin
insert into dbo.tbl_visit(member_id, check_in)
  SELECT FLOOR(RAND()*(10401-10001)+10001), 
    DATEADD(
    MINUTE,
    ABS(CHECKSUM(NEWID())) % DATEDIFF(MINUTE, '2011-01-01 09:00', getDate()) + DATEDIFF(MINUTE, 0, '2011-01-01 09:00'),
    0
  )
  select @id = @id + 1
end
END


GO
/****** Object:  UserDefinedFunction [dbo].[CapitalizeFirstLetter]    Script Date: 1/26/2016 6:33:23 PM ******/
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
/****** Object:  UserDefinedFunction [dbo].[GetAgeGroup]    Script Date: 1/26/2016 6:33:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
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
/****** Object:  UserDefinedFunction [dbo].[RemoveNonASCII]    Script Date: 1/26/2016 6:33:23 PM ******/
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
/****** Object:  Table [dbo].[tbl_admin]    Script Date: 1/26/2016 6:33:23 PM ******/
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
	[num_logins] [int] NULL,
	[is_temp_password] [char](1) NULL,
	[email_addr] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_comment]    Script Date: 1/26/2016 6:33:23 PM ******/
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
/****** Object:  Table [dbo].[tbl_facility]    Script Date: 1/26/2016 6:33:23 PM ******/
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
/****** Object:  Table [dbo].[tbl_log]    Script Date: 1/26/2016 6:33:23 PM ******/
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
/****** Object:  Table [dbo].[tbl_member]    Script Date: 1/26/2016 6:33:23 PM ******/
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
/****** Object:  Table [dbo].[tbl_member_bak]    Script Date: 1/26/2016 6:33:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_member_bak](
	[title] [nvarchar](20) NOT NULL,
	[firstname] [nvarchar](50) NULL,
	[lastname] [nvarchar](100) NULL,
	[fullname] [nvarchar](100) NULL,
	[gender] [nchar](1) NULL,
	[dob] [date] NULL,
	[address_street] [nvarchar](100) NULL,
	[address_town] [nvarchar](50) NULL,
	[email_address] [nvarchar](100) NULL,
	[home_phone] [nvarchar](20) NULL,
	[office_phone] [nvarchar](20) NULL,
	[mobile_phone] [nvarchar](20) NULL,
	[club] [nvarchar](20) NULL,
	[registration_date] [date] NULL,
	[is_active] [nchar](1) NULL,
	[membership_type] [nvarchar](30) NULL,
	[heard_about_us] [nvarchar](50) NULL,
	[employer_name] [nvarchar](50) NULL,
	[payment_until] [date] NULL,
	[reason_for_leaving] [nvarchar](500) NULL,
	[created_by] [nvarchar](30) NULL,
	[updated_by] [nvarchar](30) NULL,
	[last_updated_on] [datetime] NULL,
	[custom_registration_fee] [float] NULL,
	[custom_monthly_fee] [float] NULL,
	[num_visits] [int] NULL,
	[last_visit] [datetime] NULL,
	[occupation] [varchar](200) NULL,
	[is_reg_paid] [char](1) NULL,
	[last_transaction_id] [int] NULL,
	[is_part_payment] [nchar](1) NULL,
	[installment_date] [date] NULL,
	[relationship_id] [int] NULL,
	[member_id] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_membership]    Script Date: 1/26/2016 6:33:23 PM ******/
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
	[is_active] [char](1) NULL,
	[updated_on] [datetime] NULL,
	[updated_by] [varchar](50) NULL,
	[display_order] [int] NULL,
	[is_system] [char](1) NULL,
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
/****** Object:  Table [dbo].[tbl_pass]    Script Date: 1/26/2016 6:33:23 PM ******/
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
	[created_on] [datetime] NULL,
	[last_visit] [datetime] NULL,
	[created_by] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[pass_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_payment]    Script Date: 1/26/2016 6:33:23 PM ******/
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
/****** Object:  Table [dbo].[tbl_receipt]    Script Date: 1/26/2016 6:33:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_receipt](
	[receipt_id] [int] IDENTITY(10000,1) NOT NULL,
	[transaction_id] [int] NOT NULL,
	[member_id] [int] NULL,
	[received_on] [datetime] NULL,
	[received_by] [nvarchar](30) NULL,
	[payment_method] [nvarchar](50) NULL,
	[amount_received] [float] NULL,
	[transaction_cancelled] [char](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_relationship]    Script Date: 1/26/2016 6:33:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_relationship](
	[relationship_id] [int] NOT NULL,
	[member_id] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_transaction]    Script Date: 1/26/2016 6:33:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_transaction](
	[transaction_id] [int] IDENTITY(100,1) NOT NULL,
	[member_id] [int] NOT NULL,
	[membership_code] [nvarchar](20) NULL,
	[transaction_date] [datetime] NOT NULL,
	[period_start_date] [date] NOT NULL,
	[period_end_date] [date] NOT NULL,
	[amount_paid] [float] NULL,
	[amount_discounted] [float] NULL,
	[amount_writtenoff] [float] NULL,
	[amount_registration] [float] NULL,
	[amount_unpaid] [float] NULL,
	[last_transaction_id] [int] NULL,
	[num_facilities_orig] [int] NULL,
	[num_facilities_left] [int] NULL,
	[comment] [nvarchar](1000) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_visit]    Script Date: 1/26/2016 6:33:23 PM ******/
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
/****** Object:  View [dbo].[v_budgeted]    Script Date: 1/26/2016 6:33:23 PM ******/
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
/****** Object:  View [dbo].[v_payment]    Script Date: 1/26/2016 6:33:23 PM ******/
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
ALTER TABLE [dbo].[tbl_admin] ADD  CONSTRAINT [DF_tbl_admin_num_logins]  DEFAULT ((0)) FOR [num_logins]
GO
ALTER TABLE [dbo].[tbl_admin] ADD  DEFAULT ('N') FOR [is_temp_password]
GO
ALTER TABLE [dbo].[tbl_facility] ADD  DEFAULT ('N') FOR [is_paid]
GO
ALTER TABLE [dbo].[tbl_membership] ADD  DEFAULT ('Y') FOR [is_active]
GO
ALTER TABLE [dbo].[tbl_membership] ADD  DEFAULT ('N') FOR [is_system]
GO
ALTER TABLE [dbo].[tbl_pass] ADD  DEFAULT (getdate()) FOR [created_on]
GO
ALTER TABLE [dbo].[tbl_pass] ADD  DEFAULT (getdate()) FOR [last_visit]
GO
ALTER TABLE [dbo].[tbl_receipt] ADD  DEFAULT (getdate()) FOR [received_on]
GO
ALTER TABLE [dbo].[tbl_receipt] ADD  DEFAULT ('N') FOR [transaction_cancelled]
GO
ALTER TABLE [dbo].[tbl_transaction] ADD  DEFAULT (getdate()) FOR [transaction_date]
GO
USE [master]
GO
ALTER DATABASE [SpaceGymDB] SET  READ_WRITE 
GO
