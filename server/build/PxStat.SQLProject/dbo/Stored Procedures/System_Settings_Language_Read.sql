
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 20/09/2018
-- Description:	Returns a Language entry based on its iso code. Returns all languages if an iso code isn't supplied
-- exec System_Settings_Language_Read 'ga'
-- =============================================
CREATE
	

 PROCEDURE System_Settings_Language_Read @LngIsoCode CHAR(2) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT LNG_ISO_CODE AS LngIsoCode
		,LNG_ISO_NAME AS LngIsoName
		,coalesce(mtr.mtrCount,0) AS MtrCount
	FROM TS_LANGUAGE
	LEFT JOIN (
		SELECT mtrInner.LNG_ID
			,count(*) AS mtrCount
		FROM (
			SELECT DISTINCT LNG_ID
				,MTR_CODE
			FROM TS_LANGUAGE
			INNER JOIN TD_MATRIX
				ON LNG_ID =MTR_LNG_ID
					AND LNG_DELETE_FLAG = 0
					AND MTR_DELETE_FLAG = 0
			) mtrInner
			GROUP BY mtrInner.LNG_ID
		) MTR
	
		ON MTR.LNG_ID = TS_LANGUAGE.LNG_ID
	WHERE LNG_DELETE_FLAG = 0
		AND (
			@LngIsoCode IS NULL
			OR (LNG_ISO_CODE = @LngIsoCode)
			)
END
