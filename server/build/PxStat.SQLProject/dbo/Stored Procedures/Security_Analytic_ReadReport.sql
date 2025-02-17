CREATE   PROCEDURE Security_Analytic_ReadReport
@DateFrom DATE, @DateTo DATE, @MtrCode NVARCHAR (20)=NULL, @PrcCode NVARCHAR (32)=NULL, @SbjCode INT=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT   MNL_MTR_CODE AS MtrCode,
             MNL_PRC_CODE AS PrcCode,
             MNL_SBJ_CODE AS SbjCode,
             MNL_PRC_VALUE AS PrcValue,
             MNL_SBJ_VALUE AS SbjValue,
             SUM(MNL_BOT) AS NltBot,
             SUM(MNL_M2M) AS NltM2m,
             SUM(MNL_USER) AS NltUser,
             SUM(MNL_WIDGET) AS NltWidget,
             SUM(MNL_TOTAL) AS Total
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
    GROUP BY MNL_MTR_CODE, MNL_PRC_CODE, MNL_SBJ_CODE, MNL_PRC_VALUE, MNL_SBJ_VALUE;
END

