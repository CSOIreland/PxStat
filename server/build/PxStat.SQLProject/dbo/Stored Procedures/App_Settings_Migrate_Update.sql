
-- For migration only
/*
 exec App_Settings_Migrate_Update 'config.global.json','{
  "title": "Pre-Production",
  "language": {
    "iso": {
      "code": "en",
      "name": "English"
    },
    "culture": "en-ie"
  },
  "url": {
    "api": {
      "restful": "https://ppws-data.nisra.gov.uk/public/api.restful",
      "static": "https://ppws-data.nisra.gov.uk/public/api.static"
    },
    "logo": "https://ppdata.nisra.gov.uk/image/logo.png",
    "application": "https://ppdata.nisra.gov.uk/"
    
  },
  "dataset": {
    "officialStatistics": true,
    "download": {
      "threshold": {
        "csv": 1048575,
        "xlsx": 1048575
      }
    }
  },

  "regex": {
    "phone": {
      "pattern": "^(\\d+ \\d+ \\d+)$",
      "placeholder": "028 9038 8474"
    },
    "password": "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[\\W]).{8,}$",
    "matrix-name": "^[A-Z0-9]+$",
    "product-code": "^[a-zA-Z0-9]+$"
  },
  "workflow": {
    "embargo": {
      "time": "09:30:00",
      "day": [
        1,
        2,
        3,
        4,
        5
      ]
    },
    "fastrack": {
      "response": {
        "approver": false,
        "poweruser": true,
        "administrator": true
      },
      "signoff": {
        "poweruser": true,
        "administrator": true
      }
    },
    "release": {
      "reasonRequired": false
    }
  },
  "build": {
    "create": {
      "moderator": true
    },
    "update": {
      "moderator": true
    },
    "import": {
      "moderator": true
    }
  },
  "session": {
    "length": 1200
  },
  "security": {
    "adOpenAccess": true,
    "demo": false
  },
  "search": {

    "maximumResults": 100
  },
  "report": {
    "date-validation": {
      "minDate": 365,
      "maxDate": -1
    }
  }
}','Json config for config.global.json',0,'okeeffene'
*/
CREATE
	

 PROCEDURE [dbo].[App_Settings_Migrate_Update] @appkey VARCHAR(200)
	,@appvalue VARCHAR(MAX)
	,@appdescription VARCHAR(MAX)
	,@appsensitivevalue BIT = NULL
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ASVID AS INT;

	
	

	SET @ASVID = (
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
				'SP: [App_Settings_Migrate_Update] failed'
				,16
				,1
				)

		RETURN 0
	END;



	DISABLE TRIGGER TRIG_TS_APP_SETTING_UPDATE ON TS_APP_SETTING;


	BEGIN TRY
		UPDATE TS_APP_SETTING
		SET APP_VALUE=@appvalue,
		APP_DESCRIPTION=@appdescription
		WHERE APP_KEY=@appkey
		AND APP_ASV_ID=@ASVID;
	END TRY
	BEGIN CATCH
		ENABLE TRIGGER TRIG_TS_APP_SETTING_UPDATE ON TS_APP_SETTING;
		THROW;
	END CATCH;
	
	ENABLE TRIGGER TRIG_TS_APP_SETTING_UPDATE ON TS_APP_SETTING;
	
	return @@rowcount
END
