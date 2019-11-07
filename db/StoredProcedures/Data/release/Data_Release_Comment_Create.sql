SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/01/2019
-- Description:	Create a new comment for a Release Code
-- exec Data_Stat_Release_Comment_Create 'OKeeffeNe',85,'This is a comment for RlsCode 85'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_Comment_Create @CcnUsername NVARCHAR(256)
	,@RlsCode INT
	,@CmmValue NVARCHAR(1024)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @CmmID INT = NULL
	DECLARE @CmmCount INT
	DECLARE @RlsCount INT
	DECLARE @eMessage VARCHAR(256)

	--Check if the RlsCode exists
	SET @RlsCount = (
			SELECT count(*)
			FROM TD_RELEASE
			WHERE RLS_CODE = @RlsCode
				AND RLS_DELETE_FLAG = 0
			)

	IF @RlsCount = 0
	BEGIN
		SET @eMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - No Release found for Release Code: ' + cast(isnull(@RlsCode, '') AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	--Check if a comment already exists for this Release Code
	SET @CmmCount = (
			SELECT count(*)
			FROM TD_RELEASE
			WHERE RLS_CODE = @RlsCode
				AND RLS_CMM_ID IS NOT NULL
			)

	IF @CmmCount > 0
	BEGIN
		SET @eMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - A comment already exists for Release Code: ' + cast(isnull(@RlsCode, '') AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Create @CcnUsername

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Error in calling Security_Auditing_Create'
				,16
				,1
				)

		RETURN 0
	END

	--Create the comment
	INSERT INTO TD_COMMENT (
		CMM_VALUE
		,CMM_DTG_ID
		,CMM_DELETE_FLAG
		)
	VALUES (
		@CmmValue
		,@DtgID
		,0
		)

	SET @CmmID = @@IDENTITY

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

	--Update the Release with the newly created comment
	UPDATE TD_RELEASE
	SET RLS_CMM_ID = @CmmID
	WHERE RLS_CODE = @RlsCode
		AND RLS_DELETE_FLAG = 0

	IF @@ROWCOUNT > 0
	BEGIN
		RETURN @CmmID
	END
	ELSE
	BEGIN
		RETURN 0
	END
END
GO


