SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 11/03/2022
-- Description:	Get a list of release codes for releases with the product code
-- exec System_Navigation_GetRlsCodesForProduct 'LR'
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_GetRlsCodesForProduct @PrcCode NVARCHAR(32)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT RLS_CODE AS RlsCode
	FROM TD_MATRIX
	INNER JOIN TD_RELEASE ON MTR_RLS_ID = RLS_ID
	AND RLS_DELETE_FLAG=0
	AND MTR_DELETE_FLAG=0
	
	INNER JOIN TD_PRODUCT ON PRC_ID = RLS_PRC_ID
	AND PRC_CODE=@PrcCode
		AND PRC_DELETE_FLAG = 0
END
GO