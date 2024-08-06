SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 05/10/2018
-- Description:	Read a Copyright entry
-- exec System_Settings_Copyright_Read 'CSO'
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Settings_Copyright_Read @CprCode NVARCHAR(256) = NULL
	,@CprValue NVARCHAR(256) = NULL
	,@CprUrl NVARCHAR(2048) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CPR_CODE AS CprCode
		,CPR_VALUE AS CprValue
		,CPR_URL AS CprUrl
		,coalesce(mtr.mtrCount,0) AS MtrCount
	FROM TS_COPYRIGHT
	LEFT JOIN (
		SELECT mtrInner.CPR_ID
			,count(*) AS mtrCount
		FROM (
			SELECT DISTINCT CPR_ID
				,MTR_CODE
			FROM TS_COPYRIGHT
			INNER JOIN TD_MATRIX
				ON CPR_ID =MTR_CPR_ID
					AND CPR_DELETE_FLAG = 0
					AND MTR_DELETE_FLAG = 0
			) mtrInner
			GROUP BY mtrInner.CPR_ID
		) MTR
	
		ON MTR.CPR_ID = TS_COPYRIGHT.CPR_ID
	WHERE CPR_DELETE_FLAG = 0
		AND (
			@CprCode IS NULL
			OR @CprCode = TS_COPYRIGHT.CPR_CODE
			)
		AND (
			@CprValue IS NULL
			OR @CprValue = TS_COPYRIGHT.CPR_VALUE
			)
		AND (
			@CprUrl IS NULL
			OR @CprUrl = TS_COPYRIGHT.CPR_URL
			)
			ORDER BY CPR_VALUE
END
GO


