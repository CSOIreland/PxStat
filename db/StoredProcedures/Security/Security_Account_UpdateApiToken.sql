SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 28/05/2024
-- Description:	Update an accounts api token
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Account_UpdateApiToken @CcnToken NVARCHAR(256)
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	UPDATE TD_ACCOUNT
	SET CCN_API_TOKEN = @CcnToken
	WHERE CCN_USERNAME = @CcnUsername
		AND CCN_DELETE_FLAG = 0

	RETURN @@ROWCOUNT
END
GO

