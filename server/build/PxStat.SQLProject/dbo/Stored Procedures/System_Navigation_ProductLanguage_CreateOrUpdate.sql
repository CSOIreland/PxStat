
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 02/01/2019
-- Description:	Creates a new entry on the ProductLanguage table if it doesn't already exist.
-- If it already exists then it updates the PlgValue with the supplied parameter
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_ProductLanguage_CreateOrUpdate @PlgValue NVARCHAR(256)
	,@PlgIsoCode CHAR(2)
	,@PlgPrcCode NVARCHAR(32)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngIsoID INT
	DECLARE @PlgPrcID INT
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @recordCount INT

	SET @PlgPrcID = (
			SELECT prd.PRC_ID
			FROM TD_PRODUCT prd
			WHERE prd.PRC_CODE = @PlgPrcCode
				AND prd.PRC_DELETE_FLAG = 0
			)

	IF @PlgPrcID = 0
		OR @PlgPrcID IS NULL
	BEGIN
		RETURN 0
	END

	SET @LngIsoID = (
			SELECT lng.LNG_ID
			FROM TS_LANGUAGE lng
			WHERE lng.LNG_ISO_CODE = @PlgIsoCode
				AND lng.LNG_DELETE_FLAG = 0
			)

	IF @LngIsoID IS NULL
		OR @LngIsoID = 0
	BEGIN
		SET @errorMessage = 'No ID found for Language Code code ' + cast(isnull(@PlgIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				)

		RETURN 0
	END

	SET @recordCount = (
			SELECT count(*)
			FROM TD_PRODUCT_LANGUAGE
			WHERE PLG_LNG_ID = @LngIsoID
				AND PLG_PRC_ID = @PlgPrcID
			)

	IF @recordCount = 0
	BEGIN
		INSERT INTO TD_PRODUCT_LANGUAGE (
			PLG_VALUE
			,PLG_LNG_ID
			,PLG_PRC_ID
			)
		VALUES (
			@PlgValue
			,@LngIsoID
			,@PlgPrcID
			)
	END
	ELSE
	BEGIN
		UPDATE TD_PRODUCT_LANGUAGE
		SET PLG_VALUE = @PlgValue
		WHERE PLG_LNG_ID = @LngIsoID
			AND PLG_PRC_ID = @PlgPrcID
	END

	RETURN @@rowcount
END
