SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 22/10/2018
-- Description:	Create a Workflow Response
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_WorkflowResponse_Create @CcnUsername NVARCHAR(256)
	,@RspCode NVARCHAR(32)
	,@CmmCode INT
	,@RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @WfRqID INT = NULL
	DECLARE @RlsID INT = NULL
	DECLARE @DtgID INT = NULL
	DECLARE @RspID INT = NULL
	DECLARE @CmmID INT
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @returnValue INT

	SET @CmmID = (
			SELECT CMM_ID
			FROM TD_COMMENT
			WHERE CMM_CODE = @CmmCode
				AND CMM_DELETE_FLAG = 0
			)

	IF @CmmID IS NULL
		OR @CmmID = 0
	BEGIN
		SET @errorMessage = 'No Comment ID found for Release Code: ' + cast(isnull(@RlsCode, 0) AS INT)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	--Getting the id of the Release entry
	SET @RlsID = (
			SELECT rls_id
			FROM td_release
			WHERE rls_code = @RlsCode
				AND TD_RELEASE.RLS_DELETE_FLAG = 0
			)

	IF @RlsId IS NULL
		OR @RlsId = 0
	BEGIN
		SET @errorMessage = 'No Release ID found for Release Code: ' + cast(isnull(@RlsCode, 0) AS INT)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	--Getting the id of the Workflow Request entry 
	SET @WfRqID = (
			SELECT wrq_id
			FROM TD_WORKFLOW_REQUEST
			WHERE WRQ_RLS_ID = @RlsID
				AND WRQ_CURRENT_FLAG = 1
				AND WRQ_DELETE_FLAG = 0
			)

	IF @WfRqID IS NULL
		OR @WfRqID = 0
	BEGIN
		SET @errorMessage = 'No WorkFlow Request ID found for Release Code: ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @RspID = (
			SELECT rsp_id
			FROM ts_response
			WHERE RSP_CODE = @RspCode
			)

	IF @RspId IS NULL
		OR @RspId = 0
	BEGIN
		SET @errorMessage = 'No Response ID found for Response code: ' + cast(isnull(@RspCode, 0) AS VARCHAR)

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
				'Error in calling Security_Auditing_Create for WorkflowResponse_Create'
				,16
				,1
				)

		RETURN 0
	END

	--we may now proceed with the create operation
	INSERT INTO TD_WORKFLOW_RESPONSE (
		WRS_RSP_ID
		,WRS_CMM_ID
		,WRS_WRQ_ID
		,WRS_DTG_ID
		)
	VALUES (
		@RspID
		,@CmmID
		,@WfRqID
		,@DtgID
		)

	RETURN @@rowcount
END
GO


