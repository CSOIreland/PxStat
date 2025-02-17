
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/03/2021
-- Description:	Inserts a new record into the TD_THEME table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Theme_Create @ThmValue NVARCHAR(256)
	,@userName NVARCHAR(256)
	,@LngIsoCode CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgId INT = NULL
	DECLARE @lngID INT = NULL
	DECLARE @errorMessage VARCHAR(256)

	EXEC @DtgId = Security_Auditing_Create @userName;

	IF @DtgId IS NULL
		OR @DtgId = 0
	BEGIN
		RAISERROR (
				'SP: Security_Auditing_Create has failed!'
				,16
				,1
				)

		RETURN 0
	END

	SET @lngID = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @lngId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - language not found: ' + cast(isnull(@LngIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	INSERT INTO TD_THEME (
		THM_CODE
		,THM_VALUE
		,THM_LNG_ID
		,THM_DTG_ID
		,THM_DELETE_FLAG
		)
	VALUES (
		DEFAULT
		,@ThmValue
		,@lngID
		,@DtgId
		,0
		)

	RETURN @@identity
END
