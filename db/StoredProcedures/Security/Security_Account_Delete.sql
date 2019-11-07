SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/10/2018
-- Description:	To delete an account. 
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Account_Delete @CcnUsernameDeleter NVARCHAR(256)
	,@CcnUsernameToBeDeleted NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	-- check record exists
	SET @DtgID = (
			SELECT CCN_DTG_ID
			FROM TD_ACCOUNT
			WHERE CCN_USERNAME = @CcnUsernameToBeDeleted
				AND CCN_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
	BEGIN
		--This record doesn't exist
		RETURN 0
	END

	-- do the soft delete
	UPDATE TD_ACCOUNT
	SET CCN_DELETE_FLAG = 1
	WHERE CCN_USERNAME = @CcnUsernameToBeDeleted
		AND CCN_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- do the auditing 
		-- Update the Record in the TD_AUDITING table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Delete @DtgID
			,@CcnUsernameDeleter

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Account delete:' + cast(isnull(@CcnUsernameToBeDeleted, 0) AS VARCHAR)

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
GO


