SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 28/09/2018
-- Description:	To update the TD_AUDITING table for an Update event. 
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Auditing_Update @DtgID INT
	,@UpdateCcnUsername VARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @eMessage VARCHAR(256)
	DECLARE @recordCount INT
	DECLARE @UpdateCcnID INT = NULL
	DECLARE @DhtId INT = NULL

	--Get the CcnID for the username
	SET @UpdateCcnID = (
			SELECT CCN_ID
			FROM TD_ACCOUNT
			WHERE CCN_USERNAME = @UpdateCcnUsername
				AND CCN_DELETE_FLAG = 0
			)

	-- check in case no CnnID was found in the account table
	IF @UpdateCcnID IS NULL
		OR @UpdateCcnID = 0
	BEGIN
		SET @eMessage = 'Error amending audit record - no account record found for @UpdateCcnID: ' + cast(isnull(@UpdateCcnUsername, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	--check if a record exists in the audit table for the DTG_ID
	SET @DhtId = (
			SELECT count(*)
			FROM TD_AUDITING
			WHERE DTG_ID = @DtgID
			)

	-- if not, raise an error
	IF @DhtId IS NULL
		OR @recordCount = 0
	BEGIN
		SET @DhtId = 'Error amending audit record - no create audit record found for DtgID: ' + cast(isnull(@DtgID, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	INSERT INTO TM_AUDITING_HISTORY (
		DHT_DTG_ID
		,DHT_DTP_ID
		,DHT_CCN_ID
		,DHT_DATETIME
		)
	VALUES (
		@DtgID
		,(
			SELECT DTP_ID
			FROM TS_AUDITING_TYPE
			WHERE DTP_CODE = 'UPDATED'
			)
		,@UpdateCcnID
		,GETDATE()
		)

	-- return the number of rows affected
	RETURN @@ROWCOUNT
END
GO


