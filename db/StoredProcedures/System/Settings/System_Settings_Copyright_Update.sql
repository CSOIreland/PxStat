SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 05/10/2018
-- Description:	Updates a Copyright entry
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Settings_Copyright_Update @CcnUsername NVARCHAR(256)
	,@CprCodeOld NVARCHAR(256)
	,@CprCodeNew NVARCHAR(256)
	,@CprValue NVARCHAR(256)
	,@CprUrl NVARCHAR(2048)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage NVARCHAR(256)
	DECLARE @updateCount INT

	SET @DtgID = (
			SELECT CPR_DTG_ID
			FROM TS_COPYRIGHT
			WHERE CPR_CODE = @CprCodeOld
				AND CPR_DELETE_FLAG = 0
			)

	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		-- the requested record doesn't exist
		RETURN 0
	END

	UPDATE TS_COPYRIGHT
	SET CPR_VALUE = @CprValue
		,CPR_CODE = @CprCodeNew
		,CPR_URL = @CprUrl
	WHERE CPR_CODE = @CprCodeOld
		AND CPR_DELETE_FLAG = 0

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
			SET @eMessage = 'Error creating entry in TD_AUDITING for Copyright update:' + cast(isnull(@CprCodeOld, 0) AS NVARCHAR)

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


