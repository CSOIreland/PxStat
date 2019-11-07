SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 11 Oct 2018
-- Description:	Inserts a new record into the TD_Subject table
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Subject_Create @SbjValue NVARCHAR(256)
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

	INSERT INTO [dbo].[TD_SUBJECT] (
		[SBJ_CODE]
		,[SBJ_VALUE]
		,[SBJ_LNG_ID]
		,[SBJ_DTG_ID]
		,[SBJ_DELETE_FLAG]
		)
	VALUES (
		DEFAULT
		,@SbjValue
		,@lngID
		,@DtgId
		,0
		)

	RETURN @@identity
END
GO


