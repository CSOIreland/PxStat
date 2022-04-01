SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 21/09/2021
-- Description:	Updates a Subscriber
-- =============================================
CREATE
	OR

ALTER PROCEDURE Subscription_Subscriber_Update @SbrUserId NVARCHAR(256)
	,@SbrPreference NVARCHAR(MAX) = NULL
	,@LngIsoCode CHAR(2)=NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	UPDATE TD_SUBSCRIBER
	SET SBR_PREFERENCE = @SbrPreference
	WHERE SBR_UID = @SbrUserId
		AND SBR_DELETE_FLAG = 0

	DECLARE @RowsUpdated INT 
	SET @RowsUpdated=@@ROWCOUNT

	DECLARE @LngId INT

	IF @LngIsoCode IS NOT NULL
	BEGIN
		SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

		IF @LngId IS NOT NULL
		BEGIN
			UPDATE TD_USER
			SET USR_LNG_ID = @LngId
			WHERE USR_ID =(SELECT SBR_USR_ID  FROM TD_SUBSCRIBER WHERE SBR_UID =@SbrUserId AND SBR_DELETE_FLAG=0)
		
		END
	END

	RETURN @RowsUpdated;
END
GO


