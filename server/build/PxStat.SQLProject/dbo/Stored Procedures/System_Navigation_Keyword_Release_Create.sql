
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 30/11/2018
-- Description:	Creates a Keyword Release
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Keyword_Release_Create @KrlValue NVARCHAR(256)
	,@KrlMandatoryFlag BIT
	,@RlsCode INT
	,@KrlSingularisedFlag BIT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @eMessage VARCHAR(256)
	DECLARE @RlsID INT

	SET @RlsID = (
			SELECT rls.RLS_ID
			FROM TD_RELEASE rls
			WHERE rls.RLS_CODE = @RlsCode
			)

	IF @RlsID IS NULL
		OR @RlsID = 0
	BEGIN
		SET @eMessage = 'Error in procedure ' + (
				SELECT OBJECT_NAME(@@PROCID)
				) + ' - No release found for RlsCode ' + cast(isnull(@RlsCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN
	END

	--Prevent duplicate values in the Keyword Release table
	DECLARE @KrlValueCount INT

	SET @KrlValueCount = (
			SELECT COUNT(*)
			FROM TD_KEYWORD_RELEASE
			WHERE KRL_RLS_ID = @RlsID
				AND KRL_VALUE = @KrlValue
			)

	IF @KrlValueCount > 0
	BEGIN
		RETURN - 1
	END

	INSERT INTO TD_KEYWORD_RELEASE (
		KRL_VALUE
		,KRL_MANDATORY_FLAG
		,KRL_RLS_ID
		,KRL_SINGULARISED_FLAG
		)
	VALUES (
		@KrlValue
		,@KrlMandatoryFlag
		,@RlsID
		,@KrlSingularisedFlag
		)

	RETURN @@Rowcount
END
