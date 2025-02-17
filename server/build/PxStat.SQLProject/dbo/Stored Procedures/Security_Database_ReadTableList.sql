
-- =============================================
-- Author:		Liam Millar
-- Create date: 08/03/2021
-- Description:	Returns a list of all Base Type Tables in the Database
-- exec Security_Database_ReadTableList 
-- =============================================
CREATE
	

 PROCEDURE [dbo].[Security_Database_ReadTableList]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TABLE_NAME
	FROM Information_Schema.tables
	WHERE TABLE_TYPE = 'BASE TABLE';
END
