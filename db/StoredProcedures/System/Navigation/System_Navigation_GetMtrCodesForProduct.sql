SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 11/03/2022
-- Description:	Get a list of matrix codes for releases with the product code
-- exec System_Navigation_GetMtrCodesForProduct 'LR'
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_GetMtrCodesForProduct @PrcCode NVARCHAR(32)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT MTR_CODE AS MtrCode
	FROM TD_MATRIX
	INNER JOIN TD_RELEASE ON MTR_RLS_ID = RLS_ID
	INNER JOIN VW_RELEASE_LIVE_NOW ON VRN_MTR_ID = MTR_ID
		AND VRN_RLS_ID = RLS_ID
	INNER JOIN TD_PRODUCT ON PRC_ID = RLS_PRC_ID
		AND PRC_CODE = @PrcCode
		AND PRC_DELETE_FLAG = 0
END
GO


