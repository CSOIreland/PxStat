
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 24 Oct 2018
-- Description:	Inserts a new record into the TD_RELEASE table
-- exec Data_Stat_Release_DeleteKeywords 118,'okeeffene'
-- =============================================
CREATE
	

 PROCEDURE Data_Release_DeleteKeywords @RlsCode INT
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	-- Error Message
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @spName VARCHAR(100)

	SET @spName = 'Data_Stat_Release_DeleteKeywords'

	-- Release lookup
	DECLARE @RlsId INT = NULL

	SELECT @RlsId = [RLS_ID]
	FROM [TD_RELEASE]
	WHERE [RLS_DELETE_FLAG] = 0
		AND [RLS_CODE] = @RlsCode

	IF @RlsId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - Release not found: ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	DELETE TD_KEYWORD_RELEASE
	WHERE KRL_RLS_ID = @RlsId
		AND KRL_MANDATORY_FLAG = 1

	RETURN 0
END
