SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 25/03/2022
-- Description:	Gets the next matrix for migration
-- exec Migration_Read_Mark 0
-- =============================================
CREATE
	OR

ALTER PROCEDURE Migration_Read_Mark @Mark BIT = NULL,
@MtrId INT=NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@MtrId is null)
	BEGIN
	-- Get the next unmigrated matrix if an id has not been supplied
	SET @MtrId = (
			SELECT TOP 1 MTR_ID
			FROM TD_MATRIX
			WHERE MTR_DELETE_FLAG = 0
				AND MTR_MIGRATION_FLAG IS NULL
			)
	END

	--Lock the matrix for selection so that it is not picked up by another process (or show it as migrated if 1 is passed)
	UPDATE TD_MATRIX
	SET MTR_MIGRATION_FLAG = @Mark
	WHERE MTR_ID = @MtrId

	--Get the required details
	SELECT MTR_ID AS MtrId
		,MTR_INPUT AS MtrInput
		,MTR_MIGRATION_FLAG AS MtrMigrationFlag
		,LNG_ISO_CODE AS LngIsoCode
	FROM TD_MATRIX
	INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
		AND LNG_DELETE_FLAG = 0
	WHERE MTR_ID = @MtrId
END
GO


