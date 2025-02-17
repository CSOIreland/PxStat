
-- =============================================
-- Author:		Paulo Patricio
-- Delete date: 15 Oct 2018
-- Description:	Deletes record(s) from the TD_Subject table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Subject_Delete @SbjCode INT
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @DtgID INT = NULL
	DECLARE @SbgID INT = NULL

	--check if record exists
	SELECT @SbgID = SBJ_ID
		,@DtgID = SBJ_DTG_ID
	FROM TD_Subject
	WHERE SBJ_CODE = @SbjCode
		AND SBJ_Delete_FLAG = 0

	IF @SbgID IS NULL
	BEGIN
		RETURN 0
	END

	-- check if has products
	IF EXISTS (
			SELECT 1
			FROM TD_PRODUCT
			WHERE PRC_SBJ_ID = @SbgID
				AND PRC_DELETE_FLAG = 0
			)
	BEGIN
		--subject has one or more products, nothing can be deleted.

		RETURN 0
	END

	UPDATE TD_Subject
	SET SBJ_DELETE_FLAG = 1
	WHERE SBJ_ID = @SbgID
		AND SBJ_Delete_FLAG = 0

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
			SET @eMessage = 'Error creating entry in TD_AUDITING for Subject delete with code ' + cast(isnull(@SbjCode, 0) AS VARCHAR)

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
