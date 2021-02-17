SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 16/10/2020
-- Description:	Get a user by the session token
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Login_ReadBySession @LgnSession VARCHAR(64)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CCN_USERNAME AS CcnUsername
		,CCN_DISPLAYNAME AS CcnDisplayName
		,CCN_EMAIL AS CcnEmail
		,CCN_ID AS CcnId
		,CCN_LOCKED_FLAG AS CcnLockedFlag
	FROM TD_ACCOUNT
	INNER JOIN TD_LOGIN
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND LGN_SESSION_EXPIRY  > getdate()
			AND LGN_SESSION  = @LgnSession
			AND CCN_LOCKED_FLAG=0
END
GO


