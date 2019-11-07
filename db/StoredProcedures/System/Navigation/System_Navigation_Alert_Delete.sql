SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 17/12/2012
-- Description:	Deletes an Alert
-- exec System_Navigation_Alert_Delete 1,'OKeeffeNe'
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Alert_Delete @LrtCode INT
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

	UPDATE TD_ALERT
	SET LRT_DELETE_FLAG = 1
	WHERE LRT_CODE = @LrtCode

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
			SET @eMessage = 'Error creating entry in TD_AUDITING for Alert delete:' + cast(isnull(@LrtCode, 0) AS VARCHAR)

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


