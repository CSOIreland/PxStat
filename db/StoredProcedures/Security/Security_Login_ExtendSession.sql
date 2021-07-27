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

ALTER PROCEDURE Security_Login_ExtendSession @CcnUsername NVARCHAR(256) = NULL
	,@LgnSessionExpiry DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
		UPDATE TD_LOGIN
		SET LGN_SESSION_EXPIRY = @LgnSessionExpiry
		FROM TD_LOGIN
		INNER JOIN TD_ACCOUNT ON CCN_ID = LGN_CCN_ID
			AND CCN_USERNAME=@CcnUsername
		    AND LGN_SESSION IS NOT NULL
			AND CCN_DELETE_FLAG=0


	RETURN @@rowcount
END
GO


