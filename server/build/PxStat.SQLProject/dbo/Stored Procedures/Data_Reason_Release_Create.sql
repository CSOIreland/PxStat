
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 13/12/2018
-- Description:	Creates a new ReasonRelease entry
-- exec Data_Reason_Release_Create 32,'DFG01','A Comment for ReasonRelease Create','OKeeffeNe'
-- =============================================
CREATE
	

 PROCEDURE Data_Reason_Release_Create @RlsCode INT
	,@RsnCode NVARCHAR(32)
	,@CmmValue NVARCHAR(1024) = NULL
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ReasonReleaseDtgID INT = NULL
	DECLARE @ReasonReleaseCommentDtgID INT = NULL
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @errorDetails VARCHAR(256)
	DECLARE @commentId INT
	DECLARE @RlsId INT
	DECLARE @RsnId INT

	SET @errorDetails = ' for ReasonRelease RlsCode:' + cast(isnull(@RlsCode, 0) AS VARCHAR) + ' RsnCode:' + cast(isnull(@RsnCode, 0) AS VARCHAR)
	SET @RlsId = (
			SELECT rls.RLS_ID
			FROM TD_RELEASE rls
			WHERE rls.RLS_CODE = @RlsCode
				AND RLS_DELETE_FLAG = 0
			)

	IF @RlsId IS NULL
		OR @RlsId = 0
	BEGIN
		RETURN 0
	END

	SET @RsnId = (
			SELECT rsn.RSN_ID
			FROM TS_REASON rsn
			WHERE rsn.RSN_CODE = @RsnCode
				AND RSN_DELETE_FLAG = 0
			)

	IF @RsnId = 0
		OR @RsnId IS NULL
	BEGIN
		RETURN 0
	END

	-- Do the create Audit for the ReasonRelease and get the new DtgID from the stored procedure
	EXEC @ReasonReleaseDtgID = Security_Auditing_Create @CcnUsername

	-- Check for problems with the audit stored procedure
	IF @ReasonReleaseDtgID = 0
		OR @ReasonReleaseDtgID IS NULL
	BEGIN
		SET @errorMessage = 'Error in calling Security_Auditing_Create' + @errorDetails

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	IF @CmmValue IS NOT NULL
	BEGIN
		-- Do the create Audit for the Comment and get the new DtgID from the stored procedure
		EXEC @ReasonReleaseCommentDtgID = Security_Auditing_Create @CcnUsername

		-- Check for problems with the audit stored procedure
		IF @ReasonReleaseCommentDtgID = 0
			OR @ReasonReleaseCommentDtgID IS NULL
		BEGIN
			SET @errorMessage = 'Error in calling Security_Auditing_Create' + @errorDetails

			RAISERROR (
					@errorMessage
					,16
					,1
					)

			RETURN 0
		END

		-- Create the comment for the ReleaseReason
		INSERT INTO TD_COMMENT (
			CMM_VALUE
			,CMM_DTG_ID
			,CMM_DELETE_FLAG
			)
		VALUES (
			@CmmValue
			,@ReasonReleaseCommentDtgID
			,0
			)

		IF @@IDENTITY = 0
		BEGIN
			SET @errorMessage = 'Unable to create comment' + @errorDetails

			RAISERROR (
					@errorMessage
					,16
					,1
					)

			RETURN 0
		END

		SET @commentID = @@IDENTITY
	END

	-- Create the ReasonRelease entry
	INSERT INTO TM_REASON_RELEASE (
		RSR_RSN_ID
		,RSR_RLS_ID
		,RSR_CMM_ID
		,RSR_DTG_ID
		,RSR_DELETE_FLAG
		)
	VALUES (
		@RsnId
		,@RlsId
		,@commentID
		,@ReasonReleaseDtgID
		,0
		)

	IF @@IDENTITY = 0
	BEGIN
		SET @errorMessage = 'Unable to create Reason Release' + @errorDetails

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	RETURN @@IDENTITY
END
