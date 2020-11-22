SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 11/11/2020
-- Description:	Returns a list of individual Matrix codes that are associated with a given copyright
-- EXEC Data_Matrix_ReadByLanguage 'ga','en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadByLanguage @LngIsoCode CHAR(2)
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
		,max(coalesce(mtrLng.MTR_TITLE, mtr.MTR_TITLE)) MtrTitle
	FROM TD_MATRIX mtr
	LEFT JOIN (
		SELECT distinct MTR_CODE
			,MTR_TITLE
		FROM TD_MATRIX
		WHERE MTR_DELETE_FLAG = 0
			AND MTR_LNG_ID =@LngId
		) mtrLng
		ON mtr.MTR_CODE = mtrLng.MTR_CODE
			AND MTR_DELETE_FLAG = 0
	INNER JOIN TS_LANGUAGE
		ON MTR_LNG_ID = LNG_ID
			AND LNG_DELETE_FLAG = 0
			AND LNG_ISO_CODE = @LngIsoCode
			and MTR_DELETE_FLAG=0
	GROUP BY mtr.MTR_CODE
	ORDER BY mtr.MTR_CODE
END
GO