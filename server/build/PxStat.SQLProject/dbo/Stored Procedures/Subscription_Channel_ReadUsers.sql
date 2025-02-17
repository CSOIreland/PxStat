
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 08/02/2022
-- Description:	Get all subcribers for a channel
-- EXEC Subscription_Channel_ReadUsers null,0,'TECHNICAL',NULL
-- =============================================
CREATE
	

 PROCEDURE Subscription_Channel_ReadUsers @LngIsoCode CHAR(2)
	,@SingleLanguage BIT
	,@ChnCode NVARCHAR(256) = NULL
	,@SbrUid NVARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngId INT

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

	DECLARE @Users TABLE ([uid] INT)

	INSERT INTO @Users 
	SELECT DISTINCT CSB_USR_ID AS uid
	FROM TM_CHANNEL_SUBSCRIPTION
	INNER JOIN TS_CHANNEL ON CHN_ID = CSB_CHN_ID
		AND (@ChnCode IS NULL
		OR (CHN_CODE = @ChnCode))
		AND CSB_DELETE_FLAG=0

	INNER JOIN TD_USER ON USR_ID = CSB_USR_ID
	AND (@LngId = USR_LNG_ID)
		OR (
			@SingleLanguage IS NULL
			OR @SingleLanguage = 0
			)
		
	LEFT JOIN TD_ACCOUNT ON CCN_USR_ID = USR_ID
	LEFT JOIN TD_SUBSCRIBER ON CCN_USR_ID = SBR_USR_ID
	WHERE CSB_USR_ID IS NOT NULL

	SELECT CCN_USERNAME AS CcnUsername
		,SBR_UID AS SbrUserId
		,CCN_EMAIL AS CcnEmail
		,CCN_DISPLAYNAME AS FullName
	FROM td_user
	LEFT JOIN td_account ON CCN_USR_ID = usr_id
	LEFT JOIN TD_SUBSCRIBER ON SBR_USR_ID = usr_id
	WHERE usr_id IN (
			SELECT [uid]
			FROM @Users
			)
END
