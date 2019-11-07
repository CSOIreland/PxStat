SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 21 Sep 2018
-- Description:	Inserts a new record into the TD_AUDITING table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Auditing_Create @userName NVARCHAR(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @CcnId INT = NULL
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @DtgId INT

	SELECT @CcnId = CCN_ID
	FROM TD_ACCOUNT
	WHERE CCN_USERNAME = @userName
		AND CCN_DELETE_FLAG = 0

	IF @CcnId IS NULL
	BEGIN
		SET @errorMessage = 'SP: Security_Auditing_Create - userName not found: ' + cast(@userName AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	INSERT INTO TD_AUDITING DEFAULT
	VALUES

	SET @DtgId = @@IDENTITY

	INSERT INTO TM_AUDITING_HISTORY (
		DHT_DTG_ID
		,DHT_DTP_ID
		,DHT_CCN_ID
		,DHT_DATETIME
		)
	VALUES (
		@DtgId
		,(
			SELECT DTP_ID
			FROM TS_AUDITING_TYPE
			WHERE DTP_CODE = 'CREATED'
			)
		,@CcnId
		,GETDATE()
		)

	RETURN @DtgId
END
GO


