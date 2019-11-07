SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 08/10/2018
-- Description:	Deletes a GroupAccount entity (i.e. removes a user from a group) on the TM_GROUP_ACCOUNT Table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_GroupAccount_Delete @CcnUsernameDeleter NVARCHAR(256)
	,@CcnUsernameDeletedUser NVARCHAR(256)
	,@GrpCode NVARCHAR(32)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @GrpID INT = NULL
	DECLARE @CcnDeletedUserID INT = NULL
	DECLARE @AuditUpdateCount INT

	SET @GrpID = (
			SELECT grp_id
			FROM td_group
			WHERE grp_code = @GrpCode
				AND grp_delete_flag = 0
			)

	IF @GrpId IS NULL
	BEGIN
		--we requested a group id but it was not found - exit
		RETURN 0
	END

	SET @CcnDeletedUserID = (
			SELECT ccn_id
			FROM td_account
			WHERE ccn_username = @CcnUsernameDeletedUser
				AND CCN_DELETE_FLAG = 0
			)

	IF @CcnDeletedUserID IS NULL
	BEGIN
		-- Requested user not found - exit
		RETURN 0
	END

	SET @DtgID = (
			SELECT GCC_DTG_ID
			FROM TM_GROUP_ACCOUNT
			WHERE GCC_CCN_ID = @CcnDeletedUserID
				AND GCC_GRP_ID = @GrpID
				AND GCC_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
	BEGIN
		-- Requested Group Account entity doesn't exist - exit
		RETURN 0
	END

	--We may now do the soft delete
	UPDATE TM_GROUP_ACCOUNT
	SET GCC_DELETE_FLAG = 1
	WHERE GCC_GRP_ID = @GrpID
		AND GCC_CCN_ID = @CcnDeletedUserID
		AND GCC_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		EXEC @AuditUpdateCount = Security_Auditing_Delete @DtgID
			,@CcnUsernameDeleter

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for GroupAccount delete - GroupCode:' + cast(isnull(@GrpCode, 0) AS VARCHAR) + ', CcnUsername: ' + cast(isnull(@CcnUsernameDeletedUser, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	-- Return the number of rows deleted
	RETURN @updateCount
END
GO


