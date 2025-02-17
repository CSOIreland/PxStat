
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 16/10/2020
-- Description:	Get a user by the session token
-- =============================================
CREATE
	

 PROCEDURE Security_Login_ReadBySession @LgnSession VARCHAR(64)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CCN_USERNAME AS CcnUsername
		,CCN_DISPLAYNAME AS CcnDisplayName
		,CCN_EMAIL AS CcnEmail
		,CCN_ID AS CcnId
		,CCN_LOCKED_FLAG AS CcnLockedFlag
		,LNG_ISO_CODE AS LngIsoCode
	FROM TD_ACCOUNT
	INNER JOIN TD_LOGIN
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND LGN_SESSION_EXPIRY  > getdate()
			AND LGN_SESSION  = @LgnSession
			AND CCN_LOCKED_FLAG=0
   INNER JOIN TD_USER
	ON CCN_USR_ID=USR_ID 
	INNER JOIN TS_LANGUAGE 
	ON USR_LNG_ID=LNG_ID
END
