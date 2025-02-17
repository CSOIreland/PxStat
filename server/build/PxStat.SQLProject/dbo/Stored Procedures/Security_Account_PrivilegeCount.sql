
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 17/10/2018
-- Description:	To return the count of accounts with a specific privilege
-- =============================================
CREATE
	

 PROCEDURE Security_Account_PrivilegeCount @PrvCode NVARCHAR(32)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Count INT
	DECLARE @PrvID INT = NULL
	DECLARE @errorMessage VARCHAR(256)

	SET @errorMessage = 'Privilege code for ' + cast(isnull(@PrvCode, 0) AS VARCHAR) + ' not found'
	SET @PrvID = (
			SELECT PRV_ID
			FROM TS_PRIVILEGE
			WHERE PRV_CODE = @PrvCode
			)

	IF @PrvID IS NULL
	BEGIN
		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	--Get a count of the Privilege code we are querying
	SET @Count = (
			SELECT count(*)
			FROM TD_ACCOUNT
			WHERE CCN_PRV_ID = @PrvID
				AND CCN_DELETE_FLAG = 0
			)

	--return whether or not there are more than one values for that Privilege code in the account table
	RETURN @Count
END
