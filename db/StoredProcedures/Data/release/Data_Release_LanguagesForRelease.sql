SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 09/01/2023
-- Description:	Get all language codes for a release
-- EXEC Data_Release_LanguagesForRelease 1362
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_LanguagesForRelease @RlsCode INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT LNG_ISO_CODE as LngIsoCode
	FROM TD_RELEASE
	INNER JOIN TD_MATRIX ON RLS_ID = MTR_RLS_ID
		AND MTR_DELETE_FLAG = 0
		AND RLS_DELETE_FLAG = 0
		AND RLS_CODE = @RlsCode
	INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
		AND LNG_DELETE_FLAG = 0
END
GO


