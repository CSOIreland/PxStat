SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 29/11/2018
-- Description:	Gets the Live Current Release for a Release Code. 
-- This is where the current data is after the start date and (before the end date or the end date is null)
-- The returned release entry must have the same Matrix Code as that of the input RlsCode
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_WIP @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT rls.RLS_CODE AS RlsCode
		,rls.RLS_VERSION AS RlsVersion
		,rls.RLS_REVISION AS RlsRevision
		,rls.RLS_LIVE_FLAG AS RlsLiveFlag
		,rls.RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
		,rls.RLS_LIVE_DATETIME_TO AS RlsLiveDatetimeTo
		,rls.RLS_DEPENDENCY_FLAG AS RlsDependencyFlag
		,rls.RLS_EMERGENCY_FLAG AS RlsEmergencyFlag
		,rls.RLS_RESERVATION_FLAG AS RlsReservationFlag
		,rls.RLS_ARCHIVE_FLAG AS RlsArchiveFlag
		,rls.RLS_ANALYTICAL_FLAG AS RlsAnalyticalFlag
		,mtrView_mtrRls.MTR_CODE AS MtrCode
	FROM (
		SELECT mtrRls.*
			,mtrView.VRW_RLS_ID
		FROM (
			SELECT mtr.MTR_CODE
				,mtr.MTR_ID
				,mtr.MTR_RLS_ID
				,rls.RLS_CODE
			FROM TD_RELEASE rls
			INNER JOIN TD_MATRIX mtr
				ON rls.RLS_ID = mtr.MTR_RLS_ID
					AND mtr.MTR_DELETE_FLAG = 0
					AND rls.RLS_DELETE_FLAG = 0
			) mtrRls
		INNER JOIN (
			SELECT wip.VRW_MTR_ID
				,wip.VRW_RLS_ID
				,mtr.MTR_CODE
			FROM VW_RELEASE_WIP wip
			INNER JOIN TD_MATRIX mtr
				ON mtr.MTR_ID = wip.VRW_MTR_ID
			) mtrView
			ON mtrRls.MTR_CODE = mtrView.MTR_CODE
		) mtrView_mtrRls
	INNER JOIN TD_RELEASE rls
		ON rls.RLS_ID = mtrView_mtrRls.MTR_RLS_ID
	WHERE rls.RLS_CODE = @RlsCode
		AND rls.RLS_DELETE_FLAG = 0
END
