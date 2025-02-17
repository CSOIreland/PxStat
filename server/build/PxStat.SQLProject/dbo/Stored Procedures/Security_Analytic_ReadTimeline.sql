
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 17/07/2023
-- Read Timeline for analytic. This is a total of flagged items under a number of categories
--EXEC Security_Analytic_ReadTimeline '2023-06-22','2023-06-29', 'en'
-- =============================================
CREATE
	

 PROCEDURE Security_Analytic_ReadTimeline @DateFrom DATE
	,@DateTo DATE
	,@LngIsoCode CHAR(2)
	,@MtrCode NVARCHAR(20) = NULL
	,@PrcCode NVARCHAR(32) = NULL
	,@SbjCode INT = NULL
	,@NltMaskedIp VARCHAR(11) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	 DECLARE @LngId INT
	 SET @LngId=(SELECT LNG_ID FROM TS_LANGUAGE WHERE LNG_ISO_CODE=@LngIsoCode AND LNG_DELETE_FLAG=0)

	 --Create a language variant temp table of matrix titles where they exist
	SELECT 
	 MTR_CODE  as MtrCode,max(MTR_TITLE) as MtrTitle,LNG_ISO_CODE as MtrLngIsoCode, LNG_ID AS mtrLngId
	into #tmpMtr
	FROM TD_MATRIX
	INNER JOIN TS_LANGUAGE
	ON LNG_ID=MTR_LNG_ID 
	AND LNG_DELETE_FLAG=0
	GROUP BY MTR_CODE,LNG_ISO_CODE,LNG_ID 




	SELECT NLT_DATE AS [date]
		,sum(Convert(INT, NLT_BOT_FLAG)) AS NltBot

		,sum(Convert(INT, NLT_M2M_FLAG)) AS NltM2m
		
		,sum(Convert(INT, NLT_USER_FLAG)) AS NltUser
		,sum(Convert(INT, NLT_WIDGET)) AS NltWidget

		,sum(Convert(INT, NLT_BOT_FLAG) + Convert(INT, NLT_M2M_FLAG) + Convert(INT, NLT_USER_FLAG) + Convert(INT, NLT_WIDGET)) AS total
 


	FROM TD_PRODUCT
	INNER JOIN TD_RELEASE ON RLS_PRC_ID = PRC_ID
	INNER JOIN TD_SUBJECT ON PRC_SBJ_ID = SBJ_ID
	INNER JOIN TD_MATRIX ON MTR_RLS_ID = RLS_ID
	INNER JOIN TD_GROUP ON RLS_GRP_ID = GRP_ID
	INNER JOIN TS_LANGUAGE AS mtrLNG ON MTR_LNG_ID = mtrLng.LNG_ID
		AND mtrLng.LNG_DELETE_FLAG = 0

	INNER JOIN TD_ANALYTIC ON MTR_ID = NLT_MTR_ID
	INNER JOIN TS_FORMAT ON NLT_FRM_ID = FRM_ID
	
	
	LEFT JOIN TS_LANGUAGE AS nltLng ON nltLng.LNG_ISO_CODE = NLT_LNG_ISO_CODE
		AND nltLng.LNG_DELETE_FLAG = 0


	WHERE NLT_DATE >= @DateFrom
		AND NLT_DATE <= @DateTo
		AND (
			@MtrCode IS NULL
			OR MTR_CODE = @MtrCode
			)
		AND (
			@PrcCode IS NULL
			OR PRC_CODE = @PrcCode
			)
		AND (
			@SbjCode IS NULL
			OR SBJ_CODE = @SbjCode
			)
		AND (
			@NltMaskedIp IS NULL
			OR NLT_MASKED_IP <> @NltMaskedIp
			)
		GROUP BY NLT_DATE
		ORDER BY NLT_DATE
END
