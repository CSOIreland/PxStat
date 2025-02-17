
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 27 Nov 2018
-- Description:	Gets the Work in progress release
-- =============================================
CREATE
	

 VIEW VW_RELEASE_WIP
AS
SELECT RLS_ID VRW_RLS_ID,
	MTR_ID VRW_MTR_ID
FROM TD_MATRIX
INNER JOIN TD_RELEASE
	ON RLS_ID = MTR_RLS_ID
		AND RLS_DELETE_FLAG = 0
		AND RLS_LIVE_FLAG = 0
		AND RLS_REVISION != 0
WHERE MTR_DELETE_FLAG = 0
