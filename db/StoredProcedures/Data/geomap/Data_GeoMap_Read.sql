SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 04/05/2021
-- Description:	Reads a Geomap based on its code
-- exec Data_GeoMap_Read 'c9989202b75921e3a062ca6942b3d8aa'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_GeoMap_Read @GmpCode NVARCHAR(32)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT GMP_CODE AS GmpCode
		,GMP_NAME AS GmpName
		,GMP_DESCRIPTION AS GmpDescription
		,GMP_GEOJSON AS GmpGeoJson
	FROM TD_GEOMAP
	WHERE GMP_DELETE_FLAG = 0
		AND GMP_CODE = @GmpCode
	ORDER BY GMP_NAME
END
GO


