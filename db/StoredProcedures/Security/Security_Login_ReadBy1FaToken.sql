SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/10/2020
-- Description:	Get a user by the 1FA token
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Login_ReadBy1FaToken @LgnToken1Fa VARCHAR(64)
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CCN_USERNAME AS CcnUsername
		,CCN_DISPLAYNAME AS CcnDisplayName
		,CCN_EMAIL AS CcnEmail
		,CCN_ID AS CcnId
	FROM TD_ACCOUNT
	INNER JOIN TD_LOGIN
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND LGN_TOKEN_1FA = @LgnToken1Fa
			AND CCN_LOCKED_FLAG = 0
			AND CCN_USERNAME = @CcnUsername
END
GO


