
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 08/10/2018
-- Description:	Checks if a copyright is in use
-- =============================================
CREATE
	

 PROCEDURE System_Settings_Copyright_Usage @CprCode NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsInUse INT
	DECLARE @CprID INT = NULL

	SET @CprID = (
			SELECT CPR_ID
			FROM TS_COPYRIGHT
			WHERE CPR_CODE = @CprCode
				AND CPR_DELETE_FLAG = 0
			)

	IF @CprID IS NULL
	BEGIN
		-- No record found, therefore no usage
		RETURN 0
	END

	SET @IsInUse = (
			SELECT (
					iif(EXISTS (
							SELECT TOP 1 1
							FROM TD_MATRIX
							WHERE MTR_CPR_ID = @CprID
								AND MTR_DELETE_FLAG = 0
							), 1, 0)
					)
			)

	RETURN @IsInUse
END
