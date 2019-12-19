SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 11/06/2019
-- Description:	Reads a history of release uploads based on either the user's group or the user's privilege
-- exec Data_Matrix_ReadHistory 'okeeffene','2019-12-01','2019-12-30','ga','en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadHistory @CcnUsername NVARCHAR(256)
	,@DateFrom DATETIME
	,@DateTo DATETIME
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);
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
	SET @DateTo = dateadd(day, 1, @DateTo)

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	SELECT q.MtrCode
		,q.RlsCode
		,q.RlsLiveDatetimeFrom
		,q.RlsLiveDatetimeTo
		,q.RlsVersion
		,q.RlsRevision
		,q.GrpCode
		,q.GrpName
		,q.CcnUsername 
		,max(q.CreateDatetime) as CreateDateTime
		,q.MtrTitle
		,q.RqsValue
		,q.RqsCode
		,q.RspValue
		,q.RspCode
		,q.SgnValue
		,q.SgnCode
	FROM (
		SELECT mtr.MTR_CODE MtrCode
			,RLS_CODE RlsCode
			,RLS_LIVE_DATETIME_FROM RlsLiveDatetimeFrom
			,RLS_LIVE_DATETIME_TO RlsLiveDatetimeTo
			,RLS_VERSION RlsVersion
			,RLS_REVISION RlsRevision
			,GRP_CODE AS GrpCode
			,GRP_NAME AS GrpName
			,CCN_USERNAME AS CcnUsername
			,COALESCE(DHT_UPDATE.DHT_DATETIME, DHT_CREATE.DHT_DATETIME) AS CreateDatetime
			,coalesce(mtrLng.MTR_TITLE, mtr.MTR_TITLE) MtrTitle
			,mtr.MTR_LNG_ID
			,RQS_VALUE RqsValue
			,RSP_VALUE RspValue
			,SGN_VALUE SgnValue
			,RQS_CODE RqsCode
			,RSP_CODE RspCode
			,SGN_CODE SgnCode
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
		INNER JOIN TD_RELEASE
			ON RLS_ID = mtr.MTR_RLS_ID
				AND RLS_DELETE_FLAG = 0
				AND MTR_DELETE_FLAG = 0
		INNER JOIN @GroupUserHasAccess g
			ON g.GRP_ID = RLS_GRP_ID
		INNER JOIN TD_GROUP
			ON RLS_GRP_ID = TD_GROUP.GRP_ID
				AND GRP_DELETE_FLAG = 0
		INNER JOIN TD_AUDITING
			ON MTR_DTG_ID = DTG_ID
		INNER JOIN TM_AUDITING_HISTORY DHT_CREATE
			ON DTG_ID = DHT_CREATE.DHT_DTG_ID
		INNER JOIN TD_ACCOUNT
			ON DHT_CREATE.DHT_CCN_ID = CCN_ID
		INNER JOIN TS_AUDITING_TYPE DTP_CREATE
			ON DHT_CREATE.DHT_DTP_ID = DTP_CREATE.DTP_ID
				AND DTP_CREATE.DTP_CODE = 'CREATED'
		LEFT JOIN TM_AUDITING_HISTORY DHT_UPDATE
			ON DTG_ID = DHT_UPDATE.DHT_DTG_ID
		LEFT JOIN TS_AUDITING_TYPE DTP_UPDATE
			ON DHT_UPDATE.DHT_DTP_ID = DTP_UPDATE.DTP_ID
				AND DHT_UPDATE.DHT_DATETIME = (
					SELECT max(DHT_DATETIME)
					FROM TM_AUDITING_HISTORY
					WHERE DHT_DTG_ID = DTG_ID
					)
				AND DTP_UPDATE.DTP_CODE = 'UPDATED'
				LEFT JOIN TD_WORKFLOW_REQUEST
			ON WRQ_RLS_ID = RLS_ID
				AND WRQ_CURRENT_FLAG = 1
				AND WRQ_DELETE_FLAG = 0
		LEFT JOIN TS_REQUEST
			ON WRQ_RQS_ID = RQS_ID
		LEFT JOIN TD_WORKFLOW_RESPONSE
			ON WRQ_ID = WRS_WRQ_ID
		LEFT JOIN TS_RESPONSE
			ON WRS_RSP_ID = RSP_ID
		LEFT JOIN TD_WORKFLOW_SIGNOFF
			ON WRS_ID = WSG_WRS_ID
		LEFT JOIN TS_SIGNOFF
			ON WSG_SGN_ID = SGN_ID
		WHERE MTR_DELETE_FLAG = 0
			AND DHT_CREATE.DHT_DATETIME >= @DateFrom
			AND DHT_CREATE.DHT_DATETIME <= @DateTo
		) q
	WHERE q.MTR_LNG_ID IN (
			@LngId
			,@LngDefaultId
			)
	GROUP BY q.MtrCode
		,q.RlsCode
		,q.RlsLiveDatetimeFrom
		,q.RlsLiveDatetimeTo
		,q.RlsVersion
		,q.RlsRevision
		,q.GrpCode
		,q.GrpName
		,q.MtrTitle
		,q.CcnUsername 
		,q.RqsValue
		,q.RqsCode
		,q.RspValue
		,q.RspCode
		,q.SgnValue
		,q.SgnCode
	ORDER BY q.MtrCode
		,q.RlsVersion
		,q.RlsRevision
END
GO


