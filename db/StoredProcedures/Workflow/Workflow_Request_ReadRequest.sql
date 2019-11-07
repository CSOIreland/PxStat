SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 20/11/2018
-- Description:	Read the static list of Requests
-- exec Workflow_Request_ReadRequest 'ROLLBACK'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_Request_ReadRequest @RqsCode VARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT rqs.RQS_CODE AS RqsCode
		,rqs.RQS_VALUE AS RqsValue
	FROM TS_REQUEST rqs
	WHERE @RqsCode IS NULL
		OR (@RqsCode = RQS_CODE)
END
GO


