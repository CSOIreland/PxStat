SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 09/12/2021
-- Cleanup procedure for TD_KEYWORD_RELEASE. Eliminates any duplicate entries for a release (while retaining a single entry) .
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Keyword_Release_EliminateDupes
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE
	FROM TD_KEYWORD_RELEASE
	WHERE KRL_ID NOT IN (
			SELECT max(krl_id) AS maxId
			FROM [TD_KEYWORD_RELEASE]
			INNER JOIN td_release ON rls_id = KRL_RLS_ID
				AND RLS_DELETE_FLAG = 0
			INNER JOIN td_matrix ON MTR_RLS_ID = RLS_ID
				AND MTR_DELETE_FLAG = 0
			GROUP BY [KRL_VALUE]
				,[KRL_RLS_ID]
				,[KRL_MANDATORY_FLAG]
				,[KRL_SINGULARISED_FLAG]
			)
END
GO


