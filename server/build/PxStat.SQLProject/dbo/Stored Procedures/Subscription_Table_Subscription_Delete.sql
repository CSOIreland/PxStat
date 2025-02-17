CREATE   PROCEDURE Subscription_Table_Subscription_Delete
@TsbTable NVARCHAR (20), @SubscriberUserId NVARCHAR (256)=NULL, @CcnUsername NVARCHAR (256)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @SubscriberUserId IS NULL
       AND @CcnUsername IS NULL
        BEGIN
            RETURN 0;
        END
    DECLARE @UserId AS INT;
    IF @SubscriberUserId IS NOT NULL
        BEGIN
            SET @UserId = (SELECT USR_ID
                           FROM   TD_USER
                                  INNER JOIN
                                  TD_SUBSCRIBER
                                  ON SBR_USR_ID = USR_ID
                                     AND SBR_UID = @SubscriberUserId
                                     AND SBR_DELETE_FLAG = 0);
        END
    ELSE
        BEGIN
            SET @UserId = (SELECT USR_ID
                           FROM   TD_USER
                                  INNER JOIN
                                  TD_ACCOUNT
                                  ON CCN_USR_ID = USR_ID
                                     AND CCN_USERNAME = @CcnUsername
                                     AND CCN_DELETE_FLAG = 0);
        END
    IF @UserId IS NULL
        BEGIN
            RETURN 0;
        END
    UPDATE TM_TABLE_SUBSCRIPTION
    SET    TSB_DELETE_FLAG = 1
    WHERE  TSB_USR_ID = @UserId
           AND TSB_TABLE = @TsbTable;
    RETURN 1;
END

