SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 12 Dec 2018
-- Description:	Reads Period for a given matrix code and language (optional)
-- EXEC Data_Matrix_ReadPeriodByRelease 32 
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadPeriodByRelease @RlsCode INT
	,@LngIsoCode CHAR(2) = NULL
	,@FrqCode NVARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MDM_CODE FrqCode
		,DMT_CODE PrdCode
		,DMT_VALUE PrdValue
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


	INNER JOIN TD_MATRIX_DIMENSION 
	ON MTR_ID=MDM_MTR_ID 
	INNER JOIN TS_DIMENSION_ROLE 
	ON MDM_DMR_ID=DMR_ID
	AND DMR_CODE='TIME'
	INNER JOIN TD_DIMENSION_ITEM 
	ON DMT_MDM_ID =MDM_ID


	WHERE RLS_CODE = @RlsCode
		AND MTR_DELETE_FLAG = 0
	ORDER BY MDM_ID
		,DMT_ID
END
GO


