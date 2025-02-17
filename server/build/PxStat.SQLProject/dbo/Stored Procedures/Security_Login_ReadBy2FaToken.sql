
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/10/2020
-- Description:	Get a user by the 2FA token
-- EXEC Security_Login_ReadBy2FaToken 'cb83f17a1db3bd6b85328920a59a8628c84162e832b8505c07df2ee1cae89d05','rtharpe@abc.ie'
-- =============================================
CREATE
	

 PROCEDURE Security_Login_ReadBy2FaToken @LgnToken2Fa VARCHAR(64)
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
			AND CCN_LOCKED_FLAG=0
			AND LGN_TOKEN_2FA = @LgnToken2Fa
			AND CCN_USERNAME =@CcnUsername 

			
END
