SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/11/2018
-- Description:	Reads Workflow History for a given Release Code
-- exec Workflow_ReadHistory 'okeeffene',43
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_ReadHistory @CcnUsername NVARCHAR(256)
	,@RlsCode INT
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

	SELECT DISTINCT rls.RLS_CODE AS RlsCode
		,rqs.RQS_CODE AS RqsCode
		,rqs.RQS_VALUE AS RqsValue
		,wrqCmm.CMM_VALUE AS WrqCmmValue
		,wrq.WRQ_DATETIME AS WrqDatetime
		,wrq.WRQ_EMERGENCY_FLAG AS WrqEmergencyFlag
		,wrq.WRQ_RESERVATION_FLAG AS WrqReservationFlag
		,wrq.WRQ_ALERT_FLAG AS WrqAlertFlag
		,wrq.WRQ_ARCHIVE_FLAG AS WrqArchiveFlag
		,wrqCcn.CCN_USERNAME AS WrqDtgCreateCcnUsername
		,wrqDht.DHT_DATETIME AS WrqDtgCreateDatetime
		,rsp.RSP_CODE AS RspCode
		,rsp.RSP_VALUE AS RspValue
		,wrsCmm.CMM_VALUE AS WrsCmmValue
		,wrsCcn.CCN_USERNAME AS WrsDtgCreateCcnUsername
		,wrsDht.DHT_DATETIME AS WrsDtgCreateDatetime
		,sgn.SGN_CODE AS SgnCode
		,sgn.SGN_VALUE AS SgnValue
		,wsgCmm.CMM_VALUE WsgCmmValue
		,wsgCcn.CCN_USERNAME AS WsgDtgCreateCcnUsername
		,wsgDht.DHT_DATETIME AS WsgDtgCreateDatetime
	FROM TD_RELEASE rls
	INNER JOIN TD_MATRIX mtr
		ON rls.RLS_ID = mtr.MTR_RLS_ID
			AND mtr.MTR_DELETE_FLAG = 0
	INNER JOIN TD_WORKFLOW_REQUEST wrq
		ON wrq.WRQ_RLS_ID = rls.RLS_ID
			AND wrq.WRQ_DELETE_FLAG = 0
	INNER JOIN TS_REQUEST rqs
		ON rqs.RQS_ID = wrq.WRQ_RQS_ID
			AND wrq.WRQ_DELETE_FLAG = 0
	INNER JOIN TD_COMMENT wrqCmm
		ON wrq.WRQ_CMM_ID = wrqCmm.CMM_ID
			AND wrqCmm.CMM_DELETE_FLAG = 0
	INNER JOIN TD_AUDITING wrqDtg
		ON wrq.WRQ_DTG_ID = wrqDtg.DTG_ID
	INNER JOIN TM_AUDITING_HISTORY wrqDht
		ON wrqDht.DHT_DTG_ID = wrqDtg.DTG_ID
	INNER JOIN TS_AUDITING_TYPE wrqDtp
		ON wrqDtp.DTP_ID = wrqDht.DHT_DTP_ID
	INNER JOIN TD_ACCOUNT wrqCcn
		ON wrqDht.DHT_CCN_ID = wrqCcn.CCN_ID
			AND wrqDtp.DTP_CODE = 'CREATED'
	LEFT JOIN TD_WORKFLOW_RESPONSE wrs
		ON wrq.WRQ_ID = wrs.WRS_WRQ_ID
			AND wrq.WRQ_DELETE_FLAG = 0
	LEFT JOIN TS_RESPONSE rsp
		ON wrs.WRS_RSP_ID = rsp.RSP_ID
	LEFT JOIN TD_COMMENT wrsCmm
		ON wrs.WRS_CMM_ID = wrsCmm.CMM_ID
	LEFT JOIN TD_AUDITING wrsDtg
		ON wrs.WRS_DTG_ID = wrsDtg.DTG_ID
	LEFT JOIN TM_AUDITING_HISTORY wrsDht
		ON wrsDtg.DTG_ID = wrsDht.DHT_DTG_ID
	LEFT JOIN TS_AUDITING_TYPE wrsDtp
		ON wrsDht.DHT_DTP_ID = wrsDtp.DTP_ID
	LEFT JOIN TD_ACCOUNT wrsCcn
		ON wrsDht.DHT_CCN_ID = wrsCcn.CCN_ID
			AND wrsDtp.DTP_CODE = 'CREATED'
	LEFT JOIN TD_WORKFLOW_SIGNOFF wsg
		ON wrs.WRS_ID = wsg.WSG_WRS_ID
	LEFT JOIN TS_SIGNOFF sgn
		ON wsg.WSG_SGN_ID = sgn.SGN_ID
	LEFT JOIN TD_COMMENT wsgCmm
		ON wsg.WSG_CMM_ID = wsgCmm.CMM_ID
	LEFT JOIN TD_AUDITING wsgDtg
		ON wsg.WSG_DTG_ID = wsgDtg.DTG_ID
	LEFT JOIN TM_AUDITING_HISTORY wsgDht
		ON wsgDtg.DTG_ID = wsgDht.DHT_DTG_ID
	LEFT JOIN TS_AUDITING_TYPE wsgDtp
		ON wsgDht.DHT_DTP_ID = wsgDtp.DTP_ID
	LEFT JOIN TD_ACCOUNT wsgCcn
		ON wsgDht.DHT_CCN_ID = wsgCcn.CCN_ID
			AND wsgDtp.DTP_CODE = 'CREATED'
			AND wsgCcn.CCN_DELETE_FLAG = 0
	LEFT JOIN TM_GROUP_ACCOUNT gcc
		ON rls.RLS_GRP_ID = gcc.GCC_ID
			AND gcc.GCC_DELETE_FLAG = 0
			AND GCC_CCN_ID = @ccnID
	LEFT JOIN TD_ACCOUNT ccn
		ON gcc.GCC_CCN_ID = ccn.CCN_ID
			AND ccn.CCN_DELETE_FLAG = 0
	WHERE rls.RLS_CODE = @RlsCode
		AND @CcnUsername IN (
			SELECT ccn.CCN_USERNAME
			FROM TD_ACCOUNT ccn
			INNER JOIN TS_PRIVILEGE prv
				ON ccn.CCN_PRV_ID = prv.PRV_ID
					AND (
						prv.PRV_CODE = 'ADMINISTRATOR'
						OR PRV.PRV_CODE = 'POWER_USER'
						OR RLS_GRP_ID IN (
							SELECT GRP_ID
							FROM TD_GROUP
							INNER JOIN TM_GROUP_ACCOUNT
								ON GRP_ID = GCC_GRP_ID
									AND GCC_CCN_ID = @ccnID
							)
						)
					AND ccn.CCN_DELETE_FLAG = 0
			)
		OR (ccn.CCN_USERNAME = @CcnUsername)
END
GO


