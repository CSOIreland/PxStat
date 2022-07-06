SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 29/04/2022
-- Description:	Gets a list of unmigrated matrixes
-- =============================================
CREATE
	OR

ALTER PROCEDURE Migration_ReadUnmigrated
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MTR_ID AS MtrId
		,MTR_CODE AS MtrCode
	FROM TD_MATRIX
	WHERE MTR_DELETE_FLAG = 0
		AND MTR_MIGRATION_FLAG IS NULL
END
GO


