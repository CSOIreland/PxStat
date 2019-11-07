SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 20/11/2018
-- Description:	Gets the Live Current Release for a Release Code. 
-- This is where the current data is after the start date and (before the end date or the end date is null)
-- The returned release entry must have the same Matrix Code as that of the input RlsCode
-- exec Data_Release_Live_Now null,'HH1','pl'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_Live_Now @RlsCode INT = NULL
	,@MtrCode NVARCHAR(256) = NULL
	,@LngIsoCode CHAR(2) = NULL
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
		,mtr.MTR_CODE AS MtrCode
		,GRP_CODE GrpCode
		,GRP_NAME GrpName
		,GRP_CONTACT_NAME GrpContactName
		,GRP_CONTACT_PHONE GrpContactPhone
		,GRP_CONTACT_EMAIL GrpContactEmail
		,CMM_CODE CmmCode
		,CMM_VALUE CmmValue
		,SBJ_CODE SbjCode
		,SBJ_VALUE SbjValue
		,PRC_CODE PrcCode
		,PRC_VALUE PrcValue
		,LNG_ISO_CODE  LngIsoCode
	FROM TD_RELEASE rls
	INNER JOIN VW_RELEASE_LIVE_NOW
		ON VRN_RLS_ID = rls.RLS_ID
	INNER JOIN TD_MATRIX mtr
		ON rls.RLS_ID = mtr.MTR_RLS_ID
			AND mtr.MTR_DELETE_FLAG = 0
			AND rls.RLS_DELETE_FLAG = 0
			AND VRN_MTR_ID = mtr.MTR_ID
	INNER JOIN TD_GROUP
		ON RLS_GRP_ID = GRP_ID
			AND GRP_DELETE_FLAG = 0
	INNER JOIN TS_LANGUAGE 
		on MTR_LNG_ID=LNG_ID
		and LNG_DELETE_FLAG=0 
	LEFT JOIN TD_COMMENT
		ON CMM_ID = RLS_CMM_ID
			AND CMM_DELETE_FLAG = 0
	LEFT JOIN TD_PRODUCT
		ON RLS_PRC_ID = PRC_ID
			AND PRC_DELETE_FLAG = 0
	LEFT JOIN TD_SUBJECT
		ON SBJ_ID = PRC_SBJ_ID
			AND SBJ_DELETE_FLAG = 0

	WHERE mtr.MTR_CODE = (
			SELECT DISTINCT MTR_CODE
			FROM TD_MATRIX mtr2
			INNER JOIN TD_RELEASE rls2
				ON rls2.RLS_ID = mtr2.MTR_RLS_ID
					AND rls2.RLS_DELETE_FLAG = 0
					AND mtr2.MTR_DELETE_FLAG = 0
			INNER JOIN TS_LANGUAGE
				ON mtr2.MTR_LNG_ID = LNG_ID
					AND LNG_DELETE_FLAG = 0
			WHERE (
					(
						@MtrCode IS NULL
						AND @RlsCode IS NOT NULL
						AND rls2.RLS_CODE = @RlsCode
						)
					OR (
						@RlsCode IS NULL
						AND @MtrCode IS NOT NULL
						AND mtr2.MTR_CODE = @MtrCode
						)
					)
				
			)
END
GO


