SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- This sp is used for migration only
CREATE
	OR

ALTER PROCEDURE [dbo].[Api_Settings_Migrate_Write] @apiversion DECIMAL(10, 2)
	,@apikey VARCHAR(200)
	,@apivalue VARCHAR(MAX)
	,@apidescription VARCHAR(MAX)
	,@apisensitivevalue BIT=NULL
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @APIASVID AS INT;

	SET @APIASVID = (
			SELECT MAX(TM_APP_SETTING_CONFIG_VERSION.ASV_ID)
			FROM TM_APP_SETTING_CONFIG_VERSION
			INNER JOIN TS_CONFIG_SETTING_TYPE ON TS_CONFIG_SETTING_TYPE.CST_ID = TM_APP_SETTING_CONFIG_VERSION.ASV_CST_ID
			WHERE (TS_CONFIG_SETTING_TYPE.CST_CODE = 'API')
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

	INSERT INTO TS_API_SETTING
	(
		API_ASV_ID,
		API_KEY,
		API_VALUE,
		API_DESCRIPTION,
		API_SENSITIVE_VALUE

	)
	VALUES (
		@APIASVID
		,@apikey
		,@apivalue
		,@apidescription
		,COALESCE(@apisensitivevalue,0)
		);
END
GO