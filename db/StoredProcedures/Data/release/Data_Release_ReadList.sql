SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 9 Nov 2018
-- Description:	Reads a list of releases for a given matrix code and username
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_ReadList @CcnUsername NVARCHAR(256)
	,@MtrCode NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	SELECT MTR_CODE MtrCode
		,RLS_CODE RlsCode
		,RLS_ANALYTICAL_FLAG RlsAnalyticalFlag
		,RLS_ARCHIVE_FLAG RlsArchiveFlag
		,RLS_LIVE_FLAG RlsLiveFlag
		,RLS_LIVE_DATETIME_TO RlsLiveDatetimeTo
		,RLS_RESERVATION_FLAG RlsReservationFlag
		,RLS_REVISION RlsRevision
		,RLS_VERSION RlsVersion
		,RLS_DEPENDENCY_FLAG RlsDependencyFlag
		,RLS_EMERGENCY_FLAG RlsEmergencyFlag
		,RLS_LIVE_DATETIME_FROM RlsLiveDatetimeFrom
	FROM TD_MATRIX
	INNER JOIN TD_RELEASE
		ON RLS_ID = MTR_RLS_ID
			AND RLS_DELETE_FLAG = 0
	INNER JOIN @GroupUserHasAccess g
		ON g.GRP_ID = RLS_GRP_ID
	WHERE MTR_DELETE_FLAG = 0
		AND MTR_CODE = @MtrCode
	GROUP BY MTR_CODE
		,RLS_CODE
		,RLS_ANALYTICAL_FLAG
		,RLS_ARCHIVE_FLAG
		,RLS_LIVE_FLAG
		,RLS_LIVE_DATETIME_TO
		,RLS_RESERVATION_FLAG
		,RLS_REVISION
		,RLS_VERSION
		,RLS_DEPENDENCY_FLAG
		,RLS_EMERGENCY_FLAG
		,RLS_LIVE_DATETIME_FROM
	ORDER BY RLS_VERSION
		,RLS_REVISION
END
GO


