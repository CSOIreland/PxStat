
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 16 Oct 2018
-- Description:	Select a record on the TD_Keyword_Product table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Keyword_Product_Read @KprCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
	,@SbjCode INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [KPR_CODE] AS KprCode
		,[KPR_VALUE] AS KprValue
		,[KPR_MANDATORY_FLAG] AS KprMandatoryFlag
		,SBJ_CODE AS SbjCode
		,SBJ_VALUE AS SbjValue
		,KPR_SINGULARISED_FLAG AS KprSingularisedFlag
	FROM [TD_KEYWORD_PRODUCT]
	INNER JOIN TD_Product
		ON PRC_ID = KPR_PRC_ID
			AND PRC_DELETE_FLAG = 0
	INNER JOIN TD_SUBJECT
		ON PRC_SBJ_ID = SBJ_ID
			AND SBJ_DELETE_FLAG = 0
	WHERE (
			(
				@KprCode IS NOT NULL
				AND KPR_CODE = @KprCode
				)
			OR @KprCode IS NULL
			)
		AND (
			(
				@PrcCode IS NOT NULL
				AND PRC_CODE = @PrcCode
				)
			OR @PrcCode IS NULL
			)
		AND (
			(
				@SbjCode IS NOT NULL
				AND SBJ_CODE = @SbjCode
				)
			OR @SbjCode IS NULL
			)
END
