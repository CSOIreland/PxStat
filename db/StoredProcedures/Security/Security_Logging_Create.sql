SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 13/06/2022
-- Description:	Creates log entries
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Logging_Create @LggThread VARCHAR(8)
	,@LggLevel VARCHAR(8)
	,@LggClass VARCHAR(256) = NULL
	,@LggMethod VARCHAR(256) = NULL
	,@LggLine VARCHAR(8) = NULL
	,@LggMessage NVARCHAR(MAX) = NULL
	,@LggException NVARCHAR(MAX) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO TD_LOGGING (
		LGG_DATETIME
		,LGG_THREAD
		,LGG_LEVEL
		,LGG_CLASS
		,LGG_METHOD
		,LGG_LINE
		,LGG_MESSAGE
		,LGG_EXCEPTION
		)
	VALUES (
		GETDATE()
		,@LggThread
		,@LggLevel
		,@LggClass
		,@LggMethod
		,@LggLine
		,@LggMessage
		,@LggException
		)

	RETURN @@rowcount
END
GO


