SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 07/12/2018
-- Description:	Hard deletes Release Keywords for a given release or for a specific KeywordRelease code
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Keyword_Release_Delete @RlsCode INT = NULL
	,@KrlMandatoryFlag BIT = NULL
	,@KrlCode INT = NULL
AS
BEGIN
	SET NOCOUNT OFF;

	DECLARE @RlsID INT
	DECLARE @eMessage VARCHAR(256)

	IF @RlsCode IS NULL
		AND @KrlCode IS NULL
	BEGIN
		SET @eMessage = 'You must supply either an RlsCode or a KrlCode'

		RAISERROR (
				@eMessage
				,16
				,1
				);

		RETURN 0
	END

	IF @RlsCode IS NOT NULL
	BEGIN --we want to delete all keyword releases for a given RlsCode
		SET @RlsID = (
				SELECT rls.RLS_ID
				FROM TD_RELEASE rls
				WHERE rls.RLS_DELETE_FLAG = 0
					AND rls.RLS_CODE = @RlsCode
				)

		IF @RlsID IS NULL
			OR @RlsID = 0
		BEGIN
			RETURN 0
		END
	END

	DELETE
	FROM TD_KEYWORD_RELEASE
	WHERE (
			@RlsID IS NULL
			OR KRL_RLS_ID = @RlsID
			)
		AND (
			@KrlCode IS NULL
			OR @KrlCode = KRL_CODE
			)
		AND (
			@KrlMandatoryFlag IS NULL
			OR @KrlMandatoryFlag = KRL_MANDATORY_FLAG
			)

	RETURN @@rowcount
END
GO


