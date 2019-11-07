-- ================================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 28/09/2018
-- Description:	Read Privileges. Null @PrvCode input parameter will result in all privileges being returned. 
-- Otherwise it returns the privilege corresponding to the @PrvCode
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Privilege_Read @PrvCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT PRV_CODE AS PrvCode
		,PRV_VALUE AS PrvValue
	FROM TS_PRIVILEGE
	WHERE (
			@PrvCode IS NULL
			OR (PRV_CODE = @PrvCode)
			)
END
GO


