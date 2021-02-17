SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 21/10/2020
-- Description:	Extends a session provided the corresponding user hasn't been deleted
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Login_ExtendSession @LgnSession VARCHAR(64)=NULL
	,@CcnUsername NVARCHAR(256)=NULL
	,@LgnSessionExpiry DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @CcnId INT

	IF @LgnSession IS NOT NULL
	BEGIN
		SET @CcnId=
		(
			SELECT CCN_ID
			FROM TD_LOGIN
			INNER JOIN TD_ACCOUNT
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND LGN_SESSION = @LgnSession
		)
	END
	ELSE IF @CcnUsername IS NOT NULL
	BEGIN
		SET @CcnId =
		(
			SELECT CCN_ID
			FROM TD_LOGIN
			INNER JOIN TD_ACCOUNT
		ON CCN_ID = LGN_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND CCN_USERNAME=@CcnUsername 
		)

	END
	ELSE
	BEGIN
		RETURN 0
	END

	UPDATE TD_LOGIN
	SET LGN_SESSION_EXPIRY = @LgnSessionExpiry
	FROM TD_LOGIN
	INNER JOIN TD_ACCOUNT
		ON CCN_ID = LGN_CCN_ID
			AND CCN_ID=@CcnId 
			AND LGN_SESSION IS NOT NULL
			AND LGN_SESSION_EXPIRY>GETDATE()

	RETURN @@rowcount
END
GO


