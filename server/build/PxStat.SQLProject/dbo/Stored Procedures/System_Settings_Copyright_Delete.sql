
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 05/10/2018
-- Description:	Deletes (soft delete) an entry in the TS_COPYRIGHT table
-- =============================================
CREATE
	

 PROCEDURE System_Settings_Copyright_Delete @CcnUsername NVARCHAR(256)
	,@CprCode NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	--some pre-auditing work first:
	SET @DtgID = (
			SELECT CPR_DTG_ID
			FROM TS_COPYRIGHT
			WHERE CPR_CODE = @CprCode
				AND CPR_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
	BEGIN
		--This record doesn't exist
		RETURN 0
	END

	--We may now do the soft delete
	UPDATE TS_COPYRIGHT
	SET cpr_DELETE_FLAG = 1
	WHERE CPR_CODE = @CprCode
		AND CPR_DELETE_FLAG = 0

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
			SET @eMessage = 'Error creating entry in TD_AUDITING for Copyright delete:' + cast(isnull(@CprCode, 0) AS VARCHAR)

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
