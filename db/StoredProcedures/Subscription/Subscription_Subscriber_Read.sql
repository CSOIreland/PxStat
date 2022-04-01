SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 21/09/2021
-- Description:	Reads all or one specific Subscriber
-- exec Subscription_Subscriber_Read
-- =============================================
CREATE
	OR

ALTER PROCEDURE Subscription_Subscriber_Read @SubscriberUserId NVARCHAR(256) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	SELECT SBR_UID AS SbrUserId
		,SBR_PREFERENCE AS SbrPreference
		,CCN_EMAIL AS CcnEmail
		,LNG_ISO_CODE AS LngIsoCode
		,SBR_KEY AS SbrKey
	FROM TD_SUBSCRIBER
	INNER JOIN TD_USER ON SBR_USR_ID = USR_ID
		AND SBR_DELETE_FLAG = 0
	INNER JOIN TS_LANGUAGE ON USR_LNG_ID = LNG_ID
	LEFT JOIN TD_ACCOUNT ON CCN_USR_ID = USR_ID
	LEFT JOIN TD_LOGIN ON LGN_CCN_ID = CCN_ID
	WHERE (
			@SubscriberUserId IS NULL
			OR SBR_UID = @SubscriberUserId
			)
END
GO


