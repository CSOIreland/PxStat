
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 21/09/2021
-- Description:	Create a new subscriber entry for a CcnUsername or a FirebaseId
-- =============================================
CREATE
	

 PROCEDURE Subscription_Subscriber_Create @Preference NVARCHAR(MAX) = NULL
	,@SubscriberUserId NVARCHAR(256)
	,@LngIsoCode CHAR(2)
	,@SbrKey NVARCHAR(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @UserId INT;
	DECLARE @Uid NVARCHAR(64)
	DECLARE @LngId INT

	--The SubscriberUserId can't be already used 
	IF (
			SELECT COUNT(*)
			FROM TD_SUBSCRIBER
			WHERE SBR_UID = @SubscriberUserId
				AND SBR_DELETE_FLAG = 0
			) > 0
	BEGIN
		RETURN 0
	END

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngId IS NULL
	BEGIN
		RETURN 0
	END

	-- Create the TD_USER entry
	INSERT TD_USER (USR_LNG_ID)
	VALUES (@LngId)

	SET @UserId = @@IDENTITY
	SET @Uid = @SubscriberUserId

	INSERT INTO TD_SUBSCRIBER (
		SBR_USR_ID
		,SBR_UID
		,SBR_PREFERENCE
		,SBR_KEY
		,SBR_DELETE_FLAG
		)
	VALUES (
		@UserId
		,@Uid
		,@Preference
		,@SbrKey
		,0
		)

	RETURN @@ROWCOUNT;
END
