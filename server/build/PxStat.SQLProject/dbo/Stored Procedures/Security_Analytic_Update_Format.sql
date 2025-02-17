CREATE   PROCEDURE Security_Analytic_Update_Format
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @minDate AS DATE;
        SET @minDate = (SELECT MAX(FNL_NLT_DATE)
                        FROM   TR_FORMAT_ANALYTIC);
        IF @minDate IS NULL
            BEGIN
                SET @minDate = '2018-01-01';
            END
        SET @minDate = DATEADD(DAY, 1, @minDate);
        INSERT INTO TR_FORMAT_ANALYTIC (FNL_NLT_DATE, FNL_TOTAL, FNL_FORMAT)
        SELECT   nlt_date,
                 COUNT(*) AS NltCount,
                 FRM_TYPE + ' ' + FRM_VERSION AS FrmTypeVersion
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
        GROUP BY nlt_date, frm_type, frm_version;
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'INFO', 'SECURITY_ANALYTIC_UPDATE', 'Update_Format', 'Rows inserted: ' + CAST (@@rowcount AS NVARCHAR));
    END TRY
    BEGIN CATCH
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'ERROR', 'SECURITY_ANALYTIC_UPDATE', 'Update_Format', error_message());
    END CATCH
END

