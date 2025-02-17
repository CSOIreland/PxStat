
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 20/09/2018
-- Description:	To create a new Language entry
-- exec System_Settings_Language_Create 'kk','dssdfhgf','okeeffene'
-- =============================================
CREATE
	

 PROCEDURE System_Settings_Language_Create @LngIsoCode CHAR(2)
	,@LngIsoName NVARCHAR(32)
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	-- If there is a previously deleted language with this LngIsoCode then we re-animate rather than create a new one
	-- This is in order to restore referential integrity with the old version of the language
	DECLARE @deletedID INT

	SET @deletedID = (
			SELECT max(LNG_ID)
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 1
			)

	DECLARE @DtgID INT = NULL


	IF (
			@deletedID IS NOT NULL
			AND @deletedID > 0
			)
	BEGIN
		SET @DtgID = (
				SELECT max(LNG_DTG_ID)
				FROM TS_LANGUAGE
				WHERE LNG_ISO_CODE = @LngIsoCode
					AND LNG_DELETE_FLAG = 1
				)

		DECLARE @updateCount INT
		DECLARE @eMessage VARCHAR(256)

		IF @DtgID IS NULL
		BEGIN
			RETURN 0
		END

		UPDATE TS_LANGUAGE
		SET LNG_ISO_NAME = @LngIsoName
			,LNG_DELETE_FLAG = 0
			,LNG_DTG_ID=@DtgID
		WHERE LNG_ID = @deletedID

		SET @updateCount = @@ROWCOUNT

		IF @updateCount > 0
		BEGIN
			-- do the auditing 
			-- Create the entry in the TD_AUDITING table
			DECLARE @AuditUpdateCount INT

			EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
				,@CcnUsername

			-- check the previous stored procedure for error
			IF @AuditUpdateCount = 0
			BEGIN
				SET @eMessage = 'Error creating entry in TD_AUDITING for language update:' + cast(isnull(@LngIsoCode, 0) AS VARCHAR)

				RAISERROR (
						@eMessage
						,16
						,1
						)

				RETURN
			END
		END

		RETURN @updateCount
	END
	ELSE
	BEGIN

		
	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Create @CcnUsername

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Error in calling Security_Auditing_Create'
				,16
				,1
				)

		RETURN 0
	END

		-- Create the new language entry
		INSERT INTO TS_LANGUAGE (
			LNG_ISO_CODE
			,LNG_ISO_NAME
			,LNG_DTG_ID
			,LNG_DELETE_FLAG
			)
		VALUES (
			@LngIsoCode
			,@LngIsoName
			,@DtgID
			,0
			)
	END

	RETURN @@IDENTITY
END
