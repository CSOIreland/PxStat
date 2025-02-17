CREATE   PROCEDURE Security_Auditing_Delete
@DtgID INT, @DeleteCcnUsername NVARCHAR (256)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @eMessage AS VARCHAR (256);
    DECLARE @recordCount AS INT;
    DECLARE @DeleteCcnID AS INT = NULL;
    SET @DeleteCcnID = (SELECT CCN_ID
                        FROM   TD_ACCOUNT
                        WHERE  CCN_USERNAME = @DeleteCcnUsername
                               AND CCN_DELETE_FLAG = 0);
    IF @DeleteCcnID IS NULL
       OR @DeleteCcnID = 0
        BEGIN
            SET @eMessage = 'Error amending audit record - no account record found for @DeleteCcnUsername: ' + CAST (isnull(@DeleteCcnUsername, 0) AS VARCHAR);
            RAISERROR (@eMessage, 16, 1);
            RETURN 0;
        END
    SET @recordCount = (SELECT count(*)
                        FROM   TD_AUDITING
                        WHERE  DTG_ID = @DtgID);
    IF @recordCount IS NULL
       OR @recordCount = 0
        BEGIN
            SET @eMessage = 'Error amending audit record - no create audit record found for DtgID: ' + CAST (isnull(@DtgID, 0) AS VARCHAR);
            RAISERROR (@eMessage, 16, 1);
            RETURN 0;
        END
    INSERT  INTO TM_AUDITING_HISTORY (DHT_DTG_ID, DHT_DTP_ID, DHT_CCN_ID, DHT_DATETIME)
    VALUES                          (@DtgID, (SELECT DTP_ID
                                              FROM   TS_AUDITING_TYPE
                                              WHERE  DTP_CODE = 'DELETED'), @DeleteCcnID, GETDATE());
    RETURN @@ROWCOUNT;
END

