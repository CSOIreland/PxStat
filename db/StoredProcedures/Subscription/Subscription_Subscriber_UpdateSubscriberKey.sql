SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/12/2021
-- Description:	Updates a Subscriber Key
-- =============================================
CREATE
	OR

ALTER PROCEDURE Subscription_Subscriber_UpdateSubscriberKey
 @SbrUserId NVARCHAR(256),
	@SbrKey NVARCHAR(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	UPDATE TD_SUBSCRIBER
	SET SBR_KEY  = @SbrKey
	WHERE SBR_UID = @SbrUserId
		AND SBR_DELETE_FLAG = 0


	RETURN @@ROWCOUNT;
END
GO
