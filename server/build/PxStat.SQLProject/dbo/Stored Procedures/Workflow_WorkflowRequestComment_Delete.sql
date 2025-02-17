
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/10/2018
-- Description:	To delete a WorkflowRequest comment
-- =============================================
CREATE
	

 PROCEDURE Workflow_WorkflowRequestComment_Delete @CcnUsername NVARCHAR(256)
	,@RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CmmID INT
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @returnValue INT

	--we're not insisting the req.WRQ_DELETE_FLAG=0 because you probably just deleted it!
	SET @CmmID = (
			SELECT req.WRQ_CMM_ID
			FROM TD_WORKFLOW_REQUEST req
			INNER JOIN TD_RELEASE rel
				ON req.WRQ_RLS_ID = rel.RLS_ID
			INNER JOIN TD_COMMENT com
				ON req.WRQ_CMM_ID = com.CMM_ID
			WHERE rel.RLS_CODE = @RlsCode
				AND rel.RLS_DELETE_FLAG = 0
				AND com.CMM_DELETE_FLAG = 0
			)

	IF @CmmID IS NULL
		OR @CmmID = 0
	BEGIN
		RETURN 0
	END

	-- execute the generic Comment delete stored procedure
	EXEC @returnValue = Workflow_Comment_Delete @CcnUsername
		,@CmmID

	IF @returnValue IS NULL
		OR @returnValue = 0
	BEGIN
		SET @errorMessage = 'Unable to delete Request Comment for Request Code ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	RETURN @returnValue
END
