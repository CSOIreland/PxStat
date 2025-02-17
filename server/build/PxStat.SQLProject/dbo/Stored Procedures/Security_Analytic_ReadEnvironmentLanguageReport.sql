CREATE   PROCEDURE Security_Analytic_ReadEnvironmentLanguageReport
@DateFrom DATE, @DateTo DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT q.* from
    (
    SELECT   NVL_LNG_ISO_CODE AS nltLngIsoCode,
             SUM(NVL_TOTAL) AS lngCount
    FROM     TR_ENVIRONMENT_LANGUAGE_ANALYTIC
    WHERE    NVL_NLT_DATE >= @DateFrom
             AND NVL_NLT_DATE <= @DateTo
    GROUP BY NVL_LNG_ISO_CODE) q
    ORDER BY Q.lngCount DESC

END

