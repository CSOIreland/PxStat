﻿
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 13/12/2014
-- Description:	Updates the comments of a ReasonRelease
-- exec Data_Reason_Release_Update 32,'DFG01','Updated Comment for Release Code 32','OKeeffeNe'
-- =============================================
CREATE
	

 PROCEDURE Data_Reason_Release_Update @RlsCode INT
	,@RsnCode NVARCHAR(32)
	,@CmmValue NVARCHAR(1024)=NULL
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ReasonReleaseCommentDtgID INT = NULL
	DECLARE @AuditUpdateCount INT
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @errorDetails VARCHAR(256)
	DECLARE @commentId INT
	DECLARE @RlsId INT
	DECLARE @RsnId INT
	DECLARE @UpdatedCount INT

	SET @errorDetails = ' for ReasonRelease RlsCode:' + cast(isnull(@RlsCode, 0) AS VARCHAR) + ' RsnCode:' + cast(isnull(@RsnCode, 0) AS VARCHAR)
	SET @RlsId = (
			SELECT rls.RLS_ID
			FROM TD_RELEASE rls
			WHERE rls.RLS_CODE = @RlsCode
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
				AND rsn.RSN_DELETE_FLAG = 0
			)

	IF @RsnId = 0
		OR @RsnId IS NULL
	BEGIN
		RETURN 0
	END

	IF @CmmValue IS NULL
	BEGIN
	UPDATE TM_REASON_RELEASE 
		SET RSR_CMM_ID=NULL
		WHERE RSR_RLS_ID=@RlsId 
		AND RSR_RSN_ID=@RsnId 
		AND RSR_DELETE_FLAG=0

		return @@ROWCOUNT
	END

	SET @commentId = (
			SELECT rsr.RSR_CMM_ID
			FROM TM_REASON_RELEASE rsr
			WHERE rsr.RSR_RLS_ID = @RlsId
				AND rsr.RSR_RSN_ID = @RsnId
				AND rsr.RSR_DELETE_FLAG = 0
			)

	IF @commentId = 0
		OR @commentId IS NULL
	BEGIN
		-- we must insert a new comment
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

		UPDATE TM_REASON_RELEASE 
		SET RSR_CMM_ID=@commentID
		WHERE RSR_RLS_ID=@RlsId 
		AND RSR_RSN_ID=@RsnId 
		AND RSR_DELETE_FLAG=0

		SET @UpdatedCount = @@ROWCOUNT
	END
	ELSE
	BEGIN
		--otherwise we must update the existing comment
		SET @ReasonReleaseCommentDtgID = (
				SELECT cmm.CMM_DTG_ID
				FROM TD_COMMENT cmm
				WHERE cmm.CMM_ID = @commentId
					AND cmm.CMM_DELETE_FLAG = 0
				)

		IF @ReasonReleaseCommentDtgID = 0
			OR @ReasonReleaseCommentDtgID IS NULL
		BEGIN
			SET @errorMessage = 'No comment DTG value found' + @errorDetails

			RAISERROR (
					@errorMessage
					,16
					,1
					)

			RETURN 0
		END

		-- Do the update Audit for the Comment and get the new DtgID from the stored procedure
		EXEC @AuditUpdateCount = Security_Auditing_Update @ReasonReleaseCommentDtgID
			,@CcnUsername

		-- Check for problems with the audit stored procedure
		IF @AuditUpdateCount = 0
			OR @AuditUpdateCount IS NULL
		BEGIN
			SET @errorMessage = 'Error in calling Security_Auditing_Update' + @errorDetails

			RAISERROR (
					@errorMessage
					,16
					,1
					)

			RETURN 0
		END

		

		UPDATE TD_COMMENT
		SET CMM_VALUE = @CmmValue
		WHERE CMM_ID = @commentId
			AND CMM_DELETE_FLAG = 0

		SET @UpdatedCount = @@ROWCOUNT
	END



	RETURN @UpdatedCount
END
