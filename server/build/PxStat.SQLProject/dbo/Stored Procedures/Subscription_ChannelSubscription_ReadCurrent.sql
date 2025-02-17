
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 28/09/2021
-- Description:	Reads the Channel subscriptions for the current user
-- exec Subscription_ChannelSubscription_ReadCurrent null,'okeeffene', 'ga'
-- =============================================
CREATE
	

 PROCEDURE Subscription_ChannelSubscription_ReadCurrent @SubscriberUserId NVARCHAR(256) = NULL
	,@CcnUsername NVARCHAR(256) = NULL
	,@LngIsoCode CHAR(2)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @SubscriberUserId IS NULL
		AND @CcnUsername IS NULL
	BEGIN
		RETURN
	END

	DECLARE @LngId INT

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngId IS NULL
	BEGIN
		SET @LngId=0
	END

	DECLARE @UserId INT

	IF @SubscriberUserId IS NOT NULL
	BEGIN
		SET @UserId = (
				SELECT USR_ID
				FROM TD_USER
				INNER JOIN TD_SUBSCRIBER ON SBR_USR_ID = USR_ID
					AND SBR_UID = @SubscriberUserId
					AND SBR_DELETE_FLAG = 0
				)
	END
	ELSE
	BEGIN
		SET @UserId = (
				SELECT USR_ID
				FROM TD_USER
				INNER JOIN TD_ACCOUNT ON CCN_USR_ID = USR_ID
					AND CCN_USERNAME = @CcnUsername
					AND CCN_DELETE_FLAG = 0
				)
	END

	IF @UserId IS NULL
	BEGIN
		RETURN
	END

	SELECT CHN_CODE AS ChnCode
		,COALESCE(CHL_CHN_NAME, CHN_NAME) AS ChnName
	FROM TD_USER
	INNER JOIN TM_CHANNEL_SUBSCRIPTION ON USR_ID = CSB_USR_ID
	INNER JOIN TS_CHANNEL ON CSB_CHN_ID = CHN_ID
	LEFT JOIN TS_CHANNEL_LANGUAGE ON CHL_CHN_ID = CHN_ID
		AND CHL_LNG_ID = @LngId
	WHERE USR_ID = @UserId
		AND CSB_DELETE_FLAG = 0
END
