CREATE   PROCEDURE Security_Analytic_Update_BrowserReport
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @minDate AS DATE;
        SET @minDate = (SELECT MAX(BRW_NLT_DATE)
                        FROM   TR_BROWSER_ANALYTIC);
        IF @minDate IS NULL
            BEGIN
                SET @minDate = '2018-01-01';
            END
        SET @minDate = DATEADD(DAY, 1, @minDate);
        INSERT INTO TR_BROWSER_ANALYTIC (BRW_BROWSER, BRW_NLT_DATE, BRW_TOTAL)
        SELECT   COALESCE (NLT_BROWSER, '-') AS NLT_BROWSER,
                 NLT_DATE,
                 COUNT(*) AS rcount
        FROM     TD_ANALYTIC
        WHERE    NLT_DATE >= @minDate
                 AND NLT_DATE <= dateadd(DAY, -1, (CONVERT (DATE, getdate())))
        GROUP BY NLT_BROWSER, NLT_DATE;
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'INFO', 'SECURITY_ANALYTIC_UPDATE', 'Update_BrowserReport', 'Rows inserted: ' + CAST (@@rowcount AS NVARCHAR));
    END TRY
    BEGIN CATCH
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'ERROR', 'SECURITY_ANALYTIC_UPDATE', 'Update_BrowserReport', error_message());
    END CATCH
END

