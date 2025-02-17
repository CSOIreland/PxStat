
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/10/2018
-- Description:	To delete a Group entity. 
-- =============================================
CREATE
	

 PROCEDURE Security_Group_Delete @GrpCode NVARCHAR(32)
	,@DeleteCcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	-- Check record exists 
	SET @DtgID = (
			SELECT GRP_DTG_ID
			FROM TD_GROUP
			WHERE GRP_CODE = @GrpCode
				AND GRP_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
	BEGIN
		--This record doesn't exist
		RETURN 0
	END

	--We may now do the soft delete
	UPDATE TD_GROUP
	SET GRP_DELETE_FLAG = 1
	WHERE GRP_CODE = @GrpCode
		AND GRP_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- do the auditing 
		-- Update the entry in the TD_AUDITING table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Delete @DtgID
			,@DeleteCcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Group delete:' + cast(isnull(@GrpCode, 0) AS VARCHAR)

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
