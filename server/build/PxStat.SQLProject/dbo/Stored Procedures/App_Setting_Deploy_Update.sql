CREATE PROCEDURE [dbo].[App_Setting_Deploy_Update]
@app_settings_version DECIMAL (10, 2), @config_setting_type VARCHAR (10), @auto_version BIT=NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @errorMessage AS VARCHAR (1300), @version_id AS DECIMAL (10, 2) = NULL, @config_setting_type_id AS INT = NULL;
    SELECT @config_setting_type_id = cst_id
    FROM   TS_CONFIG_SETTING_TYPE
    WHERE  cst_code = @config_setting_type;
    IF @config_setting_type_id IS NULL
        BEGIN
            SELECT @errorMessage = CONCAT(@config_setting_type, ' setting type not found for code : ' + @config_setting_type);
            RAISERROR (@errorMessage, 16, 1);
            RETURN 0;
        END
    IF @auto_version = 1
        BEGIN
            SELECT @version_id = max(ASV_ID)
            FROM   TM_APP_SETTING_CONFIG_VERSION
            WHERE  ASV_CST_ID = @config_setting_type_id;
        END
    ELSE
        BEGIN
            SELECT @version_id = ASV_ID
            FROM   TM_APP_SETTING_CONFIG_VERSION
            WHERE  ASV_VERSION = @app_settings_version
                   AND ASV_CST_ID = @config_setting_type_id;
        END
    IF @version_id IS NULL
        BEGIN
            SELECT @errorMessage = CONCAT(@config_setting_type, ' setting version not found for version : ', @app_settings_version);
            RAISERROR (@errorMessage, 16, 1);
            RETURN 0;
        END
    DECLARE @machineIP AS VARCHAR (100) = NULL;
    SELECT @machineIP = CONVERT (VARCHAR (100), CONNECTIONPROPERTY('client_net_address'));
    DECLARE @historyID AS INT = NULL;
    SELECT @historyID = max(HSV_ID)
    FROM   TM_HISTORY_APP_SETTING_CONFIG_VERSION
    WHERE  HSV_ASV_ID = @version_id;
    INSERT  INTO [TM_HISTORY_APP_SETTING_CONFIG_VERSION_DEPLOY] ([HCD_HSV_ID], [HCD_IP_ADDRESS])
    VALUES                                                     (@historyID, @machineIP);
END

