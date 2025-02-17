
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 22/10/2018
-- Description:	Deletes a Workflow_Request entity. This is done as a soft delete.
-- =============================================
CREATE
	

 PROCEDURE Workflow_WorkflowRequest_Delete @CcnUsername NVARCHAR(256)
	,@RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @RlsID INT = NULL
	DECLARE @WrqID INT = NULL
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList  @CcnUsername



	SELECT @DtgID = WRQ_DTG_ID
		,@WrqID = WRQ_ID
	FROM (
		SELECT req.WRQ_DTG_ID
			,req.WRQ_ID
		FROM TD_WORKFLOW_REQUEST req
		INNER JOIN TD_RELEASE rel
			ON req.WRQ_RLS_ID = rel.RLS_ID
		WHERE req.WRQ_DELETE_FLAG = 0
			AND rel.RLS_DELETE_FLAG = 0
			AND rel.RLS_CODE = @RlsCode
			AND req.WRQ_CURRENT_FLAG <> 0
		) query

	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		SET @eMessage = 'No workflow found for Release Code: ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN
	END



	UPDATE TD_WORKFLOW_REQUEST
	SET WRQ_DELETE_FLAG = 1,
	WRQ_CURRENT_FLAG=0
	FROM TD_WORKFLOW_REQUEST req
	INNER JOIN TD_RELEASE rel
		ON req.WRQ_RLS_ID = rel.RLS_ID
	INNER JOIN @GroupUserHasAccess
	ON rel.RLS_GRP_ID=GRP_ID 
	WHERE rel.RLS_CODE = @RlsCode
		AND rel.RLS_DELETE_FLAG = 0
		AND req.WRQ_DELETE_FLAG = 0
		AND req.WRQ_CURRENT_FLAG=1
	 

	SET @updateCount = @@rowcount

	IF @updateCount > 0
	BEGIN
		-- do the auditing 
		-- Create the entry in the TD_AUDITING table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Delete @DtgID
			,@CcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Workflow request delete - Release Code: ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	-- Return the number of rows deleted
	RETURN @updateCount
END
