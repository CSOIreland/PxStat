SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 05/05/2021
-- Description:	Reads the full list of GeoMaps
-- EXEC Data_GeoMap_ReadCollection 'https://dev-ws.cso.ie/public/api.restful/PxStat.Data.GeoMap_API.Read/'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_GeoMap_ReadCollection @UrlStub NVARCHAR(2048)
	,@GmpCode NVARCHAR(32) = NULL
	,@GlrCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT GMP_CODE AS GmpCode
		,GMP_NAME AS GmpName
		,GMP_DESCRIPTION AS GmpDescription
		,GLR_NAME AS GlrName
		,GLR_CODE AS GlrCode
		,GMP_FEATURE_COUNT AS GmpFeatureCount
		,MTR_ID
		,CASE 
			WHEN MTR_ID IS NULL
				THEN 0
			ELSE 1
			END AS HasTable
	INTO #MtrCls
	FROM TD_GEOMAP
	INNER JOIN TD_GEOLAYER ON GMP_GLR_ID = GLR_ID
	LEFT JOIN TD_MATRIX_DIMENSION ON (@UrlStub + GMP_CODE) = MDM_GEO_URL
	LEFT JOIN TD_MATRIX ON MDM_MTR_ID = MTR_ID
		AND MTR_DELETE_FLAG = 0
	WHERE GMP_DELETE_FLAG = 0
		AND (
			@GmpCode IS NULL
			OR GMP_CODE = @GmpCode
			)
		AND (
			@GlrCode IS NULL
			OR GLR_CODE = @GlrCode
			)

	SELECT GmpCode
		,GmpName
		,GmpDescription
		,GlrName
		,GlrCode
		,GmpFeatureCount
		,sum(HasTable) AS TableCount
	FROM #MtrCls
	GROUP BY GmpCode
		,GmpName
		,GmpDescription
		,GlrName
		,GlrCode
		,GmpFeatureCount
END
GO


