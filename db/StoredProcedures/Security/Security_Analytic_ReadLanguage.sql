SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 15/05/2019
-- Description:	Returns a count of matrices queried per language
-- exec Security_Analytic_ReadLanguage '2020-08-01','2022-11-21','okeeffene'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Analytic_ReadLanguage @DateFrom DATE
	,@DateTo DATE
	,@CcnUsername NVARCHAR(256)
	,@NltInternalNetworkMask VARCHAR(12) = NULL
	,@MtrCode NVARCHAR(20) = NULL
	,@SbjCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @NltInternalNetworkMask = @NltInternalNetworkMask + '%'

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	SELECT LNG_ISO_NAME AS LngIsoName
		,GRP_ID AS GrpId
		,count(*) AS lngCount
	FROM TD_ANALYTIC
	INNER JOIN TD_MATRIX
		ON NLT_MTR_ID = MTR_ID
			AND MTR_DELETE_FLAG = 0
	INNER JOIN TS_LANGUAGE
		ON MTR_LNG_ID = LNG_ID
			AND LNG_DELETE_FLAG = 0

	INNER JOIN TD_RELEASE
		ON RLS_ID = MTR_RLS_ID
			AND TD_RELEASE.RLS_DELETE_FLAG = 0
	INNER JOIN TD_GROUP 
		ON GRP_ID=RLS_GRP_ID 
	LEFT JOIN TD_PRODUCT
		ON RLS_PRC_ID = PRC_ID
			AND PRC_DELETE_FLAG = 0
	LEFT JOIN TD_SUBJECT
		ON PRC_SBJ_ID = SBJ_ID
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
	GROUP BY LNG_ISO_NAME,GRP_ID

	SELECT GRP_ID AS GrpId
	FROM @GroupUserHasAccess;
END
GO


