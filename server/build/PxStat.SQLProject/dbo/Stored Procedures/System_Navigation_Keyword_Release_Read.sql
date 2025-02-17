
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 06/12/2018
-- Description:	Reads the Keyword Release entities
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Keyword_Release_Read @RlsCode INT = NULL
	,@KrlCode INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT krl.KRL_CODE AS KrlCode
		,krl.KRL_VALUE AS KrlValue
		,krl.KRL_MANDATORY_FLAG AS KrlMandatoryFlag
		,rls.RLS_CODE AS RlsCode
		,krl.KRL_SINGULARISED_FLAG as KrlSingularisedFlag
	FROM TD_KEYWORD_RELEASE krl
	INNER JOIN TD_RELEASE rls
		ON rls.RLS_ID = krl.KRL_RLS_ID
			AND rls.RLS_DELETE_FLAG = 0
	WHERE (
			@RlsCode IS NULL
			OR @RlsCode = rls.RLS_CODE
			)
		AND (
			@KrlCode IS NULL
			OR @KrlCode = krl.KRL_CODE
			)
END
