
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 11/12/2018
-- Description:	Updates a Keyword Release
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Keyword_Release_Update @KrlCode INT
	,@KrlValue NVARCHAR(256)
	,@KrlSingularisedFlag BIT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @KrlRlsID INT

	SET @KrlRlsID = (
			SELECT KRL_RLS_ID
			FROM TD_KEYWORD_RELEASE
			WHERE KRL_CODE = @KrlCode
			)

	DECLARE @KrlValueCount INT

	SET @KrlValueCount = (
			SELECT COUNT(*)
			FROM TD_KEYWORD_RELEASE
			WHERE KRL_RLS_ID = @KrlRlsID
				AND KRL_VALUE = @KrlValue
				AND KRL_CODE <> @KrlCode
			)

	IF @KrlValueCount > 0
	BEGIN
		RETURN - 1
	END

	UPDATE TD_KEYWORD_RELEASE
	SET KRL_VALUE = @KrlValue
		,KRL_SINGULARISED_FLAG = @KrlSingularisedFlag
	WHERE KRL_CODE = @KrlCode

	RETURN @@rowcount
END
