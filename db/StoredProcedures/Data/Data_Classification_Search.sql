SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 15/03/2019
-- Description:	Searches for Classification based on a keyword. Also searches the related variables of the classification.
-- If a language is specified then return only classifications related to matrices in that language. Otherwise return all qualifying Classifications.
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Classification_Search @Search ValueVarchar Readonly
	,@LngIsoCode CHAR(2) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngId INT
	DECLARE @errorMessage VARCHAR(256)

	IF @LngIsoCode IS NOT NULL
	BEGIN
		SET @LngId = (
				SELECT LNG_ID
				FROM TS_LANGUAGE
				WHERE LNG_ISO_CODE = @LngIsoCode
				)

		IF (
				@LngId IS NULL
				OR @LngId = 0
				)
		BEGIN
			SET @errorMessage = 'SP: Data_Classification_Search - language not found: ' + cast(@LngIsoCode AS VARCHAR)

			RAISERROR (
					@errorMessage
					,16
					,1
					);
		END
	END

	SELECT DISTINCT MDM_VALUE ClsValue
		,MDM_CODE ClsCode
		,MDM_ID ClsID
		,MDM_GEO_URL AS ClsGeoUrl
		,MTR_CODE AS MtrCode
		,CASE 
			WHEN rcount IS NULL
				THEN 0
			ELSE rcount
			END AS VrbCount
	FROM TD_MATRIX
	INNER JOIN TD_MATRIX_DIMENSION ON MTR_ID = MDM_MTR_ID
		AND MTR_DELETE_FLAG = 0
	INNER JOIN TS_DIMENSION_ROLE ON DMR_ID = MDM_DMR_ID
		AND DMR_CODE = 'CLASSIFICATION'
	INNER JOIN TD_DIMENSION_ITEM ON DMT_MDM_ID = MDM_ID
	INNER JOIN VW_RELEASE_LIVE_NOW ON MTR_ID = VRN_MTR_ID
	LEFT JOIN (
		SELECT MDM_ID AS clsID
			,COUNT(*) AS rcount
		FROM TD_MATRIX_DIMENSION
		INNER JOIN TS_DIMENSION_ROLE ON DMR_ID = MDM_DMR_ID
			AND DMR_CODE = 'CLASSIFICATION'
		INNER JOIN TD_DIMENSION_ITEM ON DMT_MDM_ID = MDM_ID
		GROUP BY MDM_ID
		) countQuery ON MDM_ID = clsID
	INNER JOIN (
		SELECT [Value]
		FROM @Search
		) search ON (MDM_VALUE LIKE '%' + search.[Value] + '%')
		OR DMT_VALUE LIKE '%' + search.[Value] + '%'
		OR MDM_CODE LIKE '%' + search.[Value] + '%'
	WHERE (
			@LngIsoCode IS NULL
			OR @LngId = MTR_LNG_ID
			)
END
GO


