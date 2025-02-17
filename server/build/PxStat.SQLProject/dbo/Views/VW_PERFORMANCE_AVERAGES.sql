
-- =============================================
-- Author:		Liam Millar
-- Create date: 02/11/2020
-- Description:	Gets the Latest Averages For the Performance Table, Grouped and ordered by server and datetime minute
-- =============================================
CREATE
	

 VIEW [dbo].[VW_PERFORMANCE_AVERAGES]
AS
SELECT PRF_SERVER
	,PRF_DATETIME
	,AVG(PRF_PROCESSOR_PERCENTAGE) AS 'AVG_PROCESSOR_PERCENTAGE'
	,AVG(PRF_MEMORY_AVAILABLE) AS 'AVG_MEMORY_AVAILABLE'
	,AVG(PRF_REQUEST_QUEUE) AS 'AVG_REQUEST_QUEUE'
	,AVG(PRF_REQUEST_PERSECOND) AS 'AVG_REQUEST_PERSECOND'
FROM TD_PERFORMANCE
GROUP BY PRF_SERVER
	,PRF_DATETIME
