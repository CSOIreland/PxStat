
-- =============================================
-- Author:		Paulo Patricio
-- Update date: 15 Oct 2018
-- Description:	Updates record(s) from the TD_Subject table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Subject_Update @SbjCode INT
	,@SbjValue NVARCHAR(256)
	,@userName NVARCHAR(256)
	,@ThmCode INT
	,@SbjId INT OUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @DtgID INT = NULL
	DECLARE @ThmID INT = NULL

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

	SELECT @ThmID =THM_ID
	FROM TD_THEME
	WHERE THM_CODE=@ThmCode 
	AND THM_DELETE_FLAG=0

	IF @ThmID IS NULL
		OR @ThmID = 0
	BEGIN
		RETURN 0
	END

	UPDATE TD_Subject
	SET SBJ_VALUE = @SbjValue,
	SBJ_THM_ID=@ThmID 
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
