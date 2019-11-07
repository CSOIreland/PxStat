SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/06/2019
-- Description:	Read log entries
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Logging_Read @LggDatetimeStart DATETIME
	,@LggDatetimeEnd DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	SELECT LGG_DATETIME AS LggDatetime
		,LGG_THREAD AS LggThread
		,LGG_LEVEL AS LggLevel
		,LGG_CLASS AS LggClass
		,LGG_METHOD AS LggMethod
		,LGG_LINE AS LggLine
		,LGG_MESSAGE AS LggMessage
		,LGG_EXCEPTION AS LggException
	FROM TD_LOGGING
	WHERE LGG_DATETIME >= @LggDatetimeStart
		AND LGG_DATETIME <= @LggDatetimeEnd
END
GO


