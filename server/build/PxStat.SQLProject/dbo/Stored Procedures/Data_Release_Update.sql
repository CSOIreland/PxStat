CREATE   PROCEDURE Data_Release_Update
@CcnUsername NVARCHAR (256), @RlsCode INT, @RlsVersion INT, @RlsRevision INT, @RlsLiveFlag BIT, @RlsLiveDatetimeFrom DATETIME=NULL, @RlsLiveDatetimeTo DATETIME=NULL, @RlsExceptionalFlag BIT, @RlsReservationFlag BIT, @RlsArchiveFlag BIT, @RlsExperimentalFlag BIT, @RlsAnalyticalFlag BIT, @PrcCode NVARCHAR (32)=NULL, @RlsCmmCode INT=NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DtgID AS INT;
    DECLARE @eMessage AS VARCHAR (256);
    DECLARE @updateCount AS INT;
    DECLARE @PrcID AS INT;
    SET @PrcID = (SELECT PRC_ID
                  FROM   TD_PRODUCT
                  WHERE  PRC_CODE = @PrcCode
                         AND PRC_DELETE_FLAG = 0);
    SET @DtgID = (SELECT RLS_DTG_ID
                  FROM   TD_RELEASE
                  WHERE  RLS_CODE = @RlsCode
                         AND RLS_DELETE_FLAG = 0);
    DECLARE @RlsCmmId AS INT
    SET @RlsCmmId=NULL
    IF @RlsCmmCode IS NOT NULL AND @RlsCode>0
    BEGIN
        SET @RlsCmmId=(SELECT TOP 1 CMM_ID FROM TD_COMMENT WHERE CMM_CODE=@RlsCmmCode )
    END
    IF @DtgID = 0
       OR @DtgID IS NULL
        BEGIN
            RETURN 0;
        END
    UPDATE TD_RELEASE
    SET    RLS_VERSION            = @RlsVersion,
           RLS_REVISION           = @RlsRevision,
           RLS_LIVE_FLAG          = @RlsLiveFlag,
           RLS_LIVE_DATETIME_FROM = @RlsLiveDatetimeFrom,
           RLS_LIVE_DATETIME_TO   = @RlsLiveDatetimeTo,
           RLS_EXCEPTIONAL_FLAG   = @RlsExceptionalFlag,
           RLS_RESERVATION_FLAG   = @RlsReservationFlag,
           RLS_ARCHIVE_FLAG       = @RlsArchiveFlag,
           RLS_ANALYTICAL_FLAG    = @RlsAnalyticalFlag,
           RLS_EXPERIMENTAL_FLAG  = @RlsExperimentalFlag,
           RLS_PRC_ID             = CASE WHEN @PrcID > 0 THEN @PrcID ELSE NULL END,
           RLS_CMM_ID             = CASE WHEN @RlsCmmId IS NOT NULL THEN @RlsCmmId ELSE RLS_CMM_ID END
    WHERE  RLS_CODE = @RlsCode
           AND RLS_DELETE_FLAG = 0;
    SET @updateCount = @@ROWCOUNT;
    IF @updateCount > 0
        BEGIN
            DECLARE @AuditUpdateCount AS INT;
            EXECUTE @AuditUpdateCount = Security_Auditing_Update @DtgID, @CcnUsername;
            IF @AuditUpdateCount = 0
                BEGIN
                    SET @eMessage = 'Error creating entry in TD_AUDITING for Release update:' + CAST (isnull(@RlsCode, 0) AS VARCHAR) + ' DTG ID: ' + @DtgID;
                    RAISERROR (@eMessage, 16, 1);
                    RETURN;
                END
        END
    IF @PrcCode IS NOT NULL
        BEGIN
            DECLARE @ReleaseId AS INT;
            DECLARE @ProductId AS INT;
            DECLARE @Count AS INT;
            SELECT @ReleaseId = RLS_ID
            FROM   TD_RELEASE
            WHERE  RLS_CODE = @RlsCode;
            SELECT @ProductId = PRC_ID
            FROM   TD_PRODUCT
            WHERE  PRC_CODE = @PrcCode;
            SELECT @Count = COUNT(*)
            FROM   TM_RELEASE_PRODUCT
            WHERE  RPR_RLS_ID = @ReleaseId
                   AND RPR_PRC_ID = @ProductId
                   AND RPR_DELETE_FLAG = 0;
            IF @Count > 0
                BEGIN
                    DECLARE @auditId AS INT = NULL;
                    EXECUTE @auditId = Security_Auditing_Create @CcnUsername;
                    IF @auditId IS NULL
                       OR @auditId = 0
                        BEGIN
                            RAISERROR ('SP: Security_Auditing_Create has failed!', 16, 1);
                            RETURN 0;
                        END
                    UPDATE TM_RELEASE_PRODUCT
                    SET    RPR_DELETE_FLAG = 1,
                           RPR_DTG_ID      = @auditId
                    WHERE  RPR_RLS_ID = @ReleaseId
                           AND RPR_PRC_ID = @ProductId
                           AND RPR_DELETE_FLAG = 0;
                END
        END
    RETURN @updateCount;
END

