CREATE   PROCEDURE Security_Analytic_ReadFormatReport
@DateFrom DATE, @DateTo DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT   SUM(FNL_TOTAL) AS NltCount,
             FNL_FORMAT AS FrmTypeVersion
    FROM     TR_FORMAT_ANALYTIC
    WHERE    FNL_NLT_DATE >= @DateFrom
             AND FNL_NLT_DATE <= @DateTo
    GROUP BY FNL_FORMAT;
END

