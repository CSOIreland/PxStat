
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 11 Oct 2018
-- Description:	Inserts a new record into the TD_Product table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Product_Create @PrcValue NVARCHAR(256)
	,@SbjCode INT
	,@PrcCode NVARCHAR(32)
	,@userName NVARCHAR(256)
	,@LngIsoCode CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	-- Error Message
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @subjectId INT = NULL
	DECLARE @auditId INT = NULL
	DECLARE @lngId INT = NULL

	SELECT @subjectId = s.SBJ_ID
	FROM TD_SUBJECT s
	WHERE s.SBJ_CODE = @SbjCode

	IF @subjectId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - subject not found: ' + cast(isnull(@SbjCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	SET @lngId = (
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

	EXEC @auditId = Security_Auditing_Create @userName;

	IF @auditId IS NULL
		OR @auditId = 0
	BEGIN
		RAISERROR (
				'SP: Security_Auditing_Create has failed!'
				,16
				,1
				)

		RETURN 0
	END

	INSERT INTO [dbo].[TD_PRODUCT] (
		[PRC_CODE]
		,[PRC_VALUE]
		,[PRC_SBJ_ID]
		,[PRC_LNG_ID]
		,[PRC_DTG_ID]
		,[PRC_DELETE_FLAG]
		)
	VALUES (
		@PrcCode
		,@PrcValue
		,@subjectId
		,@lngId
		,@auditId
		,0
		)

	RETURN @@identity
END
