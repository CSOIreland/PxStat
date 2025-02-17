/******************************************************************************** 
Author Name                           :  Stephen Lane 
Date written                          :  04/11/2022 
Version                               :  1 
Description   
              
Reads the application settings based on the version in the TM_APP_SETTING_VERSION table which is passed in. 
For c# application will come from appsettings.


REVISION HISTORY 
---------------- 
CHANGE NO.    DATE          CHANGED BY          REASON 
01			22/06/2023		Stephen Lane	dont return sensitive values unless requested
02			13/09/2023		Stephen Lane	changing mask to null to work with always encrypted
03			24/11/2023		Stephen Lane	app_settings_version being null means get max pk version
04			07/11/2023		Stephen Lane	always return two select statements

PEER REVIEW HISTORY 
------------------- 
REVIEW NO.    DATE          REVIEWED BY         COMMENTS 

exec api_settings_read 1

*************************************************************************************/
CREATE   PROCEDURE [dbo].[Api_Settings_Read] @app_settings_version DECIMAL(10, 2),
@read_sensitive_value BIT=1
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @errorMessage VARCHAR(1300)
		,@version_id DECIMAL(10, 2) = NULL
		,@config_setting_type_id INT = NULL;

	SELECT @config_setting_type_id = cst_id
	FROM TS_CONFIG_SETTING_TYPE
	WHERE cst_code = 'API';

	IF @config_setting_type_id IS NULL
	BEGIN
		SELECT @errorMessage = 'Api setting type not found for code : API';

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	SELECT @version_id = ASV_ID
	FROM TM_APP_SETTING_CONFIG_VERSION
	WHERE ASV_VERSION = @app_settings_version
		AND ASV_CST_ID = @config_setting_type_id;

	IF @version_id IS NULL  AND @app_settings_version is not null
	BEGIN
		SELECT @errorMessage = CONCAT (
				'Api setting version not found for version : '
				,@app_settings_version
				);

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END


	declare @versionToFind DECIMAL(10, 2) = NULL;

	IF @app_settings_version is null
		BEGIN
			DECLARE @max_version_id INT = NULL
				,@max_version_number DECIMAL(10, 2) = NULL;

			SELECT @max_version_id = max(ASV_ID)
				,@max_version_number = ASV_VERSION
			FROM TM_APP_SETTING_CONFIG_VERSION
			WHERE ASV_CST_ID = @config_setting_type_id
			GROUP BY ASV_ID
				,ASV_VERSION;

			IF @max_version_id IS NULL
				BEGIN
					SELECT @errorMessage = 'Api setting version not found for latest version';

						RAISERROR (
								@errorMessage
								,16
								,1
								)
							
						RETURN 0
				END
			ELSE
				BEGIN
					set @versionToFind = @max_version_id;
			END
		END
	ELSE
		BEGIN
			set @versionToFind = @version_id;
		END


		IF @read_sensitive_value = 1
			BEGIN
				SELECT	API_KEY
				,API_VALUE
				,API_DESCRIPTION 
				,API_SENSITIVE_VALUE
				FROM TS_API_SETTING
				WHERE API_ASV_ID = @versionToFind;
			END
		ELSE
		BEGIN
			SELECT	API_KEY
			,CASE WHEN API_SENSITIVE_VALUE=0 THEN API_VALUE ELSE null END AS API_VALUE
			,API_DESCRIPTION 
			,API_SENSITIVE_VALUE
			FROM TS_API_SETTING
			WHERE API_ASV_ID = @versionToFind;
		END


		IF @app_settings_version is null
			begin
				SELECT @max_version_number AS max_version_number;
			end
		else
			begin
				SELECT ASV_VERSION AS max_version_number
				FROM TM_APP_SETTING_CONFIG_VERSION
				WHERE ASV_CST_ID = @config_setting_type_id
				and asv_id = @versionToFind
			end

END
