SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 28/09/2018
-- Description: To check whether or not a language has associated entries in the TD_MATRIX table
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Settings_Language_Usage @LngIsoCode CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngId INT
	DECLARE @IsInUse INT

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngId IS NULL
		OR @LngId = 0
	BEGIN
		-- No error to be returned, just the fact that there are no related records
		RETURN 0
	END

	SET @IsInUse = (
			SELECT (
					iif(EXISTS (
							SELECT TOP 1 1
							FROM TD_MATRIX
							WHERE MTR_LNG_ID = @LngId
								AND MTR_DELETE_FLAG = 0
							), 1, 0)
					)
			)

	RETURN @IsInUse
END
GO


