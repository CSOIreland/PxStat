SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 08/04/2019
-- Description:	Reads the next release date/time after the supplied release date
-- exec Data_Stat_Release_ReadNext '2019-04-05'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_ReadNext @ReleaseDate DATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT min(RLS_LIVE_DATETIME_TO) AS NextRelease
	FROM TD_RELEASE
	INNER JOIN VW_RELEASE_LIVE_NOW
		ON VRN_RLS_ID = RLS_ID
	WHERE RLS_LIVE_DATETIME_TO >= @ReleaseDate
		AND RLS_LIVE_DATETIME_TO <= dateadd(dd, 1, @ReleaseDate)
END
GO


