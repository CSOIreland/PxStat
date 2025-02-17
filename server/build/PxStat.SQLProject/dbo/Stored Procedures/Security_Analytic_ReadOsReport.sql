CREATE   PROCEDURE Security_Analytic_ReadOsReport
@DateFrom DATE, @DateTo DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT   SNL_OS AS NltOs,
             SUM(SNL_TOTAL) AS NltCount
    FROM     TR_OS_ANALYTIC
    WHERE    SNL_NLT_DATE >= @DateFrom
             AND SNL_NLT_DATE <= @DateTo
    GROUP BY SNL_OS;
END

