
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 07/01/2021
-- Description:	Tests if an account (defined by username) has a valid token to change 2FA
-- exec Security_Login_ReadOpen2Fa 
-- =============================================
CREATE
	

 PROCEDURE Security_Login_ReadOpen2Fa @CcnUsername NVARCHAR(256)
AS
BEGIN
	SELECT LGN_ID AS LgnId
	FROM TD_LOGIN
	INNER JOIN TD_ACCOUNT
		ON LGN_CCN_ID = CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND LGN_TOKEN_2FA IS NOT NULL
			AND CCN_USERNAME = @CcnUsername
END
