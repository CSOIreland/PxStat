
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 15/06/2020
-- Description:	Clone a comment from a release defined by @RlsCode and apply it to the Release in @RlsIdNew
-- =============================================
CREATE
	

 PROCEDURE Data_Release_Clone_Comment @RlsCode INT
	,@CcnUsername NVARCHAR(256)
	,@RlsIdNew INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @RlsId INT = NULL
	DECLARE @CmmId INT = NULL
	DECLARE @errorMessage NVARCHAR(256)
	DECLARE @spName NVARCHAR(100)
	DECLARE @CmmValue NVARCHAR(1024)
	DECLARE @DtgID INT

	SET @spName = (
			SELECT OBJECT_NAME(@@PROCID)
			)

	SELECT @RlsId = RLS_ID
	FROM TD_RELEASE
	WHERE RLS_DELETE_FLAG = 0
		AND RLS_CODE = @RlsCode

	IF @RlsId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - release code not found: ' + cast(isnull(@RlsCode, '') AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN - 1
	END

	SET @CmmId = (
			SELECT RLS_CMM_ID
			FROM TD_RELEASE
			WHERE RLS_ID = @RlsId
				AND RLS_DELETE_FLAG = 0
			)

	IF @CmmId IS NULL
		OR @CmmId = 0
	BEGIN
		RETURN 0
	END

	SET @CmmValue = (
			SELECT CMM_VALUE
			FROM TD_COMMENT
			WHERE CMM_ID = @CmmId
				AND CMM_DELETE_FLAG = 0
			)

	IF @CmmValue IS NULL
	BEGIN
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

	SET @CmmId = @@IDENTITY

	UPDATE TD_RELEASE
	SET RLS_CMM_ID = @CmmId
	WHERE RLS_ID = @RlsIdNew
		AND RLS_DELETE_FLAG = 0

	RETURN @CmmId
END
