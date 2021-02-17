SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 30/12/2020
-- Description:	Create an invitation token for a user and locks the account
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Login_Update2FaToken @CcnUsername NVARCHAR(256),
@LgnToken2FA VARCHAR(64)

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CcnId INT

	SELECT @CcnId = CCN_ID
	FROM TD_ACCOUNT
	INNER JOIN TD_LOGIN
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND CCN_USERNAME = @CcnUsername

	IF @CcnId IS NOT NULL
	BEGIN
		UPDATE TD_LOGIN
		SET LGN_TOKEN_2FA=@LgnToken2FA
		WHERE LGN_CCN_ID = @CcnId

	END

	RETURN @@ROWCOUNT
END
GO


