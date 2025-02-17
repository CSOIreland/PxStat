CREATE   PROCEDURE Security_Analytic_Update_ReadReport
@DefaultLngIsoCode CHAR (2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @minDate AS DATE;
        SET @minDate = (SELECT MAX(MNL_NLT_DATE)
                        FROM   TR_MATRIX_ANALYTIC);
        IF @minDate IS NULL
            BEGIN
                SET @minDate = '2018-01-01';
            END
        SET @minDate = DATEADD(DAY, 1, @minDate);
        DROP TABLE IF EXISTS #tmpPrc;
        DROP TABLE IF EXISTS #tmpSbj;
        SELECT PRC_CODE,
               PRC_ID,
               PLG_VALUE,
               PLG_ID,
               LNG_ISO_CODE
        INTO   #tmpPrc
        FROM   TD_PRODUCT
               INNER JOIN
               TD_PRODUCT_LANGUAGE
               ON PRC_ID = PLG_PRC_ID
               INNER JOIN
               TS_LANGUAGE
               ON PLG_LNG_ID = LNG_ID
                  AND LNG_DELETE_FLAG = 0
                  AND LNG_ISO_CODE = @DefaultLngIsoCode;
        SELECT DISTINCT SBJ_CODE,
                        SBJ_ID,
                        SLG_VALUE,
                        SLG_LNG_ID,
                        LNG_ISO_CODE
        INTO   #tmpSbj
        FROM   TD_SUBJECT
               INNER JOIN
               TD_SUBJECT_LANGUAGE
               ON SBJ_ID = SLG_SBJ_ID
               INNER JOIN
               TS_LANGUAGE
               ON SLG_LNG_ID = LNG_ID
                  AND LNG_DELETE_FLAG = 0
                  AND LNG_ISO_CODE = @DefaultLngIsoCode;
        INSERT INTO TR_MATRIX_ANALYTIC (MNL_MTR_CODE, MNL_PRC_CODE, MNL_PRC_VALUE, MNL_SBJ_CODE, MNL_SBJ_VALUE, MNL_BOT, MNL_M2M, MNL_USER, MNL_WIDGET, MNL_TOTAL, MNL_NLT_DATE)
        SELECT   MtrCode,
                 PrcCode,
                 PrcValue,
                 SbjCode,
                 SbjValue,
                 sum(COALESCE (NltBot, 0)) AS NltBot,
                 sum(COALESCE (NltM2m, 0)) AS NltM2m,
                 sum(COALESCE (NltUser, 0)) AS NltUser,
                 sum(COALESCE (NltWidget, 0)) AS NltWidget,
                 sum(COALESCE (Total, 0)) AS Total,
                 NltDate
        FROM     (SELECT CONVERT (INT, NLT_BOT_FLAG) AS NltBot,
                         CONVERT (INT, NLT_M2M_FLAG) AS NltM2m,
                         CONVERT (INT, NLT_USER_FLAG) AS NltUser,
                         CONVERT (INT, NLT_WIDGET) AS NltWidget,
                         TD_MATRIX.MTR_CODE AS MtrCode,
                         TD_PRODUCT.PRC_CODE AS PrcCode,
                         COALESCE (#tmpPrc.PLG_VALUE, PRC_VALUE) AS PrcValue,
                         TD_SUBJECT.SBJ_CODE AS SbjCode,
                         COALESCE (#tmpSbj.SLG_VALUE, SBJ_VALUE) AS SbjValue,
                         CONVERT (INT, NLT_BOT_FLAG) + CONVERT (INT, NLT_M2M_FLAG) + CONVERT (INT, NLT_USER_FLAG) + CONVERT (INT, NLT_WIDGET) AS Total,
                         NLT_DATE AS NltDate
                  FROM   TD_PRODUCT
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
                         LEFT OUTER JOIN
                         TS_LANGUAGE AS nltLng
                         ON nltLng.LNG_ISO_CODE = NLT_LNG_ISO_CODE
                            AND nltLng.LNG_DELETE_FLAG = 0
                         LEFT OUTER JOIN
                         #tmpPrc
                         ON TD_PRODUCT.PRC_CODE = #tmpPrc.PRC_CODE
                         LEFT OUTER JOIN
                         #tmpSbj
                         ON TD_SUBJECT.SBJ_CODE = #tmpSbj.SBJ_CODE
                  WHERE  NLT_DATE >= @minDate
                         AND NLT_DATE <= dateadd(DAY, -1, (CONVERT (DATE, getdate())))) AS q
        WHERE    SbjCode > 0
                 AND PrcCode IS NOT NULL
        GROUP BY MtrCode, SbjCode, SbjValue, PrcCode, PrcValue, SbjValue, NltDate
        ORDER BY NltDate;
        DECLARE @RowCount AS INT;
        SET @RowCount = @@rowcount;
        DROP TABLE IF EXISTS #tmpPrc;
        DROP TABLE IF EXISTS #tmpSbj;
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'INFO', 'SECURITY_ANALYTIC_UPDATE', 'Update_ReadReport', 'Rows inserted: ' + CAST (@RowCount AS NVARCHAR));
    END TRY
    BEGIN CATCH
        INSERT  INTO TD_LOGGING (LGG_DATETIME, LGG_THREAD, LGG_LEVEL, LGG_CLASS, LGG_METHOD, LGG_MESSAGE)
        VALUES                 (GETDATE(), '0', 'ERROR', 'SECURITY_ANALYTIC_UPDATE', 'Update_ReadReport', error_message());
    END CATCH
END

