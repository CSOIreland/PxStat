SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 25/10/2018
-- Description:	To update a Workflow Response Comment
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_WorkflowResponseComment_Update @CcnUsername NVARCHAR(256)
	,@RlsCode INT
	,@CmmValue NVARCHAR(1024)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CmmID INT
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @returnValue INT

	SET @CmmID = (
			SELECT com.CMM_ID
			FROM TD_COMMENT com
			INNER JOIN TD_WORKFLOW_RESPONSE res
				ON com.CMM_ID = res.WRS_CMM_ID
			INNER JOIN TD_WORKFLOW_REQUEST req
				ON res.WRS_WRQ_ID = req.WRQ_ID
			INNER JOIN TD_RELEASE rel
				ON req.WRQ_RLS_ID = rel.RLS_ID
			WHERE rel.RLS_CODE = @RlsCode
				AND rel.RLS_DELETE_FLAG = 0
				AND req.WRQ_DELETE_FLAG = 0
				AND com.CMM_DELETE_FLAG = 0
			)

	IF @CmmID IS NULL
		OR @CmmID = 0
	BEGIN
		RETURN 0
	END

	EXEC @returnValue = Workflow_Comment_Update @CcnUsername
		,@CmmID
		,@CmmValue

	RETURN @returnValue
END
GO


