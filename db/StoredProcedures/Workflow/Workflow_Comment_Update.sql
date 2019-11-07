SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 23/10/2018
-- Description:	To update a Comment
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_Comment_Update @CcnUsername NVARCHAR(256)
	,@CmmCode INT
	,@CmmValue NVARCHAR(1024)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgId INT = NULL
	DECLARE @updateCount INT
	DECLARE @eMessage VARCHAR(256)

	SET @DtgID = (
			SELECT CMM_DTG_ID
			FROM TD_COMMENT
			WHERE CMM_CODE = @CmmCode
				AND CMM_DELETE_FLAG = 0
			)

	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		-- the requested record doesn't exist
		RETURN 0
	END

	UPDATE TD_COMMENT
	SET CMM_VALUE = @CmmValue
	WHERE CMM_CODE = @CmmCode
		AND CMM_DELETE_FLAG = 0

	SET @updateCount = @@rowcount

	IF @updateCount > 0
	BEGIN
		-- Auditing:
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@CcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Comment update, CmmCode:' + cast(isnull(@CmmCode, 0) AS VARCHAR)

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
GO


