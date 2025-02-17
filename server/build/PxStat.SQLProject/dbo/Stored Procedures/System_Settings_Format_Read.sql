
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 27 Sep 2018
-- Description:	Returns records from the Format Table
-- =============================================
CREATE
	

 PROCEDURE System_Settings_Format_Read @FrmType NVARCHAR(32) = NULL
	,@FrmVersion NVARCHAR(32) = NULL
	,@FrmDirection NVARCHAR(20) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FRM_TYPE AS FrmType
		,FRM_VERSION AS FrmVersion
		,FRM_DIRECTION AS FrmDirection
	FROM TS_FORMAT
	WHERE (
			@FrmType IS NULL
			OR FRM_TYPE = @FrmType
			)
		AND (
			@FrmVersion IS NULL
			OR FRM_VERSION = @FrmVersion
			)
		AND (
			@FrmDirection IS NULL
			OR FRM_DIRECTION = @FrmDirection
			)

	ORDER BY FRM_TYPE ASC
		,FRM_VERSION DESC
END
