SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 13/12/2018
-- Description:	Deletes a ReasonRelease and its associated comment
-- exec Data_Reason_Release_Delete 32,'DFG01','OKeeffeNe'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Reason_Release_Delete @RlsCode INT
	,@RsnCode NVARCHAR(32)
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ReasonReleaseDtgID INT = NULL
	DECLARE @ReasonReleaseCommentDtgID INT = NULL
	DECLARE @AuditUpdateCount INT
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @errorDetails VARCHAR(256)
	DECLARE @commentId INT
	DECLARE @RlsId INT
	DECLARE @RsnId INT
	DECLARE @DeleteCount INT

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
			INNER JOIN TM_REASON_RELEASE
				ON RSN_ID = RSR_RSN_ID
			WHERE rsn.RSN_CODE = @RsnCode
				AND RSR_RLS_ID = @RlsId
				AND RSR_DELETE_FLAG = 0
			)

	IF @RsnId = 0
		OR @RsnId IS NULL
	BEGIN
		RETURN 0
	END

	SELECT @commentId = rsr.RSR_CMM_ID
		,@ReasonReleaseDtgID = rsr.RSR_DTG_ID
	FROM TM_REASON_RELEASE rsr
	WHERE rsr.RSR_RLS_ID = @RlsId
		AND rsr.RSR_RSN_ID = @RsnId
		AND rsr.RSR_DELETE_FLAG = 0

IF @commentId IS NOT NULL AND @commentId>0
BEGIN			
	SELECT @ReasonReleaseCommentDtgID = cmm.CMM_DTG_ID
	FROM TD_COMMENT cmm
	WHERE CMM_ID = @commentId

	

	--soft delete the comment
	UPDATE TD_COMMENT
	SET CMM_DELETE_FLAG = 1
	WHERE CMM_ID = @commentId
		AND CMM_DELETE_FLAG = 0

	SET @DeleteCount = @@ROWCOUNT

	IF @DeleteCount > 0
	BEGIN
		-- do the auditing for the comment
		-- Create the entry in the TD_AUDITING table
		EXEC @AuditUpdateCount = Security_Auditing_Delete @ReasonReleaseCommentDtgID
			,@CcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @errorMessage = 'Error creating entry in TD_AUDITING comment ' + @errorDetails

			RAISERROR (
					@errorMessage
					,16
					,1
					)

			RETURN 0
		END
	END
END

	--soft delete the ReasonRelease
	UPDATE TM_REASON_RELEASE
	SET RSR_DELETE_FLAG = 1
	WHERE RSR_RLS_ID = @RlsId
		AND RSR_RSN_ID = @RsnId

	SET @DeleteCount = @@ROWCOUNT

	IF @DeleteCount > 0
	BEGIN
		-- do the auditing for the Reason Release
		-- Create the entry in the TD_AUDITING table
		EXEC @AuditUpdateCount = Security_Auditing_Delete @ReasonReleaseDtgID
			,@CcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @errorMessage = 'Error creating entry in TD_AUDITING Reason Release' + @errorDetails

			RAISERROR (
					@errorMessage
					,16
					,1
					)

			RETURN 0
		END
	END

	RETURN @DeleteCount
END
GO


