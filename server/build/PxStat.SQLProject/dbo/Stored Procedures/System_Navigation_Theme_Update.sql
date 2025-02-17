
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/03/2021
-- Description:	Updates record(s) from the TD_THEME table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Theme_Update @ThmCode INT
	,@ThmValue NVARCHAR(256)
	,@userName NVARCHAR(256)
	,@ThmId INT OUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @DtgID INT = NULL

	--check if record exists
	SELECT @ThmID = THM_ID
		,@DtgID = THM_DTG_ID
	FROM TD_THEME
	WHERE THM_CODE = @ThmCode 
		AND THM_DELETE_FLAG  = 0

	IF @ThmID IS NULL
		OR @ThmID = 0
	BEGIN
		RETURN 0
	END



	UPDATE TD_THEME
	SET THM_VALUE = @ThmValue
	WHERE THM_ID = @ThmID
		AND THM_DELETE_FLAG = 0

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
			SET @eMessage = 'Error creating entry in TD_AUDITING for THEME Update with code ' + cast(isnull(@ThmCode, 0) AS VARCHAR)

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
