CREATE   PROCEDURE Security_Analytic_ReadTimelineReport
@DateFrom DATE, @DateTo DATE, @MtrCode NVARCHAR (20)=NULL, @PrcCode NVARCHAR (32)=NULL, @SbjCode INT=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT q.DATE,
           q.NltBot,
           q.NltM2m,
           q.NltUser,
           q.NltWidget,
           NltBot + NltM2m + NltUser + NltWidget AS total
    FROM   (SELECT   MNL_NLT_DATE AS DATE,
                     SUM(MNL_BOT) AS NltBot,
                     SUM(MNL_M2M) AS NltM2m,
                     SUM(MNL_USER) AS NltUser,
                     SUM(MNL_WIDGET) AS NltWidget
            FROM     TR_MATRIX_ANALYTIC
            WHERE    MNL_NLT_DATE >= @DateFrom
                     AND MNL_NLT_DATE <= @DateTo
                     AND (@MtrCode IS NULL
                          OR @MtrCode = MNL_MTR_CODE)
                     AND (@PrcCode IS NULL
                          OR @PrcCode = MNL_PRC_CODE)
                     AND ((@SbjCode IS NULL
                           OR @SbjCode = 0)
                          OR @SbjCode = MNL_SBJ_CODE)
            GROUP BY MNL_NLT_DATE) AS q;
END

