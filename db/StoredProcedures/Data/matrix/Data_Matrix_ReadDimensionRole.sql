SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 06/02/2020
-- Description:	For a matrix code, get the roles of its various dimensions. Live matrices only.
-- You must supply either a valid currently live matrix code or a Release Code plus LngIsoCode
-- exec Data_Matrix_ReadDimensionRole 'STATISTIC','NEA06',724,'en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadDimensionRole @ContentVariable NVARCHAR(256)
	,@MtrCode NVARCHAR(20) = NULL
	,@RlsCode INT = NULL
	,@LngIsoCode CHAR(2) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MtrId INT

	IF @MtrCode IS NOT NULL -- We are just reading a live release for the matrix code
	BEGIN
		SET @MtrId = (
				SELECT MTR_ID
				FROM TD_MATRIX
				INNER JOIN VW_RELEASE_LIVE_NOW
					ON MTR_ID = VRN_MTR_ID
				WHERE MTR_CODE = @MtrCode
				)
	END
	ELSE
	BEGIN -- We are reading based on the Release for the specific language version
		SET @MtrId = (
				SELECT MTR_ID
				FROM TD_MATRIX
				INNER JOIN TS_LANGUAGE
					ON MTR_LNG_ID = LNG_ID
						AND MTR_DELETE_FLAG = 0
						AND LNG_DELETE_FLAG = 0
						AND LNG_ISO_CODE = @LngIsoCode
				INNER JOIN TD_RELEASE
					ON MTR_RLS_ID = RLS_ID
						AND RLS_CODE = @RlsCode
						AND RLS_DELETE_FLAG = 0
				)
	END

	IF (@MtrId IS NULL)
	BEGIN
		RETURN
	END

	SELECT CLS_CODE AS CODE
		,CASE 
			WHEN CLS_GEO_FLAG = 1
				THEN 'geo'
			ELSE NULL
			END AS ROLE
	FROM TD_CLASSIFICATION
	WHERE CLS_MTR_ID = @MtrId
	
	UNION
	
	SELECT FRQ_CODE AS CODE
		,'time' AS ROLE
	FROM TD_FREQUENCY
	WHERE FRQ_MTR_ID = @MtrId
	
	UNION
	
	SELECT @ContentVariable AS DimensionCode
		,'metric' AS ROLE
END
GO


