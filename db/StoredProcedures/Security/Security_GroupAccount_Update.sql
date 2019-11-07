SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 15/10/2018
-- Description:	To update a GroupAccount, i.e. to change the GrpApproverFlag to true or false
-- Moderator can be Editor or Approver
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_GroupAccount_Update @CcnUsernameUpdater NVARCHAR(256)
	,@CcnUsernameUpdatedUser NVARCHAR(256)
	,@GrpCode NVARCHAR(32)
	,@GrpApproveFlag BIT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @GrpID INT = NULL
	DECLARE @CcnAmendedID INT = NULL

	SET @CcnAmendedID = (
			SELECT CCN_ID
			FROM TD_ACCOUNT
			WHERE CCN_USERNAME = @CcnUsernameUpdatedUser
				AND TD_ACCOUNT.CCN_DELETE_FLAG = 0
			)

	IF @CcnAmendedID IS NULL
	BEGIN
		RETURN 0
	END

	SET @GrpID = (
			SELECT GRP_ID
			FROM TD_GROUP
			WHERE GRP_CODE = @Grpcode
				AND TD_GROUP.GRP_DELETE_FLAG = 0
			)

	IF @GrpID IS NULL
	BEGIN
		RETURN 0
	END

	SET @DtgID = (
			SELECT GCC_DTG_ID
			FROM TM_GROUP_ACCOUNT
			WHERE GCC_GRP_ID = @GrpID
				AND GCC_CCN_ID = @CcnAmendedID
				AND GCC_DELETE_FLAG = 0
			)

	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		-- the requested record doesn't exist
		RETURN 0
	END

	-- we may now do the update
	UPDATE TM_GROUP_ACCOUNT
	SET GCC_APPROVE_FLAG = @GrpApproveFlag
	WHERE GCC_GRP_ID = @GrpID
		AND GCC_CCN_ID = @CcnAmendedID
		AND GCC_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- Auditing:
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@CcnUsernameUpdater

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error amending entry in TD_AUDITING for GroupAccount update, Group:' + cast(isnull(@GrpCode, 0) AS VARCHAR) + ' user:' + cast(isnull(@CcnUsernameUpdater, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	--Return the number of rows updated
	RETURN @updateCount
END
GO


