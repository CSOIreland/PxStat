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
	,@PrvCode NVARCHAR(32)
	,@CcnNotificationFlag BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @PrvID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT

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

	IF @CcnNotificationFlag IS NULL
	BEGIN
		SET @CcnNotificationFlag = 1
	END

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

	UPDATE TD_ACCOUNT
	SET CCN_PRV_ID = @PrvID,
	CCN_NOTIFICATION_FLAG=@CcnNotificationFlag 
	WHERE CCN_USERNAME = @UpdatedCcnUsername
		AND CCN_DELETE_FLAG = 0

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


