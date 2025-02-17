
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 20/10/2020
-- Description:	Returns a CcnUsername if the 1FA is correct
-- =============================================
CREATE
	

 PROCEDURE Security_Login_Authenticate1FA @CcnUsername NVARCHAR(256)
	,@Lgn1FA NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;


	SELECT CCN_USERNAME as CcnUsername, CCN_LOCKED_FLAG as CcnLockedFlag
	FROM TD_ACCOUNT
	INNER JOIN TD_LOGIN
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND CCN_USERNAME = @CcnUsername
			AND CCN_LOCKED_FLAG=0
			AND LGN_1FA = @Lgn1FA
			
END
