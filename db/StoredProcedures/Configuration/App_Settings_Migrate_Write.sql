SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- For migration only
CREATE
	OR

ALTER PROCEDURE [dbo].[App_Settings_Migrate_Write] @appkey VARCHAR(200)
	,@appvalue VARCHAR(MAX)
	,@appdescription VARCHAR(MAX)
	,@appsensitivevalue BIT = NULL
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @APPID AS INT;

	SET @APPID = (
			SELECT MAX(TM_APP_SETTING_CONFIG_VERSION.ASV_ID)
			FROM TM_APP_SETTING_CONFIG_VERSION
			INNER JOIN TS_CONFIG_SETTING_TYPE ON TS_CONFIG_SETTING_TYPE.CST_ID = TM_APP_SETTING_CONFIG_VERSION.ASV_CST_ID
			WHERE (TS_CONFIG_SETTING_TYPE.CST_CODE = 'APP')
			);

	DECLARE @DtgId INT = NULL

	EXEC @DtgId = Security_Auditing_Create @userName;

	IF @DtgId IS NULL
		OR @DtgId = 0
	BEGIN
		RAISERROR (
				'SP: Security_Auditing_Create has failed!'
				,16
				,1
				)

		RETURN 0
	END

	INSERT INTO TS_APP_SETTING
	VALUES (
		@APPID
		,@appkey
		,@appvalue
		,@appdescription
		,COALESCE(@appsensitivevalue, 0)
		);
END
GO