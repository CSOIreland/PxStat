
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:        Liam Millar
-- Create date: Revised 28/10/2022
-- Description:    Gets the ever live releases that were public in a particluar time period based on group and reason
-- exec Report_Release_Audit '2022-10-13','2022-11-10',NULL,NULL,'en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE [dbo].[Report_TableAudit] @StartDate DATETIME = NULL
	,@EndDate DATETIME = NULL
	,@GrpCode NVARCHAR(MAX) = NULL
	,@RsnCode NVARCHAR(MAX) = NULL
	,@LngIsoCode CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GroupTable TABLE (Groupie NVARCHAR(MAX));
	DECLARE @ReasonTable TABLE (Reason NVARCHAR(MAX));

	IF @GrpCode IS NOT NULL
	BEGIN
		INSERT INTO @GroupTable (Groupie) (SELECT value FROM STRING_SPLIT(@GrpCode, ','));
	END

	IF @RsnCode IS NOT NULL
	BEGIN
		INSERT INTO @ReasonTable (Reason) (SELECT value FROM STRING_SPLIT(@RsnCode, ','));
	END

	SELECT DISTINCT RSN_CODE AS RsnCode
		,RSN_VALUE_EXTERNAL AS RsnValueExternal
		,RSN_VALUE_INTERNAL AS RsnValueInternal
		,GRP_CODE AS GrpCode
		,GRP_NAME AS GrpName
		,TD_MATRIX.MTR_CODE AS MtrCode
		,TD_MATRIX.MTR_TITLE AS MtrTitle
		,MDM_CODE AS FrqCode
		,MDM_VALUE AS FrqValue
		,RLS_VERSION AS RlsVersion
		,RLS_REVISION AS RlsRevision
		,MTR_NOTE AS MtrNote
		,RLS_EXCEPTIONAL_FLAG AS RlsExceptionalFlag
		,RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
		,RLS_LIVE_DATETIME_To AS RlsLiveDatetimeTo
	FROM TD_RELEASE
	LEFT JOIN TM_REASON_RELEASE ON RSR_RLS_ID = RLS_ID
		AND RSR_DELETE_FLAG = 0
	LEFT JOIN TS_REASON ON RSR_RSN_ID = RSN_ID
	INNER JOIN TD_MATRIX ON MTR_RLS_ID = RLS_ID
	INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
		AND LNG_ISO_CODE = @LngIsoCode
	INNER JOIN TD_GROUP ON RLS_GRP_ID = GRP_ID
	INNER JOIN TD_MATRIX_DIMENSION ON MDM_MTR_ID = MTR_ID
	INNER JOIN TS_DIMENSION_ROLE ON MDM_DMR_ID = DMR_ID
	WHERE (
			@GrpCode IS NULL
			OR (
				GRP_CODE IN (
					SELECT Groupie
					FROM @GroupTable
					)
				)
			)
		AND (
			@RsnCode IS NULL
			OR (
				RSN_CODE IN (
					SELECT Reason
					FROM @ReasonTable
					)
				)
			)
		AND (
			RLS_LIVE_DATETIME_FROM BETWEEN @StartDate
				AND @EndDate
			)
		AND DMR_CODE = 'TIME'
		AND RLS_LIVE_FLAG = 1;

	RETURN;
END
