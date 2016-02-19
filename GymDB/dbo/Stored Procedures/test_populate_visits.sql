
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