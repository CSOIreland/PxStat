
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 23/10/2018
-- Description:	Create a Workflow signoff entry
-- =============================================
CREATE
	

 PROCEDURE Workflow_WorkflowSignoff_Create @CcnUsername NVARCHAR(256)
	,@SgnCode NVARCHAR(256)
	,@CmmCode INT
	,@RlsCode NVARCHAR(32)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @WrsID INT = NULL
	DECLARE @SgnID INT = NULL
	DECLARE @DtgID INT = NULL
	DECLARE @CmmID INT
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
		SET @errorMessage = 'No Comment ID found for Release Code: ' + cast(isnull(@RlsCode, 0) AS INT)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @SgnID = (
			SELECT SGN_ID
			FROM TS_SIGNOFF
			WHERE SGN_CODE = @SgnCode
			)

	IF @SgnID IS NULL
		OR @SgnID = 0
	BEGIN
		SET @errorMessage = 'No @SgnID found for Signoff code ' + cast(isnull(@SgnCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @WrsID = (
			SELECT wrs.WRS_ID
			FROM TD_WORKFLOW_RESPONSE wrs
			INNER JOIN TD_WORKFLOW_REQUEST wrp
				ON wrs.WRS_WRQ_ID = wrp.WRQ_ID
			INNER JOIN TD_RELEASE rl
				ON wrp.WRQ_RLS_ID = rl.RLS_ID
			WHERE rl.RLS_CODE = @RlsCode
				AND wrp.WRQ_DELETE_FLAG = 0
				AND rl.RLS_DELETE_FLAG = 0
				AND wrp.WRQ_CURRENT_FLAG = 1
			)

	IF @WrsID IS NULL
		OR @WrsID = 0
	BEGIN
		SET @errorMessage = 'No Response found for RlsCode ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

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

	INSERT INTO TD_WORKFLOW_SIGNOFF (
		WSG_SGN_ID
		,WSG_CMM_ID
		,WSG_WRS_ID
		,WSG_DTG_ID
		)
	VALUES (
		@SgnID
		,@CmmID
		,@WrsID
		,@DtgID
		)

	RETURN @@rowcount
END
