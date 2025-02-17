
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 27 Nov 2018
-- Description:	Gets the release that is live now
-- =============================================
CREATE
	

 VIEW VW_RELEASE_LIVE_NOW
AS
SELECT RLS_ID VRN_RLS_ID,
	MTR_ID VRN_MTR_ID
FROM TD_MATRIX
INNER JOIN TD_RELEASE
	ON RLS_ID = MTR_RLS_ID
		AND RLS_DELETE_FLAG = 0
		AND RLS_LIVE_FLAG = 1
		AND RLS_VERSION != 0
		AND RLS_REVISION = 0
		AND getDate() >= RLS_LIVE_DATETIME_FROM
		AND (
			RLS_LIVE_DATETIME_TO IS NULL
			OR getDate() < RLS_LIVE_DATETIME_TO
			)
WHERE MTR_DELETE_FLAG = 0
