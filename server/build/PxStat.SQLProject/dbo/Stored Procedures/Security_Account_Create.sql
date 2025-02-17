
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/10/2018
-- Description:	Create Account
-- =============================================
CREATE
	

 PROCEDURE Security_Account_Create @CcnUsernameCreator NVARCHAR(256)
	,@CcnUsernameNewAccount NVARCHAR(256)
	,@PrvCode NVARCHAR(32)
	,@CcnNotificationFlag BIT = NULL
	,@CcnLockedFlag BIT
	,@CcnADFlag BIT
	,@CcnDisplayName NVARCHAR(256) = NULL
	,@CcnEmail NVARCHAR(256) = NULL
	,@LngIsoCode CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @PrivilegeID INT = NULL
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @LngId INT
	DECLARE @UserId INT

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Create @CcnUsernameCreator

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

	--Get the Privilege ID corresponding to the supplied @PrvCode
	SET @PrivilegeID = (
			SELECT PRV_ID
			FROM TS_PRIVILEGE
			WHERE PRV_CODE = @PrvCode
			)

	IF @PrivilegeID IS NULL
	BEGIN
		SET @errorMessage = 'No Privilege entry found for Privilege code: ' + cast(isnull(@PrvCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	IF @CcnNotificationFlag IS NULL
	BEGIN
		SET @CcnNotificationFlag = 1
	END

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngId IS NULL
	BEGIN
		RETURN 0
	END

	INSERT INTO TD_USER (USR_LNG_ID)
	VALUES (@LngId)

	SET @UserId=@@IDENTITY

	INSERT INTO TD_ACCOUNT (
		CCN_USERNAME
		,CCN_PRV_ID
		,CCN_NOTIFICATION_FLAG
		,CCN_DTG_ID
		,CCN_DELETE_FLAG
		,CCN_LOCKED_FLAG
		,CCN_AD_FLAG
		,CCN_DISPLAYNAME
		,CCN_EMAIL
		,CCN_USR_ID 
		)
	VALUES (
		@CcnUsernameNewAccount
		,@PrivilegeID
		,@CcnNotificationFlag
		,@DtgID
		,0
		,@CcnLockedFlag
		,@CcnADFlag
		,@CcnDisplayName
		,@CcnEmail
		,@UserId 
		)

	RETURN @@IDENTITY
END
