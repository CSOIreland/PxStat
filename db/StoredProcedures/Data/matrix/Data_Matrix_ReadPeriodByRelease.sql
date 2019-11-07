SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 12 Dec 2018
-- Description:	Reads Period for a given matrix code and language (optional)
-- EXEC Data_Stat_Matrix_ReadPeriodByRelease 32, 'fi' , 'TLIST(M1)' 
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadPeriodByRelease @RlsCode INT
	,@LngIsoCode CHAR(2) = NULL
	,@FrqCode NVARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FRQ_CODE FrqCode
		,PRD_CODE PrdCode
		,PRD_VALUE PrdValue
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
	INNER JOIN TD_FREQUENCY
		ON FRQ_MTR_ID = MTR_ID
			AND (
				@FrqCode IS NULL
				OR @FrqCode = FRQ_CODE
				)
	INNER JOIN TD_PERIOD
		ON PRD_FRQ_ID = FRQ_ID
	WHERE RLS_CODE = @RlsCode
		AND MTR_DELETE_FLAG = 0
	ORDER BY FRQ_ID
		,PRD_ID
END
GO


