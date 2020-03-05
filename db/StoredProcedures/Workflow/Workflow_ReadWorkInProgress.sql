SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:	Paulo Patricio
-- Read date: 12 Nov 2018
-- Description:	Reads record(s) from 
-- exec Workflow_ReadWorkInProgress 'okeeffene','en','en',99
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_ReadWorkInProgress @CcnUsername NVARCHAR(256)
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
	,@RlsCode INT = NULL
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

	SELECT distinct q.RlsCode
		,q.MtrCode
		,q.GrpCode
		,q.GrpName
		,q.RqsValue
		,q.CcnUsername 
		,q.DhtDatetime
		,q.MtrTitle
	FROM (
		SELECT RLS_CODE RlsCode
			,mtr.MTR_CODE MtrCode
			,GRP_CODE GrpCode
			,GRP_NAME GrpName
			,CCN_USERNAME CcnUsername
			,RQS_VALUE AS RqsValue
			,max(convert(nvarchar,DHT_DATETIME,120)) DhtDatetime
			,coalesce(mtrLng.MTR_TITLE, mtr.MTR_TITLE) MtrTitle
			,MTR_LNG_ID
			,WRQ_ID 
			,WRS_ID 
			,WSG_ID 
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
		INNER JOIN VW_RELEASE_WIP
			ON VRW_RLS_ID = RLS_ID
				AND VRW_MTR_ID = MTR_ID
		INNER JOIN @GroupUserHasAccess g
			ON g.GRP_ID = RLS_GRP_ID
		INNER JOIN TD_GROUP GRP
			ON GRP.GRP_ID = RLS_GRP_ID
				AND GRP_DELETE_FLAG = 0
		INNER JOIN TM_AUDITING_HISTORY
			ON MTR_DTG_ID = DHT_DTG_ID
		INNER JOIN TS_AUDITING_TYPE
			ON DHT_DTP_ID = DTP_ID
				AND DTP_CODE = 'CREATED'
		INNER JOIN TD_ACCOUNT
			ON DHT_CCN_ID = CCN_ID
				AND CCN_DELETE_FLAG = 0
		LEFT JOIN TD_WORKFLOW_REQUEST
			ON RLS_ID = WRQ_RLS_ID
		LEFT JOIN TS_REQUEST
			ON WRQ_RQS_ID = RQS_ID
		LEFT JOIN TD_WORKFLOW_RESPONSE  
		ON WRS_WRQ_ID =WRQ_ID
		LEFT JOIN TD_WORKFLOW_SIGNOFF 
		ON WSG_WRS_ID = WRS_ID 
		WHERE MTR_DELETE_FLAG = 0
			AND (
				@RlsCode IS NULL
				OR @RlsCode = RLS_CODE
				)
		GROUP BY RLS_CODE
			,mtr.MTR_CODE
			,GRP_CODE
			,GRP_NAME
			,CCN_USERNAME
			,RQS_VALUE
			,mtrLng.MTR_TITLE
			,mtr.MTR_TITLE
			,MTR_LNG_ID
			,WRQ_ID 
			,WRS_ID 
			,WSG_ID 
		) q
	WHERE q.MTR_LNG_ID IN (
			@LngId
			,@LngDefaultId
			)
			AND WRQ_ID IS NULL
			AND WRS_ID IS NULL
			AND WSG_ID IS NULL
	
	ORDER BY q.MtrCode
END
GO


