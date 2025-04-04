﻿
-- =============================================
-- Author:	Neil O'Keeffe
-- Read date: 18/12/2019
-- Description:	Reads pending live records
-- exec Workflow_ReadPendingLive 'okeeffene',null,'en','en'
-- =============================================
CREATE
	

 PROCEDURE Workflow_ReadPendingLive @CcnUsername NVARCHAR(256)
	,@RlsCode INT = NULL
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

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

	SELECT DISTINCT *
	FROM (
		SELECT RLS_CODE RlsCode
			,mtr.MTR_CODE MtrCode
			,GRP_CODE GrpCode
			,GRP_NAME GrpName
			,RQS_CODE RqsCode
			,RQS_VALUE RqsValue
			,CCN_USERNAME CcnUsername
			,RLS_RESERVATION_FLAG RlsReservationFlag
			,RLS_ARCHIVE_FLAG RlsArchiveFlag
			,MTR_OFFICIAL_FLAG MtrOfficialFlag
			,RLS_LIVE_DATETIME_FROM RlsLiveDatimeFrom
			,WRQ_DATETIME WrqDatetime
			,max(DHT_DATETIME) DhtDatetime
			,coalesce(mtrLng.MTR_TITLE, mtr.MTR_TITLE) MtrTitle
		FROM TD_MATRIX mtr
		LEFT JOIN (
			SELECT MTR_CODE
				,MTR_TITLE
				,MTR_RLS_ID
			FROM TD_MATRIX
			WHERE MTR_DELETE_FLAG = 0
				AND MTR_LNG_ID = @LngId
			) mtrLng ON mtr.MTR_CODE = mtrLng.MTR_CODE
			AND MTR.MTR_RLS_ID = mtrLng.MTR_RLS_ID
		INNER JOIN TD_RELEASE ON RLS_ID = mtr.MTR_RLS_ID
			AND RLS_DELETE_FLAG = 0
		INNER JOIN VW_RELEASE_PENDING_LIVE ON VPL_RLS_ID = RLS_ID
			AND VPL_MTR_ID = MTR_ID
		INNER JOIN @GroupUserHasAccess g ON g.GRP_ID = RLS_GRP_ID
		INNER JOIN TD_GROUP GRP ON GRP.GRP_ID = RLS_GRP_ID
			AND GRP_DELETE_FLAG = 0
		LEFT JOIN TD_WORKFLOW_REQUEST ON RLS_ID = WRQ_RLS_ID
			AND WRQ_DELETE_FLAG = 0
		LEFT JOIN TS_REQUEST ON WRQ_RQS_ID = RQS_ID
		LEFT JOIN TD_WORKFLOW_RESPONSE ON WRQ_ID = WRS_WRQ_ID
		LEFT JOIN TS_RESPONSE ON WRS_RSP_ID = RSP_ID
		INNER JOIN TD_WORKFLOW_SIGNOFF ON WRS_ID = WSG_WRS_ID
		INNER JOIN TS_SIGNOFF ON WSG_SGN_ID = SGN_ID
			AND SGN_CODE = 'APPROVED'
		LEFT JOIN TM_AUDITING_HISTORY ON WSG_DTG_ID = DHT_DTG_ID
		INNER JOIN TD_ACCOUNT ON DHT_CCN_ID = CCN_ID
			AND CCN_DELETE_FLAG = 0
		WHERE MTR_DELETE_FLAG = 0
			AND (
				@RlsCode IS NULL
				OR @RlsCode = RLS_CODE
				)
			AND mtr.MTR_LNG_ID IN (
				@LngId
				,@LngDefaultId
				)
			AND WRQ_DATETIME > getdate()
		GROUP BY RLS_CODE
			,mtr.MTR_CODE
			,GRP_CODE
			,GRP_NAME
			,RQS_CODE
			,RQS_VALUE
			,CCN_USERNAME
			,RLS_RESERVATION_FLAG
			,RLS_ARCHIVE_FLAG
			,MTR_OFFICIAL_FLAG
			,RLS_LIVE_DATETIME_FROM
			,WRQ_DATETIME
			,mtrLng.MTR_TITLE
			,mtr.MTR_TITLE
		) Q
	WHERE Q.RlsLiveDatimeFrom = Q.WrqDatetime
	ORDER BY Q.MtrCode
END
