
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 20/01/2022
-- Description:	For a given user list subscription channels and whether or not the user is subscribed. Language invariant.
-- exec Subscription_ChannelSubscription_Read null, 'okeeffene','en'
-- =============================================
CREATE
	

 PROCEDURE Subscription_ChannelSubscription_Read @SubscriberUserId NVARCHAR(256) = NULL
	,@CcnUsername NVARCHAR(256) = NULL
	,@LngIsoCode CHAR(2)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	IF @SubscriberUserId IS NULL
		AND @CcnUsername IS NULL
	BEGIN
		RETURN
	END

	DECLARE @LngId INT
	DECLARE @IsTrue BIT
	DECLARE @IsFalse BIT

	SET @IsTrue = 1
	SET @IsFalse = 0
	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngId IS NULL
	BEGIN
		SET @LngId = 0
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

	SELECT chn.CHN_CODE AS ChnCode
		,chn.CHN_NAME AS ChnName
		,CASE 
			WHEN CSB_CHN_ID IS NULL
				THEN @IsFalse
			ELSE @IsTrue
			END AS ChnSubscribed
	FROM (
		SELECT CHN_ID
			,COALESCE(CHL_CHN_NAME, CHN_NAME) AS CHN_NAME
			,CHN_CODE
		FROM TS_CHANNEL
		LEFT JOIN TS_CHANNEL_LANGUAGE ON CHN_ID = CHL_CHN_ID
			AND CHL_LNG_ID = @LngId
		) chn
	LEFT JOIN (
		SELECT CSB_ID
			,CSB_CHN_ID
			,CSB_USR_ID
		FROM TM_CHANNEL_SUBSCRIPTION
		WHERE CSB_DELETE_FLAG = 0
			AND CSB_USR_ID = @UserId
		) sub ON chn.CHN_ID = CSB_CHN_ID
END
