SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/01/2018
-- Description:	Deletes a comment from a release
-- exec Data_Stat_Release_Comment_Delete 'OKeeffeNe',85
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_Comment_Delete @CcnUsername NVARCHAR(256)
	,@RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgCommentID INT = NULL
	DECLARE @DtgReleaseID INT = NULL
	DECLARE @CmmID INT = NULL
	DECLARE @eMessage VARCHAR(256)
	DECLARE @DeleteCount INT

	SELECT @CmmID = RLS_CMM_ID
		,@DtgReleaseID = RLS_DTG_ID
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

	SELECT @DtgCommentID = CMM_DTG_ID
	FROM TD_COMMENT
	WHERE CMM_ID = @CmmID
		AND CMM_DELETE_FLAG = 0

	DECLARE @AuditUpdateCount INT

	EXEC @AuditUpdateCount = Security_Auditing_Delete @DtgCommentID
		,@CcnUsername

	-- check the previous stored procedure for error
	IF @AuditUpdateCount = 0
	BEGIN
		SET @eMessage = 'Error creating entry in TD_AUDITING for Release Comment Delete:' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN
	END

	UPDATE TD_COMMENT
	SET CMM_DELETE_FLAG = 1
	WHERE CMM_ID = @CmmID

	SET @DeleteCount = @@rowcount

	EXEC @AuditUpdateCount = Security_Auditing_Update @DtgReleaseID
		,@CcnUsername

	-- check the previous stored procedure for error
	IF @AuditUpdateCount = 0
	BEGIN
		SET @eMessage = 'Error creating entry in TD_AUDITING for Release Update while deleting its comment:' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN
	END

	UPDATE TD_RELEASE
	SET RLS_CMM_ID = NULL
	WHERE RLS_CODE = @RlsCode
		AND RLS_DELETE_FLAG = 0

	RETURN @DeleteCount
END
GO


