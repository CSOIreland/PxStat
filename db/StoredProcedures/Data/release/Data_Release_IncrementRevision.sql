SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 24 Oct 2018
-- Description:	Inserts a new record into the TD_RELEASE table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_IncrementRevision @RlsCode INT
	,@userName NVARCHAR(256)
	,@GrpCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	-- Error Message
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @spName VARCHAR(100) = 'Data_Stat_Release_IncrementRevision'
	DECLARE @DtgId INT = NULL
	-- Release lookup
	DECLARE @RlsId INT = NULL
	DECLARE @updateCount INT = 0
	DECLARE @eMessage VARCHAR(256)

	SELECT @RlsId = [RLS_ID]
		,@DtgId = RLS_DTG_ID
	FROM [TD_RELEASE]
	WHERE [RLS_DELETE_FLAG] = 0
		AND [RLS_CODE] = @RlsCode

	IF @RlsId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - Release not found: ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	UPDATE TD_RELEASE
	SET RLS_REVISION = RLS_REVISION + 1
	WHERE RLS_ID = @RlsId
		AND RLS_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	--Update the Group ID of the release if necessary
	IF @GrpCode IS NOT NULL
	BEGIN
		DECLARE @GrpId INT

		SET @GrpId = (
				SELECT GRP_ID
				FROM TD_GROUP
				WHERE GRP_CODE = @GrpCode
					AND GRP_DELETE_FLAG = 0
				)

		IF @GrpId IS NULL
			OR @GrpId = 0
		BEGIN
			SET @errorMessage = 'SP: ' + @spName + ' - Group Not Found: ' + cast(isnull(@GrpCode, 0) AS VARCHAR)

			RAISERROR (
					@errorMessage
					,16
					,1
					);

			RETURN 0
		END

		UPDATE TD_RELEASE
		SET RLS_GRP_ID = @GrpId
		WHERE RLS_ID = @RlsId
			AND RLS_DELETE_FLAG = 0
	END

	IF @updateCount > 0
	BEGIN
		-- Auditing:
		-- update record on auditing table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@userName

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Release update:' + cast(isnull(@RlsCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	--Return the number of rows updated
	RETURN @RlsId
END
GO


