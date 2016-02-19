USE [master]
GO
/****** Object:  Database [GymDB]    Script Date: 28-Jan-16 7:31:04 AM ******/
CREATE DATABASE [GymDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MembershipManagementDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\MembershipManagementDB.mdf' , SIZE = 7232KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'MembershipManagementDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\MembershipManagementDB_log.ldf' , SIZE = 43968KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [GymDB] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [GymDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [GymDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [GymDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [GymDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [GymDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [GymDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [GymDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [GymDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [GymDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [GymDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [GymDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [GymDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [GymDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [GymDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [GymDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [GymDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [GymDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [GymDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [GymDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [GymDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [GymDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [GymDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [GymDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [GymDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [GymDB] SET  MULTI_USER 
GO
ALTER DATABASE [GymDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [GymDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [GymDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [GymDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [GymDB] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'GymDB', N'ON'
GO
USE [GymDB]
GO
/****** Object:  User [memadmin]    Script Date: 28-Jan-16 7:31:05 AM ******/
CREATE USER [memadmin] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[memadmin]
GO
ALTER ROLE [db_owner] ADD MEMBER [memadmin]
GO
/****** Object:  Schema [memadmin]    Script Date: 28-Jan-16 7:31:05 AM ******/
CREATE SCHEMA [memadmin]
GO
/****** Object:  UserDefinedFunction [dbo].[CapitalizeFirstLetter]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
/****** Object:  UserDefinedFunction [dbo].[InitCap]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
/****** Object:  UserDefinedFunction [dbo].[RemoveNonASCII]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
/****** Object:  Table [dbo].[DATA_BAK]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DATA_BAK](
	[id] [int] NULL,
	[title] [nvarchar](20) NULL,
	[firstname] [nvarchar](50) NULL,
	[lastname] [nvarchar](50) NULL,
	[gender] [nchar](1) NULL,
	[dob] [varchar](10) NULL,
	[is_active] [varchar](2) NULL,
	[membership_type] [nvarchar](30) NULL,
	[last_pay_date] [varchar](10) NULL,
	[fee] [float] NULL,
	[comment] [nvarchar](2000) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_admin]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
/****** Object:  Table [dbo].[tbl_comment]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
/****** Object:  Table [dbo].[tbl_data]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_data](
	[id] [int] NULL,
	[title] [nvarchar](20) NULL,
	[firstname] [nvarchar](50) NULL,
	[lastname] [nvarchar](50) NULL,
	[gender] [nchar](1) NULL,
	[dob] [varchar](10) NULL,
	[is_active] [varchar](2) NULL,
	[membership_type] [nvarchar](30) NULL,
	[last_pay_date] [varchar](10) NULL,
	[fee] [float] NULL,
	[comment] [nvarchar](2000) NULL,
	[dob_cleansed] [date] NULL,
	[last_payment_cleansed] [date] NULL,
	[payment_until_date] [date] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_facility]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
	[is_paid] [char](1) NULL DEFAULT ('N'),
	[installment_num] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_load]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_load](
	[title] [nvarchar](20) NOT NULL,
	[firstname] [nvarchar](50) NULL,
	[lastname] [nvarchar](50) NULL,
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
	[next_payment_date] [date] NULL,
	[reason_for_leaving] [nvarchar](500) NULL,
	[created_by] [nvarchar](30) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_load_jobs]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_load_jobs](
	[member_id] [int] NULL,
	[job] [nvarchar](200) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_log]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
/****** Object:  Table [dbo].[tbl_member]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_member](
	[member_id] [int] IDENTITY(20401,1) NOT NULL,
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
	[relationship_id] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_member_data]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_member_data](
	[member_id] [int] IDENTITY(20401,1) NOT NULL,
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
	[is_reg_paid] [char](1) NULL DEFAULT ('N'),
	[last_transaction_id] [int] NULL,
	[is_part_payment] [nchar](1) NULL,
	[installment_date] [date] NULL,
	[relationship_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[member_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_member_test]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_member_test](
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
/****** Object:  Table [dbo].[tbl_membership]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_payment]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
/****** Object:  Table [dbo].[tbl_receipt]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
/****** Object:  Table [dbo].[tbl_relationship]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_relationship](
	[relationship_id] [int] NOT NULL,
	[member_id] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_relationship_test]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_relationship_test](
	[relationship_id] [int] NOT NULL,
	[member_id] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_transaction]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
	[amount_writtenoff] [float] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_visit]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
/****** Object:  StoredProcedure [dbo].[BackupGymDB]    Script Date: 28-Jan-16 7:31:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BackupGymDB] 
@path varchar(500)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @sqladmin varchar(200)
declare @sqlmember varchar(200)
declare @sqlpayment varchar(200)
declare @sqlreceipt varchar(200)
declare @sqltransaction varchar(200)
declare @sqlrelationship varchar(200)
declare @sqlvisit varchar(200)

select @sqladmin =
--'bcp "SELECT * FROM GymDB.dbo.tbl_admin" queryout "'+@path+ '"admin.csv" -c -t, -T -S' + @@servername
'bcp "SELECT * FROM GymDB.dbo.tbl_admin" queryout c:\test2\admin.csv -c -t, -T -S' + @@servername

exec master..xp_cmdshell @sqladmin
END

GO
/****** Object:  StoredProcedure [dbo].[test_populate_visits]    Script Date: 28-Jan-16 7:31:05 AM ******/
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
USE [master]
GO
ALTER DATABASE [GymDB] SET  READ_WRITE 
GO
