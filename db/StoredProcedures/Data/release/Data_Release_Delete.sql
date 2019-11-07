SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 15/11/2018
-- Description:	Soft Delete a Release based on Release Code
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_Delete @CcnUsername NVARCHAR(256)
	,@RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

	--some pre-auditing work first:
	SET @DtgID = (
			SELECT rls.RLS_DTG_ID
			FROM TD_RELEASE rls
			WHERE rls.RLS_CODE = @RlsCode
				AND rls.RLS_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
		OR @DtgID = 0
	BEGIN
		--This record doesn't exist
		RETURN 0
	END

	UPDATE TD_RELEASE
	SET RLS_DELETE_FLAG = 1
	WHERE RLS_CODE = @RlsCode
		AND RLS_DELETE_FLAG = 0

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
			SET @eMessage = 'Error creating entry in TD_AUDITING for Release delete - Release Code: ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN 0
		END
	END

	RETURN @updateCount
END
GO


