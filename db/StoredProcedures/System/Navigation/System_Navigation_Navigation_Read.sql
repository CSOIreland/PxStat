SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Read date: 04/01/2019
-- Description:	Reads record(s) from the TD_Product table along with Subject data
-- The LngIso code will ensure that it will return SbjValue and PrcValue in the requested language if it exists
-- If there are no values in the requested language, the sp will return the values in the default language
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Navigation_Read (
	@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
	)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngId AS INT;

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			);

	IF @LngId = 0
		OR @LngId IS NULL
	BEGIN
		RETURN;
	END

	DECLARE @LngIdDefault AS INT;

	SET @LngIdDefault = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCodeDefault
				AND LNG_DELETE_FLAG = 0
			);

	IF @LngIdDefault = 0
		OR @LngIdDefault IS NULL
	BEGIN
		RETURN;
	END
	SELECT * from (
	SELECT sbj.SBJ_CODE AS SbjCode
		,sbj.SBJ_VALUE AS SbjValue
		,prd.PRC_CODE AS PrcCode
		,prd.PRC_VALUE AS PrcValue
		,(
			CASE 
				WHEN RelatedCountPerProduct IS NULL
					THEN 0
				ELSE RelatedCountPerProduct
				END
			) + (
			CASE 
				WHEN AssociatedReleaseCountPerProduct IS NULL
					THEN 0
				ELSE AssociatedReleaseCountPerProduct
				END
			)  AS PrcReleaseCount
		,thm.THM_VALUE AS ThmValue
		,thm.THM_CODE AS ThmCode
	FROM TD_PRODUCT prd
	INNER JOIN TD_SUBJECT sbj ON PRC_SBJ_ID = SBJ_ID
		AND SBJ_DELETE_FLAG = 0
	INNER JOIN TD_THEME thm ON SBJ_THM_ID = THM_ID
		AND THM_DELETE_FLAG = 0
	LEFT JOIN (
		SELECT PRC_ID
			,PRC_CODE
			,PRC_VALUE
			,COUNT(*) AS RelatedCountPerProduct
		FROM TD_RELEASE
		INNER JOIN TD_PRODUCT ON RLS_PRC_ID = PRC_ID
			AND RLS_DELETE_FLAG = 0
			AND PRC_DELETE_FLAG = 0
		INNER JOIN VW_RELEASE_LIVE_NOW ON RLS_ID = VRN_RLS_ID
		GROUP BY PRC_ID
			,PRC_CODE
			,PRC_VALUE
		) rcp ON prd.PRC_ID = rcp.PRC_ID
		AND PRC_DELETE_FLAG = 0
	LEFT JOIN (
		SELECT PRC_CODE
			,PRC_ID 
			,COALESCE(PLG_Value, PRC_VALUE) AS PRC_VALUE
			,SBJ_CODE
			,COALESCE(SLG_VALUE, SBJ_VALUE) AS SBJ_VALUE
			,COUNT(*) AS AssociatedReleaseCountPerProduct
			,COALESCE(TLG_VALUE, THM_VALUE) AS THM_VALUE
			,THM_CODE
		FROM TD_RELEASE
		INNER JOIN TM_RELEASE_PRODUCT ON RLS_ID = RPR_RLS_ID
		INNER JOIN TD_PRODUCT ON RPR_PRC_ID = PRC_ID
		AND RLS_DELETE_FLAG = 0
		AND PRC_DELETE_FLAG = 0
		AND RPR_DELETE_FLAG = 0
		INNER JOIN (
			SELECT DISTINCT VRN_RLS_ID
			FROM VW_RELEASE_LIVE_NOW
			) dvrn ON RLS_ID = dvrn.VRN_RLS_ID
		INNER JOIN TD_SUBJECT ON PRC_SBJ_ID = SBJ_ID
			AND SBJ_DELETE_FLAG = 0
		INNER JOIN TD_THEME ON SBJ_THM_ID = THM_ID
			AND THM_DELETE_FLAG = 0
		LEFT OUTER JOIN (
			SELECT SLG_SBJ_ID
				,SLG_VALUE
			FROM TD_SUBJECT_LANGUAGE
			INNER JOIN TS_LANGUAGE ON SLG_LNG_ID = LNG_ID
				AND LNG_DELETE_FLAG = 0
				AND LNG_ISO_CODE = @LngIsoCode
			) AS INNER_SLG ON SBJ_ID = INNER_SLG.SLG_SBJ_ID
		LEFT OUTER JOIN (
			SELECT prg.PLG_PRC_ID
				,prg.PLG_VALUE
			FROM TD_PRODUCT_LANGUAGE AS prg
			INNER JOIN TS_LANGUAGE ON prg.PLG_LNG_ID = LNG_ID
				AND LNG_DELETE_FLAG = 0
				AND LNG_ISO_CODE = @LngIsoCode
			) AS INNER_PRG ON PRC_ID = INNER_PRG.PLG_PRC_ID
		LEFT OUTER JOIN (
			SELECT TLG_THM_ID
				,TLG_VALUE
			FROM TD_THEME_LANGUAGE
			INNER JOIN TS_LANGUAGE ON TLG_LNG_ID = LNG_ID
				AND LNG_DELETE_FLAG = 0
				AND LNG_ISO_CODE = @LngIsoCode
			) AS INNER_TLG ON INNER_TLG.TLG_THM_ID = THM_ID
		INNER JOIN (
			SELECT mtr.MTR_RLS_ID
				,mtrLng.MtrRlsIdLng
				,MTR_CODE
				,mtrCodeLng
				,COALESCE(mtrCodeLng, MTR_CODE) AS code
			FROM (
				SELECT Mtr_CODE
					,MTR_RLS_ID
				FROM TD_MATRIX
				WHERE MTR_DELETE_FLAG = 0
					AND MTR_LNG_ID = @LngIdDefault
				) AS mtr
			LEFT OUTER JOIN (
				SELECT MTR_CODE AS mtrCodeLng
					,MTR_RLS_ID AS MtrRlsIdLng
				FROM TD_MATRIX
				INNER JOIN VW_RELEASE_LIVE_NOW ON MTR_ID = VRN_MTR_ID
					AND MTR_RLS_ID = VRN_RLS_ID
				WHERE MTR_LNG_ID = @LngId
					AND MTR_DELETE_FLAG = 0
				) AS mtrLng ON mtr.MTR_RLS_ID = mtrLng.MtrRlsIdLng
				AND mtr.MTR_CODE = mtrLng.mtrCodeLng
			) AS mtr ON mtr.MTR_RLS_ID = RLS_ID
		GROUP BY PRC_CODE
			,PLG_Value
			,PRC_VALUE
			,SBJ_CODE
			,SLG_VALUE
			,SBJ_VALUE
			,TLG_VALUE
			,THM_VALUE
			,THM_CODE
			,PRC_ID 
		) arc ON arc.PRC_ID  = prd.PRC_ID 
	GROUP BY prd.PRC_VALUE
		,prd.PRC_CODE
		,sbj.SBJ_CODE
		,sbj.SBJ_VALUE
		,thm.THM_CODE
		,thm.THM_VALUE
		,arc.AssociatedReleaseCountPerProduct
		,rcp.RelatedCountPerProduct) qry
	WHERE qry.PrcReleaseCount > 0
	ORDER BY ThmValue
		,SbjValue
		,PrcValue
END
GO


