
-- =============================================
-- Author:		Damian Chapman
-- Create date: 27/09/2021
-- Description:	Delete a Query
-- =============================================
CREATE 



PROCEDURE [dbo].[Subscription_Query_Delete] @UserQueryId int=NULL
    ,@SubscriberUserId NVARCHAR(256)=NULL
	,@CcnUsername NVARCHAR(256) = NULL	
AS
BEGIN
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
    DELETE
    FROM   TD_USER_QUERY
    WHERE  (@UserQueryId IS NULL
            OR SQR_ID = @UserQueryId)
	AND SQR_USR_ID = @UserId
	RETURN @@ROWCOUNT
END

