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