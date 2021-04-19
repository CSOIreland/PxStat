SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/01/2019
-- Description:	Creates a new ThemeLanguage entry or just updates it if it exists already
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_ThemeLanguage_CreateOrUpdate @ThmCode INT
	,@TlgValue NVARCHAR(256)
	,@ThmIsoCode CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngIsoID INT
	DECLARE @TlgThmID INT
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @recordCount INT

	SET @TlgThmID = (
			SELECT THM_ID
			FROM TD_THEME
			WHERE THM_CODE = @ThmCode
				AND THM_DELETE_FLAG = 0
			)

	IF @TlgThmID IS NULL
		OR @TlgThmID = 0
	BEGIN
		RETURN 0
	END

	SET @LngIsoID = (
			SELECT lng.LNG_ID
			FROM TS_LANGUAGE lng
			WHERE lng.LNG_ISO_CODE = @ThmIsoCode
				AND lng.LNG_DELETE_FLAG = 0
			)

	IF @LngIsoID IS NULL
		OR @LngIsoID = 0
	BEGIN
		SET @errorMessage = 'No ID found for Language Code code ' + cast(isnull(@ThmIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @recordCount = (
			SELECT count(*)
			FROM TD_THEME_LANGUAGE
			WHERE TLG_LNG_ID = @LngIsoID
				AND TLG_THM_ID = @TlgThmID
			)

	IF @recordCount = 0
	BEGIN
		INSERT INTO TD_THEME_LANGUAGE (
			TLG_VALUE
			,TLG_LNG_ID
			,TLG_THM_ID
			)
		VALUES (
			@TlgValue
			,@LngIsoID
			,@TlgThmID
			)
	END
	ELSE
	BEGIN
		UPDATE TD_THEME_LANGUAGE
		SET TLG_VALUE = @TlgValue
		WHERE TLG_LNG_ID = @LngIsoID
			AND TLG_THM_ID = @TlgThmID
	END

	RETURN @@rowcount
END
GO


