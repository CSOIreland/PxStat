SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 17/12/2018
-- Description:	Updates an Alert 
-- exec System_Navigation_Alert_Update 1,'The LRT non-Title','This is not an alert message','2018-12-26',1,'OKeeffeNe'
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Alert_Update @LrtCode INT
	,@LrtMessage NVARCHAR(1024)
	,@LrtDatetime DATETIME
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	--some pre-auditing work first:
	SET @DtgID = (
			SELECT LRT_DTG_ID
			FROM TD_ALERT
			WHERE LRT_CODE = @LrtCode
				AND LRT_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
	BEGIN
		RETURN 0
	END

	-- we may now do the update
	UPDATE TD_ALERT
	SET LRT_MESSAGE = @LrtMessage
		,LRT_DATETIME = @LrtDatetime
	WHERE LRT_CODE = @LrtCode
		AND LRT_DELETE_FLAG = 0

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
			SET @eMessage = 'Error creating entry in TD_AUDITING for Alert update:' + cast(isnull(@LrtCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	-- Return the number of rows updated
	RETURN @updateCount
END
GO


