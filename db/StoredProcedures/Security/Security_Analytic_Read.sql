-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 21/01/2019
-- Description:	Reads a summary of analytic data
-- exec Security_Analytic_Read '2020-09-20T12:00:00','2020-12-02T12:00:00', 1, "1",""
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Analytic_Read @DateFrom DATE
	,@DateTo DATE
	,@SbjCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
	,@NltInternalNetworkMask VARCHAR(12) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SET @NltInternalNetworkMask = @NltInternalNetworkMask + '%'

	SELECT MTR_CODE AS MtrCode
		,MTR_TITLE AS MtrTitle
		,RLS_LIVE_DATETIME_FROM AS PublishDate
		,PRC_CODE AS PrcCode
		,PRC_VALUE AS PrcValue
		,SBJ_CODE AS SbjCode
		,SBJ_Value AS SbjValue
		,Bots AS NltBot
		,M2M AS NltM2m
		,NltUser
		,Total
	FROM (
		SELECT MTR_CODE
			,MTR_TITLE
			,PRC_CODE
			,PRC_VALUE
			,SBJ_CODE
			,SBJ_VALUE
			,max(RLS_LIVE_DATETIME_FROM) as RLS_LIVE_DATETIME_FROM
			,sum(cast(NLT_BOT_FLAG AS INT)) AS Bots
			,sum(cast(NLT_M2M_FLAG AS INT)) AS M2M
			,sum(cast(NLT_USER_FLAG AS INT)) AS NltUser
			,count(*) AS Total
		FROM TD_ANALYTIC
		INNER JOIN TD_MATRIX
			ON NLT_MTR_ID = MTR_ID
				AND MTR_DELETE_FLAG = 0
		INNER JOIN TD_RELEASE
			ON RLS_ID = MTR_RLS_ID
				AND TD_RELEASE.RLS_DELETE_FLAG = 0
		LEFT JOIN TD_PRODUCT
			ON RLS_PRC_ID = PRC_ID
				AND PRC_DELETE_FLAG = 0
		LEFT JOIN TD_SUBJECT
			ON PRC_SBJ_ID = SBJ_ID
				AND SBJ_DELETE_FLAG = 0
		WHERE NLT_DATE >= @DateFrom
			AND NLT_DATE <= @DateTo
			AND (
				@SbjCode = SBJ_CODE
				OR @SbjCode IS NULL
				)
			AND (
				@PrcCode = PRC_CODE
				OR @PrcCode IS NULL
				)
			AND (
				@NltInternalNetworkMask IS NULL
				OR NLT_MASKED_IP NOT LIKE @NltInternalNetworkMask
				)
		GROUP BY MTR_CODE
			,MTR_TITLE
			,SBJ_CODE
			,SBJ_VALUE
			--,RLS_LIVE_DATETIME_FROM
			,PRC_CODE
			,PRC_VALUE
		) counts
END
GO


