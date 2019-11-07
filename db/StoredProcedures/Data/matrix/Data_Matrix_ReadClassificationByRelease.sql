SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 12 Dec 2018
-- Description:	Reads CLASSIFICATION Recordsassociated with a given matrix code and language (optional)
-- EXEC Data_Stat_Matrix_ReadClassificationByRelease 32,'en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadClassificationByRelease @RlsCode INT
	,@LngIsoCode CHAR(2) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CLS_CODE ClsCode
		,CLS_VALUE ClsValue
		,CLS_GEO_FLAG ClsGeoFlag
		,CLS_GEO_URL ClsGeoUrl
		,LNG_ISO_CODE LngIsoCode
		,LNG_ISO_NAME LngIsoName
		,ROW_NUMBER() OVER (
			ORDER BY CLS_ID ASC
			) AS [Order]
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
	INNER JOIN TD_CLASSIFICATION
		ON CLS_MTR_ID = MTR_ID
	WHERE RLS_CODE = @RlsCode
		AND MTR_DELETE_FLAG = 0
	ORDER BY CLS_ID
END
GO


