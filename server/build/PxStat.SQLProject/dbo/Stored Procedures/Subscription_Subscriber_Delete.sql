
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 18/10/2021
-- Description:	Soft deletes a subscriber
-- =============================================
CREATE
	

 PROCEDURE Subscription_Subscriber_Delete @SbrUserId NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE TD_SUBSCRIBER
	SET SBR_DELETE_FLAG = 1
	WHERE SBR_UID = @SbrUserId

	RETURN @@ROWCOUNT
END
