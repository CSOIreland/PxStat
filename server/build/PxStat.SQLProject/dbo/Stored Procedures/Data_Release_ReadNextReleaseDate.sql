
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 07/12/2020
-- Description:	Gets the next scheduled release date
-- =============================================
CREATE
	

 PROCEDURE Data_Release_ReadNextReleaseDate
AS
BEGIN
	SET NOCOUNT ON;

	SELECT min(RLS_LIVE_DATETIME_FROM) AS RlsDatetimeNext
	FROM td_release
	WHERE RLS_LIVE_DATETIME_FROM > getdate()
	AND RLS_DELETE_FLAG=0
END
