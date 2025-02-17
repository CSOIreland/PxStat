CREATE   PROCEDURE Security_Auditing_Create
@userName NVARCHAR (256)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CcnId AS INT = NULL;
    DECLARE @errorMessage AS VARCHAR (256) = NULL;
    DECLARE @DtgId AS INT = NULL;
    SELECT @CcnId = CCN_ID
    FROM   TD_ACCOUNT
    WHERE  CCN_USERNAME = @userName
           AND CCN_DELETE_FLAG = 0;
    IF @CcnId IS NULL
        BEGIN
            SET @errorMessage = 'SP: Security_Auditing_Create - userName not found: ' + CAST (@userName AS VARCHAR);
            RAISERROR (@errorMessage, 16, 1);
            RETURN 0;
        END
    INSERT  INTO TD_AUDITING
    DEFAULT VALUES;
    SET @DtgId = @@IDENTITY;
    BEGIN TRY
        INSERT  INTO TM_AUDITING_HISTORY (DHT_DTG_ID, DHT_DTP_ID, DHT_CCN_ID, DHT_DATETIME)
        VALUES                          (@DtgId, (SELECT DTP_ID
                                                  FROM   TS_AUDITING_TYPE
                                                  WHERE  DTP_CODE = 'CREATED'), @CcnId, GETDATE());
    END TRY
    BEGIN CATCH
        RETURN NULL;
    END CATCH
    RETURN @DtgId;
END

