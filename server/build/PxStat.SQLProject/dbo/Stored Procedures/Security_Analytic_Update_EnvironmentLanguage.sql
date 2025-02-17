CREATE   PROCEDURE Security_Analytic_Update_EnvironmentLanguage
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @minDate AS DATE;
        SET @minDate = (SELECT MAX(NVL_NLT_DATE)
                        FROM   TR_ENVIRONMENT_LANGUAGE_ANALYTIC);
        IF @minDate IS NULL
            BEGIN
                SET @minDate = '2018-01-01';
            END
        SET @minDate = DATEADD(DAY, 1, @minDate);
        INSERT INTO TR_ENVIRONMENT_LANGUAGE_ANALYTIC (NVL_NLT_DATE, NVL_LNG_ISO_CODE, NVL_TOTAL)
        SELECT   NLT_DATE,
                 COALESCE (NLT_LNG_ISO_CODE, '-') AS NLT_LNG_ISO_CODE,
                 COUNT(*) AS rcount
        FROM     TD_ANALYTIC
        WHERE    NLT_DATE >= @minDate
                 AND NLT_DATE <= dateadd(DAY, -1, (CONVERT (DATE, getdate())))
        GROUP BY nlt_date, NLT_LNG_ISO_CODE;
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'INFO', 'SECURITY_ANALYTIC_UPDATE', 'Update_EnvironmentLanguage', 'Rows inserted: ' + CAST (@@rowcount AS NVARCHAR));
    END TRY
    BEGIN CATCH
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'ERROR', 'SECURITY_ANALYTIC_UPDATE', 'Update_EnvironmentLanguage', error_message());
    END CATCH
END

