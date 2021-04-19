SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 12 Dec 2018
-- Description:	Reads VARIABLE associated with a given matrix code and language (optional)
-- EXEC Data_Stat_Matrix_ReadVariableByRelease 32, null ,'Region'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadVariableByRelease @RlsCode VARCHAR(256)
	,@LngIsoCode CHAR(2) = NULL
	,@ClsCode NVARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CLS_CODE ClsCode
		,VRB_CODE VrbCode
		,VRB_VALUE VrbValue
		,LNG_ISO_CODE LngIsoCode
		,LNG_ISO_NAME LngIsoName
		,VRB_ELIMINATION_FLAG VrbEliminationFlag
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
			AND (
				@ClsCode IS NULL
				OR @ClsCode = CLS_CODE
				)
	INNER JOIN TD_VARIABLE
		ON VRB_CLS_ID = CLS_ID
	WHERE RLS_CODE = @RlsCode
		AND MTR_DELETE_FLAG = 0
	ORDER BY CLS_ID
		,VRB_ID
END
GO


