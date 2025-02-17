
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 07/01/2021
-- Description:	Tests if an account (defined by email) has a valid token to change 1FA
-- =============================================
CREATE
	

 PROCEDURE Security_Login_ReadOpen1Fa @CcnEmail NVARCHAR(256)
AS
BEGIN
	SELECT LGN_ID AS LgnId
	FROM TD_LOGIN
	INNER JOIN TD_ACCOUNT
		ON LGN_CCN_ID = CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND LGN_TOKEN_1FA IS NOT NULL
			AND CCN_EMAIL = @CcnEmail
END
