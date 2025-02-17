
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/10/2018
-- Description:	Check if a Release Code has related Response entities
-- =============================================
CREATE
	

 PROCEDURE Workflow_Response_Usage @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @result INT

	SET @result = (
			SELECT iif(EXISTS (
						SELECT resp.WRS_ID
						FROM TD_WORKFLOW_REQUEST req
						INNER JOIN TD_WORKFLOW_RESPONSE resp
							ON resp.WRS_WRQ_ID = req.WRQ_ID
						INNER JOIN TD_RELEASE rel
							ON req.WRQ_RLS_ID = rel.RLS_ID
						WHERE rel.RLS_CODE = @RlsCode
							AND req.wrq_delete_flag = 0
							AND rel.rls_delete_flag = 0
							AND req.wrq_current_flag = 1
						), 1, 0)
			)

	RETURN @result
END
