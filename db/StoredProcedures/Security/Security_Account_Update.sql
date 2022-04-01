SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/10/2018
-- Description:	To update an account entity. 
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Account_Update @CcnUsernameUpdater NVARCHAR(256)
	,@UpdatedCcnUsername NVARCHAR(256)
	,@PrvCode NVARCHAR(32) = NULL
	,@CcnNotificationFlag BIT = NULL
	,@CcnLockedFlag BIT = NULL
	,@LngIsoCode CHAR(2)=NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @PrvID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @LngId INT

	SET @DtgID = (
			SELECT CCN_DTG_ID
			FROM TD_ACCOUNT
			WHERE CCN_USERNAME = @UpdatedCcnUsername
				AND CCN_DELETE_FLAG = 0
			)

	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		-- the requested record doesn't exist
		RETURN 0
	END

	IF @LngIsoCode IS NOT NULL
	BEGIN
		SET @LngId = (
				SELECT LNG_ID
				FROM TS_LANGUAGE
				WHERE LNG_ISO_CODE = @LngIsoCode
					AND LNG_DELETE_FLAG = 0
				)

		IF @LngId = 0
			OR @LngId IS NULL
		BEGIN
			RETURN 0
		END
	END

	IF @CcnNotificationFlag IS NULL
	BEGIN
		SET @CcnNotificationFlag = 1
	END

	IF @PrvCode IS NOT NULL
	BEGIN
		SET @PrvID = (
				SELECT PRV_ID
				FROM TS_PRIVILEGE
				WHERE PRV_CODE = @PrvCode
				)

		IF @PrvID = 0
			OR @PrvID IS NULL
		BEGIN
			--the requested Privilege Code does not exist
			RETURN 0
		END
	END

	IF @LngIsoCode IS NOT NULL
	BEGIN
	--Update the language preference in the TD_USER table
	UPDATE TD_USER
		SET USR_LNG_ID = @LngId
		FROM TD_USER
		INNER JOIN TD_ACCOUNT ON CCN_USERNAME = @UpdatedCcnUsername
			AND CCN_USR_ID = USR_ID
	END

	UPDATE TD_ACCOUNT
	SET CCN_PRV_ID =CASE WHEN @PrvCode IS NOT NULL THEN @PrvID ELSE CCN_PRV_ID END
		,CCN_NOTIFICATION_FLAG = Coalesce(@CcnNotificationFlag, CCN_NOTIFICATION_FLAG)
		,CCN_LOCKED_FLAG = Coalesce(@CcnLockedFlag, CCN_LOCKED_FLAG)
	WHERE CCN_USERNAME = @UpdatedCcnUsername
		AND CCN_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @CcnLockedFlag = 1
	BEGIN
		UPDATE TD_LOGIN
		SET LGN_SESSION = NULL
			,LGN_SESSION_EXPIRY = NULL
			,LGN_TOKEN_1FA = NULL
			,LGN_TOKEN_2FA = NULL
		FROM TD_LOGIN
		INNER JOIN TD_ACCOUNT ON LGN_CCN_ID = CCN_ID
			AND CCN_USERNAME = @UpdatedCcnUsername
			AND CCN_DELETE_FLAG = 0
	END

	IF @updateCount > 0
	BEGIN
		-- Auditing:
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@CcnUsernameUpdater

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Account update:' + cast(isnull(@UpdatedCcnUsername, 0) AS VARCHAR)

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


