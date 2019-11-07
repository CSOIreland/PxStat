SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/01/2019
-- Description:	Updates a comment for a release
-- exec Data_Stat_Release_Comment_Update 'OKeeffeNe',85,'An amended comment for Rls 85'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_Comment_Update @CcnUsername NVARCHAR(256)
	,@RlsCode INT
	,@CmmValue NVARCHAR(1024)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @CmmID INT = NULL
	DECLARE @eMessage VARCHAR(256)

	SELECT @CmmID = RLS_CMM_ID
	FROM TD_RELEASE
	WHERE RLS_CODE = @RlsCode
		AND RLS_DELETE_FLAG = 0

	IF @CmmID = 0
		OR @CmmID IS NULL
	BEGIN
		SET @eMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - No comment exists for Release Code: ' + cast(isnull(@RlsCode, '') AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	SELECT @DtgID = CMM_DTG_ID
	FROM TD_COMMENT
	WHERE CMM_ID = @CmmID
		AND CMM_DELETE_FLAG = 0

	DECLARE @AuditUpdateCount INT

	EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
		,@CcnUsername

	-- check the previous stored procedure for error
	IF @AuditUpdateCount = 0
	BEGIN
		SET @eMessage = 'Error creating entry in TD_AUDITING for Release Comment Update:' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN
	END

	UPDATE TD_COMMENT
	SET CMM_VALUE = @CmmValue
	WHERE CMM_ID = @CmmID
		AND CMM_DELETE_FLAG = 0

	RETURN @@rowcount
END
GO


