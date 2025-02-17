
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 23/09/2021
-- Description:	Create a subscription to a table
-- =============================================
CREATE
	

 PROCEDURE Subscription_Table_Subscription_Create @TsbTable NVARCHAR(20)
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

	--Ensure this subscription doesn't exist already

	IF (SELECT COUNT(*) FROM TM_TABLE_SUBSCRIPTION WHERE TSB_TABLE=@TsbTable AND TSB_USR_ID=@UserId AND TSB_DELETE_FLAG=0) >0
	BEGIN
		RETURN 0;
	END

	INSERT INTO TM_TABLE_SUBSCRIPTION (
		TSB_TABLE
		,TSB_USR_ID
		,TSB_DELETE_FLAG
		)
	VALUES (
		@TsbTable
		,@UserId
		,0
		)

	RETURN @@IDENTITY
END
