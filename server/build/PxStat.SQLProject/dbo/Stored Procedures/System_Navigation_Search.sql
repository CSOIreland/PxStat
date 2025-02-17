
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 19/01/2021
-- Description:	Search Release Keywords based on a table of supplied terms. All other parameters are optional.
-- For keywords, the results of the search are prioritised by assigning a score multiplier depending on which table the match occurs
-- exec System_Navigation_Search 'en'
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Search @LngIsoCode CHAR(2)
	,@Search KeyValueVarcharAttribute Readonly
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MTR_CODE AS MtrCode
		,[Value] sValue
		,[key] sKey
		,Attribute
		,KRL_VALUE AS FoundValue
		,'Krl' AS KwrSource
	FROM @Search
	INNER JOIN TD_KEYWORD_RELEASE ON [Value] = KRL_VALUE
	INNER JOIN TD_RELEASE ON RLS_ID = KRL_RLS_ID
		AND RLS_DELETE_FLAG = 0
	INNER JOIN TD_MATRIX ON MTR_RLS_ID = RLS_ID
		AND MTR_DELETE_FLAG = 0
	INNER JOIN VW_RELEASE_LIVE_NOW ON RLS_ID = VRN_RLS_ID
	AND MTR_ID=VRN_MTR_ID 
	
	UNION
	
	SELECT MTR_CODE AS MtrCode
		,[Value] sValue
		,[key] sKey
		,Attribute
		,KSB_VALUE AS FoundValue
		,'Ksb' AS KwrSource
	FROM @Search
	INNER JOIN TD_KEYWORD_SUBJECT ON [Value] = KSB_VALUE
	INNER JOIN TD_SUBJECT ON KSB_SBJ_ID = SBJ_ID
		AND SBJ_DELETE_FLAG = 0
	INNER JOIN TD_PRODUCT ON SBJ_ID = PRC_SBJ_ID
		AND PRC_DELETE_FLAG = 0
	INNER JOIN TD_RELEASE ON RLS_PRC_ID = PRC_ID
	
	
	INNER JOIN TD_MATRIX ON MTR_RLS_ID = RLS_ID
		AND MTR_DELETE_FLAG = 0
	INNER JOIN VW_RELEASE_LIVE_NOW ON RLS_ID = VRN_RLS_ID
	AND MTR_ID=VRN_MTR_ID 
	
	UNION
	
	SELECT MTR_CODE AS MtrCode
		,[Value] sValue
		,[key] sKey
		,Attribute
		,KPR_VALUE AS FoundValue
		,'Kpr' AS KwrSource
	FROM @Search
	INNER JOIN TD_KEYWORD_PRODUCT ON [Value] = KPR_VALUE
	INNER JOIN TD_PRODUCT ON PRC_ID = KPR_PRC_ID
		AND PRC_DELETE_FLAG = 0
	INNER JOIN TD_RELEASE ON RLS_PRC_ID = PRC_ID
	INNER JOIN TD_MATRIX ON MTR_RLS_ID = RLS_ID
		AND MTR_DELETE_FLAG = 0
	INNER JOIN VW_RELEASE_LIVE_NOW ON RLS_ID = VRN_RLS_ID
	AND MTR_ID=VRN_MTR_ID 
END
