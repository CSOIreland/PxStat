﻿
--Use for migration only
CREATE
	

 PROCEDURE [dbo].[Api_Settings_Delete] @apiversion DECIMAL(10, 2)
	,@apikey VARCHAR(200)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @APIASVID AS INT;
	DECLARE @APIID AS INT;

	SET @APIASVID = (
			SELECT TM_APP_SETTING_CONFIG_VERSION.ASV_ID
			FROM TM_APP_SETTING_CONFIG_VERSION
			INNER JOIN TS_CONFIG_SETTING_TYPE ON TS_CONFIG_SETTING_TYPE.CST_ID = TM_APP_SETTING_CONFIG_VERSION.ASV_CST_ID
			WHERE (TS_CONFIG_SETTING_TYPE.CST_CODE = 'API')
				AND TM_APP_SETTING_CONFIG_VERSION.ASV_VERSION = @apiversion
			);
	SET @APIID = (
			SELECT API_ID
			FROM TS_API_SETTING
			WHERE API_ASV_ID = @APIASVID
				AND API_KEY = @apikey
			)

	DELETE
	FROM TS_HISTORY_API_SETTING
	WHERE HPI_API_ID = @APIID

	DELETE
	FROM TS_API_SETTING
	WHERE API_ASV_ID = @APIASVID
		AND API_KEY = @apikey
END
