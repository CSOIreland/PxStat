SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 22 Oct 2018
-- Description:	Reads record(s) from the TD_MATRIX and TD_Release table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadLanguage @RlsCode INT
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	SELECT LNG_ISO_CODE LngIsoCode
		,LNG_ISO_NAME LngIsoName
	FROM TD_MATRIX
	INNER JOIN TD_RELEASE
		ON RLS_ID = MTR_RLS_ID
			AND RLS_CODE = @RlsCode
			AND RLS_DELETE_FLAG = 0
	INNER JOIN @GroupUserHasAccess g
		ON g.GRP_ID = RLS_GRP_ID
	INNER JOIN TS_LANGUAGE
		ON LNG_ID = MTR_LNG_ID
			AND LNG_DELETE_FLAG = 0
	WHERE MTR_DELETE_FLAG = 0
END
GO


