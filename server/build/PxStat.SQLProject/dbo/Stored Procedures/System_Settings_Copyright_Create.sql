
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 05/10/2018
-- Description:	Create a new copyright entry
-- =============================================
CREATE
	

 PROCEDURE System_Settings_Copyright_Create @CcnUsername NVARCHAR(256)
	,@CprCode NVARCHAR(256)
	,@CprValue NVARCHAR(256)
	,@CprUrl NVARCHAR(2048)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @updateCount INT
	-- If there is a previously deleted copyright with this Copyright Code then we re-animate rather than create a new one
	-- This is in order to restore referential integrity with the older version of the copyright
	

	SET @DtgID = (
			SELECT max(CPR_DTG_ID)
			FROM TS_COPYRIGHT
			WHERE CPR_CODE =  @CprCode
				AND CPR_DELETE_FLAG = 1
			)

	IF @DtgID IS NOT NULL
		AND @DtgID > 0
	BEGIN
		UPDATE TS_COPYRIGHT
		SET CPR_DELETE_FLAG = 0,
		CPR_CODE=@CprCode,
		CPR_VALUE=@CprValue,
		CPR_URL=@CprUrl 
		WHERE CPR_DTG_ID = @DtgID

		

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
				SET @errorMessage = 'Error creating entry in TD_AUDITING for copyright update:' + cast(isnull(@CprCode, 0) AS VARCHAR)

				RAISERROR (
						@errorMessage
						,16
						,1
						)

				RETURN
			END

			RETURN @updateCount
		END
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

		INSERT INTO TS_COPYRIGHT (
			CPR_CODE
			,CPR_VALUE
			,CPR_URL
			,CPR_DTG_ID
			,CPR_DELETE_FLAG
			)
		VALUES (
			@CprCode
			,@CprValue
			,@CprUrl
			,@DtgID
			,0
			)


	END

	RETURN @@IDENTITY
END
