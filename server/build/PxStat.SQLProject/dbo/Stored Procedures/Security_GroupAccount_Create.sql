
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/10/2018
-- Description: Add a user to a Group in the TM_GROUP_ACCOUNT table
-- =============================================
CREATE
	

 PROCEDURE Security_GroupAccount_Create @CcnUsernameAdder NVARCHAR(256)
	,@CcnUsernameAddedUser NVARCHAR(256)
	,@GrpCode NVARCHAR(32)
	,@GrpApproveFlag BIT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @CcnID INT = NULL
	DECLARE @GroupId INT = NULL

	-- Get the CcnID for the @CcnUsernameAddedUser parameter
	SET @CcnID = (
			SELECT CCN_ID
			FROM TD_ACCOUNT
			WHERE CCN_USERNAME = @CcnUsernameAddedUser
				AND TD_ACCOUNT.CCN_DELETE_FLAG = 0
			)

	IF @CcnID IS NULL
	BEGIN
		--user not found: exit
		RETURN 0
	END

	SET @GroupId = (
			SELECT GRP_ID
			FROM TD_GROUP
			WHERE GRP_CODE = @GrpCode
				AND TD_GROUP.GRP_DELETE_FLAG = 0
			)

	IF @GroupId IS NULL
	BEGIN
		--group not found: exit
		RETURN 0
	END

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Create @CcnUsernameAdder

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Error in calling Security_GroupAccount_Create'
				,16
				,1
				)

		RETURN 0
	END

	INSERT INTO TM_GROUP_ACCOUNT (
		GCC_GRP_ID
		,GCC_CCN_ID
		,GCC_APPROVE_FLAG
		,GCC_DTG_ID
		,GCC_DELETE_FLAG
		)
	VALUES (
		@GroupId
		,@CcnID
		,@GrpApproveFlag
		,@DtgID
		,0
		)

	RETURN @@IDENTITY
END
