SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/01/2019
-- Description:	Creates a new SubjectLanguage entry or just updates it if it exists already
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_SubjectLanguage_CreateOrUpdate @SbjCode INT
	,@SlgValue NVARCHAR(256)
	,@SlgIsoCode CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngIsoID INT
	DECLARE @SlgSbjID INT
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @recordCount INT

	SET @SlgSbjID = (
			SELECT SBJ_ID
			FROM TD_SUBJECT
			WHERE SBJ_CODE = @SbjCode
				AND SBJ_DELETE_FLAG = 0
			)

	IF @SlgSbjID IS NULL
		OR @SlgSbjID = 0
	BEGIN
		RETURN 0
	END

	SET @LngIsoID = (
			SELECT lng.LNG_ID
			FROM TS_LANGUAGE lng
			WHERE lng.LNG_ISO_CODE = @SlgIsoCode
				AND lng.LNG_DELETE_FLAG = 0
			)

	IF @LngIsoID IS NULL
		OR @LngIsoID = 0
	BEGIN
		SET @errorMessage = 'No ID found for Language Code code ' + cast(isnull(@SlgIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @recordCount = (
			SELECT count(*)
			FROM TD_SUBJECT_LANGUAGE
			WHERE SLG_LNG_ID = @LngIsoID
				AND SLG_SBJ_ID = @SlgSbjID
			)

	IF @recordCount = 0
	BEGIN
		INSERT INTO TD_SUBJECT_LANGUAGE (
			SLG_VALUE
			,SLG_LNG_ID
			,SLG_SBJ_ID
			)
		VALUES (
			@SlgValue
			,@LngIsoID
			,@SlgSbjID
			)
	END
	ELSE
	BEGIN
		UPDATE TD_SUBJECT_LANGUAGE
		SET SLG_VALUE = @SlgValue
		WHERE SLG_LNG_ID = @LngIsoID
			AND SLG_SBJ_ID = @SlgSbjID
	END

	RETURN @@rowcount
END
GO


