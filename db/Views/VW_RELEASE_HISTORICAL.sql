SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 06/10/2020
-- Description:	Gets Historical Releases
-- =============================================
CREATE
	OR

ALTER VIEW VW_RELEASE_HISTORICAL
AS
SELECT RLS_ID VRW_RLS_ID,
	MTR_ID VRW_MTR_ID
FROM TD_MATRIX
INNER JOIN TD_RELEASE
	ON RLS_ID = MTR_RLS_ID
		AND RLS_DELETE_FLAG = 0
		AND RLS_LIVE_FLAG = 1
		AND RLS_REVISION = 0
		AND getdate()>RLS_LIVE_DATETIME_TO

WHERE MTR_DELETE_FLAG = 0
GO


	
