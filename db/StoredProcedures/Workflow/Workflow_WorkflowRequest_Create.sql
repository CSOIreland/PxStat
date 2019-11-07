SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 22/10/2018
-- Description:	Creates a WorkflowRequest
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_WorkflowRequest_Create @CcnUsername NVARCHAR(256)
	,@RqsCode NVARCHAR(32)
	,@CmmCode INT
	,@WrqDatetime DATETIME = NULL
	,@WrqEmergencyFlag BIT = NULL
	,@WrqReservationFlag BIT = NULL
	,@WrqArchiveFlag BIT = NULL
	,@WrqAlertFlag BIT = NULL
	,@RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @RqsID INT = NULL
	DECLARE @RlsID INT = NULL
	DECLARE @CmmID INT = NULL
	DECLARE @DtgID INT = NULL
	DECLARE @errorMessage VARCHAR(256)

	SET @CmmID = (
			SELECT CMM_ID
			FROM TD_COMMENT
			WHERE CMM_CODE = @CmmCode
				AND CMM_DELETE_FLAG = 0
			)

	IF @CmmID IS NULL
		OR @CmmID = 0
	BEGIN
		SET @errorMessage = 'No comment ID found for Release code: ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @RqsID = (
			SELECT Rqs_id
			FROM TS_REQUEST
			WHERE RQS_CODE = @RqsCode
			)

	IF @RqsID IS NULL
		OR @RqsID = 0
	BEGIN
		SET @errorMessage = 'No Request ID found for Request code: ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @RlsID = (
			SELECT rls_id
			FROM td_release
			WHERE rls_code = @rlscode
				AND TD_RELEASE.RLS_DELETE_FLAG = 0
			)

	IF @RlsId IS NULL
		OR @RlsId = 0
	BEGIN
		SET @errorMessage = 'No Release ID found for Release Code: ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Create @CcnUsername

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Error in calling Security_Auditing_Create for WorkflowRequest_Create'
				,16
				,1
				)

		RETURN 0
	END

	INSERT INTO TD_WORKFLOW_REQUEST (
		WRQ_RLS_ID
		,WRQ_CMM_ID
		,WRQ_DATETIME
		,WRQ_EMERGENCY_FLAG
		,WRQ_RESERVATION_FLAG
		,WRQ_ARCHIVE_FLAG
		,WRQ_ALERT_FLAG
		,WRQ_RQS_ID
		,WRQ_DTG_ID
		,WRQ_DELETE_FLAG
		,WRQ_CURRENT_FLAG
		)
	VALUES (
		@RlsId
		,@CmmID
		,@WrqDatetime
		,@WrqEmergencyFlag
		,@WrqReservationFlag
		,@WrqArchiveFlag
		,@WrqAlertFlag
		,@RqsID
		,@DtgID
		,0
		,1
		)

	RETURN @@identity
END
GO


