SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 12/12/2019
-- Description:	Reads counts of Formats from Analytics
-- EXEC Security_Analytic_ReadFormat '2022-01-01','2022-11-01','okeeffene',null
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Analytic_ReadFormat  @DateFrom DATE
	,@DateTo DATE
	,@CcnUsername NVARCHAR(256)
	,@NltInternalNetworkMask VARCHAR(12) = NULL
	,@MtrCode NVARCHAR(20) = NULL
	,@SbjCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SET @NltInternalNetworkMask = @NltInternalNetworkMask + '%'

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername


		SELECT FRM_TYPE + ' ' + FRM_VERSION as FrmTypeVersion,
		GRP_ID as GrpId,
		count(*) AS NltCount
		FROM TD_ANALYTIC
		INNER JOIN TD_MATRIX
			ON NLT_MTR_ID = MTR_ID
				AND MTR_DELETE_FLAG = 0
		INNER JOIN TD_RELEASE
			ON RLS_ID = MTR_RLS_ID
				AND TD_RELEASE.RLS_DELETE_FLAG = 0
		INNER JOIN TS_FORMAT 
		ON NLT_FRM_ID=FRM_ID
		INNER JOIN TD_GROUP 
		ON GRP_ID=RLS_GRP_ID 
		AND GRP_DELETE_FLAG=0
		LEFT JOIN TD_PRODUCT
			ON RLS_PRC_ID = PRC_ID
				AND PRC_DELETE_FLAG = 0
		LEFT JOIN TD_SUBJECT
			ON PRC_SBJ_ID = SBJ_ID
				AND SBJ_DELETE_FLAG = 0
		WHERE NLT_DATE >= @DateFrom
			AND NLT_DATE <= @DateTo
			AND (
				@MtrCode IS NULL
				OR @MtrCode = MTR_CODE
				)
			AND (
				@NltInternalNetworkMask IS NULL
				OR NLT_MASKED_IP NOT LIKE @NltInternalNetworkMask
				)
			AND (
				@SbjCode = SBJ_CODE
				OR @SbjCode IS NULL
				)
			AND (
				@PrcCode = PRC_CODE
				OR @PrcCode IS NULL
				)
		GROUP BY FRM_TYPE,FRM_VERSION,GRP_ID  
	
	SELECT GRP_ID AS GrpId
	FROM @GroupUserHasAccess;
END
GO


