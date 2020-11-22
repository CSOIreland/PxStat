SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 11/11/2020
-- Description:	Returns a list of individual Matrix codes that are associated with a given copyright
-- EXEC Data_Matrix_ReadByCopyright 'CSO','en','en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadByCopyright @CprCode NVARCHAR(32)
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;


	DECLARE @LngId INT
	DECLARE @LngDefaultId INT

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)
	SET @LngDefaultId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCodeDefault
				AND LNG_DELETE_FLAG = 0
			)

	SELECT DISTINCT mtr.MTR_CODE MtrCode
		,max(coalesce(mtrCpr.MTR_TITLE, mtr.MTR_TITLE)) MtrTitle
		
	FROM TD_MATRIX mtr
	LEFT JOIN (
		SELECT distinct MTR_CODE
			,MTR_TITLE
		FROM TD_MATRIX
		INNER JOIN TS_COPYRIGHT 
		ON CPR_ID=MTR_CPR_ID 
		AND MTR_DELETE_FLAG = 0
		AND CPR_DELETE_FLAG=0
			AND CPR_CODE =@CprCode
		) mtrCpr
		ON mtr.MTR_CODE = mtrCpr.MTR_CODE
			AND MTR_DELETE_FLAG = 0
	INNER JOIN TS_COPYRIGHT
		ON MTR_CPR_ID = CPR_ID
			AND CPR_DELETE_FLAG = 0
			AND CPR_CODE = @CprCode
			AND MTR_DELETE_FLAG=0
	LEFT JOIN VW_RELEASE_LIVE_NOW
		ON MTR_ID = VRN_MTR_ID
	GROUP BY mtr.MTR_CODE 
	ORDER BY mtr.MTR_CODE
END
GO


