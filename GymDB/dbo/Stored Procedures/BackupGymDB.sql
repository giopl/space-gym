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