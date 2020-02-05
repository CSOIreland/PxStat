SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 08/04/2019
-- Description:	Reads the next release date/time after the supplied release date
-- exec Data_Release_ReadNext '2020-02-04T10:58:00'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_ReadNext @ReleaseDate DATETIME
AS
BEGIN

	SET NOCOUNT ON;

	SELECT min(RLS_LIVE_DATETIME_FROM) AS NextRelease
	FROM TD_RELEASE
	INNER JOIN VW_RELEASE_LIVE_NEXT
		ON VRX_RLS_ID  = RLS_ID
	WHERE RLS_LIVE_DATETIME_FROM >= @ReleaseDate 
		AND RLS_LIVE_DATETIME_TO  is null
END
GO


