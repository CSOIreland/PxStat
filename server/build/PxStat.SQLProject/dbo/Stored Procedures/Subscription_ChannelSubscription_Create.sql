
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 28/092021
-- Description:	Creates a subscription for a user to a channel
-- =============================================
CREATE
	

 PROCEDURE Subscription_ChannelSubscription_Create @ChnCode NVARCHAR(256)
	,@SubscriberUserId NVARCHAR(256) = NULL
	,@CcnUsername NVARCHAR(256) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @SubscriberUserId IS NULL
		AND @CcnUsername IS NULL
	BEGIN
		RETURN 0
	END

	DECLARE @UserId INT
	DECLARE @ChnId INT

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
		RETURN 0;
	END

	SET @ChnId = (
			SELECT CHN_ID
			FROM TS_CHANNEL
			WHERE CHN_CODE = @ChnCode
			)

	IF @ChnId IS NULL
	BEGIN
		RETURN 0
	END

	--Ensure this subscription doesn't exist already
	IF (
			SELECT COUNT(*)
			FROM TM_CHANNEL_SUBSCRIPTION
			WHERE CSB_CHN_ID = @ChnId
				AND CSB_USR_ID = @UserId
				AND CSB_DELETE_FLAG = 0
			) > 0
	BEGIN
		RETURN 0
	END

	INSERT INTO TM_CHANNEL_SUBSCRIPTION (
		CSB_CHN_ID
		,CSB_USR_ID
		,CSB_DELETE_FLAG
		)
	VALUES (
		@ChnId
		,@UserId
		,0
		)

	RETURN @@ROWCOUNT
END
