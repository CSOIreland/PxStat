CREATE   PROCEDURE Security_Auditing_Update
@DtgID INT, @UpdateCcnUsername VARCHAR (256)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @eMessage AS VARCHAR (256);
    DECLARE @recordCount AS INT;
    DECLARE @UpdateCcnID AS INT = NULL;
    DECLARE @DhtId AS INT = NULL;
    SET @UpdateCcnID = (SELECT CCN_ID
                        FROM   TD_ACCOUNT
                        WHERE  CCN_USERNAME = @UpdateCcnUsername
                               AND CCN_DELETE_FLAG = 0);
    IF @UpdateCcnID IS NULL
       OR @UpdateCcnID = 0
        BEGIN
            SET @eMessage = 'Error amending audit record - no account record found for @UpdateCcnID: ' + CAST (isnull(@UpdateCcnUsername, 0) AS VARCHAR);
            RAISERROR (@eMessage, 16, 1);
            RETURN 0;
        END
    SET @DhtId = (SELECT count(*)
                  FROM   TD_AUDITING
                  WHERE  DTG_ID = @DtgID);
    IF @DhtId IS NULL
       OR @recordCount = 0
        BEGIN
            SET @DhtId = 'Error amending audit record - no create audit record found for DtgID: ' + CAST (isnull(@DtgID, 0) AS VARCHAR);
            RAISERROR (@eMessage, 16, 1);
            RETURN 0;
        END
    INSERT  INTO TM_AUDITING_HISTORY (DHT_DTG_ID, DHT_DTP_ID, DHT_CCN_ID, DHT_DATETIME)
    VALUES                          (@DtgID, (SELECT DTP_ID
                                              FROM   TS_AUDITING_TYPE
                                              WHERE  DTP_CODE = 'UPDATED'), @UpdateCcnID, GETDATE());
    RETURN @@ROWCOUNT;
END

