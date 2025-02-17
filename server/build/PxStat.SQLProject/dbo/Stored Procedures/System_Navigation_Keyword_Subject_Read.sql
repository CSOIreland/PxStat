
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 16 Oct 2018
-- Description:	Select a record on the TD_Keyword_Subject table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Keyword_Subject_Read @KsbCode INT = NULL
	,@SbjCode INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [KSB_CODE] AS KsbCode
		,[KSB_VALUE] AS KsbValue
		,[KSB_MANDATORY_FLAG] AS KsbMandatoryFlag
		,SBJ_CODE AS SbjCode
		,SBJ_VALUE AS SbjValue
		,KSB_SINGULARISED_FLAG as KsbSingularisedFlag
	FROM [TD_KEYWORD_SUBJECT]
	INNER JOIN TD_SUBJECT
		ON SBJ_ID = KSB_SBJ_ID
			AND SBJ_DELETE_FLAG = 0
	WHERE (
			(
				@KsbCode IS NOT NULL
				AND KSB_CODE = @KsbCode
				)
			OR @KsbCode IS NULL
			)
		AND (
			(
				@SbjCode IS NOT NULL
				AND SBJ_CODE = @SbjCode
				)
			OR @SbjCode IS NULL
			)
END
