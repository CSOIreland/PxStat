
CREATE
	

 PROCEDURE [dbo].[System_Navigation_Entity_Associated_Search] @MtrCode NVARCHAR(20) = NULL
	,@MtrOfficialFlag BIT = NULL
	,@ThmCode INT=NULL
	,@SbjCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
	,@CprCode NVARCHAR(32) = NULL
	,@RlsExceptionalFlag BIT = NULL
	,@RlsReservationFlag BIT = NULL
	,@RlsArchiveFlag BIT = NULL
	,@RlsAnalyticalFlag BIT = NULL
	,@RlsExperimentalFlag BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;



	SELECT DISTINCT MTR_CODE AS MtrCode
		,NULL AS sValue
		,NULL AS sKey
		,NULL AS Attribute
		,NULL AS FoundValue
		,'Entity' AS KwrSource
	FROM TD_RELEASE
	INNER JOIN (
		SELECT DISTINCT VRN_RLS_ID
		FROM VW_RELEASE_LIVE_NOW
		) AS LIVE_NOW ON RLS_ID = VRN_RLS_ID
		AND RLS_DELETE_FLAG = 0
	INNER JOIN TM_RELEASE_PRODUCT ON RPR_RLS_ID = RLS_ID
	AND RPR_DELETE_FLAG = 0
	INNER JOIN TD_PRODUCT ON RPR_PRC_ID=PRC_ID
	AND PRC_DELETE_FLAG=0
	INNER JOIN TD_SUBJECT ON PRC_SBJ_ID=SBJ_ID
	AND SBJ_DELETE_FLAG=0
	INNER JOIN TD_THEME ON SBJ_THM_ID=THM_ID
	AND THM_DELETE_FLAG=0
	INNER JOIN TD_MATRIX ON MTR_RLS_ID = RPR_RLS_ID		
		AND MTR_DELETE_FLAG = 0
	INNER JOIN TS_COPYRIGHT ON MTR_CPR_ID = CPR_ID
		AND CPR_DELETE_FLAG = 0
	WHERE (
			@MtrCode IS NULL
			OR @MtrCode = MTR_CODE
			)
		AND (@ThmCode IS NULL
			OR @ThmCode=THM_CODE)
		AND (@SbjCode IS NULL
			OR @SbjCode=SBJ_CODE)
		AND (@PrcCode IS NULL
			OR @PrcCode =PRC_CODE)
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
			@RlsExperimentalFlag IS NULL
			OR @RlsExperimentalFlag = RLS_EXPERIMENTAL_FLAG
			);
END
