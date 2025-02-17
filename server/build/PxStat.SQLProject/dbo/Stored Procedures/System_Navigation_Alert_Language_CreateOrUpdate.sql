
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 17/09/2018
-- Description:	Creates a new entry or updates an existing entry for a translated Alert
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Alert_Language_CreateOrUpdate @LlnMessage NVARCHAR(256)
	,@LngIsoCode CHAR(2)
	,@LrtCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngId INT
	DECLARE @LrtId INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @returnVal INT

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngID IS NULL
		OR @LngID = 0
	BEGIN
		SET @eMessage = 'No ID found for Language Code ' + cast(isnull(@LngIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @LrtId = (
			SELECT LRT_ID
			FROM TD_ALERT
			WHERE LRT_CODE = @LrtCode
				AND LRT_DELETE_FLAG = 0
			)

	IF @LrtId IS NULL
		OR @LrtId = 0
	BEGIN
		SET @eMessage = 'No ID found for Alert Code ' + cast(isnull(@LrtCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	IF (
			SELECT COUNT(*)
			FROM TD_ALERT_LANGUAGE
			WHERE LLN_LRT_ID = @LrtId
				AND LLN_LNG_ID = @LngId
			) > 0
	BEGIN
		--This is an update
		UPDATE TD_ALERT_LANGUAGE
		SET LLN_MESSAGE = @LlnMessage
		WHERE LLN_LNG_ID = @LngId
			AND LLN_LRT_ID = @LrtId

		SET @returnVal = @@ROWCOUNT
	END
	ELSE
	BEGIN
		--This is a new entry
		INSERT INTO TD_ALERT_LANGUAGE (
			LLN_MESSAGE
			,LLN_LNG_ID
			,LLN_LRT_ID
			)
		VALUES (
			@LlnMessage
			,@LngId
			,@LrtId
			)

		SET @returnVal = @@IDENTITY
	END

	RETURN @returnVal
END
