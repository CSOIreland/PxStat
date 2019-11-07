SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 03/10/2018
-- Description: To check whether or not an account has associated entries in the TM_GROUP_ACCOUNT table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Account_Usage @CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CcnId INT = NULL
	DECLARE @IsInUse INT

	SET @CcnId = (
			SELECT CCN_ID
			FROM TD_ACCOUNT
			WHERE CCN_USERNAME = @CcnUsername
				AND CCN_DELETE_FLAG = 0
			)

	IF @CcnId IS NULL
	BEGIN
		RETURN 0
	END

	SET @IsInUse = (
			SELECT (
					iif(EXISTS (
							SELECT NULL
							FROM TM_GROUP_ACCOUNT
							WHERE GCC_CCN_ID = @CcnId
								AND GCC_DELETE_FLAG = 0
							), 1, 0)
					)
			)

	RETURN @IsInUse
END
GO


