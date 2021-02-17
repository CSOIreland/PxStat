SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 12/01/2020
-- Logout a user - uses the session to identify the user
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Login_Logout @LgnSession VARCHAR(64)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE TD_LOGIN
	SET LGN_SESSION=NULL,
	LGN_SESSION_EXPIRY=NULL
	WHERE LGN_SESSION=@LgnSession 

	return @@rowcount
END
GO


