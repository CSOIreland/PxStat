
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/05/2024
-- Description:	Read Account details when an API token is available
-- exec Security_Account_ReadByTokenAndCcnUsername 'dissemination@cso.ie','b62b34ff3c798c6fda2282dbfc37f2b94afc90d363cf28ae03b3c31b2ae566b1'
-- =============================================
CREATE
	

 PROCEDURE Security_Account_ReadByTokenAndCcnUsername @CcnUsername NVARCHAR(256)
	,@CcnApiToken NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ccn.CCN_USERNAME AS CcnUsername
		,prv.PRV_CODE AS PrvCode
		,prv.PRV_VALUE AS PrvValue
		,ccn.CCN_NOTIFICATION_FLAG AS CcnNotificationFlag
		,ccn.CCN_DISPLAYNAME AS CcnDisplayName
		,ccn.CCN_EMAIL AS CcnEmail
		,CCN_ID AS CcnId
		,CCN_AD_FLAG AS CcnAdFlag
		,CCN_LOCKED_FLAG AS CcnLockedFlag
		,LGN_2FA AS Lgn2Fa
		,LNG_ISO_CODE AS LngIsoCode
	FROM TD_ACCOUNT ccn
	INNER JOIN TS_PRIVILEGE prv ON ccn.CCN_PRV_ID = prv.PRV_ID
		AND ccn.CCN_DELETE_FLAG = 0
	INNER JOIN TD_USER ON CCN_USR_ID = USR_ID
	INNER JOIN TS_LANGUAGE ON USR_LNG_ID = LNG_ID
	LEFT JOIN TD_LOGIN ON CCN_ID = LGN_CCN_ID
	WHERE @CcnUsername = ccn.CCN_USERNAME
		AND @CcnApiToken = ccn.CCN_API_TOKEN
		AND CCN_DELETE_FLAG=0
		AND CCN_LOCKED_FLAG=0
END