SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 11 Oct 2018
-- Description:	Reads record(s) from the TD_Product table
-- exec System_Navigation_Product_Read null,null,'en','Wheat'
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Product_Read @PrcCode NVARCHAR(32) = NULL
	,@SbjCode INT = NULL
	,@LngIsoCode CHAR(2) = NULL
	,@PrcValue NVARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT PRC_CODE AS PrcCode
		,PRC_VALUE AS PrcValue
		,SBJ_CODE AS SbjCode
		,SBJ_VALUE AS SbjValue
		,sum((
				CASE 
					WHEN RLS_ID IS NOT NULL
						THEN 1
					ELSE 0
					END
				)) AS PrcReleaseCount
	FROM (
		SELECT PRC_CODE
			,SBJ_CODE
			,coalesce(PLG_Value, PRC_VALUE) AS PRC_VALUE
			,coalesce(SLG_VALUE, SBJ_VALUE) AS SBJ_VALUE
			,RLS_ID
			,PRC_DELETE_FLAG
		FROM TD_PRODUCT
		INNER JOIN [TD_SUBJECT]
			ON SBJ_ID = PRC_SBJ_ID
				AND SBJ_DELETE_FLAG = 0
		LEFT JOIN (
			SELECT SLG_SBJ_ID
				,SLG_VALUE
			FROM TD_SUBJECT_LANGUAGE
			JOIN TS_LANGUAGE
				ON SLG_LNG_ID = LNG_ID
					AND LNG_DELETE_FLAG = 0
					AND LNG_ISO_CODE = @LngIsoCode
			) INNER_SLG
			ON SBJ_ID = INNER_SLG.SLG_SBJ_ID
		LEFT JOIN (
			SELECT prg.PLG_PRC_ID
				,prg.PLG_VALUE
			FROM TD_PRODUCT_LANGUAGE prg
			JOIN TS_LANGUAGE
				ON prg.PLG_LNG_ID = LNG_ID
					AND LNG_DELETE_FLAG = 0
					AND LNG_ISO_CODE = @LngIsoCode
			) INNER_PRG
			ON PRC_ID = INNER_PRG.PLG_PRC_ID
		LEFT JOIN TD_RELEASE
			ON RLS_PRC_ID = PRC_ID
				AND RLS_DELETE_FLAG = 0
				AND RLS_id IN (
					SELECT VRN_RLS_ID
					FROM VW_RELEASE_LIVE_NOW
					)
		) INNER_SELECT
	WHERE (
			(
				@PrcCode IS NOT NULL
				AND [PRC_CODE] = @PrcCode
				)
			OR @PrcCode IS NULL
			)
		AND (
			(
				@SbjCode IS NOT NULL
				AND SBJ_CODE = @SbjCode
				)
			OR @SbjCode IS NULL
			)
		AND (
			(
				@PrcValue IS NOT NULL
				AND PRC_VALUE = @PrcValue
				)
			OR @PrcValue IS NULL
			)
		AND PRC_DELETE_FLAG = 0
	GROUP BY PRC_CODE
		,PRC_VALUE
		,SBJ_CODE
		,SBJ_VALUE
END
GO


