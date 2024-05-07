-- ================================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 27/07/2023
-- Read Browser analytic data
--EXEC Security_Analytic_ReadBrowser '2023-06-22','2023-06-29', 'en','FY006B'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Analytic_ReadBrowser @DateFrom DATE
	,@DateTo DATE
	,@LngIsoCode CHAR(2)
	,@MtrCode NVARCHAR(20) = NULL
	,@PrcCode NVARCHAR(32) = NULL
	,@SbjCode INT = NULL
	,@NltMaskedIp VARCHAR(11) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @LngId INT

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)


	SELECT 
		CASE 
			WHEN NLT_BROWSER IS NULL
				OR NLT_BROWSER = ''
				THEN '-'
			ELSE NLT_BROWSER
			END AS NltBrowser,
			COUNT(*) as NltCount
	FROM TD_PRODUCT
	INNER JOIN TD_RELEASE ON RLS_PRC_ID = PRC_ID
	INNER JOIN TD_SUBJECT ON PRC_SBJ_ID = SBJ_ID
	INNER JOIN TD_MATRIX ON MTR_RLS_ID = RLS_ID
	INNER JOIN TD_ANALYTIC ON MTR_ID = NLT_MTR_ID
	WHERE NLT_DATE >= @DateFrom
		AND NLT_DATE <= @DateTo
		AND (
			@MtrCode IS NULL
			OR MTR_CODE = @MtrCode
			)
		AND (
			@PrcCode IS NULL
			OR PRC_CODE = @PrcCode
			)
		AND (
			@SbjCode IS NULL
			OR SBJ_CODE = @SbjCode
			)
		AND (
			@NltMaskedIp IS NULL
			OR NLT_MASKED_IP <> @NltMaskedIp
			)
		GROUP BY NLT_BROWSER
END
GO

