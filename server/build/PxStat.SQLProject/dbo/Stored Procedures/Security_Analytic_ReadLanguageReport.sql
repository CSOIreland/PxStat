CREATE   PROCEDURE Security_Analytic_ReadLanguageReport
@DateFrom DATE, @DateTo DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT   DSL_LNG_ISO_CODE AS mtrLngIsoCode,
             DSL_LNG_ISO_NAME AS mtrLngName,
             SUM(DSL_TOTAL) AS lngCount
    FROM     TR_DATASET_LANGUAGE_ANALYTIC
    WHERE    DSL_NLT_DATE >= @DateFrom
             AND DSL_NLT_DATE <= @DateTo
    GROUP BY DSL_LNG_ISO_CODE, DSL_LNG_ISO_NAME;
END

