CREATE   PROCEDURE Security_Analytic_ReadReferersReport
@DateFrom DATE, @DateTo DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT   RFR_REFERER AS NltReferer,
             SUM(RFR_TOTAL) AS NltCount
    FROM     TR_REFERER_ANALYTIC
    WHERE    RFR_NLT_DATE >= @DateFrom
             AND RFR_NLT_DATE <= @DateTo
    GROUP BY RFR_REFERER;
END

