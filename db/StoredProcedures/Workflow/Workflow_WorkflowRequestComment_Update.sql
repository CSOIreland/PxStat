SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/10/2018
-- Description:	To delete a WorkflowRequest comment
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_WorkflowRequestComment_Update @CcnUsername NVARCHAR(256)
	,@RlsCode INT
	,@CmmValue NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @RlsID INT
	set @RlsID = 0
	DECLARE @CmmID INT 
	set @CmmID = 0
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @returnValue INT

	--we need a comment id
	SET @CmmID = (
			SELECT req.WRQ_CMM_ID
			FROM TD_WORKFLOW_REQUEST req
			INNER JOIN TD_RELEASE rel
				ON req.WRQ_RLS_ID = rel.RLS_ID
			WHERE rel.RLS_CODE = @RlsCode
				AND rel.RLS_DELETE_FLAG = 0
				AND req.WRQ_DELETE_FLAG = 0
			)

	IF @CmmID IS NULL
		OR @CmmID = 0
	BEGIN
		RETURN 0
	END

	IF @CmmID IS NULL
		OR @CmmID = 0
	BEGIN
		RETURN 0
	END

	-- execute the generic Comment update stored procedure
	EXEC @returnValue = Workflow_Comment_Update @CcnUsername
		,@CmmID
		,@CmmValue

	RETURN @returnValue
END
GO


