SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/03/2021
-- Description:	Deletes record(s) from the TD_THEME table
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Theme_Delete @ThmCode INT
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @DtgID INT = NULL
	DECLARE @ThmID INT = NULL

	--check if record exists
	SELECT @ThmID = THM_ID
		,@DtgID = THM_DTG_ID
	FROM TD_THEME
	WHERE THM_CODE = @ThmCode
		AND THM_DELETE_FLAG = 0

	IF @ThmID IS NULL
	BEGIN
		RETURN 0
	END

	-- check if has subjects
	IF EXISTS (
			SELECT 1
			FROM TD_SUBJECT
			WHERE SBJ_THM_ID = @ThmID
				AND SBJ_DELETE_FLAG = 0
			)
	BEGIN
		--theme has one or more subjects, nothing can be deleted.

		RETURN 0
	END

	UPDATE TD_THEME
	SET THM_DELETE_FLAG = 1
	WHERE THM_ID = @ThmID
		AND THM_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- do the auditing 
		-- Create the entry in the TD_AUDITING table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Delete @DtgID
			,@userName

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Theme delete with code ' + cast(isnull(@ThmCode , 0) AS VARCHAR)

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


