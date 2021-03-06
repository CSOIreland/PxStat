SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/09/2018
-- Description:	To update the TD_AUDITING table for a Delete event. 
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Auditing_Delete @DtgID INT
	,@DeleteCcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @eMessage VARCHAR(256)
	DECLARE @recordCount INT
	DECLARE @DeleteCcnID INT = NULL

	--Get the CcnID for the username
	SET @DeleteCcnID = (
			SELECT CCN_ID
			FROM TD_ACCOUNT
			WHERE CCN_USERNAME = @DeleteCcnUsername
				AND CCN_DELETE_FLAG = 0
			)

	-- check in case no CcnID was found in the account table
	IF @DeleteCcnID IS NULL
		OR @DeleteCcnID = 0
	BEGIN
		SET @eMessage = 'Error amending audit record - no account record found for @DeleteCcnUsername: ' + cast(isnull(@DeleteCcnUsername, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	-- check if a record exists in the audit table for the DTG_ID
	SET @recordCount = (
			SELECT count(*)
			FROM TD_AUDITING
			WHERE DTG_ID = @DtgID
			)

	-- if not, raise an error
	IF @recordCount IS NULL
		OR @recordCount = 0
	BEGIN
		SET @eMessage = 'Error amending audit record - no create audit record found for DtgID: ' + cast(isnull(@DtgID, 0) AS VARCHAR)

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
			WHERE DTP_CODE = 'DELETED'
			)
		,@DeleteCcnID
		,GETDATE()
		)

	-- return the number of rows affected
	RETURN @@ROWCOUNT
END
