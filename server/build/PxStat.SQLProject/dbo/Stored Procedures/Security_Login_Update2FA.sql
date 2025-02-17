
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 16/10/2020
-- Description:	Sets up a 2FA based on the login token
-- =============================================
CREATE
	

 PROCEDURE Security_Login_Update2FA @LgnToken2FA VARCHAR(64)
	,@Lgn2FA NVARCHAR(MAX)
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CcnId INT

	SELECT @CcnId = CCN_ID
	FROM TD_ACCOUNT
	INNER JOIN TD_LOGIN
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND LGN_TOKEN_2FA = @LgnToken2FA
			AND CCN_LOCKED_FLAG=0
			AND CCN_USERNAME=@CcnUsername 

	IF @CcnId IS NOT NULL
	BEGIN
		UPDATE TD_LOGIN
		SET LGN_2FA = @Lgn2FA,
		LGN_TOKEN_2FA=NULL
		WHERE LGN_CCN_ID = @CcnId

	END
	RETURN @@ROWCOUNT
END
