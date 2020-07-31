SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 04/12/2018
-- Description:	Search Release Keywords based on a table of supplied terms. All other parameters are optional.
-- For keywords, the results of the search are prioritised by assigning a score multiplier depending on which table the match occurs
-- exec System_Navigation_Search 'en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Search @LngIsoCode CHAR(2)
	,@Search KeyValueVarcharAttribute Readonly
	,@SearchTermCount INT
	,@MtrCode NVARCHAR(20) = NULL
	,@MtrOfficialFlag BIT = NULL
	,@SbjCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
	,@CprCode NVARCHAR(32) = NULL
	,@RlsExceptionalFlag BIT = NULL
	,@RlsReservationFlag BIT = NULL
	,@RlsArchiveFlag BIT = NULL
	,@RlsAnalyticalFlag BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MULTIPLIER_PRIORITY_1 INT
	DECLARE @MULTIPLIER_PRIORITY_2 INT
	DECLARE @MULTIPLIER_PRIORITY_3 INT

	SET @MULTIPLIER_PRIORITY_1 = 100
	SET @MULTIPLIER_PRIORITY_2 = 10
	SET @MULTIPLIER_PRIORITY_3 = 1

	DECLARE @LngID INT
	--Check if an entity search term has been passed
	DECLARE @EntitySearchNotNull BIT

	IF @MtrCode IS NULL
		AND @MtrOfficialFlag IS NULL
		AND @SbjCode IS NULL
		AND @PrcCode IS NULL
		AND @CprCode IS NULL
		AND @RlsExceptionalFlag IS NULL
		AND @RlsReservationFlag IS NULL
		AND @RlsArchiveFlag IS NULL
		AND @RlsAnalyticalFlag IS NULL
	BEGIN
		SET @EntitySearchNotNull = 0
	END
	ELSE
	BEGIN
		SET @EntitySearchNotNull = 1
	END

	--Check if we are to have an empty search, i.e. return nothing
	DECLARE @EmptySearch BIT

	--If no valid search terms have been passed then we will return nothing
	IF @EntitySearchNotNull = 0
		AND @SearchTermCount = 0
	BEGIN
		RETURN
	END

	SET @LngID = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngID IS NULL
		OR @LngID = 0
	BEGIN
		RETURN
	END

	--We must get a version of the Product table that is language invariant
	SELECT PRC_ID
		,PRC_CODE
		,PRC_SBJ_ID
		,COALESCE(PLG_VALUE, PRC_VALUE) AS PRC_VALUE
		,COALESCE(PLG_LNG_ID, PRC_LNG_ID) AS PRC_LNG_ID
	INTO #Product
	FROM TD_PRODUCT
	LEFT JOIN TD_PRODUCT_LANGUAGE
		ON PRC_ID = PLG_PRC_ID
			AND PRC_DELETE_FLAG = 0
			AND PLG_LNG_ID = @LngID

	--We must get a version of the Subject table that is language invariant
	SELECT SBJ_ID
		,SBJ_CODE
		,COALESCE(SLG_VALUE, SBJ_VALUE) AS SBJ_VALUE
	INTO #Subject
	FROM TD_SUBJECT
	LEFT JOIN TD_SUBJECT_LANGUAGE
		ON SBJ_ID = SLG_SBJ_ID
			AND SBJ_DELETE_FLAG = 0
			AND SLG_LNG_ID = @LngID

	DECLARE @synSearch TABLE ([Value] NVARCHAR(256))

	INSERT INTO @synSearch
	SELECT DISTINCT [value]
	FROM @Search

	--Now we must get a list of matrixes that correspond to the 
	-- keyword search terms as a temp table
	-- This creates the temp table for Release Keywords
	SELECT q2.RlsId AS RlsId
		,sum(q2.rcount) * @MULTIPLIER_PRIORITY_1  AS score
	INTO #kwRelease
	FROM (
		SELECT q.RlsId
			,[value]
			,KRL_SINGULARISED_FLAG
			,sum(q.[Priority]) AS rcount
		FROM (
			SELECT [key]
				,[value]
				,krl.KRL_RLS_ID AS RlsId
				,KRL_SINGULARISED_FLAG
				,Attribute as [Priority]
			FROM TD_KEYWORD_RELEASE krl
			INNER JOIN @Search
				ON [key] = krl.KRL_VALUE
			INNER JOIN td_release
				ON RLS_ID = KRL_RLS_ID
			INNER JOIN VW_RELEASE_LIVE_NOW
				ON KRL_RLS_ID = VRN_RLS_ID
			) q
		GROUP BY RlsId
			,[value]
			,KRL_SINGULARISED_FLAG
		) q2
	GROUP BY RlsId
		,KRL_SINGULARISED_FLAG
	HAVING count(*) = @SearchTermCount
		OR KRL_SINGULARISED_FLAG = 0

	-- This creates the temp table for Product Keywords
	SELECT q2.RlsId AS RlsId
		,KPR_SINGULARISED_FLAG
		,sum(rcount) * @MULTIPLIER_PRIORITY_2 AS score
	INTO #kwProduct
	FROM (
		SELECT q.RlsId
			,[value]
			,KPR_SINGULARISED_FLAG
			,sum([Priority]) AS rcount
		FROM (
			SELECT [key]
				,[value]
				,RLS_ID AS RlsId
				,KPR_SINGULARISED_FLAG
				,Attribute as [Priority]
			FROM TD_KEYWORD_PRODUCT kpr
			INNER JOIN @Search
				ON [key] = kpr.KPR_VALUE
			INNER JOIN #Product
				ON kpr.KPR_PRC_ID = PRC_ID
			INNER JOIN td_release
				ON RLS_PRC_ID = PRC_ID
			INNER JOIN VW_RELEASE_LIVE_NOW
				ON RLS_ID = VRN_RLS_ID
			) q
		GROUP BY RlsId
			,[value]
			,KPR_SINGULARISED_FLAG
		) q2
	GROUP BY RlsId
		,KPR_SINGULARISED_FLAG
	HAVING count(*) = @SearchTermCount
		OR KPR_SINGULARISED_FLAG = 0

	-- This creates the temp table for Subject Keywords
	SELECT q2.RlsId AS RlsId
		,sum(rcount) * @MULTIPLIER_PRIORITY_3 AS score
		,KSB_SINGULARISED_FLAG
	INTO #kwSubject
	FROM (
		SELECT q.RlsId
			,[value]
			,KSB_SINGULARISED_FLAG
			,sum([Priority]) AS rcount
		FROM (
			SELECT [key]
				,[value]
				,RLS_ID AS RlsId
				,KSB_SINGULARISED_FLAG
				,Attribute as [Priority]
			FROM TD_KEYWORD_SUBJECT ksb
			INNER JOIN @Search
				ON [key] = ksb.KSB_VALUE
			INNER JOIN TD_SUBJECT
				ON SBJ_ID = ksb.KSB_SBJ_ID
			INNER JOIN TD_PRODUCT
				ON PRC_SBJ_ID = SBJ_ID
			INNER JOIN td_release
				ON PRC_ID = RLS_PRC_ID
			INNER JOIN VW_RELEASE_LIVE_NOW
				ON RLS_ID = VRN_RLS_ID
			) q
		GROUP BY RlsId
			,[value]
			,KSB_SINGULARISED_FLAG
		) q2
	GROUP BY RlsId
		,KSB_SINGULARISED_FLAG
	HAVING count(*) = 1
		OR KSB_SINGULARISED_FLAG = 0

	--This is the Keyword search
	-- The results are placed into the temporary table #KeywordsSearch
	INSERT INTO @synSearch
	SELECT DISTINCT [value]
	FROM @Search

	--This is the Keyword search
	-- The results are placed into the temporary table #KeywordsSearch
	SELECT RlsCode
		,MtrCode
		,MtrTitle
		,MtrOfficialFlag
		,SbjCode
		,SbjValue
		,PrcCode
		,PrcValue
		,RlsLiveDatetimeFrom
		,RlsExceptionalFlag
		,RlsReservationFlag
		,RlsArchiveFlag
		,RlsAnalyticalFlag
		,RlsDependencyFlag
		,CprCode
		,CprValue
		,ClsCode
		,ClsId
		,ClsValue
		,ClsGeoFlag
		,ClsGeoUrl
		,FrqCode
		,FrqValue
		,PrdValue
		,sum(score) AS Score
		,MtrLngID
	INTO #KeywordsSearch
	FROM (
		-- First we search the Keyword Release table
		SELECT DISTINCT RLS_CODE RlsCode
			,score
			,MTR_CODE AS MtrCode
			,MTR_TITLE AS MtrTitle
			,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
			,SBJ_CODE AS SbjCode
			,SBJ_VALUE AS SbjValue
			,PRC_CODE AS PrcCode
			,PRC_VALUE AS PrcValue
			,RLS_LIVE_DATETIME_FROM RlsLiveDatetimeFrom
			,RLS_EXCEPTIONAL_FLAG RlsExceptionalFlag
			,RLS_RESERVATION_FLAG RlsReservationFlag
			,RLS_ARCHIVE_FLAG RlsArchiveFlag
			,RLS_ANALYTICAL_FLAG RlsAnalyticalFlag
			,RLS_DEPENDENCY_FLAG RlsDependencyFlag
			,CPR_CODE AS CprCode
			,CPR_VALUE AS CprValue
			,CLS_CODE AS ClsCode
			,CLS_ID AS ClsId
			,CLS_VALUE AS ClsValue
			,CLS_GEO_FLAG AS ClsGeoFlag
			,CLS_GEO_URL AS ClsGeoUrl
			,FRQ_CODE AS FrqCode
			,FRQ_VALUE AS FrqValue
			,PRD_VALUE AS PrdValue
			,MTR_LNG_ID AS MtrLngID
		FROM #kwRelease
		INNER JOIN TD_Release
			ON #kwRelease.RlsId = RLS_ID
		INNER JOIN TD_MATRIX
			ON MTR_RLS_ID = RlsID
				AND MTR_DELETE_FLAG = 0
		LEFT JOIN #Product
			ON #Product.PRC_ID = RLS_PRC_ID
		LEFT JOIN #Subject
			ON #Product.PRC_SBJ_ID = SBJ_ID
		INNER JOIN TS_COPYRIGHT
			ON MTR_CPR_ID = CPR_ID
				AND CPR_DELETE_FLAG = 0
		LEFT JOIN TD_CLASSIFICATION
			ON MTR_ID = CLS_MTR_ID
		LEFT JOIN TD_FREQUENCY
			ON MTR_ID = FRQ_MTR_ID
		LEFT JOIN TD_PERIOD
			ON FRQ_ID = PRD_FRQ_ID
		
		UNION
		
		--We union to the Keyword Product table
		SELECT RLS_CODE RlsCode
			,score
			,MTR_CODE AS MtrCode
			,MTR_TITLE AS MtrTitle
			,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
			,SBJ_CODE AS SbjCode
			,SBJ_VALUE AS SbjValue
			,PRC_CODE AS PrcCode
			,PRC_VALUE AS PrcValue
			,RLS_LIVE_DATETIME_FROM RlsLiveDatetimeFrom
			,RLS_EXCEPTIONAL_FLAG RlsExceptionalFlag
			,RLS_RESERVATION_FLAG RlsReservationFlag
			,RLS_ARCHIVE_FLAG RlsArchiveFlag
			,RLS_ANALYTICAL_FLAG RlsAnalyticalFlag
			,RLS_DEPENDENCY_FLAG RlsDependencyFlag
			,CPR_CODE AS CprCode
			,CPR_VALUE AS CprValue
			,CLS_CODE AS ClsCode
			,CLS_ID AS ClsId
			,CLS_VALUE AS ClsValue
			,CLS_GEO_FLAG AS ClsGeoFlag
			,CLS_GEO_URL AS ClsGeoUrl
			,FRQ_CODE AS FrqCode
			,FRQ_VALUE AS FrqValue
			,PRD_VALUE AS PrdValue
			,MTR_LNG_ID AS MtrLngID
		FROM #kwProduct
		INNER JOIN TD_RELEASE
			ON RlsId = RLS_ID
		INNER JOIN TD_MATRIX
			ON MTR_RLS_ID = RlsID
				AND MTR_DELETE_FLAG = 0
		LEFT JOIN #Product
			ON #Product.PRC_ID = RLS_PRC_ID
		LEFT JOIN #Subject
			ON #Product.PRC_SBJ_ID = SBJ_ID
		INNER JOIN TS_COPYRIGHT
			ON MTR_CPR_ID = CPR_ID
				AND CPR_DELETE_FLAG = 0
		LEFT JOIN TD_CLASSIFICATION
			ON MTR_ID = CLS_MTR_ID
		LEFT JOIN TD_FREQUENCY
			ON MTR_ID = FRQ_MTR_ID
		LEFT JOIN TD_PERIOD
			ON FRQ_ID = PRD_FRQ_ID
		
		UNION
		
		--Finally we union to the Keyword Subject table
		SELECT RLS_CODE RlsCode
			,score
			,MTR_CODE AS MtrCode
			,MTR_TITLE AS MtrTitle
			,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
			,SBJ_CODE AS SbjCode
			,SBJ_VALUE AS SbjValue
			,PRC_CODE AS PrcCode
			,PRC_VALUE AS PrcValue
			,RLS_LIVE_DATETIME_FROM RlsLiveDatetimeFrom
			,RLS_EXCEPTIONAL_FLAG RlsExceptionalFlag
			,RLS_RESERVATION_FLAG RlsReservationFlag
			,RLS_ARCHIVE_FLAG RlsArchiveFlag
			,RLS_ANALYTICAL_FLAG RlsAnalyticalFlag
			,RLS_DEPENDENCY_FLAG RlsDependencyFlag
			,CPR_CODE AS CprCode
			,CPR_VALUE AS CprValue
			,CLS_CODE AS ClsCode
			,CLS_ID AS ClsId
			,CLS_VALUE AS ClsValue
			,CLS_GEO_FLAG AS ClsGeoFlag
			,CLS_GEO_URL AS ClsGeoUrl
			,FRQ_CODE AS FrqCode
			,FRQ_VALUE AS FrqValue
			,PRD_VALUE AS PrdValue
			,MTR_LNG_ID AS MtrLngID
		FROM #kwSubject
		INNER JOIN TD_RELEASE
			ON RlsId = RLS_ID
		INNER JOIN TD_MATRIX
			ON MTR_RLS_ID = RlsID
				AND MTR_DELETE_FLAG = 0
		LEFT JOIN #Product
			ON #Product.PRC_ID = RLS_PRC_ID
		LEFT JOIN #Subject
			ON #Product.PRC_SBJ_ID = SBJ_ID
		INNER JOIN TS_COPYRIGHT
			ON MTR_CPR_ID = CPR_ID
				AND CPR_DELETE_FLAG = 0
		LEFT JOIN TD_CLASSIFICATION
			ON MTR_ID = CLS_MTR_ID
		LEFT JOIN TD_FREQUENCY
			ON MTR_ID = FRQ_MTR_ID
		LEFT JOIN TD_PERIOD
			ON FRQ_ID = PRD_FRQ_ID
		) searchQuery
	WHERE @SearchTermCount > 0
	GROUP BY RlsCode
		,MtrCode
		,MtrTitle
		,MtrOfficialFlag
		,SbjCode
		,SbjValue
		,PrcCode
		,PrcValue
		,RlsLiveDatetimeFrom
		,RlsExceptionalFlag
		,RlsReservationFlag
		,RlsArchiveFlag
		,RlsAnalyticalFlag
		,RlsDependencyFlag
		,CprCode
		,CprValue
		,ClsCode
		,ClsId
		,ClsValue
		,ClsGeoFlag
		,ClsGeoUrl
		,FrqCode
		,FrqValue
		,PrdValue
		,MtrLngID

	--This is the entity search
	-- The results are placed into the temporary table #EntitySearch
	SELECT RLS_CODE AS RlsCode
		,MTR_CODE AS MtrCode
		,MTR_TITLE AS MtrTitle
		,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
		,SBJ_CODE AS SbjCode
		,SBJ_VALUE AS SbjValue
		,PRC_CODE AS PrcCode
		,PRC_VALUE AS PrcValue
		,RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
		,RLS_EXCEPTIONAL_FLAG AS RlsExceptionalFlag
		,RLS_RESERVATION_FLAG AS RlsReservationFlag
		,RLS_ARCHIVE_FLAG AS RlsArchiveFlag
		,RLS_ANALYTICAL_FLAG AS RlsAnalyticalFlag
		,RLS_DEPENDENCY_FLAG AS RlsDependencyFlag
		,CPR_CODE AS CprCode
		,CPR_VALUE AS CprValue
		,CLS_CODE AS ClsCode
		,CLS_ID AS ClsId
		,CLS_VALUE AS ClsValue
		,CLS_GEO_FLAG AS ClsGeoFlag
		,CLS_GEO_URL AS ClsGeoUrl
		,FRQ_CODE AS FrqCode
		,FRQ_VALUE AS FrqValue
		,PRD_VALUE AS PrdValue
		,0 AS Score
		,-- This is zero because if we get anything in search it overrides any keywords and all results are equally important
		MTR_LNG_ID AS MtrLngID
	INTO #EntitySearch
	FROM TD_RELEASE
	INNER JOIN (
		SELECT DISTINCT VRN_RLS_ID
		FROM VW_RELEASE_LIVE_NOW
		) LIVE_NOW
		ON RLS_ID = VRN_RLS_ID
	LEFT JOIN #Product
		ON RLS_PRC_ID = PRC_ID
	LEFT JOIN #Subject
		ON #Product.PRC_SBJ_ID = SBJ_ID
	INNER JOIN TD_MATRIX
		ON MTR_RLS_ID = RLS_ID
			AND RLS_DELETE_FLAG = 0
			AND MTR_DELETE_FLAG = 0
	INNER JOIN TS_COPYRIGHT
		ON MTR_CPR_ID = CPR_ID
			AND CPR_DELETE_FLAG = 0
	LEFT JOIN TD_CLASSIFICATION
		ON MTR_ID = CLS_MTR_ID
	LEFT JOIN TD_FREQUENCY
		ON MTR_ID = FRQ_MTR_ID
	LEFT JOIN TD_PERIOD
		ON FRQ_ID = PRD_FRQ_ID
	WHERE (
			@PrcCode IS NULL
			OR @PrcCode = PRC_CODE
			)
		AND (
			@SbjCode IS NULL
			OR @SbjCode = SBJ_CODE
			)
		AND (
			@MtrCode IS NULL
			OR @MtrCode = MTR_CODE
			)
		AND (
			@MtrOfficialFlag IS NULL
			OR @MtrOfficialFlag = MTR_OFFICIAL_FLAG
			)
		AND (
			@CprCode IS NULL
			OR @CprCode = CPR_CODE
			)
		AND (
			@RlsExceptionalFlag IS NULL
			OR @RlsExceptionalFlag = RLS_EXCEPTIONAL_FLAG
			)
		AND (
			@RlsReservationFlag IS NULL
			OR @RlsReservationFlag = RLS_RESERVATION_FLAG
			)
		AND (
			@RlsArchiveFlag IS NULL
			OR @RlsArchiveFlag = RLS_ARCHIVE_FLAG
			)
		AND (
			@RlsAnalyticalFlag IS NULL
			OR @RlsAnalyticalFlag = RLS_ANALYTICAL_FLAG
			)
		AND (
			@EntitySearchNotNull = 1
			OR @EmptySearch = 1
			)

	IF @EntitySearchNotNull = 0
		AND @SearchTermCount > 0
	BEGIN
		-- No entities were supplied as parameters but a search term was supplied
		SELECT #KeywordsSearch.*
			,LNG_ISO_CODE AS LngIsoCode
			,LNG_ISO_NAME AS LngIsoName
		FROM #KeywordsSearch
		INNER JOIN TS_LANGUAGE
			ON MtrLngID = LNG_ID
				AND LNG_DELETE_FLAG = 0
	END
	ELSE IF (
			@EntitySearchNotNull = 1
			AND @SearchTermCount = 0
			)
		OR @EmptySearch = 1
	BEGIN
		--At least one entity parameter was supplied but no search term (or else we requested an empty search)
		SELECT #EntitySearch.*
			,LNG_ISO_CODE AS LngIsoCode
			,LNG_ISO_NAME AS LngIsoName
		FROM #EntitySearch
		INNER JOIN TS_LANGUAGE
			ON MtrLngID = LNG_ID
				AND LNG_DELETE_FLAG = 0
	END
	ELSE IF @EntitySearchNotNull = 1
		AND @SearchTermCount > 0
	BEGIN
		-- At least one entity parameter was supplied and so was a search term
		SELECT final.*
			,#KeywordsSearch.Score
			,LNG_ISO_CODE AS LngIsoCode
			,LNG_ISO_NAME AS LngIsoName
		FROM (
			SELECT RlsCode
				,MtrCode
				,MtrTitle
				,MtrOfficialFlag
				,SbjCode
				,SbjValue
				,PrcCode
				,PrcValue
				,RlsLiveDatetimeFrom
				,RlsExceptionalFlag
				,RlsReservationFlag
				,RlsArchiveFlag
				,RlsAnalyticalFlag
				,RlsDependencyFlag
				,CprCode
				,CprValue
				,ClsCode
				,ClsId
				,ClsValue
				,ClsGeoFlag
				,ClsGeoUrl
				,FrqCode
				,FrqValue
				,PrdValue
				,MtrLngID
			FROM #KeywordsSearch
			
			INTERSECT
			
			SELECT RlsCode
				,MtrCode
				,MtrTitle
				,MtrOfficialFlag
				,SbjCode
				,SbjValue
				,PrcCode
				,PrcValue
				,RlsLiveDatetimeFrom
				,RlsExceptionalFlag
				,RlsReservationFlag
				,RlsArchiveFlag
				,RlsAnalyticalFlag
				,RlsDependencyFlag
				,CprCode
				,ClsId
				,CprValue
				,ClsCode
				,ClsValue
				,ClsGeoFlag
				,ClsGeoUrl
				,FrqCode
				,FrqValue
				,PrdValue
				,MtrLngID
			FROM #EntitySearch
			) final
		INNER JOIN TS_LANGUAGE
			ON MtrLngID = LNG_ID
				AND LNG_DELETE_FLAG = 0
		LEFT OUTER JOIN #KeywordsSearch
			ON final.RlsCode = #KeywordsSearch.RlsCode
		ORDER BY Score DESC
			,MtrCode ASC
	END
END
GO


