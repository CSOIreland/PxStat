
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 9 Nov 2018
-- Description:	Reads a list of releases for a given matrix code and username
--exec Data_Release_ReadList 'okeeffene','AHM04X','en','en'
-- =============================================
CREATE
	

 PROCEDURE Data_Release_ReadList @CcnUsername NVARCHAR(256)
	,@MtrCode NVARCHAR(256)
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	DECLARE @LngId INT
	DECLARE @LngIdDefault INT

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)
	SET @LngIdDefault = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCodeDefault
				AND LNG_DELETE_FLAG = 0
			)

	SELECT DISTINCT q.MtrCode
		,q.RlsCode
		,q.RlsAnalyticalFlag
		,q.RlsArchiveFlag
		,q.RlsExperimentalFlag 
		,q.RlsLiveFlag
		,q.RlsLiveDatetimeTo
		,q.RlsReservationFlag
		,q.RlsVersion
		,q.RlsRevision
		,q.RlsExceptionalFlag
		,q.RlsLiveDatetimeFrom
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
			,RLS_ANALYTICAL_FLAG RlsAnalyticalFlag
			,RLS_ARCHIVE_FLAG RlsArchiveFlag
			,RLS_EXPERIMENTAL_FLAG RlsExperimentalFlag
			,RLS_LIVE_FLAG RlsLiveFlag
			,RLS_LIVE_DATETIME_TO RlsLiveDatetimeTo
			,RLS_RESERVATION_FLAG RlsReservationFlag
			,RLS_REVISION RlsRevision
			,RLS_VERSION RlsVersion
			,RLS_EXCEPTIONAL_FLAG RlsExceptionalFlag
			,RLS_LIVE_DATETIME_FROM RlsLiveDatetimeFrom
			,coalesce(mtrLng.MTR_TITLE, mtr.MTR_TITLE) MtrTitle
			,RQS_VALUE RqsValue
			,mtr.MTR_LNG_ID
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
		LEFT JOIN TD_WORKFLOW_REQUEST
			ON WRQ_RLS_ID = RLS_ID
			and WRQ_CURRENT_FLAG=1
			and WRQ_DELETE_FLAG=0
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
		INNER JOIN @GroupUserHasAccess g
			ON g.GRP_ID = RLS_GRP_ID
		WHERE MTR_DELETE_FLAG = 0
			AND mtr.MTR_CODE = @MtrCode
			AND mtr.MTR_LNG_ID IN (
				@LngId
				,@LngIdDefault
				)

		) q
	ORDER BY q.RlsVersion
		,q.RlsRevision
END
