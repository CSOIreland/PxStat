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
--exec Data_Classification_Search 'region'
CREATE
	OR

ALTER PROCEDURE Data_Classification_Search @Search NVARCHAR(256)
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

	SELECT DISTINCT CLS_VALUE ClsValue
		,Cls_Code ClsCode
		,CLS_ID ClsID
		,Cls_Geo_url AS ClsGeoUrl
		,MTR_CODE AS MtrCode
		,CASE 
			WHEN rcount IS NULL
				THEN 0
			ELSE rcount
			END AS VrbCount
	FROM TD_VARIABLE
	INNER JOIN TD_CLASSIFICATION
		ON VRB_CLS_ID = CLS_ID
	INNER JOIN TD_MATRIX
		ON CLS_MTR_ID = MTR_ID
			AND MTR_DELETE_FLAG = 0
	INNER JOIN VW_RELEASE_LIVE_NOW
		ON MTR_ID = VRN_MTR_ID
	LEFT JOIN (
		SELECT CLS_ID AS clsID
			,COUNT(*) AS rcount
		FROM TD_CLASSIFICATION
		INNER JOIN TD_VARIABLE
			ON VRB_CLS_ID = CLS_ID
		GROUP BY CLS_ID
		) countQuery
		ON CLS_ID = clsID
	WHERE (
			CLS_VALUE LIKE '%' + @Search + '%'
			OR VRB_VALUE LIKE '%' + @Search + '%'
			OR CLS_CODE LIKE '%' + @Search + '%'
			)
		AND (
			@LngIsoCode IS NULL
			OR @LngId = MTR_LNG_ID
			)
END
GO


