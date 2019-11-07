SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 22 Oct 2018
-- Description:	Reads record(s) from the TD_Release & dependant tables
-- =============================================
CREATE
	OR

--exec Data_Stat_Release_Read_dev 'OKeeffeNe',null,178
ALTER PROCEDURE Data_Release_Read @CcnUsername NVARCHAR(256)
	,@RlsCode INT = NULL
	,@RlsID INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @RlsCode IS NULL
		AND @RlsID IS NULL
	BEGIN
		RETURN
	END

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	SELECT MTR_CODE MtrCode
		,RLS_CODE RlsCode
		,RLS_VERSION RlsVersion
		,RLS_REVISION RlsRevision
		,RLS_LIVE_FLAG RlsLiveFlag
		,RLS_LIVE_DATETIME_FROM RlsLiveDatetimeFrom
		,RLS_LIVE_DATETIME_TO RlsLiveDatetimeTo
		,RLS_DEPENDENCY_FLAG RlsDependencyFlag
		,RLS_EMERGENCY_FLAG RlsEmergencyFlag
		,RLS_RESERVATION_FLAG RlsReservationFlag
		,RLS_ARCHIVE_FLAG RlsArchiveFlag
		,RLS_ANALYTICAL_FLAG RlsAnalyticalFlag
		,GRP_CODE GrpCode
		,GRP_NAME GrpName
		,CMM_CODE CmmCode
		,CMM_VALUE CmmValue
		,SBJ_CODE SbjCode
		,SBJ_VALUE SbjValue
		,PRC_CODE PrcCode
		,PRC_VALUE PrcValue
	FROM TD_MATRIX
	INNER JOIN TD_RELEASE
		ON RLS_ID = MTR_RLS_ID
			AND RLS_DELETE_FLAG = 0
	INNER JOIN TD_GROUP
		ON RLS_GRP_ID = GRP_ID
			AND GRP_DELETE_FLAG = 0
	INNER JOIN @GroupUserHasAccess g
		ON g.GRP_ID = RLS_GRP_ID
	LEFT JOIN TD_COMMENT
		ON CMM_ID = RLS_CMM_ID
			AND CMM_DELETE_FLAG = 0
	LEFT JOIN TD_PRODUCT
		ON RLS_PRC_ID = PRC_ID
			AND PRC_DELETE_FLAG = 0
	LEFT JOIN TD_SUBJECT
		ON SBJ_ID = PRC_SBJ_ID
			AND SBJ_DELETE_FLAG = 0
	WHERE MTR_DELETE_FLAG = 0
		AND (
			@RlsCode IS NULL
			OR @RlsCode = RLS_CODE
			)
		AND (
			@RlsID IS NULL
			OR @RlsID = RLS_ID
			)
	GROUP BY MTR_CODE
		,RLS_CODE
		,RLS_VERSION
		,RLS_REVISION
		,RLS_LIVE_FLAG
		,RLS_LIVE_DATETIME_FROM
		,RLS_LIVE_DATETIME_TO
		,RLS_DEPENDENCY_FLAG
		,RLS_EMERGENCY_FLAG
		,RLS_RESERVATION_FLAG
		,RLS_ARCHIVE_FLAG
		,RLS_ANALYTICAL_FLAG
		,GRP_CODE
		,GRP_NAME
		,CMM_CODE
		,CMM_VALUE
		,SBJ_CODE
		,SBJ_VALUE
		,PRC_CODE
		,PRC_VALUE
END
GO


