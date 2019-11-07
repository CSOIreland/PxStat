SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 12/10/2018
-- Description:	Updates a Reason entity
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Settings_Reason_Update @CcnUsername NVARCHAR(256)
	,@RsnCode NVARCHAR(32)
	,@RsnValueInternal NVARCHAR(256)
	,@RsnValueExternal NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	SET @DtgID = (
			SELECT RSN_DTG_ID
			FROM TS_REASON
			WHERE RSN_CODE = @RsnCode
				AND RSN_DELETE_FLAG = 0
			)

	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		-- the requested record doesn't exist
		RETURN 0
	END

	UPDATE TS_REASON
	SET RSN_VALUE_INTERNAL = @RsnValueInternal
		,RSN_VALUE_EXTERNAL = @RsnValueExternal
	WHERE RSN_CODE = @RsnCode
		AND RSN_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- Auditing:
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@CcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Reason update:' + cast(isnull(@RsnCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	--Return the number of rows updated
	RETURN @updateCount
END
GO


