
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 21/10/2020
-- Description:	Creates a new session, e.g. at login
-- =============================================
CREATE
	

 PROCEDURE Security_Login_CreateSession @LgnSession VARCHAR(64)
	,@LgnSessionExpiry DATETIME
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE TD_LOGIN
	SET LGN_SESSION = @LgnSession
		,LGN_SESSION_EXPIRY = @LgnSessionExpiry
	FROM TD_LOGIN
	INNER JOIN TD_ACCOUNT
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND CCN_USERNAME = @CcnUsername

	RETURN @@rowcount
END
