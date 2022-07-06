SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/03/2022
-- Description:	Get lists of currently migrated and unmigrated datasets
-- EXEC Migration_ReadStatus
-- =============================================
CREATE
	OR

ALTER PROCEDURE Migration_ReadStatus
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT MTR_ID AS MtrID
		,MTR_CODE AS MtrCode
		,MTR_MIGRATION_FLAG AS MtrMigrationFlag
	FROM TD_MATRIX
	WHERE MTR_DELETE_FLAG = 0
END
GO


