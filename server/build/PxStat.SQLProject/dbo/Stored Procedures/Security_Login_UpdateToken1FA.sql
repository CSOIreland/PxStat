
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 27/10/2020
-- Description:	Update Login token, e.g. to create a new token for an existing user (e.g. lost password)
-- =============================================
CREATE
	

 PROCEDURE Security_Login_UpdateToken1FA @CcnUsername NVARCHAR(256)
	,@LgnNewToken1FA VARCHAR(64)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CcnId INT
	DECLARE @LgnId INT
	DECLARE @errorMessage VARCHAR(256)

	SELECT @CcnId = LGN_CCN_ID
		,@LgnId = LGN_ID
	FROM TD_LOGIN
	INNER JOIN TD_ACCOUNT
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND CCN_LOCKED_FLAG=0
	WHERE CCN_USERNAME = @CcnUsername

	IF @LgnId IS NULL
	BEGIN
		RETURN 0
	END

	UPDATE TD_LOGIN
	SET LGN_TOKEN_1FA = @LgnNewToken1FA
	WHERE LGN_ID = @LgnId

	RETURN @@ROWCOUNT
END
