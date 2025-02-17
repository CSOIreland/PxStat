
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 17/07/2023
-- Description:	Once off read for analytics. The result will be cached for the given parameters and re-used for all analytics API calls
-- We are NOT excluding deleted matrixes,groups, products or subjects as they may have been valid at the time of access
--EXEC Security_Analytic_ReadForAll '2023-06-22','2023-06-29', 'en'
-- =============================================
CREATE
	

 PROCEDURE Security_Analytic_ReadForAll @DateFrom DATE
	,@DateTo DATE
	,@LngIsoCode CHAR(2)
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

	SELECT NLT_ID AS NltId
		,RLS_LIVE_DATETIME_FROM as NltPublishedDate
		,NLT_MASKED_IP AS NltMaskedIp
		,CASE 
			WHEN NLT_OS IS NULL
				OR NLT_OS = ''
				THEN '-'
			ELSE NLT_OS
			END AS NltOs
		,CASE 
			WHEN NLT_BROWSER IS NULL
				OR NLT_BROWSER = ''
				THEN '-'
			ELSE NLT_BROWSER
			END AS NltBrowser
		,Convert(INT, NLT_BOT_FLAG) AS NltBotFlag
		,CASE 
			WHEN NLT_REFERER IS NULL
				OR NLT_REFERER = ''
				THEN '-'
			ELSE NLT_REFERER
			END AS NltReferer
		,Convert(INT, NLT_M2M_FLAG) AS NltM2mFlag
		,NLT_DATE AS [date]
		,Convert(INT, NLT_USER_FLAG) AS NltUserFlag
		,Convert(INT, NLT_WIDGET) AS NltWidget
		,TD_MATRIX.MTR_CODE AS MtrCode
		,COALESCE(#tmpMtr.MtrTitle,TD_MATRIX.MTR_TITLE) AS MtrTitle
		,mtrLng.LNG_ISO_CODE AS mtrLngIsoCode
		,mtrLng.LNG_ISO_NAME AS mtrLngName
		,CASE 
			WHEN nltLng.LNG_ISO_CODE IS NULL
				OR nltLng.LNG_ISO_CODE = ''
				THEN '-'
			ELSE nltLng.LNG_ISO_CODE
			END AS nltLngIsoCode
		,CASE 
			WHEN nltLng.LNG_ISO_NAME IS NULL
				OR nltLng.LNG_ISO_NAME = ''
				THEN '-'
			ELSE nltLng.LNG_ISO_NAME
			END AS nltLngName
		,RLS_CODE AS RlsCode
		,GRP_CODE AS GrpCode
		,TD_PRODUCT.PRC_CODE AS PrcCode
		, COALESCE(#tmpPrc.PLG_VALUE,PRC_VALUE) AS PrcValue
		,TD_SUBJECT.SBJ_CODE AS SbjCode
		, COALESCE (#tmpSbj.SLG_VALUE,SBJ_VALUE) AS SbjValue
		,FRM_TYPE AS FrmType
		,FRM_VERSION AS FrmVersion
		,FRM_TYPE + ' ' + FRM_VERSION AS FrmTypeVersion
		,Convert(INT, NLT_BOT_FLAG) + Convert(INT, NLT_M2M_FLAG) + Convert(INT, NLT_USER_FLAG) + Convert(INT, NLT_WIDGET) AS total
 


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
	LEFT JOIN #tmpMtr ON MTR_CODE=MtrCode
	AND #tmpMtr.MtrLngIsoCode=@LngIsoCode 

	left join #tmpPrc on TD_PRODUCT.PRC_CODE=#tmpPrc.PRC_CODE 
	and #tmpMtr.MtrLngIsoCode=#tmpPrc.LNG_ISO_CODE 

	left join #tmpSbj on TD_SUBJECT.SBJ_CODE=#tmpSbj.SBJ_CODE 
	AND #tmpMtr.MtrLngIsoCode=#tmpSbj.LNG_ISO_CODE 

	WHERE NLT_DATE >= @DateFrom
		AND NLT_DATE <= @DateTo
END
