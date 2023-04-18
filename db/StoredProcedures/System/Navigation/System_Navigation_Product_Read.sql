SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 11 Oct 2018
-- Description:	Reads record(s) from the TD_Product table
-- exec System_Navigation_Product_Read null, 1, 'en', null
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
						AND RLS_DELETE_FLAG = 0
						THEN 1
					ELSE 0
					END
				)) AS RlsLiveCount
		,coalesce(MtrCount, 0) AS MtrCount
		,coalesce(MtrAssociatedCount,0) as MtrAssociatedCount 

	FROM (
		SELECT TD_PRODUCT.PRC_CODE
			,SBJ_CODE
			,coalesce(PLG_Value, PRC_VALUE) AS PRC_VALUE
			,coalesce(SLG_VALUE, SBJ_VALUE) AS SBJ_VALUE
			,RLS_ID
			,PRC_DELETE_FLAG
			,RLS_DELETE_FLAG
			,mtrCount AS MtrCount
			,MtrAssociatedCount 
		FROM TD_PRODUCT
		INNER JOIN [TD_SUBJECT]
			ON SBJ_ID = PRC_SBJ_ID
				AND SBJ_DELETE_FLAG = 0

				LEFT JOIN
		(
			SELECT PRC_ID as PrcIdAssociated
			,PRC_SBJ_ID as SbjIdAssociated
			,COUNT(*)  AS MtrAssociatedCount
			FROM TD_PRODUCT 
			INNER JOIN TM_RELEASE_PRODUCT 
			ON PRC_ID=RPR_PRC_ID 
			AND (@PrcCode IS NULL OR @PrcCode=PRC_CODE)
			AND PRC_DELETE_FLAG=0
			AND RPR_DELETE_FLAG=0	
			INNER JOIN (select distinct VRN_RLS_ID  from VW_RELEASE_LIVE_NOW ) vrn
			ON RPR_RLS_ID=VRN_RLS_ID 
			INNER JOIN TD_SUBJECT 
			ON PRC_SBJ_ID=SBJ_ID
			AND SBJ_DELETE_FLAG=0
			AND (@SbjCode IS NULL OR @SbjCode=SBJ_CODE)			
			GROUP BY PRC_ID,PRC_SBJ_ID,PRC_VALUE 
		) AS MTR_ASSOCIATED
		on PrcIdAssociated=PRC_ID 

		LEFT JOIN (
			SELECT PRC_CODE
				,COUNT(*) AS mtrCount
			FROM (
				SELECT DISTINCT PRC_CODE
					,MTR_CODE
				FROM TD_PRODUCT
				INNER JOIN TD_RELEASE
					ON PRC_ID = RLS_PRC_ID
						AND PRC_DELETE_FLAG = 0
						AND RLS_DELETE_FLAG = 0
				INNER JOIN TD_MATRIX
					ON MTR_RLS_ID = RLS_ID
						AND MTR_DELETE_FLAG = 0
				) prcInner
			GROUP BY PRC_CODE
			) prcMtrCounter
			ON TD_PRODUCT.PRC_CODE = prcMtrCounter.PRC_CODE
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
		,MtrCount
		,MtrAssociatedCount
END
GO


