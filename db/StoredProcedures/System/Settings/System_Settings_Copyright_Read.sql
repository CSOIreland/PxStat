SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 05/10/2018
-- Description:	Read a Copyright entry
-- exec System_Settings_Copyright_Read
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Settings_Copyright_Read @source NVARCHAR(256) = NULL
	,@CprValue NVARCHAR(256) = NULL
	,@CprUrl NVARCHAR(2048) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CPR_CODE AS CprCode
		,CPR_VALUE AS CprValue
		,CPR_URL AS CprUrl
		,CASE 
			WHEN RlsCount IS NULL
				THEN 0
			ELSE RlsCount
			END AS RlsCount
	FROM TS_COPYRIGHT
	LEFT JOIN (
		SELECT MTR_CPR_ID
			,count(*) AS RlsCount
		FROM TD_MATRIX
		INNER JOIN TD_RELEASE
			ON MTR_RLS_ID = RLS_ID
		WHERE MTR_DELETE_FLAG = 0
			AND RLS_DELETE_FLAG = 0
		GROUP BY MTR_CPR_ID
		) RLS
		ON RLS.MTR_CPR_ID = CPR_ID
	WHERE CPR_DELETE_FLAG = 0
		AND (
			@source IS NULL
			OR @source = TS_COPYRIGHT.CPR_CODE
			)
		AND (
			@CprValue IS NULL
			OR @CprValue = TS_COPYRIGHT.CPR_VALUE
			)
		AND (
			@CprUrl IS NULL
			OR @CprUrl = TS_COPYRIGHT.CPR_URL
			)
END
GO


