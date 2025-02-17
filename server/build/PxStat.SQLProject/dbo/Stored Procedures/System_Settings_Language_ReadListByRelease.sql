
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 5 December 2019
-- Description:	Returns a list of Languages based on a Release Code
-- =============================================
CREATE
	

 PROCEDURE System_Settings_Language_ReadListByRelease @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT LNG_ISO_CODE AS LngIsoCode
		,LNG_ISO_NAME AS LngIsoName
	FROM TS_LANGUAGE
	INNER JOIN TD_MATRIX
		ON MTR_LNG_ID = LNG_ID
			AND MTR_DELETE_FLAG = 0
	INNER JOIN TD_RELEASE
		ON MTR_RLS_ID = RLS_ID
			AND RLS_CODE = @RlsCode
			AND RLS_DELETE_FLAG = 0
	WHERE LNG_DELETE_FLAG = 0
END
