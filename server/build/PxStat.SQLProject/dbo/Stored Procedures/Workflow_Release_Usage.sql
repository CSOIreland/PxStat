
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/10/2018
-- Description:	Checks if a ReleaseCode already has related entities
-- =============================================
CREATE
	

 PROCEDURE Workflow_Release_Usage @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @result INT

	SET @result = (
			SELECT iif(EXISTS (
						SELECT req.wrq_id
						FROM TD_WORKFLOW_REQUEST req
						INNER JOIN TD_RELEASE rel
							ON req.WRQ_RLS_ID = rel.RLS_ID
						WHERE rel.RLS_CODE = @RlsCode
							AND req.wrq_delete_flag = 0
							AND rel.rls_delete_flag = 0
						), 1, 0)
			)

	RETURN @result
END
