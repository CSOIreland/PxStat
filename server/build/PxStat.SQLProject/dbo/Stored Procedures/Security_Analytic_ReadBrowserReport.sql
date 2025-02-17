CREATE   PROCEDURE Security_Analytic_ReadBrowserReport
@DateFrom DATE, @DateTo DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT   BRW_BROWSER AS NltBrowser,
             SUM(BRW_TOTAL) AS NltCount
    FROM     TR_BROWSER_ANALYTIC
    WHERE    BRW_NLT_DATE >= @DateFrom
             AND BRW_NLT_DATE <= @DateTo
    GROUP BY BRW_BROWSER;
END

