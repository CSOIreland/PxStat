
-- =============================================
-- Author:		Liam Millar
-- Create date: 29/10/2020
-- Description:	Read performance averaged entries grouped by server and minute via VW_PERFORMANCE_AVERAGES
-- =============================================
CREATE
	

 PROCEDURE [dbo].[Security_Performance_Read] @PrfDatetimeStart DATETIME
	,@PrfDatetimeEnd DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	SELECT PRF_SERVER AS PrfServer
		,PRF_DATETIME AS PrfDatetime
		,AVG_PROCESSOR_PERCENTAGE AS PrfProcessor
		,AVG_MEMORY_AVAILABLE AS PrfMemory
		,AVG_REQUEST_QUEUE AS PrfQueue
		,AVG_REQUEST_PERSECOND AS PrfRequestspersecond
	FROM VW_PERFORMANCE_AVERAGES
	WHERE PRF_DATETIME >= @PrfDatetimeStart
		AND PRF_DATETIME <= @PrfDatetimeEnd
	ORDER BY PrfServer
		,PrfDatetime;
END
