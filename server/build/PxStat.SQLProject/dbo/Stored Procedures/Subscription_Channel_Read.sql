
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 02/12/2021
-- Description:	Read one or all Subscription Channel
-- EXEC Subscription_Channel_Read 'TECHNICAL','sw'
-- =============================================
CREATE
	

 PROCEDURE Subscription_Channel_Read

	@ChnCode NVARCHAR(256) = NULL
	,@LngIsoCode CHAR(2) 
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
		SET @LngId=0
	END

	SELECT CHN_CODE As ChnCode,
	COALESCE(CHL_CHN_NAME, CHN_NAME) AS ChnName
	FROM TS_CHANNEL
	LEFT JOIN TS_CHANNEL_LANGUAGE ON CHL_CHN_ID = CHN_ID
		AND CHL_LNG_ID = @LngId
	WHERE (
			@ChnCode IS NULL
			OR CHN_CODE = @ChnCode
			)
END
