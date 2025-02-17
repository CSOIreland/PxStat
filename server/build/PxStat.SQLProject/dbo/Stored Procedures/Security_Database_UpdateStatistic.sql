
-- =============================================
-- Author:		Liam Millar
-- Create date: 08/03/2021
-- Description:	Reorganize indexes for a specific table 
-- exec Security_Database_UpdateStatistic 
-- =============================================
CREATE
	

 PROCEDURE [dbo].[Security_Database_UpdateStatistic] @TableName NVARCHAR(128)
	WITH EXECUTE AS OWNER
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @counter INT

	SET @counter = (
			SELECT COUNT(*)
			FROM Information_Schema.tables
			WHERE TABLE_TYPE = 'BASE TABLE'
				AND TABLE_NAME = @TableName
			)

	IF @counter > 0
	BEGIN
		EXEC ('ALTER INDEX ALL ON ' + @TableName + ' REORGANIZE')
		EXEC ('UPDATE STATISTICS ' + @TableName )
	END
END
