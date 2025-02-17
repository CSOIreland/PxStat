CREATE   PROCEDURE Security_Analytic_Update_DatasetLanguage
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @minDate AS DATE;
        SET @minDate = (SELECT MAX(DSL_NLT_DATE)
                        FROM   TR_DATASET_LANGUAGE_ANALYTIC);
        IF @minDate IS NULL
            BEGIN
                SET @minDate = '2018-01-01';
            END
        SET @minDate = DATEADD(DAY, 1, @minDate);
        INSERT INTO TR_DATASET_LANGUAGE_ANALYTIC (DSL_NLT_DATE, DSL_TOTAL, DSL_LNG_ISO_NAME, DSL_LNG_ISO_CODE)
        SELECT   NLT_DATE,
                 COUNT(*) AS lngCount,
                 LNG_ISO_NAME,
                 LNG_ISO_CODE
        FROM     TD_PRODUCT
                 INNER JOIN
                 TD_RELEASE
                 ON RLS_PRC_ID = PRC_ID
                 INNER JOIN
                 TD_SUBJECT
                 ON PRC_SBJ_ID = SBJ_ID
                 INNER JOIN
                 TD_MATRIX
                 ON MTR_RLS_ID = RLS_ID
                 INNER JOIN
                 TD_GROUP
                 ON RLS_GRP_ID = GRP_ID
                 INNER JOIN
                 TS_LANGUAGE
                 ON MTR_LNG_ID = LNG_ID
                    AND LNG_DELETE_FLAG = 0
                 INNER JOIN
                 TD_ANALYTIC
                 ON MTR_ID = NLT_MTR_ID
        WHERE    NLT_DATE >= @minDate
                 AND NLT_DATE <= dateadd(DAY, -1, (CONVERT (DATE, getdate())))
        GROUP BY NLT_DATE, LNG_ISO_NAME, LNG_ISO_CODE;
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'INFO', 'SECURITY_ANALYTIC_UPDATE', 'Update_DatasetLanguage', 'Rows inserted: ' + CAST (@@rowcount AS NVARCHAR));
    END TRY
    BEGIN CATCH
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'ERROR', 'SECURITY_ANALYTIC_UPDATE', 'Update_DatasetLanguage ', error_message());
    END CATCH
END

