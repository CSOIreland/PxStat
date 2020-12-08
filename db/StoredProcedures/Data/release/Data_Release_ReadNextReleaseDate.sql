SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 07/12/2020
-- Description:	Gets the next scheduled release date
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_ReadNextReleaseDate
AS
BEGIN
	SET NOCOUNT ON;

	SELECT min(RLS_LIVE_DATETIME_FROM) AS RlsDatetimeNext
	FROM td_release
	WHERE RLS_LIVE_DATETIME_FROM > getdate()
END
GO


