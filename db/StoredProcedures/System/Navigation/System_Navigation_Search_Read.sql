SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 19/01/2021
-- Description:	Returns the metadata for the search results
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Search_Read @LngIsoCode CHAR(2)
	,@Result KeyValueVarcharAttribute Readonly
	,@DefaultLngIsoCode CHAR(2)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @LngID INT

	SET @LngID = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngID IS NULL
		OR @LngID = 0
	BEGIN
		RETURN
	END

	DECLARE @DefaultLngId INT

	SET @DefaultLngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @DefaultLngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @DefaultLngID IS NULL
		OR @DefaultLngID = 0
	BEGIN
		RETURN
	END

	--We must get a version of the Product table that is language invariant
	SELECT PRC_ID
		,PRC_CODE
		,PRC_SBJ_ID
		,COALESCE(PLG_VALUE, PRC_VALUE) AS PRC_VALUE
		,COALESCE(PLG_LNG_ID, PRC_LNG_ID) AS PRC_LNG_ID
	INTO #Product
	FROM TD_PRODUCT
	LEFT JOIN TD_PRODUCT_LANGUAGE
		ON PRC_ID = PLG_PRC_ID
			AND PRC_DELETE_FLAG = 0
			AND PLG_LNG_ID = @LngID

	--We must get a version of the Subject table that is language invariant
	SELECT SBJ_ID
		,SBJ_CODE
		,COALESCE(SLG_VALUE, SBJ_VALUE) AS SBJ_VALUE
	INTO #Subject
	FROM TD_SUBJECT
	LEFT JOIN TD_SUBJECT_LANGUAGE
		ON SBJ_ID = SLG_SBJ_ID
			AND SBJ_DELETE_FLAG = 0
			AND SLG_LNG_ID = @LngID



	SELECT  RLS_CODE AS RlsCode
		,m1.MTR_CODE AS MtrCode
		,coalesce(m2.MTR_TITLE,m1.mtr_title) AS MtrTitle
		,m1.MTR_OFFICIAL_FLAG AS MtrOfficialFlag
		,SBJ_CODE AS SbjCode
		,SBJ_VALUE AS SbjValue
		,PRC_CODE AS PrcCode
		,PRC_VALUE AS PrcValue
		,RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
		,RLS_EXCEPTIONAL_FLAG AS RlsExceptionalFlag
		,RLS_RESERVATION_FLAG AS RlsReservationFlag
		,RLS_ARCHIVE_FLAG AS RlsArchiveFlag
		,RLS_ANALYTICAL_FLAG AS RlsAnalyticalFlag
		,RLS_EXPERIMENTAL_FLAG AS RlsExperimentalFlag
		,CPR_CODE AS CprCode
		,CPR_VALUE AS CprValue

		,mdmCls.MDM_ID AS ClsId
		,mdmCls.MDM_CODE AS ClsCode
		,mdmCls.MDM_VALUE AS ClsValue
		,mdmCls.MDM_GEO_FLAG AS ClsGeoFlag
		,mdmCls.MDM_GEO_URL AS ClsGeoUrl

		,mdmFrq.MDM_CODE AS FrqCode
		,mdmFrq.MDM_VALUE AS FrqValue

		,dmtTime.DMT_CODE AS PrdCode
		,dmtTime.DMT_VALUE AS PrdValue
		,dmtTime.DMT_ID AS PrdId

		,m1.MTR_LNG_ID AS MtrLngID
		,LNG_ISO_CODE AS LngIsoCode
		,LNG_ISO_NAME AS LngIsoName
		,Attribute AS Score
	FROM @Result
	INNER JOIN TD_MATRIX m1
		ON [KEY] = MTR_CODE
			AND MTR_DELETE_FLAG = 0
	INNER JOIN TD_RELEASE
		ON MTR_RLS_ID = RLS_ID
			AND RLS_DELETE_FLAG = 0
	INNER JOIN VW_RELEASE_LIVE_NOW
		ON RLS_ID = VRN_RLS_ID
	INNER JOIN #Product
		ON RLS_PRC_ID = PRC_ID
	INNER JOIN #Subject
		ON PRC_SBJ_ID = SBJ_ID
	INNER JOIN TS_COPYRIGHT
		ON MTR_CPR_ID = CPR_ID
			AND CPR_DELETE_FLAG = 0

	INNER JOIN TD_MATRIX_DIMENSION mdmCls
		ON MTR_ID = mdmCls.MDM_MTR_ID
	INNER JOIN TS_DIMENSION_ROLE 
	ON mdmCls.MDM_DMR_ID =DMR_ID
	AND DMR_CODE='CLASSIFICATION'

	INNER JOIN TD_MATRIX_DIMENSION mdmFrq
		ON MTR_ID = mdmFrq.MDM_MTR_ID
	INNER JOIN TS_DIMENSION_ROLE frqDM
	ON mdmFrq.MDM_DMR_ID =frqDM.DMR_ID
	AND frqDM.DMR_CODE='TIME'

	INNER JOIN TD_DIMENSION_ITEM dmtTime
	ON dmtTime.DMT_MDM_ID =mdmfrq.MDM_ID 

	INNER JOIN TS_LANGUAGE
		ON MTR_LNG_ID = LNG_ID
			AND LNG_DELETE_FLAG = 0
	left JOIN TD_MATRIX m2
	on m1.MTR_CODE=m2.MTR_CODE
	and m1.MTR_RLS_ID=m2.MTR_RLS_ID 
	and m1.MTR_DELETE_FLAG=0
	and m2.MTR_DELETE_FLAG=0
	and m2.MTR_LNG_ID =@LngID
	and @LngID <>@DefaultLngId
	
END
GO


