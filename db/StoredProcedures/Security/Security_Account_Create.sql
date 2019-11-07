SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/10/2018
-- Description:	Create Account
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Account_Create @CcnUsernameCreator NVARCHAR(256)
	,@CcnUsernameNewAccount NVARCHAR(256)
	,@PrvCode NVARCHAR(32)
	,@CcnNotificationFlag BIT =null
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @PrivilegeID INT = NULL
	DECLARE @errorMessage VARCHAR(256)

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

	if @CcnNotificationFlag is null
	begin
		set @CcnNotificationFlag=1
	end

	INSERT INTO TD_ACCOUNT (
		CCN_USERNAME
		,CCN_PRV_ID
		,CCN_NOTIFICATION_FLAG 
		,CCN_DTG_ID
		,CCN_DELETE_FLAG
		)
	VALUES (
		@CcnUsernameNewAccount
		,@PrivilegeID
		,@CcnNotificationFlag
		,@DtgID
		,0
		)

	RETURN @@IDENTITY
END
GO


