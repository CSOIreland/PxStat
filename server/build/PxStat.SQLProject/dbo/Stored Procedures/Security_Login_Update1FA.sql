
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/10/2020
-- Description:	Update Login with 1FA and associated details
 --exec Security_Login_Update1FA 'd7bd094923ce6c0ec77a09eb86498de712296c6816b4c6e3f7cabfca5a306a4e','2021-12-31','fzappass1@','8e4a91bca04790c9483450214719c2bfa882f880c2e6b8422b77093a2559b446','fzap@hse.ie'
-- =============================================
CREATE
	

 PROCEDURE Security_Login_Update1FA @LgnToken1FA VARCHAR(64)
	,@Lgn1FA VARCHAR(64)
	,@LgnNewToken VARCHAR(64)
	,@CcnEmail NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CcnId INT
	DECLARE @LgnId INT
	DECLARE @errorMessage VARCHAR(256)

	SELECT @CcnId = LGN_CCN_ID
		,@LgnId = LGN_ID
	FROM TD_LOGIN
	WHERE LGN_TOKEN_1FA = @LgnToken1FA

	IF @LgnId IS NULL
	BEGIN
		RETURN 0
	END

	
	IF (
			SELECT COUNT(*)
			FROM TD_ACCOUNT
			WHERE CCN_ID = @CcnId
				AND CCN_DELETE_FLAG = 0
			) = 0
	BEGIN
		RETURN 0
	END

	IF (
			SELECT COUNT(*)
			FROM TD_ACCOUNT
			INNER JOIN TD_LOGIN
				ON LGN_CCN_ID = CCN_ID
					AND CCN_DELETE_FLAG = 0
					AND CCN_USERNAME =@CcnEmail
					AND LGN_TOKEN_1FA = @LgnToken1FA
			) = 0
	BEGIN
		RETURN 0
	END

	UPDATE TD_LOGIN
	SET LGN_1FA = @Lgn1FA
		,LGN_TOKEN_1FA=NULL
	WHERE LGN_ID = @LgnId

	RETURN @@ROWCOUNT
END
