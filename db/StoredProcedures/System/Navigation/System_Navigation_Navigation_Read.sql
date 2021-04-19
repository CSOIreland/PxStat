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
-- exec System_Navigation_Navigation_Read 'en','en'
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

	DECLARE @LngId INT

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngId = 0
		OR @LngId IS NULL
	BEGIN
		RETURN
	END

	DECLARE @LngIdDefault INT

	SET @LngIdDefault = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCodeDefault
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngIdDefault = 0
		OR @LngIdDefault IS NULL
	BEGIN
		RETURN
	END

	SELECT SBJ_CODE AS SbjCode
		,SBJ_VALUE AS SbjValue
		,PRC_CODE AS PrcCode
		,PRC_VALUE AS PrcValue
		,sum((
				CASE 
					WHEN RLS_ID IS NOT NULL
						THEN 1
					ELSE 0
					END
				)) AS PrcReleaseCount
		,THM_VALUE AS ThmValue
		,THM_CODE AS ThmCode
	FROM (
		SELECT PRC_CODE
			,SBJ_CODE
			,coalesce(PLG_Value, PRC_VALUE) AS PRC_VALUE
			,coalesce(SLG_VALUE, SBJ_VALUE) AS SBJ_VALUE
			,RLS_ID
			,PRC_DELETE_FLAG
			,coalesce(TLG_VALUE, THM_VALUE) AS THM_VALUE
			,THM_CODE
		FROM TD_PRODUCT
		INNER JOIN [TD_SUBJECT] ON SBJ_ID = PRC_SBJ_ID
			AND SBJ_DELETE_FLAG = 0
		INNER JOIN TD_THEME ON SBJ_THM_ID = THM_ID
			AND THM_DELETE_FLAG = 0
		LEFT JOIN (
			SELECT SLG_SBJ_ID
				,SLG_VALUE
			FROM TD_SUBJECT_LANGUAGE
			JOIN TS_LANGUAGE ON SLG_LNG_ID = LNG_ID
				AND LNG_DELETE_FLAG = 0
				AND LNG_ISO_CODE = @LngIsoCode
			) INNER_SLG ON SBJ_ID = INNER_SLG.SLG_SBJ_ID
		LEFT JOIN (
			SELECT prg.PLG_PRC_ID
				,prg.PLG_VALUE
			FROM TD_PRODUCT_LANGUAGE prg
			JOIN TS_LANGUAGE ON prg.PLG_LNG_ID = LNG_ID
				AND LNG_DELETE_FLAG = 0
				AND LNG_ISO_CODE = @LngIsoCode
			) INNER_PRG ON PRC_ID = INNER_PRG.PLG_PRC_ID
		LEFT JOIN TD_RELEASE ON RLS_PRC_ID = PRC_ID
			AND RLS_DELETE_FLAG = 0
		LEFT JOIN (
			SELECT TLG_THM_ID
				,TLG_VALUE
			FROM TD_THEME_LANGUAGE
			INNER JOIN TS_LANGUAGE ON TLG_LNG_ID = LNG_ID
				AND LNG_DELETE_FLAG = 0
				AND LNG_ISO_CODE = @LngIsoCode
			) INNER_TLG ON INNER_TLG.TLG_THM_ID = THM_ID
		INNER JOIN (
			--get matrix items where a language version exists, otherwise get the matrixes in the main language
			SELECT mtr.MTR_RLS_ID
				,mtrLng.MtrRlsIdLng
				,MTR_CODE
				,mtrCodeLng
				,coalesce(mtrCodeLng, MTR_CODE) AS code
			FROM (
				SELECT Mtr_CODE
					,MTR_RLS_ID
				FROM TD_MATRIX
				WHERE MTR_DELETE_FLAG = 0
					AND MTR_LNG_ID = @LngIdDefault
				) mtr
			LEFT JOIN (
				SELECT MTR_CODE AS mtrCodeLng
					,MTR_RLS_ID AS MtrRlsIdLng
				FROM TD_MATRIX
				WHERE MTR_LNG_ID = @LngId
					AND MTR_DELETE_FLAG = 0
				) mtrLng ON mtr.MTR_RLS_ID = mtrLng.MtrRlsIdLng
				AND mtr.MTR_CODE = mtrLng.mtrCodeLng
			) mtr ON mtr.MTR_RLS_ID = RLS_ID
		WHERE RLS_ID IN (
				SELECT VRN_RLS_ID
				FROM VW_RELEASE_LIVE_NOW
				)
		) INNER_SELECT
	WHERE PRC_DELETE_FLAG = 0
	GROUP BY PRC_CODE
		,PRC_VALUE
		,SBJ_CODE
		,SBJ_VALUE
		,THM_CODE
		,THM_VALUE
	ORDER BY ThmValue
		,SbjValue
		,PrcValue
END
GO


