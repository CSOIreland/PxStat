
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 11 Oct 2018
-- Description:	Inserts a new record into the TD_Subject table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Subject_Create @SbjValue NVARCHAR(256)
	,@userName NVARCHAR(256)
	,@LngIsoCode CHAR(2)
	,@ThmCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgId INT = NULL
	DECLARE @lngID INT = NULL
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @ThmId INT = NULL

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

	set @ThmId=(SELECT THM_ID FROM TD_THEME WHERE THM_CODE=@ThmCode AND THM_DELETE_FLAG=0)

	IF @ThmId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - theme not found: ' + cast(isnull(@ThmCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	INSERT INTO [dbo].[TD_SUBJECT] (
		[SBJ_CODE]
		,[SBJ_VALUE]
		,[SBJ_LNG_ID]
		,[SBJ_DTG_ID]
		,[SBJ_DELETE_FLAG]
		,[SBJ_THM_ID] 
		)
	VALUES (
		DEFAULT
		,@SbjValue
		,@lngID
		,@DtgId
		,0
		,@ThmId
		)

	RETURN @@identity
END
