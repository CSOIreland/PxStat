SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 12/10/2018
-- Description:	Soft Delete a Reason entry
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Settings_Reason_Delete @CcnUsername NVARCHAR(256)
	,@RsnCode NVARCHAR(32)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	--some pre-auditing work first:
	SET @DtgID = (
			SELECT RSN_DTG_ID
			FROM TS_REASON
			WHERE RSN_CODE = @RsnCode
				AND RSN_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
	BEGIN
		--This record doesn't exist
		RETURN 0
	END

	--We may now do the soft delete
	UPDATE TS_REASON
	SET RSN_DELETE_FLAG = 1
	WHERE RSN_CODE = @RsnCode
		AND RSN_DELETE_FLAG = 0

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
			SET @eMessage = 'Error creating entry in TD_AUDITING for Reason delete:' + cast(isnull(@RsnCode, 0) AS VARCHAR)

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


