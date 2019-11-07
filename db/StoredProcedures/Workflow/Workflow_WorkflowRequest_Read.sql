SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 15/11/2018
-- Description:	Reads workflow requests for a Release code. If the WrqCurrentFlag parameter is supplied, it only returns the requests with that flag value
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_WorkflowRequest_Read @RlsCode INT
	,@WrqCurrentFlag BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT cmm.CMM_VALUE AS CmmValue
		,wrq.WRQ_DATETIME AS WrqDatetime
		,wrq.WRQ_EMERGENCY_FLAG AS WrqEmergencyFlag
		,wrq.WRQ_RESERVATION_FLAG AS WrqReservationFlag
		,wrq.WRQ_ARCHIVE_FLAG AS WrqArchiveFlag
		,wrq.WRQ_ALERT_FLAG AS WrqAlertFlag
		,wrq.WRQ_CURRENT_FLAG AS WrqCurrentFlag
		,rqs.RQS_VALUE AS RqsValue
		,rqs.RQS_CODE AS RqsCode
	FROM TD_WORKFLOW_REQUEST wrq
	INNER JOIN TD_RELEASE rls
		ON wrq.WRQ_RLS_ID = rls.RLS_ID
			AND wrq.WRQ_DELETE_FLAG = 0
			AND rls.RLS_DELETE_FLAG = 0
	INNER JOIN TD_COMMENT cmm
		ON wrq.WRQ_CMM_ID = cmm.CMM_ID
			AND cmm.CMM_DELETE_FLAG = 0
	INNER JOIN TS_REQUEST rqs
		ON wrq.WRQ_RQS_ID = rqs.RQS_ID
	WHERE rls.RLS_CODE = @RlsCode
		AND (
			@WrqCurrentFlag IS NULL
			OR wrq.WRQ_CURRENT_FLAG = @WrqCurrentFlag
			)
END
GO


