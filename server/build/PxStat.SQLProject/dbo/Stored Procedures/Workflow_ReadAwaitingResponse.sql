
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/11/2018
-- Description:	Read Workflows Awaiting Approval
-- exec Workflow_ReadAwaitingResponse 'okeeffene','ga','en'
-- =============================================
CREATE
	

 PROCEDURE Workflow_ReadAwaitingResponse @CcnUsername NVARCHAR(256)
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
	,@RlsCode INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ccnID AS INT;

	SET @ccnID = (
			SELECT ccn_id
			FROM TD_ACCOUNT
			WHERE CCN_DELETE_FLAG = 0
				AND CCN_USERNAME = @CcnUsername
			);

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
		,cmm.CMM_VALUE AS CmmRequestValue
		,RLS_EXCEPTIONAL_FLAG AS RlsExceptionalFlag
		,WRQ_DATETIME AS WrqDatetime
		,DHT_DATETIME AS DhtDatetime
		,CCN_USERNAME AS CcnUsername
		,coalesce(mtrLng.MTR_TITLE, mtr.MTR_TITLE) MtrTitle
	FROM TD_MATRIX mtr
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
	INNER JOIN TD_RELEASE rls
		ON mtr.MTR_RLS_ID = rls.RLS_ID
			AND rls.RLS_DELETE_FLAG = 0
			AND mtr.MTR_DELETE_FLAG = 0
	INNER JOIN TD_GROUP AS grp
		ON rls.RLS_GRP_ID = grp.GRP_ID
			AND grp.GRP_DELETE_FLAG = 0
			AND rls.RLS_DELETE_FLAG = 0
	INNER JOIN @GroupUserHasAccess g
		ON g.GRP_ID = rls.RLS_GRP_ID
	INNER JOIN TD_WORKFLOW_REQUEST AS wrq
		ON rls.RLS_ID = wrq.WRQ_RLS_ID
			AND wrq.WRQ_DELETE_FLAG = 0
			AND rls.RLS_DELETE_FLAG = 0
	INNER JOIN TD_COMMENT cmm
		ON wrq.WRQ_CMM_ID = cmm.CMM_ID
	INNER JOIN TM_AUDITING_HISTORY
		ON WRQ_DTG_ID = DHT_DTG_ID
	INNER JOIN TS_AUDITING_TYPE
		ON DHT_DTP_ID = DTP_ID
			AND DTP_CODE = 'CREATED'
	INNER JOIN TD_ACCOUNT
		ON DHT_CCN_ID = CCN_ID
			AND CCN_DELETE_FLAG = 0
	LEFT OUTER JOIN TS_REQUEST AS rqs
		ON rqs.RQS_ID = wrq.WRQ_RQS_ID
			AND wrq.WRQ_DELETE_FLAG = 0
	LEFT OUTER JOIN TD_WORKFLOW_RESPONSE wrs
		ON wrq.WRQ_ID = wrs.WRS_WRQ_ID
			AND wrq.WRQ_DELETE_FLAG = 0
	WHERE wrs.WRS_ID IS NULL --We are awaiting response, so this must be null
		AND (
			wrq.WRQ_CURRENT_FLAG IS NULL
			OR wrq.WRQ_CURRENT_FLAG <> 0
			)
		AND (
			@RlsCode IS NULL
			OR @RlsCode = rls.RLS_CODE
			)
		AND mtr.MTR_LNG_ID IN (
			@LngId
			,@LngDefaultId
			)
END
