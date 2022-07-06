SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/12/2019
-- Description:	Returns a list of individual Matrix codes that are associated with a given GeoMap
-- EXEC Data_Matrix_ReadByGeoMap 'c9989202b75921e3a062ca6942b3d8aa','en','en','https://dev-ws.cso.ie/public/api.restful/PxStat.Data.GeoMap_API.Read/'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadByGeoMap @GmpCode NVARCHAR(32)
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
	,@UrlStub NVARCHAR(2048)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngId INT
	DECLARE @LngDefaultId INT
	DECLARE @Url NVARCHAR(2048)

	SET @Url = @UrlStub + @GmpCode
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
		SELECT MTR_CODE
			,MTR_TITLE
			,MTR_RLS_ID
		FROM TD_MATRIX
		WHERE MTR_DELETE_FLAG = 0
			AND MTR_LNG_ID = @LngId
		) mtrLng ON mtr.MTR_CODE = mtrLng.MTR_CODE
		AND MTR.MTR_RLS_ID = mtrLng.MTR_RLS_ID
	INNER JOIN TD_RELEASE ON mtr.MTR_RLS_ID = RLS_ID
		AND MTR_DELETE_FLAG = 0
		AND RLS_DELETE_FLAG = 0
	INNER JOIN TD_MATRIX_DIMENSION
		ON MDM_MTR_ID = MTR_ID
	INNER JOIN TS_DIMENSION_ROLE 
		ON MDM_DMR_ID=DMR_ID
		AND DMR_CODE='CLASSIFICATION'
	AND MDM_GEO_URL=@Url
	GROUP BY mtr.MTR_CODE
	ORDER BY mtr.MTR_CODE
END
GO


