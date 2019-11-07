SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 12 Dec 2018
-- Description:	Reads Statistic Records for a given matrix code and language (optional)
-- EXEC Data_Stat_Matrix_ReadStatisticByRelease 52, 'en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadStatisticByRelease @RlsCode INT
	,@LngIsoCode CHAR(2) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT STT_CODE SttCode
		,STT_VALUE SttValue
		,STT_UNIT SttUnit
		,STT_DECIMAL SttDecimal
		,LNG_ISO_CODE LngIsoCode
		,LNG_ISO_NAME LngIsoName
	FROM TD_RELEASE
	INNER JOIN TD_MATRIX
		ON MTR_RLS_ID = RLS_ID
			AND MTR_DELETE_FLAG = 0
	INNER JOIN TS_LANGUAGE
		ON MTR_LNG_ID = LNG_ID
			AND LNG_DELETE_FLAG = 0
			AND (
				@LngIsoCode IS NULL
				OR @LngIsoCode = LNG_ISO_CODE
				)
	INNER JOIN TD_STATISTIC
		ON STT_MTR_ID = MTR_ID
	WHERE RLS_CODE = @RlsCode
		AND RLS_DELETE_FLAG = 0
	ORDER BY STT_ID
END
GO


