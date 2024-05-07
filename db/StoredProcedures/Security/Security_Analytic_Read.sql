-- ================================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 17/07/2023
-- Analytic - read a summary for each Mtr Code
--EXEC Security_Analytic_Read '2023-08-01','2023-08-08', 'en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Analytic_Read @DateFrom DATE
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




	--Create a language variant temp table of product values where they exist
	SELECT PRC_CODE,PRC_ID,PLG_VALUE,PLG_ID,LNG_ISO_CODE 
	INTO #tmpPrc
	FROM TD_PRODUCT 
	INNER JOIN TD_PRODUCT_LANGUAGE 
	ON PRC_ID=PLG_PRC_ID 
	INNER JOIN TS_LANGUAGE
	ON PLG_LNG_ID=LNG_ID
	AND LNG_DELETE_FLAG=0
	AND LNG_ISO_CODE=@LngIsoCode 

	--Create a language variant temp table of subject values where they exist
	SELECT DISTINCT SBJ_CODE,SBJ_ID, SLG_VALUE,SLG_LNG_ID,LNG_ISO_CODE   
	INTO #tmpSbj
	FROM TD_SUBJECT 
	INNER JOIN TD_SUBJECT_LANGUAGE 
	ON SBJ_ID=SLG_SBJ_ID 
	INNER JOIN TS_LANGUAGE 
	ON SLG_LNG_ID=LNG_ID 
	AND LNG_DELETE_FLAG=0
	AND LNG_ISO_CODE=@LngIsoCode 

	SELECT MtrCode
	,PrcCode
	,SbjCode
	,max(PublishDate) as PublishDate
	,PrcValue
	,SbjValue
	,sum(NltBot) as NltBot
	,sum(NltM2m) as NltM2m
	,sum(NltUser) as NltUser
	,sum(NltWidget) as NltWidget
	,sum(Total) as Total
	from
	(
	SELECT RLS_LIVE_DATETIME_FROM as PublishDate
		,Convert(INT, NLT_BOT_FLAG) AS NltBot
		,Convert(INT, NLT_M2M_FLAG) AS NltM2m
		,Convert(INT, NLT_USER_FLAG) AS NltUser
		,Convert(INT, NLT_WIDGET) AS NltWidget
		,TD_MATRIX.MTR_CODE AS MtrCode
		,TD_PRODUCT.PRC_CODE AS PrcCode
		, COALESCE(#tmpPrc.PLG_VALUE,PRC_VALUE) AS PrcValue
		,TD_SUBJECT.SBJ_CODE AS SbjCode
		, COALESCE (#tmpSbj.SLG_VALUE,SBJ_VALUE) AS SbjValue
		,Convert(INT, NLT_BOT_FLAG) + Convert(INT, NLT_M2M_FLAG) + Convert(INT, NLT_USER_FLAG) + Convert(INT, NLT_WIDGET) AS Total
 


	FROM TD_PRODUCT
	INNER JOIN TD_RELEASE ON RLS_PRC_ID = PRC_ID
	INNER JOIN TD_SUBJECT ON PRC_SBJ_ID = SBJ_ID
	INNER JOIN TD_MATRIX ON MTR_RLS_ID = RLS_ID
	INNER JOIN TD_GROUP ON RLS_GRP_ID = GRP_ID
	INNER JOIN TS_LANGUAGE AS mtrLNG ON MTR_LNG_ID = mtrLng.LNG_ID
		AND mtrLng.LNG_DELETE_FLAG = 0

	INNER JOIN TD_ANALYTIC ON MTR_ID = NLT_MTR_ID

	
	
	LEFT JOIN TS_LANGUAGE AS nltLng ON nltLng.LNG_ISO_CODE = NLT_LNG_ISO_CODE
		AND nltLng.LNG_DELETE_FLAG = 0

	left join #tmpPrc on TD_PRODUCT.PRC_CODE=#tmpPrc.PRC_CODE 

	left join #tmpSbj on TD_SUBJECT.SBJ_CODE=#tmpSbj.SBJ_CODE 

	WHERE NLT_DATE >= @DateFrom
		AND NLT_DATE <= @DateTo
		AND (
			@MtrCode IS NULL
			OR MTR_CODE = @MtrCode
			)
		AND (
			@PrcCode IS NULL
			OR TD_PRODUCT.PRC_CODE = @PrcCode
			)
		AND (
			@SbjCode IS NULL
			OR TD_SUBJECT.SBJ_CODE = @SbjCode
			)
		AND (
			@NltMaskedIp IS NULL
			OR NLT_MASKED_IP <> @NltMaskedIp
			)

			) q
	GROUP BY 
	 MtrCode
	,PrcCode
	,SbjCode
	,PrcValue
	,SbjValue
		

END
GO

