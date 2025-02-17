
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/08/2019
-- Description:	Creates a value in Reason_Language (when a language other than the main one is specified).
-- Updates the Reason_Language if it exists already
-- =============================================
CREATE
	

 PROCEDURE System_Settings_ReasonLanguage_CreateOrUpdate @RsnCode NVARCHAR(32)
	,@RlgValueInternal NVARCHAR(256)
	,@RlgValueExternal NVARCHAR(256)
	,@LngIsoCode CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngIsoID INT
	DECLARE @RsnID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @recordCount INT

	SET @LngIsoID = (
			SELECT lng.LNG_ID
			FROM TS_LANGUAGE lng
			WHERE lng.LNG_ISO_CODE = @LngIsoCode
				AND lng.LNG_DELETE_FLAG = 0
			)

	IF @LngIsoID IS NULL
		OR @LngIsoID = 0
	BEGIN
		SET @eMessage = 'No ID found for Language Code code ' + cast(isnull(@LngIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @RsnID = (
			SELECT RSN_ID
			FROM TS_REASON
			WHERE RSN_CODE = @RsnCode
				AND RSN_DELETE_FLAG = 0
			)

	IF @RsnID IS NULL
		OR @RsnID = 0
	BEGIN
		SET @eMessage = 'No ID found for Reason Code code ' + cast(isnull(@RsnCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @recordCount = (
			SELECT count(*)
			FROM TS_REASON_LANGUAGE
			WHERE RLG_LNG_ID = @LngIsoID
				AND RLG_RSN_ID = @RsnID
			)

	IF @recordCount = 0 -- We need to create a new entry
	BEGIN
		INSERT INTO TS_REASON_LANGUAGE (
			RLG_VALUE_INTERNAL
			,RLG_VALUE_EXTERNAL
			,RLG_LNG_ID
			,RLG_RSN_ID
			)
		VALUES (
			@RlgValueInternal
			,@RlgValueExternal
			,@LngIsoID
			,@RsnID
			)
	END
	ELSE
	BEGIN --This is an update
		UPDATE TS_REASON_LANGUAGE
		SET RLG_VALUE_INTERNAL = @RlgValueInternal
			,RLG_VALUE_EXTERNAL = @RlgValueExternal
		WHERE RLG_RSN_ID = @RsnID
			AND RLG_LNG_ID = @LngIsoID
	END

	RETURN @@rowcount
END
