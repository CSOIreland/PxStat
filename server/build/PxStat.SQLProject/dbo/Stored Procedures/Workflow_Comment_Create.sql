
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 22/10/2018
-- Description:	Create a comment
-- =============================================
CREATE
	

 PROCEDURE Workflow_Comment_Create @CcnUsername NVARCHAR(256)
	,@CmmValue NVARCHAR(1024)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @CommentCode INT

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Create @CcnUsername

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Error in calling Security_Auditing_Create for Workflow_Comment_Create'
				,16
				,1
				)

		RETURN 0
	END

	INSERT INTO TD_COMMENT (
		CMM_DELETE_FLAG
		,CMM_DTG_ID
		,CMM_VALUE
		)
	VALUES (
		0
		,@DtgID
		,@CmmValue
		)

	SELECT @CommentCode = CMM_CODE
	FROM TD_COMMENT
	WHERE CMM_ID = @@IDENTITY

	RETURN @CommentCode
END
