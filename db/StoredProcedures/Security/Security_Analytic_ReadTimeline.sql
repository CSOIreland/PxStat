SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 22/01/2019
-- Description:	Reads Analytic data by date
-- exec Security_Analytic_ReadTimeline '2019-04-13','2019-05-15','3.9',null,4,4
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Analytic_ReadTimeline @DateFrom DATE
	,@DateTo DATE
	,@NltInternalNetworkMask VARCHAR(12) = NULL
	,@MtrCode NVARCHAR(20) = NULL
	,@SbjCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SET @NltInternalNetworkMask = @NltInternalNetworkMask + '%'

	SELECT counts.NLT_DATE AS [date]
		,bots AS NltBot
		,USERS AS NltUser
		,m2m AS NltM2m
		,nltCount AS total
	FROM (
		SELECT NLT_DATE
			,sum(cast(NLT_BOT_FLAG AS INT)) AS Bots
			,sum(cast(NLT_M2M_FLAG AS INT)) AS M2M
			,sum(cast(NLT_USER_FLAG AS INT)) AS USERS
			,count(NLT_ID) AS nltCount
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
		GROUP BY NLT_DATE
		) counts
	ORDER BY counts.NLT_DATE ASC
END
GO


