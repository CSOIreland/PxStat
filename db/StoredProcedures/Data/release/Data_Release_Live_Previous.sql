SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/11/2018
-- Description:	Read the previous live release based on the current RlsCode
-- The returned release entry must have the same Ma trix Code as that of the input RlsCode
-- exec Data_Release_Live_Previous 70
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_Live_Previous @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MtrCode NVARCHAR(20)

	SET @MtrCode = (
			SELECT DISTINCT MTR_CODE
			FROM TD_MATRIX
			INNER JOIN TD_RELEASE
				ON RLS_ID = MTR_RLS_ID
					AND MTR_DELETE_FLAG = 0
					AND RLS_DELETE_FLAG = 0
			WHERE RLS_CODE = @RlsCode
			)

	IF @MtrCode IS NULL
	BEGIN
		RETURN
	END

	SELECT rlsPrevious.RLS_CODE AS RlsCode
		,rlsPrevious.RLS_VERSION AS RlsVersion
		,rlsPrevious.RLS_REVISION AS RlsRevision
		,rlsPrevious.RLS_LIVE_FLAG AS RlsLiveFlag
		,rlsPrevious.RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
		,rlsPrevious.RLS_LIVE_DATETIME_TO AS RlsLiveDatetimeTo
		,rlsPrevious.RLS_DEPENDENCY_FLAG AS RlsDependencyFlag
		,rlsPrevious.RLS_EMERGENCY_FLAG AS RlsEmergencyFlag
		,rlsPrevious.RLS_RESERVATION_FLAG AS RlsReservationFlag
		,rlsPrevious.RLS_ARCHIVE_FLAG AS RlsArchiveFlag
		,rlsPrevious.RLS_ANALYTICAL_FLAG AS RlsAnalyticalFlag
		,@MtrCode AS MtrCode
		,GRP_CODE GrpCode
		,GRP_NAME GrpName
		,CMM_CODE CmmCode
		,CMM_VALUE CmmValue
		,SBJ_CODE SbjCode
		,SBJ_VALUE SbjValue
		,PRC_CODE PrcCode
		,PRC_VALUE PrcValue
	FROM VW_RELEASE_LIVE_NOW vrn
	INNER JOIN TD_RELEASE rlsNow
		ON vrn.VRN_RLS_ID = rlsNow.RLS_ID
	INNER JOIN TD_MATRIX mtrNow
		ON rlsNow.RLS_ID = mtrNow.MTR_RLS_ID
	INNER JOIN TD_MATRIX mtrPrevious
		ON mtrPrevious.MTR_CODE = mtrNow.MTR_CODE
	INNER JOIN TD_RELEASE rlsPrevious
		ON mtrPrevious.MTR_RLS_ID = rlsPrevious.RLS_ID
	INNER JOIN VW_RELEASE_LIVE_PREVIOUS vrp
		ON mtrPrevious.MTR_RLS_ID = vrp.VRP_RLS_ID
			AND mtrPrevious.MTR_ID = vrp.VRP_MTR_ID
	INNER JOIN TD_GROUP
		ON rlsPrevious.RLS_GRP_ID = GRP_ID
			AND GRP_DELETE_FLAG = 0
	LEFT JOIN TD_COMMENT
		ON CMM_ID = rlsPrevious.RLS_CMM_ID
			AND CMM_DELETE_FLAG = 0
	LEFT JOIN TD_PRODUCT
		ON rlsPrevious.RLS_PRC_ID = PRC_ID
			AND PRC_DELETE_FLAG = 0
	LEFT JOIN TD_SUBJECT
		ON SBJ_ID = PRC_SBJ_ID
			AND SBJ_DELETE_FLAG = 0
	where mtrPrevious.MTR_CODE=@MtrCode 
END
GO


