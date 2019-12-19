SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/11/2018
-- Description:	Reads Workflows Awaiting Signoff
-- exec Workflow_ReadAwaitingSignoff 'okeeffene','ss','en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_ReadAwaitingSignoff @CcnUsername NVARCHAR(256)
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
	,@RlsCode INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ccnID INT

	SET @ccnID = (
			SELECT ccn_id
			FROM TD_ACCOUNT
			WHERE CCN_DELETE_FLAG = 0
				AND CCN_USERNAME = @CcnUsername
			)

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	DECLARE @LngId INT
	DECLARE @LngDefaultId INT

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)
	SET @LngDefaultId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCodeDefault
				AND LNG_DELETE_FLAG = 0
			)

	SELECT DISTINCT rls.RLS_CODE AS RlsCode
		,mtr.MTR_CODE AS MtrCode
		,grp.GRP_CODE AS GrpCode
		,grp.GRP_NAME AS GrpName
		,rqs.RQS_CODE AS RqsCode
		,rqs.Rqs_Value AS RqsValue
		,cmmRequest.CMM_VALUE AS CmmRequestValue
		,cmmResponse.CMM_VALUE AS CmmResponseValue
		,RLS_EMERGENCY_FLAG AS RlsEmergencyFlag
		,WRQ_DATETIME AS WrqDatetime
		,DHT_DATETIME AS DhtDatetime
		,CCN_USERNAME AS CcnUsername
		,coalesce(mtrLng.MTR_TITLE, mtr.MTR_TITLE) MtrTitle
	FROM TD_RELEASE rls
	INNER JOIN TD_MATRIX mtr
		ON mtr.MTR_RLS_ID = rls.RLS_ID
			AND rls.RLS_DELETE_FLAG = 0
			AND mtr.MTR_DELETE_FLAG = 0
	LEFT JOIN (
		SELECT MTR_CODE
			,MTR_TITLE
			,MTR_RLS_ID
		FROM TD_MATRIX
		WHERE MTR_DELETE_FLAG = 0
			AND MTR_LNG_ID = @LngId
		) mtrLng
		ON mtr.MTR_CODE = mtrLng.MTR_CODE
			AND MTR.MTR_RLS_ID = mtrLng.MTR_RLS_ID
	INNER JOIN TD_GROUP grp
		ON rls.RLS_GRP_ID = grp.GRP_ID
			AND grp.GRP_DELETE_FLAG = 0
			AND rls.RLS_DELETE_FLAG = 0
	INNER JOIN TD_WORKFLOW_REQUEST wrq
		ON rls.RLS_ID = wrq.WRQ_RLS_ID
			AND wrq.WRQ_DELETE_FLAG = 0
			AND rls.RLS_DELETE_FLAG = 0
	INNER JOIN TS_REQUEST rqs
		ON rqs.RQS_ID = wrq.WRQ_RQS_ID
			AND wrq.WRQ_DELETE_FLAG = 0
	INNER JOIN TD_WORKFLOW_RESPONSE wrs
		ON wrq.WRQ_ID = wrs.WRS_WRQ_ID
			AND wrq.WRQ_DELETE_FLAG = 0
	INNER JOIN TD_COMMENT cmmRequest
		ON wrq.WRQ_CMM_ID = cmmRequest.CMM_ID
			AND cmmRequest.CMM_DELETE_FLAG = 0
	INNER JOIN TD_COMMENT cmmResponse
		ON wrs.WRS_CMM_ID = cmmResponse.CMM_ID
			AND cmmResponse.CMM_DELETE_FLAG = 0
	INNER JOIN TM_AUDITING_HISTORY
		ON WRS_DTG_ID = DHT_DTG_ID
	INNER JOIN TS_AUDITING_TYPE
		ON DHT_DTP_ID = DTP_ID
			AND DTP_CODE = 'CREATED'
	INNER JOIN TD_ACCOUNT
		ON CCN_ID = DHT_CCN_ID
			AND CCN_DELETE_FLAG = 0
	INNER JOIN @GroupUserHasAccess g
		ON g.GRP_ID = rls.RLS_GRP_ID
	LEFT JOIN TD_WORKFLOW_SIGNOFF wsg
		ON wrs.WRS_ID = wsg.WSG_WRS_ID
	WHERE WSG_ID IS NULL -- must be null as there is no signoff yet		
		AND wrq.WRQ_CURRENT_FLAG <> 0
		AND (
			@RlsCode IS NULL
			OR @RlsCode = rls.RLS_CODE
			)
		AND mtr.MTR_LNG_ID IN (
			@LngId
			,@LngDefaultId
			)
END
GO


