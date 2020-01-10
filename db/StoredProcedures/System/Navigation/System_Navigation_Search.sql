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
	,@Search KeyValueVarchar Readonly
	,@SearchTermCount INT
	,@MtrCode NVARCHAR(20) = NULL
	,@MtrOfficialFlag BIT = NULL
	,@SbjCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
	,@CprCode NVARCHAR(32) = NULL
	,@RlsEmergencyFlag BIT = NULL
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
		AND @RlsEmergencyFlag IS NULL
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

	--Check if we are to have an empty search, i.e. return everything
	DECLARE @EmptySearch BIT

	--If no valid search terms have been passed then we assume we want to return everything
	IF @EntitySearchNotNull = 0
		AND @SearchTermCount = 0
	BEGIN
		SET @EmptySearch = 1
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
		,RlsEmergencyFlag
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
		,sum(searchQuery.score) AS Score
		,MtrLngID
	INTO #KeywordsSearch
	FROM (
		-- First we search the Keyword Release table
		SELECT RlsCode
			,score
			,MTR_CODE AS MtrCode
			,MTR_TITLE AS MtrTitle
			,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
			,SBJ_CODE AS SbjCode
			,SBJ_VALUE AS SbjValue
			,PRC_CODE AS PrcCode
			,PRC_VALUE AS PrcValue
			,RlsLiveDatetimeFrom
			,RlsEmergencyFlag
			,RlsReservationFlag
			,RlsArchiveFlag
			,RlsAnalyticalFlag
			,RlsDependencyFlag
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
		FROM (
			SELECT rls.RLS_CODE AS RlsCode
				,rls.RLS_ID AS RlsID
				,rls.RLS_PRC_ID AS PrcID
				,RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
				,RLS_EMERGENCY_FLAG AS RlsEmergencyFlag
				,RLS_RESERVATION_FLAG AS RlsReservationFlag
				,RLS_ARCHIVE_FLAG AS RlsArchiveFlag
				,RLS_ANALYTICAL_FLAG AS RlsAnalyticalFlag
				,RLS_DEPENDENCY_FLAG AS RlsDependencyFlag
				,count(*) * @MULTIPLIER_PRIORITY_1 AS score
			FROM TD_KEYWORD_RELEASE krl
			INNER JOIN @synSearch st
				ON krl.KRL_VALUE = st.[value]
					OR krl.KRL_VALUE IN (
						SELECT [key]
						FROM @Search
						WHERE [Value] = st.[Value]
						)
			INNER JOIN TD_RELEASE rls
				ON krl.KRL_RLS_ID = rls.RLS_ID
					AND rls.RLS_DELETE_FLAG = 0
			WHERE RLS_ID IN (
					SELECT DISTINCT VRN_RLS_ID
					FROM VW_RELEASE_LIVE_NOW
					)
			GROUP BY rls.RLS_CODE
				,rls.RLS_ID
				,rls.RLS_PRC_ID
				,rls.RLS_LIVE_DATETIME_FROM
				,rls.RLS_EMERGENCY_FLAG
				,rls.RLS_RESERVATION_FLAG
				,rls.RLS_ARCHIVE_FLAG
				,rls.RLS_ANALYTICAL_FLAG
				,rls.RLS_DEPENDENCY_FLAG
				,krl.KRL_SINGULARISED_FLAG
			HAVING (
					count(*) = @SearchTermCount
					OR krl.KRL_SINGULARISED_FLAG = 0
					)
			) releaseQuery
		INNER JOIN TD_MATRIX
			ON MTR_RLS_ID = RlsID
				AND MTR_DELETE_FLAG = 0
		LEFT JOIN #Product
			ON PrcID = PRC_ID
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
		SELECT RlsCode
			,score
			,MTR_CODE AS MtrCode
			,MTR_TITLE AS MtrTitle
			,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
			,SBJ_CODE AS SbjCode
			,SBJ_VALUE AS SbjValue
			,PRC_CODE AS PrcCode
			,PRC_VALUE AS PrcValue
			,RlsLiveDatetimeFrom
			,RlsEmergencyFlag
			,RlsReservationFlag
			,RlsArchiveFlag
			,RlsAnalyticalFlag
			,RlsDependencyFlag
			,CPR_CODE AS CprCode
			,CPR_VALUE AS CprValue
			,CLS_CODE AS ClsCode
			,CLS_ID As ClsId
			,CLS_VALUE AS ClsValue
			,CLS_GEO_FLAG AS ClsGeoFlag
			,CLS_GEO_URL AS ClsGeoUrl
			,FRQ_CODE AS FrqCode
			,FRQ_VALUE AS FrqValue
			,PRD_VALUE AS PrdValue
			,MTR_LNG_ID AS MtrLngID
		FROM (
			SELECT rls.RLS_CODE AS RlsCode
				,rls.RLS_ID AS RlsID
				,rls.RLS_PRC_ID AS PrcID
				,RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
				,RLS_EMERGENCY_FLAG AS RlsEmergencyFlag
				,RLS_RESERVATION_FLAG AS RlsReservationFlag
				,RLS_ARCHIVE_FLAG AS RlsArchiveFlag
				,RLS_ANALYTICAL_FLAG AS RlsAnalyticalFlag
				,RLS_DEPENDENCY_FLAG AS RlsDependencyFlag
				,KPR_SINGULARISED_FLAG AS KprSingularisedFlag
				,count(*) * @MULTIPLIER_PRIORITY_2 AS score
			FROM TD_KEYWORD_PRODUCT kpr
			INNER JOIN @synSearch st
				ON kpr.KPR_VALUE = st.[value]
					OR KPR_VALUE IN (
						SELECT [key]
						FROM @Search
						WHERE [Value] = st.[Value]
						)
			INNER JOIN TD_PRODUCT prc
				ON kpr.KPR_PRC_ID = prc.PRC_ID
					AND prc.PRC_DELETE_FLAG = 0
			INNER JOIN TD_RELEASE rls
				ON prc.PRC_ID = rls.RLS_PRC_ID
					AND prc.PRC_DELETE_FLAG = 0
					AND rls.RLS_DELETE_FLAG = 0
			WHERE RLS_ID IN (
					SELECT DISTINCT VRN_RLS_ID
					FROM VW_RELEASE_LIVE_NOW
					)
			GROUP BY rls.RLS_CODE
				,rls.RLS_ID
				,rls.RLS_PRC_ID
				,rls.RLS_LIVE_DATETIME_FROM
				,rls.RLS_EMERGENCY_FLAG
				,rls.RLS_RESERVATION_FLAG
				,rls.RLS_ARCHIVE_FLAG
				,rls.RLS_ANALYTICAL_FLAG
				,rls.RLS_DEPENDENCY_FLAG
				,KPR_SINGULARISED_FLAG
			HAVING (
					count(*) = @SearchTermCount
					OR KPR_SINGULARISED_FLAG = 0
					)
			) productQuery
		INNER JOIN TD_MATRIX
			ON MTR_RLS_ID = RlsID
				AND MTR_DELETE_FLAG = 0
		LEFT JOIN #Product
			ON PrcID = PRC_ID
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
		SELECT RlsCode
			,score
			,MTR_CODE AS MtrCode
			,MTR_TITLE AS MtrTitle
			,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
			,SBJ_CODE AS SbjCode
			,SBJ_VALUE AS SbjValue
			,PRC_CODE AS PrcCode
			,PRC_VALUE AS PrcValue
			,RlsLiveDatetimeFrom
			,RlsEmergencyFlag
			,RlsReservationFlag
			,RlsArchiveFlag
			,RlsAnalyticalFlag
			,RlsDependencyFlag
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
		FROM (
			SELECT rls.RLS_CODE AS RlsCode
				,rls.RLS_ID AS RlsID
				,rls.RLS_PRC_ID AS PrcID
				,RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
				,RLS_EMERGENCY_FLAG AS RlsEmergencyFlag
				,RLS_RESERVATION_FLAG AS RlsReservationFlag
				,RLS_ARCHIVE_FLAG AS RlsArchiveFlag
				,RLS_ANALYTICAL_FLAG AS RlsAnalyticalFlag
				,RLS_DEPENDENCY_FLAG AS RlsDependencyFlag
				,KSB_SINGULARISED_FLAG AS KsbSingularisedFlag
				,count(*) * @MULTIPLIER_PRIORITY_3 AS score
			FROM TD_KEYWORD_SUBJECT ksb
			INNER JOIN @synSearch st
				ON ksb.KSB_VALUE = st.[value]
					OR KSB_VALUE IN (
						SELECT [key]
						FROM @Search
						WHERE [Value] = st.[Value]
						)
			INNER JOIN TD_SUBJECT sbj
				ON ksb.KSB_SBJ_ID = sbj.SBJ_ID
					AND sbj.SBJ_DELETE_FLAG = 0
			INNER JOIN TD_PRODUCT prc
				ON sbj.SBJ_ID = prc.PRC_SBJ_ID
					AND prc.PRC_DELETE_FLAG = 0
			INNER JOIN TD_RELEASE rls
				ON prc.PRC_ID = rls.RLS_PRC_ID
					AND rls.RLS_DELETE_FLAG = 0
			WHERE RLS_ID IN (
					SELECT DISTINCT VRN_RLS_ID
					FROM VW_RELEASE_LIVE_NOW
					)
			GROUP BY rls.RLS_CODE
				,rls.RLS_ID
				,rls.RLS_PRC_ID
				,rls.RLS_LIVE_DATETIME_FROM
				,rls.RLS_EMERGENCY_FLAG
				,rls.RLS_RESERVATION_FLAG
				,rls.RLS_ARCHIVE_FLAG
				,rls.RLS_ANALYTICAL_FLAG
				,rls.RLS_DEPENDENCY_FLAG
				,KSB_SINGULARISED_FLAG
			HAVING (
					count(*) = @SearchTermCount
					OR KSB_SINGULARISED_FLAG = 0
					)
			) subjectQuery
		INNER JOIN TD_MATRIX
			ON MTR_RLS_ID = RlsID
				AND MTR_DELETE_FLAG = 0
		LEFT JOIN #Product
			ON PrcID = PRC_ID
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
		,RlsEmergencyFlag
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
		,RLS_EMERGENCY_FLAG AS RlsEmergencyFlag
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
			@RlsEmergencyFlag IS NULL
			OR @RlsEmergencyFlag = RLS_EMERGENCY_FLAG
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
				,RlsEmergencyFlag
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
				,RlsEmergencyFlag
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


