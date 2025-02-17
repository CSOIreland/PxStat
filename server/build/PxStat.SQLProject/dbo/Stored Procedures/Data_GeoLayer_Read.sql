
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 30/04/2021
-- Description:	Read one or all GeoLayers
-- exec Data_GeoLayer_Read '04089f73b1fbde4aa916974673ac63f9' --,'Gaeltacht Boundaries 2020'
-- =============================================
CREATE
	

 PROCEDURE Data_GeoLayer_Read @GlrCode NVARCHAR(256) = NULL
	,@GlrName NVARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT GLR_NAME AS GlrName
		,GLR_CODE AS GlrCode
		,SUM(hasMap) AS GmpCount
	FROM (
		SELECT GLR_NAME
			,GLR_CODE
			,CASE 
				WHEN GMP_ID IS NULL
					THEN 0
				ELSE 1
				END AS hasMap
		FROM TD_GEOLAYER
		LEFT JOIN TD_GEOMAP ON GMP_GLR_ID = GLR_ID
			AND GMP_DELETE_FLAG = 0
			AND GLR_DELETE_FLAG = 0
		WHERE (
				@GlrCode IS NULL
				OR @GlrCode = GLR_CODE
				)
			AND (
				@GlrName IS NULL
				OR @GlrName = GLR_NAME
				)
			AND GLR_DELETE_FLAG = 0
		) q
	GROUP BY GLR_NAME
		,GLR_CODE
	ORDER BY GLR_NAME 
END
