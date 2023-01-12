SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 22/01/2019
-- Description:	Reads counts of Browsers
-- exec Security_Analytic_ReadBrowser '2022-11-14','2022-11-21','okeeffene'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Analytic_ReadBrowser @DateFrom DATE
	,@DateTo DATE
	,@CcnUsername NVARCHAR(256)
	,@NltInternalNetworkMask VARCHAR(12) = NULL
	,@MtrCode NVARCHAR(20) = NULL
	,@SbjCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SET @NltInternalNetworkMask = @NltInternalNetworkMask + '%';

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXECUTE Security_Group_AccessList @CcnUsername;

	SELECT CASE 
			WHEN NLT_BROWSER IS NULL
				THEN '-'
			ELSE NLT_BROWSER
			END AS NltBrowser
		,GRP_ID AS GrpId
		,count(*) as Counter
	FROM TD_ANALYTIC
	INNER JOIN TD_MATRIX ON NLT_MTR_ID = MTR_ID
		AND MTR_DELETE_FLAG = 0
	INNER JOIN TD_RELEASE ON RLS_ID = MTR_RLS_ID
		AND TD_RELEASE.RLS_DELETE_FLAG = 0
	INNER JOIN TD_GROUP ON GRP_ID = RLS_GRP_ID
		AND GRP_DELETE_FLAG = 0
	LEFT OUTER JOIN TD_PRODUCT ON RLS_PRC_ID = PRC_ID
		AND PRC_DELETE_FLAG = 0
	LEFT OUTER JOIN TD_SUBJECT ON PRC_SBJ_ID = SBJ_ID
		AND SBJ_DELETE_FLAG = 0
	WHERE NLT_DATE >= @DateFrom
		AND NLT_DATE <= @DateTo
		AND (
			@MtrCode IS NULL
			OR @MtrCode = MTR_CODE
			)
		AND (
			@NltInternalNetworkMask IS NULL
			OR NLT_MASKED_IP NOT LIKE @NltInternalNetworkMask
			)
		AND (
			@SbjCode = SBJ_CODE
			OR @SbjCode IS NULL
			)
		AND (
			@PrcCode = PRC_CODE
			OR @PrcCode IS NULL
			)
		GROUP BY NLT_BROWSER,GRP_ID 

	SELECT GRP_ID AS GrpId
	FROM @GroupUserHasAccess;
END
GO


