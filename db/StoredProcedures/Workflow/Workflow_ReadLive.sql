SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:	Neil O'Keeffe
-- Read date: 06/11/2019
-- Description:	Reads live records
-- exec Workflow_ReadLive 'okeeffene'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_ReadLive @CcnUsername NVARCHAR(256)
	,@RlsCode INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	SELECT RLS_CODE RlsCode
		,MTR_CODE MtrCode
		,GRP_CODE GrpCode
		,GRP_NAME GrpName
		,CCN_USERNAME CcnUsername
		,RLS_RESERVATION_FLAG RlsReservationFlag
		,RLS_ARCHIVE_FLAG RlsArchiveFlag
		,MTR_OFFICIAL_FLAG MtrOfficialFlag
		,RLS_LIVE_DATETIME_FROM RlsLiveDatimeFrom
	FROM TD_MATRIX
	INNER JOIN TD_RELEASE
		ON RLS_ID = MTR_RLS_ID
			AND RLS_DELETE_FLAG = 0
	INNER JOIN VW_RELEASE_LIVE_NOW
		ON VRN_RLS_ID = RLS_ID
			AND VRN_MTR_ID = MTR_ID
	INNER JOIN @GroupUserHasAccess g
		ON g.GRP_ID = RLS_GRP_ID
	INNER JOIN TD_GROUP GRP
		ON GRP.GRP_ID = RLS_GRP_ID
			AND GRP_DELETE_FLAG = 0
	INNER JOIN TM_AUDITING_HISTORY
		ON MTR_DTG_ID = DHT_DTG_ID
	INNER JOIN TD_ACCOUNT
		ON DHT_CCN_ID = CCN_ID
			AND CCN_DELETE_FLAG = 0
	WHERE MTR_DELETE_FLAG = 0
		AND (
			@RlsCode IS NULL
			OR @RlsCode = RLS_CODE
			)
	ORDER BY MTR_CODE
END
GO


