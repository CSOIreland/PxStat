
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 21/09/2018
-- Description:	Delete a language. 
-- =============================================
CREATE
	

 PROCEDURE System_Settings_Language_Delete @LngIsoCode CHAR(2)
	,@DeleteCcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	--some pre-auditing work first:
	SET @DtgID = (
			SELECT LNG_DTG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
	BEGIN
		RETURN 0
	END

	-- we may now do the soft delete by updating the LNG_DELETE_FLAG to 1
	UPDATE TS_LANGUAGE
	SET LNG_DELETE_FLAG = 1
	WHERE LNG_ISO_CODE = @LngIsoCode
		AND LNG_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- do the auditing 
		-- Create the entry in the TD_AUDITING table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Delete @DtgID
			,@DeleteCcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for language delete:' + cast(isnull(@LngIsoCode, 0) AS VARCHAR)

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
