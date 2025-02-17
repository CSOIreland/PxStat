CREATE   PROCEDURE Security_Analytic_Update_OS
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @minDate AS DATE;
        SET @minDate = (SELECT MAX(SNL_NLT_DATE)
                        FROM   TR_OS_ANALYTIC);
        IF @minDate IS NULL
            BEGIN
                SET @minDate = '2018-01-01';
            END
        INSERT INTO TR_OS_ANALYTIC (SNL_TOTAL, SNL_OS, SNL_NLT_DATE)
        SELECT   COUNT(*) AS NltCount,
                 CASE WHEN NLT_OS IS NULL
                           OR NLT_OS = '' THEN '-' ELSE NLT_OS END AS NltOs,
                 NLT_DATE
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
                 TS_LANGUAGE AS mtrLNG
                 ON MTR_LNG_ID = mtrLng.LNG_ID
                    AND mtrLng.LNG_DELETE_FLAG = 0
                 INNER JOIN
                 TD_ANALYTIC
                 ON MTR_ID = NLT_MTR_ID
                 INNER JOIN
                 TS_FORMAT
                 ON NLT_FRM_ID = FRM_ID
        WHERE    NLT_DATE >= @minDate
                 AND NLT_DATE <= dateadd(DAY, -1, (CONVERT (DATE, getdate())))
        GROUP BY NLT_OS, nlt_date;
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'INFO', 'SECURITY_ANALYTIC_UPDATE', 'Update_OS', 'Rows inserted: ' + CAST (@@rowcount AS NVARCHAR));
    END TRY
    BEGIN CATCH
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'ERROR', 'SECURITY_ANALYTIC_UPDATE', 'Update_OS', error_message());
    END CATCH
END

