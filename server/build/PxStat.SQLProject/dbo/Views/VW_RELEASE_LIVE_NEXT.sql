
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 27 Nov 2018
-- Description:	Gets the release that will be live in the future ( not now )
-- =============================================
CREATE
	

 VIEW VW_RELEASE_LIVE_NEXT
AS
-- by design we always have RLS_LIVE_DATETIME_TO set to NULL for the next release
-- once it goes to LIVE NOW, it will have RLS_LIVE_DATETIME_TO set once the following next release exists, or open - NULL otherwise
SELECT RLS_ID VRX_RLS_ID,
	MTR_ID VRX_MTR_ID
FROM TD_MATRIX
INNER JOIN TD_RELEASE
	ON RLS_ID = MTR_RLS_ID
		AND RLS_DELETE_FLAG = 0
		AND RLS_LIVE_FLAG = 1
		AND RLS_VERSION != 0
		AND RLS_REVISION = 0
		AND getDate() < RLS_LIVE_DATETIME_FROM
		AND RLS_LIVE_DATETIME_TO IS NULL
WHERE MTR_DELETE_FLAG = 0
