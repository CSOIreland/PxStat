
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 23/10/2018
-- Description:	Delete a comment. This is done as a soft delete.
-- =============================================
CREATE
	

 PROCEDURE Workflow_Comment_Delete @CcnUsername NVARCHAR(256)
	,@CmmCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	--some pre-auditing work first:
	SET @DtgID = (
			SELECT CMM_DTG_ID
			FROM TD_COMMENT
			WHERE CMM_CODE = @CmmCode
				AND CMM_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
		OR @DtgID = 0
	BEGIN
		--This record doesn't exist
		RETURN 0
	END

	UPDATE TD_COMMENT
	SET CMM_DELETE_FLAG = 1
	WHERE CMM_CODE = @CmmCode
		AND CMM_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

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
			SET @eMessage = 'Error creating entry in TD_AUDITING for Comment request delete - Comment Code: ' + cast(isnull(@CmmCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN 0
		END
	END

	RETURN @updateCount
END
