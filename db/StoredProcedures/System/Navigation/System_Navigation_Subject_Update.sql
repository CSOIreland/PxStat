SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Update date: 15 Oct 2018
-- Description:	Updates record(s) from the TD_Subject table
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Subject_Update @SbjCode INT
	,@SbjValue NVARCHAR(256)
	,@userName NVARCHAR(256)
	,@SbjId INT OUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @DtgID INT = NULL

	--check if record exists
	SELECT @SbjID = SBJ_ID
		,@DtgID = SBJ_DTG_ID
	FROM TD_Subject
	WHERE SBJ_CODE = @SbjCode
		AND SBJ_Delete_FLAG = 0

	IF @SbjID IS NULL
		OR @SbjID = 0
	BEGIN
		RETURN 0
	END

	UPDATE TD_Subject
	SET SBJ_VALUE = @SbjValue
	WHERE SBJ_ID = @SbjID
		AND SBJ_Delete_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- do the auditing 
		-- Create the entry in the TD_AUDITING table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@userName

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Subject Update with code ' + cast(isnull(@SbjCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	-- Return the number of rows Updated
	RETURN @updateCount
END
GO


