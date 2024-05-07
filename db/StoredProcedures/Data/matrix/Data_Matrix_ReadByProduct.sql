SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/12/2019
-- Description:	Returns a list of individual Matrix codes that are associated with a given product code (via the Release entity)
-- EXEC Data_Matrix_ReadByProduct 'CP1HII','ga','en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadByProduct @PrcCode NVARCHAR(32)
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
	,@AssociatedOnly INT
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
	
	IF (@AssociatedOnly = 0)
	BEGIN
		SELECT DISTINCT mtr.MTR_CODE MtrCode
		,max(coalesce(mtrLng.MTR_TITLE, mtr.MTR_TITLE)) MtrTitle
		FROM TD_MATRIX mtr
		LEFT JOIN (
				SELECT MTR_CODE
					,MTR_TITLE
					,MTR_RLS_ID
				FROM TD_MATRIX
				WHERE MTR_DELETE_FLAG = 0
					AND MTR_LNG_ID = @LngId
				) mtrLng
				ON mtr.MTR_CODE = mtrLng.MTR_CODE
					AND MTR.MTR_RLS_ID = mtrLng.MTR_RLS_ID
		INNER JOIN TD_RELEASE
			ON mtr.MTR_RLS_ID = RLS_ID
				AND MTR_DELETE_FLAG = 0
				AND RLS_DELETE_FLAG = 0
		INNER JOIN TD_PRODUCT
			ON RLS_PRC_ID = PRC_ID
				AND PRC_DELETE_FLAG = 0
		WHERE PRC_CODE = @PrcCode
		GROUP BY mtr.MTR_CODE
		ORDER BY mtr.MTR_CODE
	END
	ELSE
		BEGIN
		SELECT DISTINCT MTR_CODE MtrCode, MTR_TITLE MtrTitle
		FROM TD_RELEASE
		INNER JOIN TM_RELEASE_PRODUCT
		on RPR_RLS_ID=RLS_ID
			AND RPR_DELETE_FLAG=0
		INNER JOIN TD_PRODUCT
		on RPR_PRC_ID=PRC_ID
			AND PRC_DELETE_FLAG=0
		INNER JOIN TD_MATRIX
		on MTR_RLS_ID=RLS_ID
			AND MTR_LNG_ID=1
		INNER JOIN VW_RELEASE_LIVE_NOW
		on RLS_ID=VRN_RLS_ID
		AND VRN_MTR_ID = MTR_ID
		WHERE    PRC_CODE = @PrcCode
		GROUP BY MTR_CODE, MTR_TITLE
		ORDER BY MTR_CODE
		END
END
GO


