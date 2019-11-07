SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 4 Dec 2018
-- Description:	Reads a list of releases for a given product
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_ReadListByProduct @CcnUsername NVARCHAR(256)
	,@PrcCode NVARCHAR(32)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MTR_CODE MtrCode
		,RLS_CODE RlsCode
		,RLS_REVISION RlsRevision
		,RLS_VERSION RlsVersion
	FROM TD_MATRIX
	INNER JOIN TD_RELEASE
		ON RLS_ID = MTR_RLS_ID
			AND RLS_DELETE_FLAG = 0
	INNER JOIN TD_PRODUCT
		ON PRC_CODE = @PrcCode
			AND PRC_ID = RLS_PRC_ID
			AND PRC_DELETE_FLAG = 0
	WHERE MTR_DELETE_FLAG = 0
	GROUP BY MTR_CODE
		,RLS_CODE
		,RLS_REVISION
		,RLS_VERSION
	ORDER BY MTR_CODE
		,RLS_VERSION
		,RLS_REVISION
END
GO


