SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 02/01/2020
-- Rewritten : 19/04/2021 Neil O'Keeffe
-- Description:	Reads current releases, referencing metadata
-- exec Data_Release_ReadMetaCollection_NewStructure 'en','en','2021-01-01','API'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_ReadMetaCollection @LngIsoCodeDefault CHAR(2)
	,@LngIsoCodeRead CHAR(2)
	,@DateFrom DATE = NULL
	,@PrcCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @PrcID INT
	DECLARE @ClsId INT
	DECLARE @FrqId INT
	DECLARE @SttId INT

	SET @ClsId = (
			SELECT DMR_ID
			FROM TS_DIMENSION_ROLE
			WHERE DMR_CODE = 'CLASSIFICATION'
			)
	SET @FrqId = (
			SELECT DMR_ID
			FROM TS_DIMENSION_ROLE
			WHERE DMR_CODE = 'TIME'
			)
	SET @SttId = (
			SELECT DMR_ID
			FROM TS_DIMENSION_ROLE
			WHERE DMR_CODE = 'STATISTIC'
			)

	IF @PrcCode IS NOT NULL
	BEGIN
		SET @PrcID = (
				SELECT PRC_ID
				FROM TD_PRODUCT
				WHERE PRC_CODE = @PrcCode
					AND PRC_DELETE_FLAG = 0
				)
	END

	DECLARE @Matrix TABLE (
		MTR_ID INT
		,MTR_CODE NVARCHAR(20)
		,LNG_ISO_CODE CHAR(2)
		,RLS_ID INT
		,CPR_ID INT
		,MTR_TITLE NVARCHAR(256)
		,LNG_ISO_NAME NVARCHAR(32)
		)

	SELECT MTR_ID
		,MTR_CODE
		,LNG_ISO_CODE
		,RLS_ID
		,MTR_CPR_ID AS CPR_ID
		,MTR_TITLE
		,LNG_ISO_NAME
	INTO #DefaultMatrix
	FROM TD_RELEASE
	INNER JOIN VW_RELEASE_LIVE_NOW ON RLS_ID = VRN_RLS_ID
		AND RLS_DELETE_FLAG = 0
		AND (
			RLS_LIVE_DATETIME_FROM > @DateFrom
			OR @DateFrom IS NULL
			)
		AND (
			@PrcCode IS NULL
			OR RLS_PRC_ID = @PrcID
			)
	INNER JOIN TD_MATRIX ON MTR_RLS_ID = RLS_ID
		AND MTR_ID = VRN_MTR_ID
		AND MTR_DELETE_FLAG = 0
	INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
		AND LNG_DELETE_FLAG = 0
		AND LNG_ISO_CODE = @LngIsoCodeDefault
	WHERE (
			@PrcID IS NULL
			OR RLS_PRC_ID = @PrcID
			)
		AND (
			@DateFrom IS NULL
			OR RLS_LIVE_DATETIME_FROM >= @DateFrom
			)

	IF @LngIsoCodeDefault <> @LngIsoCodeRead
	BEGIN
		SELECT MTR_ID
			,MTR_CODE
			,LNG_ISO_CODE
			,RLS_ID
			,MTR_CPR_ID AS CPR_ID
			,MTR_TITLE
			,LNG_ISO_NAME
		INTO #ReadMatrix
		FROM TD_RELEASE
		INNER JOIN VW_RELEASE_LIVE_NOW ON RLS_ID = VRN_RLS_ID
			AND RLS_DELETE_FLAG = 0
			AND (
				RLS_LIVE_DATETIME_FROM > @DateFrom
				OR @DateFrom IS NULL
				)
			AND (
				@PrcCode IS NULL
				OR RLS_PRC_ID = @PrcID
				)
		INNER JOIN TD_MATRIX ON MTR_RLS_ID = RLS_ID
			AND MTR_ID = VRN_MTR_ID
			AND MTR_DELETE_FLAG = 0
		INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
			AND LNG_DELETE_FLAG = 0
			AND LNG_ISO_CODE = @LngIsoCodeRead
		WHERE (
				@PrcID IS NULL
				OR RLS_PRC_ID = @PrcID
				)
			AND (
				@DateFrom IS NULL
				OR RLS_LIVE_DATETIME_FROM >= @DateFrom
				)

		INSERT INTO @Matrix
		SELECT coalesce(#ReadMatrix.MTR_ID, #DefaultMatrix.MTR_ID) AS MTR_ID
			,#DefaultMatrix.MTR_CODE
			,coalesce(#ReadMatrix.LNG_ISO_CODE, #DefaultMatrix.LNG_ISO_CODE) AS LNG_ISO_CODE
			,#DefaultMatrix.RLS_ID
			,#DefaultMatrix.CPR_ID
			,coalesce(#ReadMatrix.MTR_TITLE, #DefaultMatrix.MTR_TITLE)
			,coalesce(#ReadMatrix.LNG_ISO_NAME, #DefaultMatrix.LNG_ISO_NAME) AS LNG_ISO_NAME
		FROM #DefaultMatrix
		LEFT JOIN #ReadMatrix ON #DefaultMatrix.MTR_CODE = #ReadMatrix.MTR_CODE
	END
	ELSE
	BEGIN
		INSERT INTO @Matrix
		SELECT MTR_ID
			,MTR_CODE
			,LNG_ISO_CODE
			,RLS_ID
			,CPR_ID
			,MTR_TITLE
			,LNG_ISO_NAME
		FROM #DefaultMatrix
	END

	SELECT RLS_CODE AS RlsCode
		,MTR_CODE AS MtrCode
		,LNG_ISO_CODE AS LngIsoCode
		,LNG_ISO_NAME AS LngIsoName
		,MTR_TITLE AS MtrTitle
		,CPR_VALUE AS CprValue
		,CPR_URL AS CprUrl
		,CPR_CODE AS CprCode
		,RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
		,RLS_LIVE_DATETIME_TO AS RlsLiveDatetimeTo
		,RLS_EXCEPTIONAL_FLAG AS ExceptionalFlag
		,mdFrq.MDM_CODE AS FrqCode
		,mdFrq.MDM_VALUE AS FrqValue
		,diPrd.DMT_CODE AS PrdCode
		,diPrd.DMT_VALUE AS PrdValue
		,mdStt.MDM_CODE AS SttCode
		,mdStt.MDM_VALUE AS SttValue
		,mdCls.MDM_CODE AS ClsCode
		,mdCls.MDM_VALUE AS ClsValue
	FROM TD_RELEASE
	INNER JOIN VW_RELEASE_LIVE_NOW ON RLS_ID = VRN_RLS_ID
		AND RLS_DELETE_FLAG = 0
		AND (
			RLS_LIVE_DATETIME_FROM > @DateFrom
			OR @DateFrom IS NULL
			)
		AND (
			@PrcCode IS NULL
			OR RLS_PRC_ID = @PrcID
			)
	INNER JOIN @Matrix m ON m.RLS_ID = TD_RELEASE.RLS_ID
		AND MTR_ID = VRN_MTR_ID
	INNER JOIN TD_MATRIX_DIMENSION mdCls ON MTR_ID = mdCls.MDM_MTR_ID
		AND mdCls.MDM_DMR_ID = @ClsId
	INNER JOIN TD_MATRIX_DIMENSION mdFrq ON MTR_ID = mdFrq.MDM_MTR_ID
		AND mdFrq.MDM_DMR_ID = @FrqId
	INNER JOIN TD_MATRIX_DIMENSION mdStt ON MTR_ID = mdStt.MDM_MTR_ID
		AND mdStt.MDM_DMR_ID = @SttId
	INNER JOIN TD_DIMENSION_ITEM diPrd ON DMT_MDM_ID = mdFrq.MDM_ID

	INNER JOIN TS_COPYRIGHT ON m.CPR_ID = TS_COPYRIGHT.CPR_ID
END
GO


